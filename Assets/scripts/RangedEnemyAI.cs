using UnityEngine;
using Pathfinding;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float attackRange = 5f;
    public float fireInterval = 1.5f;

    [Header("Combat Movement")]
    public float preferredDistance = 3f;     // The enemy wants to stay this far away
    public float distanceBuffer = 0.5f;      // Small buffer to prevent jittering (e.g. stop between 2.5 and 3.5)

    [Header("Wander Settings")]
    public float wanderStartDistance = 7f;   // REDUCED: Enemy detects player later
    public float wanderRadius = 6f;
    public float wanderPointMinDistance = 2f;
    public float obstacleCheckRadius = 1.0f; // Safety check radius
    public int wanderRetries = 20;

    [Header("Wander Limits")]
    public int maxWanderSteps = 100;
    private int currentWanderStep = 0;

    private float fireCooldown;
    private AIPath aiPath;
    private AIDestinationSetter destSetter;

    private Transform player;
    private bool isWandering = false;
    private bool finishedWandering = false;

    private Transform targetHelper; // Reusable transform for movement targets

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();

        // Create a hidden helper object to act as the dynamic target
        GameObject wt = new GameObject("AI_Target_Helper");
        targetHelper = wt.transform;

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null) player = foundPlayer.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        fireCooldown -= Time.deltaTime;

        // =========================================================
        // STATE 1: COMBAT (Player is visible/near)
        // =========================================================
        if (distanceToPlayer <= wanderStartDistance)
        {
            isWandering = false;
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
        
        // 1. TOO FAR: Approach Player
        // If we are further than (3.0 + 0.5), move closer
        if (distanceToPlayer > preferredDistance + distanceBuffer)
        {
            destSetter.target = player;
        }
        // 2. TOO CLOSE: Back Away (Flee)
        // If we are closer than (3.0 - 0.5), move backwards
        else if (distanceToPlayer < preferredDistance - distanceBuffer)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;
            Vector3 fleePosition = transform.position + directionAway * 2f; // Try to move 2 units away

            // Check if flee point is walkable using our safety check
            // If backing up hits a wall, we might as well just stand ground or strafe (simple fallback: stand still)
            if (IsPathSafe(fleePosition))
            {
                targetHelper.position = fleePosition;
                destSetter.target = targetHelper;
            }
            else
            {
                // Wall behind us! Stop moving so we don't vibrate against the wall
                aiPath.canMove = false;
            }
        }
        // 3. SWEET SPOT: Stop moving
        else
        {
            // We are roughly at distance 3. Stop moving to aim better.
            aiPath.canMove = false;
        }
    }

    // ==============================================
    // WANDER LOGIC (Walk -> Stop -> Walk)
    // ==============================================
    private void HandleWandering()
    {
        if (finishedWandering)
        {
            aiPath.canMove = false;
            return;
        }

        aiPath.canMove = true;

        if (!isWandering)
        {
            isWandering = true;
            PickNewWanderPoint();
        }

        if (aiPath.reachedDestination && !aiPath.pathPending)
        {
            PickNewWanderPoint();
        }
    }

    private void PickNewWanderPoint()
    {
        if (currentWanderStep >= maxWanderSteps)
        {
            finishedWandering = true;
            return;
        }

        for (int i = 0; i < wanderRetries; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            var info = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);

            if (info.node == null || !info.node.Walkable) continue;

            Vector3 candidatePos = info.position;

            if (Vector2.Distance(transform.position, candidatePos) < wanderPointMinDistance)
                continue;

            if (!IsPathSafe(candidatePos))
                continue;

            // Found valid point
            targetHelper.position = candidatePos;
            destSetter.target = targetHelper;
            currentWanderStep++;
            return;
        }

        // Fallback
        targetHelper.position = transform.position;
        destSetter.target = targetHelper;
    }

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
        if (distance <= attackRange && fireCooldown <= 0f)
        {
            ShootAtPlayer();
            fireCooldown = fireInterval;
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        if (bullet.TryGetComponent(out EnemyProjectile proj)) 
            proj.SetDirection(direction);
    }
}