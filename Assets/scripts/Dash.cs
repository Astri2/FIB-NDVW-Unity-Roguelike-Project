using System.Collections;
using Unity.VisualScripting;
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
    }

    public override void Attack()
    {
        if (timer <= 0)
        {
            timer += base.cooldown;
            base.Attack();
            Debug.Log("movement");

            StartCoroutine(AttackTimer());
        }
    }

    public IEnumerator AttackTimer()
    {
        playerManager.SetMoveSpeed(5 * distance);
        yield return new WaitForSeconds(0.1f);
        playerManager.SetMoveSpeed(5f);
    }
}
