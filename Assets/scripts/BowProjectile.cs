using UnityEngine;

public class BowProjectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;
    public float distance;
    public LayerMask layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyProjectile", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, distance, layerMask);
        if(hit.collider != null)
        {
            if (hit.collider.GetType() == typeof(CapsuleCollider2D) && hit.collider.CompareTag("Enemy"))
            {
                AI_ChasingEnemy en = hit.collider.GetComponent<AI_ChasingEnemy>();
                en.SetHP(en.GetHP() - this.damage);
                DestroyProjectile();
            }
        }

        this.transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float val)
    {
        damage = val;
    }
}
