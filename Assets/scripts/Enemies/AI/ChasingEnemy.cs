using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
public abstract class ChasingEnemy : StateBasedEnemy
{
    [Header("Moving Speed")]
    [SerializeField] protected float roamingSpeed = 2f;
    [SerializeField] protected float chasingSpeed = 3f;

    [Header("Chasing Settings")]
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float losePlayerRange = 7f;
    [SerializeField] protected LayerMask obstacleMask;

    [Header("Returning to spawn Settings")]
    [SerializeField] protected Vector3 spawnPoint;

    [Header("Patrol Settings")]
    [SerializeField] protected int patrolPointCount = 4;
    [SerializeField] protected Vector3[] patrolPoints;
    [SerializeField] protected int patrolIndex;

    // pathfinding
    [Header("Pathfinding")]
    [SerializeField] protected AIPath aiPath;
    [SerializeField] protected AIDestinationSetter destSetter;
    [SerializeField] protected Transform targetHelper;
    [SerializeField] protected float obstacleCheckRadius = 1.0f; // Safety check radius
    [SerializeField] protected int maxPointSelectionTries = 20;
    [SerializeField] protected float pathFindTimer;
    [SerializeField] protected float maxPathFindTimer = 8f;

    // --- NEW: SEPARATION SETTINGS ---
    [Header("Avoidance")]
    [SerializeField] protected LayerMask enemyLayerMask;            // Select the layer your enemies are on
    [SerializeField] protected float separationRadius = 1.5f;       // How close to check for neighbors
    [SerializeField] protected float separationStrength = 5.0f;     // How hard to push away
    [SerializeField] protected float separationSmoothing = 5.0f;    // Smoothing speed
    [SerializeField] protected Vector3 currentFixedDestination;    // Where we want to go BEFORE separation
    [SerializeField] protected Vector3 currentSeparationVector;    // Smoothed separation offset
    [SerializeField] protected Collider2D[] separationBuffer = new Collider2D[10]; // Buffer for physics checks

    public override void Start()
    {
        base.Start();
        spawnPoint = transform.position;
        currentFixedDestination = transform.position;

        aiPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();

        // Create a hidden helper object to act as the dynamic target
        // Store it into a gameobject to avoid using hierachy root
        GameObject helpers = GameObject.Find("AITargetHelpers");
        if (helpers == null) helpers = new GameObject("AITargetHelpers");

        GameObject wt = new GameObject("AI_Target_Helper");
        wt.transform.parent = helpers.transform;
        targetHelper = wt.transform;
    }

    protected override EnemyState_ GetInitialState()
    {
        return EnemyState_.Roaming;
    }

    protected override void SetStateCallbacks()
    {
        base.SetStateCallbacks();
        RegisterState(EnemyState_.Roaming, new System.Action(Roaming));
        RegisterState(EnemyState_.Patrolling, new System.Action(Patrolling));
        RegisterState(EnemyState_.ReturningToSpawn, new System.Action(ReturningToSpawn));
        RegisterState(EnemyState_.Chasing, new System.Action(Chasing));
    }

    protected override void PreUpdate()
    {
        CheckPlayerVisibility();

        // base implement death detection, which has higher priority, so it should be set after
        base.PreUpdate();
    }

    protected override void SetState(EnemyState_ enemyState)
    {
        base.SetState(enemyState);
        //aiPath.maxSpeed = (this.GetState() == EnemyState_.Chasing ? chasingSpeed : roamingSpeed);
        aiPath.canMove = true; // reset the lock
    }

    protected virtual void Roaming()
    {
        pathFindTimer -= Time.deltaTime;

        if (!aiPath.hasPath || aiPath.reachedDestination || pathFindTimer <= 0f)
        {
            if (Random.value < 0.3f)
            {
                SetState(EnemyState_.Patrolling);
                return;
            }

            if(!aiPath.pathPending) ChooseNewRoamingPoint();
        }

        // change the enemy goal to avoid other enemies (steering)
        Vector3 separation = ComputeSeparationVector();
        targetHelper.position = currentFixedDestination + separation;
        destSetter.target = targetHelper;
    }
        
