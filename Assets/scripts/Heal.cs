using System.Collections;
using UnityEngine;

public class Heal : DefensiveWeapon
{
    [SerializeField]
    private PlayerManager playerManager;
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime*speed;
        }
    }
    public override void Attack()
    {
        if (timer <= 0)
        {
            timer += base.cooldown;
            base.Attack();
            StartCoroutine(AttackTimer());
        }
    }

    public IEnumerator AttackTimer()
    {
        playerManager.GetRenderer().color = Color.green;
        playerManager.SetHP(playerManager.GetHP() + Mathf.Min(playerManager.GetMaxHP()*0.3f, 8f));
        yield return new WaitForSeconds(0.2f);
        playerManager.GetRenderer().color = Color.white;
    }
}