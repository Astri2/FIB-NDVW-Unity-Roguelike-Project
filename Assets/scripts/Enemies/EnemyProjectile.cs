using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;
    public float distance;
    public LayerMask layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Invoke("DestroyProjectile", lifetime);
    }

    // Update is called once per frame
    public void Update()
    {
        this.transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager en = collision.gameObject.GetComponent<PlayerManager>();
            if(!en.IsParrying())en.SetHP(en.GetHP() - this.damage);
            DestroyProjectile();
        }
        else if (collision.gameObject.layer == 8)
        {
            DestroyProjectile();
        }
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
