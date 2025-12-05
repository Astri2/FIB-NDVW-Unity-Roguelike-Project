using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Transform shotPos;
    [SerializeField]
    private BowProjectile projScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime * speed;
        }
    }

    public override void Attack()
    {
        if (timer <= 0)
        {
            timer += cooldown;
            base.Attack();
            GameObject.Instantiate(projectile, shotPos.position, transform.rotation);
        }
    }

    public void Scale()
    {
        projScript.SetDamage(projScript.GetDamage() * 1.15f);
        speed = 1.15f * speed;
    }
}
