using System.Collections;
using UnityEngine;

public class AreaHeal : Weapon
{
    [SerializeField]
    private BoxCollider2D boxCollider;
    public bool isAttacking;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        isAttacking = false;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime * speed;
        }
        //Debug.Log("weapon timer : " + timer);
    }

    public override bool Attack()
    {
        if (timer <= 0)
        {
            timer += cooldown;
            StartCoroutine(AttackTimer());
            return true;
        }
        return false;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy != null && isAttacking)
        {
            enemy.SetHP(enemy.GetHP() + this.damage);
            if (enemy.GetHP() > enemy.GetMaxHP()) enemy.SetHP(enemy.GetMaxHP());
        }
    }

    public IEnumerator AttackTimer()
    {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        isAttacking = false;
    }
}
