using Edgar.Unity.Examples;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float cooldown;
    public float timer = 0;
    [SerializeField]
    private BoxCollider2D boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (timer > 0) {
            timer -= Time.fixedDeltaTime;
        }
        //Debug.Log("weapon timer : " + timer);
    }

    public override void Attack()
    {
        if (timer <= 0)
        {
            timer += cooldown;
            base.Attack();
            StartCoroutine(AttackTimer());
            Debug.Log("weapon attack");
        }
        else {
            Debug.Log("weapon is in cooldown");
        }
        boxCollider.enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("weapon hit");
    }

    public IEnumerator AttackTimer()
    {
        boxCollider.enabled = false;
        Debug.Log("should enable collider");
        yield return new WaitForSeconds(2);
    }
}
