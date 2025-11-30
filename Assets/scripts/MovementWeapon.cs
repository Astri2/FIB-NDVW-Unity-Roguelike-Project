using UnityEngine;

public class MovementWeapon : Weapon
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float cooldown;
    public float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
    }

    public override void Attack()
    {
        if(timer <= 0)
        {
            timer += cooldown;
            base.Attack();
            Debug.Log("movement");
        }
    }
}