    protected virtual void Patrolling()
    {
        pathFindTimer -= Time.deltaTime;

        if (patrolPoints == null || patrolPoints.Length == 0) setPatrolingPoints();
        
        if (!aiPath.hasPath || aiPath.reachedDestination || pathFindTimer <= 0)
        {
            if (Random.value < 0.3f)
            {
                SetState(EnemyState_.Roaming);
                return;
            }

            if (!aiPath.pathPending)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                if (IsPathSafe(patrolPoints[patrolIndex])) SetPathfindingDestination(patrolPoints[patrolIndex]);
            }
        }

        // change the enemy goal to avoid other enemies (steering)
        Vector3 separation = ComputeSeparationVector();
        targetHelper.position = currentFixedDestination + separation;
        destSetter.target = targetHelper;
    }
            
    protected virtual void ReturningToSpawn()
    {
        pathFindTimer -= Time.deltaTime;

        if (!aiPath.hasPath || aiPath.reachedDestination || pathFindTimer <= 0)
        {
            SetState(EnemyState_.Roaming);
        }
    }

    protected abstract void Chasing();

    protected void SetPathfindingDestination(Vector3 pos)
    {
        currentFixedDestination = pos; // serves as a backup to recover goal in case helper moves to avoid other enemies
        targetHelper.position = currentFixedDestination;
        destSetter.target = targetHelper;
        pathFindTimer = maxPathFindTimer;
    }

    protected override void deadState()
    {
        Destroy(targetHelper.gameObject);
        base.deadState();
    }

    private void CheckPlayerVisibility()
    {
        if (player == null) return;

        if (GetState() == EnemyState_.Stun) return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, detectionRange, obstacleMask);
        bool canSeePlayer = hit.collider == null || hit.collider.CompareTag("Player");

        if (GetState() != EnemyState_.Chasing && distance < detectionRange && canSeePlayer)
            SetState(EnemyState_.Chasing);

        else if (GetState() == EnemyState_.Chasing && distance > losePlayerRange)
        {
            // try to find a way back to spawn
            if (IsPathSafe(spawnPoint))
            {
                SetPathfindingDestination(spawnPoint);
                SetState(EnemyState_.ReturningToSpawn);
            }
            // if you can't find a way, just roam
            else
            {
                SetState(EnemyState_.Roaming);
            }
            
        }
            
    }

    private Vector3 getRandomPointInRoom()
    {
        for (int i = 0; i < maxPointSelectionTries; i++)
        {
            int ptIndexRandom = Random.Range(0, GetRoomPoints().Count - 1);
            Vector2 random2Dpt = GetRoomPoints()[ptIndexRandom];
            Vector3 randomPoint = new Vector3(random2Dpt.x + 0.5f, random2Dpt.y + 0.5f, 0f) + GetDungeonCenteringShift();

            var info = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);

            if (info.node == null || !info.node.Walkable) continue;

            Vector3 candidatePos = info.position;


            if (!IsPathSafe(candidatePos))
                continue;

            // Found valid point
            return candidatePos;
        }

        // Fallback
        return transform.position;
    }
    private void ChooseNewRoamingPoint()
    {
        SetPathfindingDestination(getRandomPointInRoom());
    }

    protected bool IsPathSafe(Vector3 position)
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
            if (info.node == null || !info.node.Walkable)
            {
                //Debug.Log("Found a non safe path");
                return false;
            }
        }

        //Debug.Log("Found a safe path");

        return true;
    }

    private void setPatrolingPoints()
    {
        HashSet<Vector3> points = new();

        // compute n unique points
        while (points.Count < patrolPointCount)
        {
            Vector3 candidate = getRandomPointInRoom();
            if(points.Contains(candidate)) continue;
            points.Add(candidate);
        }

        // compute their centroid
        float cx = points.Sum(p => p.x);
        float cy = points.Sum(p => p.y);

        // order them by angle around the centroid. This should tend to generate a non crossing polygon
        patrolPoints = points.OrderBy(p => Mathf.Atan2(p.y - cy, p.x - cx)).ToArray();
    }

    protected Vector3 ComputeSeparationVector()
    {
        return Vector3.zero;

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
}