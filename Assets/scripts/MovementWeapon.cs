using UnityEngine;

public class MovementWeapon : Weapon
{
    [SerializeField]
    private float speed;
    [SerializeField]
    public float cooldown;
    public float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void Attack()
    {
        
    }
}
