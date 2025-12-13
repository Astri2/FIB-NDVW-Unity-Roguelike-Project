using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Transform shotPos;
    [SerializeField]
    private BowProjectile projScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime * speed;
        }
    }

    public override bool Attack()
    {
        if (timer <= 0)
        {
            timer += cooldown;
            GameObject.Instantiate(projectile, shotPos.position, transform.rotation);
            return true;
        }
        return false;
    }

    public new void Scale()
    {
        projScript.SetDamage(projScript.GetDamage() * 1.15f);
        speed = 1.15f * speed;
    }
}
