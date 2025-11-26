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
    [SerializeField]
    private Animator animator;

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
            animator.SetBool("Attacking", true);
            StartCoroutine(AttackTimer());
            Debug.Log("weapon attack");
        }
        else {
            Debug.Log("weapon is in cooldown");
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("weapon hit");
        PlayerManager w = collider.gameObject.GetComponent<PlayerManager>();
        AI_ChasingEnemy enemy = collider.gameObject.GetComponent<AI_ChasingEnemy>();
        if (w != null)
        {
            w.SetHP(w.GetHP() - this.damage);
        }
        else if (enemy != null) { 
            enemy.SetHP(enemy.GetHP() - this.damage);
        }

        
    }

    public IEnumerator AttackTimer()
    {
        boxCollider.enabled = true;
        Debug.Log("should enable collider");
        yield return new WaitForSeconds(0.5f);
        boxCollider.enabled = false;
        animator.SetBool("Attacking", false);
    }
}
