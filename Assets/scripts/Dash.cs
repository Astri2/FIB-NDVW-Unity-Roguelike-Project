using System.Collections;
using UnityEngine;

public class Dash : MovementWeapon
{
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private float distance;
    [SerializeField]
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        rb = playerManager.GetComponent<Rigidbody2D>();
    }

    public override void Attack()
    {
        base.Attack();

        StartCoroutine(AttackTimer());
    }

    public IEnumerator AttackTimer()
    {
        playerManager.SetMoveSpeed(5 * distance);
        yield return new WaitForSeconds(0.1f);
        playerManager.SetMoveSpeed(5f);
    }
}
