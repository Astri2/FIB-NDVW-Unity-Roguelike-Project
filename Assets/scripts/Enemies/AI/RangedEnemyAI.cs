using UnityEngine;
using Pathfinding;
using System.Collections.Generic; // --- NEW: Added for List usage if needed

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]
public class RangedEnemyAI : MonoBehaviour
{
    Enemy enemy;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float attackRange = 5f;
    public float fireInterval = 1.5f;

    [Header("Combat Movement")]
    public float preferredDistance = 3f;     // The enemy wants to stay this far away
    public float distanceBuffer = 0.5f;      // Small buffer to prevent jittering (e.g. stop between 2.5 and 3.5)

    // --- NEW: SEPARATION SETTINGS ---
    [Header("Separation (Avoidance)")]
    public LayerMask enemyLayerMask;            // Select the layer your enemies are on
    public float separationRadius = 1.5f;       // How close to check for neighbors
    public float separationStrength = 5.0f;     // How hard to push away
    public float separationSmoothing = 5.0f;    // Smoothing speed
    // -------------------------------

    [Header("Wander Settings")]
    public float wanderStartDistance = 7f;   // REDUCED: Enemy detects player later
    public float wanderRadius = 6f;
    public float wanderPointMinDistance = 2f;
    public float obstacleCheckRadius = 1.0f; // Safety check radius
    public int wanderRetries = 20;

    // --- NEW: TIMEOUT SETTING ---
    public float wanderTimeout = 5.0f;       // If stuck for 5s, pick new point
    // ----------------------------

    [Header("Wander Limits")]
    private int currentWanderStep = 0;

    // private float fireCooldown;
    private AIPath aiPath;
    private AIDestinationSetter destSetter;

    private bool isWandering = false;

    private Transform targetHelper; // Reusable transform for movement targets

    // --- NEW: INTERNAL VARIABLES ---
    private float wanderTimer = 0f;             // Tracks how long we've been trying to reach the point
    private Vector3 currentFixedDestination;    // Where we want to go BEFORE separation
    private Vector3 currentSeparationVector;    // Smoothed separation offset
    private Collider2D[] separationBuffer = new Collider2D[10]; // Buffer for physics checks
    // -------------------------------

    [Header("Kieran's weapon system")]
    public BowEnemy bow;

    public void Start()
    {
        aiPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();
        enemy = GetComponent<Enemy>();

        // Create a hidden helper object to act as the dynamic target
        // Store it into a gameobject to avoid using hierachy root
        GameObject helpers = GameObject.Find("AITargetHelpers");
        if (helpers == null) helpers = new GameObject("AITargetHelpers");

        // --- MODIFIED: Added unique ID so enemies don't share the same helper object ---
        GameObject wt = new GameObject($"AI_Target_Helper_{gameObject.GetInstanceID()}");
        wt.transform.parent = helpers.transform;
        targetHelper = wt.transform;
        
        // --- NEW: Init fixed destination ---
        currentFixedDestination = transform.position;
    }

    public void Update()
    {
        if (enemy.GetPlayerTransform() == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, enemy.GetPlayerTransform().position);
        // fireCooldown -= Time.deltaTime;

        // =========================================================
        // STATE 1: COMBAT (Player is visible/near)
        // =========================================================
        if (distanceToPlayer <= wanderStartDistance)
        {
            isWandering = false;
            
            // --- NEW: Reset wander timer when entering combat so it's fresh later ---
            wanderTimer = 0f; 
            
            // enemy start aiming at the player as soon as he sees it
            bow.RotateTowards(enemy.GetPlayerTransform().position);
            
            HandleCombatMovement(distanceToPlayer);
            AttackWhenInRange(distanceToPlayer);
        }
        // =========================================================
        // STATE 2: WANDERING (Player is far/hidden)
        // =========================================================
        else
        {
            HandleWandering();
        }
    }

    // ==============================================
    // COMBAT MOVEMENT (Keep Distance)
    // ==============================================
    private void HandleCombatMovement(float distanceToPlayer)
    {
        aiPath.canMove = true;
        
        // --- NEW: Variable to store where we WANT to be before separation ---
        Vector3 desiredPosition = transform.position;
        bool shouldMove = false;
        
        // 1. TOO FAR: Approach Player
        // If we are further than (3.0 + 0.5), move closer
        if (distanceToPlayer > preferredDistance + distanceBuffer)
        {
            // --- MODIFIED: Don't set target directly yet, save to variable ---
            desiredPosition = enemy.GetPlayerTransform().position;
            shouldMove = true;
        }
        // 2. TOO CLOSE: Back Away (Flee)
        // If we are closer than (3.0 - 0.5), move backwards
        else if (distanceToPlayer < preferredDistance - distanceBuffer)
        {
            Vector3 directionAway = (transform.position - enemy.GetPlayerTransform().position).normalized;
            Vector3 fleePosition = transform.position + directionAway * 2f; // Try to move 2 units away

            // Check if flee point is walkable using our safety check
            // If backing up hits a wall, we might as well just stand ground or strafe (simple fallback: stand still)
            if (IsPathSafe(fleePosition))
            {
                // --- MODIFIED: Save to variable instead of setting directly ---
                desiredPosition = fleePosition;
                shouldMove = true;
            }
            else
            {
                // Wall behind us! Stop moving so we don't vibrate against the wall
                aiPath.canMove = false;
                shouldMove = false;
            }
        }
        // 3. SWEET SPOT: Stop moving
        else
        {
            // We are roughly at distance 3. Stop moving to aim better.
            aiPath.canMove = false;
            shouldMove = false;
        }

        // --- NEW: APPLY SEPARATION LOGIC ---
        if (shouldMove)
        {
            // Calculate push from other enemies
            Vector3 separation = ComputeSeparationVector();
            
            // Apply push to the desired position
            targetHelper.position = desiredPosition + separation;
            destSetter.target = targetHelper;
        }
    }

