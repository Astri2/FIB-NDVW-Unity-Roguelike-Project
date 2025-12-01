using UnityEngine;

public class DefensiveWeapon : Weapon
{
    [SerializeField]
    public float cooldown;
    public float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        base.Attack();
    }
}
