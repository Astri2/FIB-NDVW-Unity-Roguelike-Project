using System.Collections;
using UnityEngine;

public class MeleeChasingEnemy : ChasingEnemy
{
    [Header("Combat")]
    [SerializeField] protected float attackDistance;
    [SerializeField] protected Weapon weapon;
    [SerializeField] protected BoxCollider2D weaponHitbox;
    [SerializeField] protected SpriteRenderer weaponRenderer;
    [SerializeField] protected float stunTimer; // gets stun after landing an attack
    [SerializeField] protected bool stunCoroutineRunning;

    private bool prevFlip;

    public override void Start()
    {
        base.Start();
        prevFlip = false;
    }

    protected override void SetStateCallbacks()
    {
        base.SetStateCallbacks();
        RegisterState(EnemyState_.Stun, new System.Action(Stun));
    }

    public void FixedUpdate()
    {
        base.FixedUpdate();
        if (flip != prevFlip)
        {
            weaponHitbox.offset = -weaponHitbox.offset;
            weaponRenderer.flipX = !weaponRenderer.flipX;
        }
        prevFlip = flip;
    }

    protected override void Chasing()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. First check if you can attack the player
        if(distanceToPlayer < attackDistance)
        {
            // if the weapon was not on cooldown, you get stun for a moment
            if(weapon.Attack())
            {
                SetState(EnemyState_.Stun);
            }
        }


        // 2. chase the player
        Vector3 desiredPosition = this.transform.position;
        bool shouldMove = false;

        // try to get very close to they player so that hits will land
        if (distanceToPlayer > attackDistance)
        {
            desiredPosition = player.position;
            shouldMove = true;
        }

        aiPath.canMove = shouldMove;
        if (shouldMove)
        {
            // Calculate push from other enemies
            Vector3 separation = base.ComputeSeparationVector();

            // Apply push to the desired position
            SetPathfindingDestination(desiredPosition + separation);
        }
    }

    protected virtual void Stun()
    {
        this.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        if (!stunCoroutineRunning) StartCoroutine(StunCoroutine(stunTimer));
    }

    private IEnumerator StunCoroutine(float delay)
    {
        stunCoroutineRunning = true;
        yield return new WaitForSeconds(delay);

        // if enemy got stunned, it means it was chasing the player
        SetState(EnemyState_.Chasing);
        stunCoroutineRunning = false;
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }
}
