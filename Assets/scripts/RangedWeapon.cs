using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField]
    private float cooldown;

    [SerializeField]
    private float startTime;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Transform shotPos;
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
        cooldown -= Time.fixedDeltaTime;
    }

    public override void Attack()
    {
        if (cooldown <= 0)
        {
            base.Attack();
            GameObject.Instantiate(projectile, shotPos.position, transform.rotation);
            cooldown = startTime;
        }
    }
}
