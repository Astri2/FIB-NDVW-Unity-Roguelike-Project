using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    UnityEngine.AI.NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (target == null) return;
        if (agent == null) return;
        if (!agent.isActiveAndEnabled) return;
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(target.position);
    }
}

