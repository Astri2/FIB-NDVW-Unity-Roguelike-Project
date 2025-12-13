using Unity.VisualScripting;
using UnityEngine;

public class RangedChasingEnemy : ChasingEnemy
{
    [Header("Combat")]
    [SerializeField] protected float preferredDistance = 3f;     // The enemy wants to stay this far away
    [SerializeField] protected float distanceBuffer = 0.5f;      // Small buffer to prevent jittering (e.g. stop between 2.5 and 3.5)
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected BowEnemy bow;

    protected override void Chasing()
    {
        // when chasing, the enemy aims to the player
        bow.RotateTowards(player.position);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. First check if you can attack the player
        if (distanceToPlayer <= attackRange) bow.Attack();

        // 2. chase the player
        Vector3 desiredPosition = transform.position;
        bool shouldMove = false;

        // if you are too far away, chase the player
        if (distanceToPlayer > preferredDistance + distanceBuffer)
        {
            desiredPosition = player.position;
            shouldMove = true;
        }
        // if you are too close, run away
        else if(distanceToPlayer < preferredDistance - distanceBuffer)
        {

            Vector3 directionAway = (transform.position - player.position).normalized;
            Vector3 fleePosition = transform.position + directionAway * 2f; // Try to move 2 units away

            // Check if flee point is walkable using our safety check
            // If backing up hits a wall, we might as well just stand ground or strafe (simple fallback: stand still)
            if (base.IsPathSafe(fleePosition))
            {
                desiredPosition = fleePosition;
                shouldMove = true;
            }
        }
        // else: SWEET SPOT: Stop moving, just shoot
        
        aiPath.canMove = shouldMove;
        if (shouldMove)
        {
            // Calculate push from other enemies
            Vector3 separation = base.ComputeSeparationVector();

            // Apply push to the desired position
            // TODO: check for path sefety?
            targetHelper.position = desiredPosition + separation;
            destSetter.target = targetHelper;
        }
    }
}
