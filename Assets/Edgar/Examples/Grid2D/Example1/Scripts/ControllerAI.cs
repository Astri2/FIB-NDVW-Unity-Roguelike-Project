using UnityEngine;

public class ControllerAI : MonoBehaviour
{
    // References
    public Transform playerTransform;
    public UnityEngine.AI.NavMeshAgent agent;

    // Distances
    public float optimalDistance = 8f;
    public float fleeDistance = 4f;
    public float maxVisionRange = 20f;

    // Speeds
    public float moveSpeed = 3.5f;
    public float fleeSpeed = 5.0f;

    // Ability Timer
    public float timeBetweenAbilities = 5f;
    private float nextAbilityTime;
    public GameObject trapPrefab;

    void Start()
    {
        agent.speed = moveSpeed;
        nextAbilityTime = Time.time + timeBetweenAbilities;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxVisionRange)
        {
            return;
        }

        HandleMovement(distanceToPlayer);
        HandleAbilities(distanceToPlayer);
    }

    void HandleMovement(float distance)
    {
        if (distance < fleeDistance)
        {
            agent.speed = fleeSpeed;
            Vector3 fleeDirection = (transform.position - playerTransform.position).normalized;
            Vector3 newPosition = transform.position + fleeDirection * (optimalDistance - distance);
            agent.SetDestination(newPosition);
        }
        else if (distance > optimalDistance)
        {
            agent.speed = moveSpeed;
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            agent.speed = moveSpeed;
            agent.ResetPath();
            transform.LookAt(playerTransform);
        }
    }

    void HandleAbilities(float distance)
    {
        if (distance <= optimalDistance && distance >= fleeDistance && Time.time >= nextAbilityTime)
        {
            CastTrap(playerTransform.position);
            nextAbilityTime = Time.time + timeBetweenAbilities;
        }
    }

    void CastTrap(Vector3 targetPosition)
    {
        if (trapPrefab != null)
        {
            Instantiate(trapPrefab, targetPosition, Quaternion.identity);
        }
    }
}
