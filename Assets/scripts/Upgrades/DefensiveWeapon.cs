using UnityEngine;

public class DefensiveWeapon : Weapon
{
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
            return true;
        }
        return false;
    }
}
