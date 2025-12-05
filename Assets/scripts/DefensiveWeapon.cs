using UnityEngine;

public class DefensiveWeapon : Weapon
{
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
        }
    }
}
