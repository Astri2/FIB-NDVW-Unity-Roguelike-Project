using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

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

    [Header("Roaming Settings")]
    [SerializeField] protected float roamingRadius = 2f;
    [SerializeField] protected float roamingInterval = 3f;
    [SerializeField] protected Vector3 roamingTarget;
    [SerializeField] protected float roamingTimer;

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
    

    public override void Start()
    {
        base.Start();
        spawnPoint = transform.position;
        
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
        stateCallbacks.Add(EnemyState_.Roaming, new System.Action(Roaming));
        stateCallbacks.Add(EnemyState_.Patrolling, new System.Action(Patrolling));
        stateCallbacks.Add(EnemyState_.ReturningToSpawn, new System.Action(ReturningToSpawn));
        stateCallbacks.Add(EnemyState_.Chasing, new System.Action(Chasing));
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
        aiPath.maxSpeed = (this.enemyState == EnemyState_.Chasing ? chasingSpeed : roamingSpeed);
    }

    protected virtual void Roaming()
    {
        roamingTimer -= Time.deltaTime;

        if (!aiPath.hasPath || aiPath.reachedDestination || roamingTimer <= 0f)
        {
            if (Random.value < 0.5f)
            {
                enemyState = EnemyState_.Patrolling;
                return;
            }

            if(!aiPath.pathPending) ChooseNewRoamingPoint();
        }
    }
        
    protected virtual void Patrolling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) setPatrolingPoints();
        
        if (!aiPath.hasPath || aiPath.reachedDestination)
        {
            if (Random.value < 0.3f)
            {
                enemyState = EnemyState_.Roaming;
                return;
            }

            if (!aiPath.pathPending)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                if (IsPathSafe(patrolPoints[patrolIndex])) SetPathfindingDestination(patrolPoints[patrolIndex]);
            }
        }
    }
            
    protected virtual void ReturningToSpawn()
    {
        if (Vector2.Distance(transform.position, spawnPoint) < 0.2f)
            enemyState = EnemyState_.Roaming;
    }

    protected abstract void Chasing();

    protected void SetPathfindingDestination(Vector3 pos)
    {
        targetHelper.position = pos;
        destSetter.target = targetHelper;
    }

    private void CheckPlayerVisibility()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, detectionRange, obstacleMask);
        bool canSeePlayer = hit.collider == null || hit.collider.CompareTag("Player");

        if (enemyState != EnemyState_.Chasing && distance < detectionRange && canSeePlayer)
            enemyState = EnemyState_.Chasing;

        else if (enemyState == EnemyState_.Chasing && (distance > losePlayerRange || !canSeePlayer))
            enemyState = EnemyState_.ReturningToSpawn;
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
}