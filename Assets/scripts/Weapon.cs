using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float speed;
    [SerializeField]
    public float cooldown;
    public float timer = 0;

    public virtual void Attack()
    {
    }

    public float GetCooldown()
    {
        return cooldown - timer;
    }

    public float GetTheoreticalCooldown()
    {
        return cooldown;
    }
    public void Scale()
    {
        damage = 1.15f * damage;
        speed = 1.15f * speed;
    }
}
