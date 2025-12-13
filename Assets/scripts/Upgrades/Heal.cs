using System.Collections;
using UnityEngine;

public class Heal : DefensiveWeapon
{
    [SerializeField]
    private PlayerManager playerManager;
    public void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    public override bool Attack()
    {
        if (timer <= 0)
        {
            timer += base.cooldown;
            base.Attack();
            StartCoroutine(AttackTimer());
            return true;
        }
        return false;
    }

    public IEnumerator AttackTimer()
    {
        playerManager.GetRenderer().color = Color.green;
        playerManager.SetHP(playerManager.GetHP() + Mathf.Min(playerManager.GetMaxHP()*0.3f, 8f));
        yield return new WaitForSeconds(0.2f);
        playerManager.GetRenderer().color = Color.white;
    }
}