using Edgar.Unity.Examples;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Very simple implementation of a player that can interact with objects.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private IInteractable interactableInFocus;
    public float MoveSpeed = 5f;

    //general player stuff
    private Animator animator;
    private Vector2 movement;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    //health stuff
    [SerializeField]
    private float hp; 
    [SerializeField]
    private float maxHp;

    //weapons
    [SerializeField]
    private Weapon leftWeapon;
    private SpriteRenderer leftWeaponRenderer;
    private BoxCollider2D leftWeaponHitbox;
    [SerializeField]
    private Weapon rightWeapon;
    private SpriteRenderer rightWeaponRenderer;
    [SerializeField]
    private Weapon spaceWeapon;

    //defense
    [SerializeField]
    private bool isParrying = false;

    //health
    [SerializeField]
    private RectTransform healthBar;
    public float width, height;

    public int chestIndex = 0;

    public void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        maxHp = 20;
        hp = maxHp;
    }

    /// <summary>
    /// If an interactable object is in focus and is allowed to interact, call its Interact() method.
    /// </summary>
    public void Update()
    {
        movement.x = InputHelper.GetHorizontalAxis();
        movement.y = InputHelper.GetVerticalAxis();

#if UNITY_6000_0_OR_NEWER
        animator.SetBool("running", rigidbody.linearVelocity.magnitude > float.Epsilon);
#else
        animator.SetBool("running", rigidbody.velocity.magnitude > float.Epsilon);
#endif

        // Flip sprite if needed
        var flipSprite = spriteRenderer.flipX ? movement.x > 0.01f : movement.x < -0.01f;
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            if (leftWeapon != null) {
                leftWeaponRenderer.flipX = !leftWeaponRenderer.flipX;
                leftWeaponHitbox.offset = -leftWeaponHitbox.offset;

            }


        }

        if (interactableInFocus != null)
        {
            if (interactableInFocus.IsInteractionAllowed())
            {
                interactableInFocus.Interact();
            }
            else
            {
                interactableInFocus.EndInteract();
                interactableInFocus = null;
            }
        }

        if (InputHelper.GetKeyDown(KeyCode.Mouse0) && !isParrying)
        {
            LeftAttack();
        }
        if (InputHelper.GetKeyDown(KeyCode.Mouse1) && !isParrying)
        {
            RightAttack();
        }
        if (InputHelper.GetKeyDown(KeyCode.Space) && !isParrying)
        {
            SpaceAttack();
        }
        if (hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void FixedUpdate()
    {
        this.Move(rigidbody.position + movement.normalized * MoveSpeed * Time.fixedDeltaTime);
    }

    public void SetMoveSpeed(float val)
    {
        MoveSpeed = val;
    }

    /// <summary>
    /// If the collision is with an interactable object that is allowed to interact,
    /// make this object the current focus of the player.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        var interactable = collider.GetComponent<IInteractable>();

        if (interactable == null || !interactable.IsInteractionAllowed())
        {
            return;
        }

        interactableInFocus?.EndInteract();
        interactableInFocus = interactable;
        interactableInFocus.BeginInteract();
    }

    /// <summary>
    /// If the collision is with the interactable object that is currently the focus
    /// of the player, make the focus null.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerExit2D(Collider2D collider)
    {
        var interactable = collider.GetComponent<IInteractable>();

        if (interactable == interactableInFocus)
        {
            interactableInFocus?.EndInteract();
            interactableInFocus = null;
        }
    }

    public void Move(Vector2 v)
    {
        rigidbody.MovePosition(v);
    }

    public Rigidbody2D GetRigidbody()
    {
        return rigidbody;
    }

    public SpriteRenderer GetRenderer()
    {
        return spriteRenderer;
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public void SetMovement(Vector2 m)
    {
        this.movement = m;
    }

    public float GetHorizontalMovement()
    {
        return InputHelper.GetHorizontalAxis();
    }

    public float GetVerticalMovement()
    {
        return InputHelper.GetVerticalAxis();
    }

    public void LeftAttack()
    {
        if (leftWeapon != null)
        {
            leftWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public void RightAttack()
    {
        if (rightWeapon != null)
        {
            rightWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public void SpaceAttack()
    {
        if(spaceWeapon != null)
        {
            spaceWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public Weapon GetLeftWeapon()
    {
        return this.leftWeapon;
    }

    public Weapon GetRightWeapon() 
    { 
        return this.rightWeapon; 
    }
    public Weapon GetSpaceWeapon()
    {
        return this.spaceWeapon;
    }

    public void SetLeftWeapon(Weapon weapon)
    {
        this.leftWeapon = weapon;
        leftWeaponRenderer = weapon.GetComponent<SpriteRenderer>();
        leftWeaponRenderer.flipX = spriteRenderer.flipX;
        leftWeaponHitbox = weapon.GetComponent<BoxCollider2D>();
    }

    public void SetRightWeapon(Weapon weapon)
    {
        this.rightWeapon = weapon;
        rightWeaponRenderer = weapon.GetComponentInChildren<SpriteRenderer>();
        rightWeaponRenderer.flipX = spriteRenderer.flipX;
    }

    public void SetSpaceWeapon(Weapon weapon)
    {
        this.spaceWeapon = weapon;
    }

    public float GetHP()
    {
        return hp;
    }

    public float GetMaxHP()
    {
        return maxHp;
    }

    public void SetHP(float hp)
    {
        this.hp = hp;
        if(hp > maxHp)
        {
            hp = maxHp;
        }
        float newWidth = (hp/maxHp)*width;
        healthBar.sizeDelta = new Vector2(newWidth, height);
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public void SetParrying(bool val)
    {
        this.isParrying = val;
    }
}