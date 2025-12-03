using UnityEngine;

public class AIChase_Simple_Trigger : MonoBehaviour
{
    public enum EnemyState { Roaming, Patrolling, Chasing, ReturningToSpawn }

    public Transform player;
    public BoxCollider2D roomBounds;

    [Header("Movement Settings")]
    public float roamingSpeed = 2f;
    public float chasingSpeed = 3f;
    public float roamingRadius = 2f;
    public float roamingInterval = 3f;

    [Header("Patrol Settings")]
    public int patrolPointCount = 4;      
    private Vector3[] patrolPoints;       

    [Header("Vision Settings")]
    public float detectionRange = 5f;
    public float losePlayerRange = 7f;
    public LayerMask obstacleMask;

    private EnemyState currentState = EnemyState.Roaming;
    private Vector3 spawnPoint;
    private Vector3 roamingTarget;
    private int patrolIndex = 0;
    private float roamingTimer;

    void Start()
    {
        spawnPoint = transform.position;
        GeneratePatrolPoints();
        ChooseNewRoamingPoint();
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        switch (currentState)
        {
            case EnemyState.Roaming: Roaming(); break;
            case EnemyState.Patrolling: Patrolling(); break;
            case EnemyState.Chasing: Chasing(); break;
            case EnemyState.ReturningToSpawn: ReturningToSpawn(); break;
        }

        CheckPlayerVisibility();
        StayInsideRoom();
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[patrolPointCount];

        for (int i = 0; i < patrolPointCount; i++)
        {
            patrolPoints[i] = GetRandomPointInsideRoom();
        }
    }

    Vector3 GetRandomPointInsideRoom()
    {
        if (roomBounds == null)
            return spawnPoint;

        Bounds b = roomBounds.bounds;
        Vector3 point;

        
        do
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            point = new Vector3(x, y, 0f);

        } while (!b.Contains(point));

        return point;
    }


    // STATES LOGIC

    void Roaming()
    {
        roamingTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, roamingTarget, roamingSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, roamingTarget) < 0.2f || roamingTimer <= 0f)
        {
            if (patrolPoints.Length > 0 && Random.value < 0.5f)
                SwitchState(EnemyState.Patrolling);
            else
                ChooseNewRoamingPoint();
        }
    }

    void Patrolling()
    {
        if (patrolPoints.Length == 0)
        {
            SwitchState(EnemyState.Roaming);
            return;
        }

        Vector3 target = patrolPoints[patrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, target, roamingSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

            if (Random.value < 0.3f)
                SwitchState(EnemyState.Roaming);
        }
    }

    void Chasing()
    {
        if (player == null)
        {
            SwitchState(EnemyState.ReturningToSpawn);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, player.position, chasingSpeed * Time.deltaTime);
    }

    void ReturningToSpawn()
    {
        transform.position = Vector2.MoveTowards(transform.position, spawnPoint, roamingSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, spawnPoint) < 0.2f)
            SwitchState(EnemyState.Roaming);
    }

    
    // VISION AND ROOM LIMITS

    void CheckPlayerVisibility()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, detectionRange, obstacleMask);
        bool canSeePlayer = hit.collider == null || hit.collider.CompareTag("Player");

        if (distance < detectionRange && canSeePlayer && currentState != EnemyState.Chasing)
            SwitchState(EnemyState.Chasing);

        else if ((distance > losePlayerRange || !canSeePlayer) && currentState == EnemyState.Chasing)
            SwitchState(EnemyState.ReturningToSpawn);
    }

    void StayInsideRoom()
    {
        if (roomBounds == null) return;

        Vector3 pos = transform.position;
        Vector3 min = roomBounds.bounds.min;
        Vector3 max = roomBounds.bounds.max;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        transform.position = pos;
    }

    // UTILITIES

    void SwitchState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Roaming)
            ChooseNewRoamingPoint();
    }

    void ChooseNewRoamingPoint()
    {
        roamingTimer = roamingInterval;
        Vector2 randomCircle = Random.insideUnitCircle * roamingRadius;
        roamingTarget = spawnPoint + new Vector3(randomCircle.x, randomCircle.y, 0f);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null) return;

        Gizmos.color = Color.yellow;

        foreach (var p in patrolPoints)
        {
            Gizmos.DrawSphere(p, 0.15f);
        }
    }
}
