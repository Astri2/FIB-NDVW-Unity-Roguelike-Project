using UnityEngine;

public class Sprint : MovementWeapon
{
    [SerializeField]
    private bool isSprinting = false;
    [SerializeField]
    private PlayerManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    public override void Attack()
    {
        base.Attack();
        isSprinting = !isSprinting;
        if (isSprinting)
        {
            playerManager.SetMoveSpeed(7.5f);
        }
        else
        {
            playerManager.SetMoveSpeed(5f);
        }
    }
}