    // ==============================================
    // WANDER LOGIC (Walk -> Stop -> Walk)
    // ==============================================
    private void HandleWandering()
    {
        aiPath.canMove = true;

        if (!isWandering)
        {
            isWandering = true;
            PickNewWanderPoint();
        }

        // --- NEW: TIMEOUT LOGIC ---
        wanderTimer += Time.deltaTime;
        
        // Check if reached destination OR timed out
        bool reached = aiPath.reachedDestination && !aiPath.pathPending;
        bool timedOut = wanderTimer >= wanderTimeout; // 5 seconds check

        if (reached || timedOut)
        {
            PickNewWanderPoint();
        }

        // --- NEW: APPLY SEPARATION WHILE WANDERING ---
        // Even if we have a fixed wander point, we nudge the helper slightly if an ally bumps us
        Vector3 separation = ComputeSeparationVector();
        targetHelper.position = currentFixedDestination + separation;
        destSetter.target = targetHelper;
    }

    private void PickNewWanderPoint()
    {
        // --- NEW: Reset timer ---
        wanderTimer = 0f;

        for (int i = 0; i < wanderRetries; i++)
        {
            /*
            // TODO: use the list of point given at the start in Enemy Component
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            */
            int ptIndexRandom = Random.Range(0, enemy.GetRoomPoints().Count - 1);
            Vector2 random2Dpt = enemy.GetRoomPoints()[ptIndexRandom];
            Vector3 randomPoint = new Vector3(random2Dpt.x + 0.5f, random2Dpt.y + 0.5f, 0f) + enemy.GetDungeonCenteringShift();


            var info = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);

            if (info.node == null || !info.node.Walkable) continue;

            Vector3 candidatePos = info.position;

            if (Vector2.Distance(transform.position, candidatePos) < wanderPointMinDistance)
                continue;

            if (!IsPathSafe(candidatePos))
                continue;

            // Found valid point
            
            // --- MODIFIED: Store in variable first ---
            currentFixedDestination = candidatePos;
            
            // --- MODIFIED: Apply immediately to targetHelper ---
            targetHelper.position = currentFixedDestination;
            destSetter.target = targetHelper;
            
            currentWanderStep++;
            return;
        }

        // Fallback
        // --- MODIFIED: Store in variable ---
        currentFixedDestination = transform.position;
        targetHelper.position = transform.position;
        destSetter.target = targetHelper;
    }

    // --- NEW: SEPARATION CALCULATION METHOD ---
    private Vector3 ComputeSeparationVector()
    {
        // Find neighbors
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, separationRadius, separationBuffer, enemyLayerMask);

        Vector3 separationForce = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = separationBuffer[i];
            
            // Skip self or non-enemies (though LayerMask should handle non-enemies)
            if (col.gameObject == gameObject) continue;

            // Calculate vector away from neighbor
            Vector3 dir = transform.position - col.transform.position;
            float dist = dir.magnitude;

            // Avoid division by zero
            if (dist < 0.01f) dist = 0.01f;

            // The closer they are, the stronger the push
            float strength = 1f - (dist / separationRadius);
            separationForce += dir.normalized * strength;
        }

        if (separationForce != Vector3.zero)
        {
            separationForce = separationForce.normalized * separationStrength;
        }

        // Smooth the result
        currentSeparationVector = Vector3.Lerp(currentSeparationVector, separationForce, Time.deltaTime * separationSmoothing);

        return currentSeparationVector;
    }
    // ------------------------------------------

    private bool IsPathSafe(Vector3 position)
    {
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, obstacleCheckRadius, 0),
            new Vector3(0, -obstacleCheckRadius, 0),
            new Vector3(obstacleCheckRadius, 0, 0),
            new Vector3(-obstacleCheckRadius, 0, 0)
        };

        foreach (Vector3 offset in offsets)
        {
            var info = AstarPath.active.GetNearest(position + offset, NNConstraint.None);
            if (info.node == null || !info.node.Walkable) return false;
        }
        return true;
    }

    // ==============================================
    // SHOOTING
    // ==============================================
    private void AttackWhenInRange(float distance)
    {
        if (distance <= attackRange /* && fireCooldown <= 0f */)
        {
            // ShootAtPlayer();
            // fireCooldown = fireInterval;
            bow.Attack();
        }
    }

    /*
    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || enemy.GetPlayerTransform() == null) return;
        
        Vector2 direction = (enemy.GetPlayerTransform().position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        if (bullet.TryGetComponent(out EnemyProjectile proj)) 
            proj.SetDirection(direction);
    }
    */
    
    // --- NEW: Debug Gizmo ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}