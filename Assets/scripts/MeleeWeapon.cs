using Edgar.Unity.Examples;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField]
    private BoxCollider2D boxCollider;
    [SerializeField]
    private Animator animator;

    public bool isAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAttacking = false;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (timer > 0) {
            timer -= Time.fixedDeltaTime * speed;
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
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("weapon hit");
        PlayerManager w = collider.gameObject.GetComponent<PlayerManager>();
        AI_ChasingEnemy enemy = collider.gameObject.GetComponent<AI_ChasingEnemy>();
        if (w != null && isAttacking && !(w.IsParrying()))
        {
            w.SetHP(w.GetHP() - enemy.GetScaling()*this.damage);
        }
        else if (enemy != null && isAttacking) { 
            enemy.SetHP(enemy.GetHP() - this.damage);
        }
    }

    public IEnumerator AttackTimer()
    {
        boxCollider.enabled = true;
        isAttacking = true;
        animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.2f);
        boxCollider.enabled = false;
        animator.SetBool("Attacking", false);
        isAttacking = false;
    }
}
