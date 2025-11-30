using System.Collections;
using UnityEngine;

public class Parry : DefensiveWeapon
{
    [SerializeField]
    private PlayerManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        base.Attack();
        StartCoroutine(AttackTimer());
    }

    public IEnumerator AttackTimer()
    { 
        playerManager.SetParrying(true);
        playerManager.GetRenderer().color = Color.gray;
        yield return new WaitForSeconds(1);
        playerManager.SetParrying(false);
        playerManager.GetRenderer().color = Color.white;
    }
}
