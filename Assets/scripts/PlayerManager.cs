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

    private Animator animator;
    private Vector2 movement;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer weaponRenderer;
    [SerializeField]
    private Weapon weapon;

    public void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
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
            if (weapon != null) {
                if (weapon is Sword) { weaponRenderer.flipX = !weaponRenderer.flipX; }
                else if (weapon is Spear) { weaponRenderer.flipY = !weaponRenderer.flipY; }
                
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

        Attack();
    }
    public void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + movement.normalized * MoveSpeed * Time.fixedDeltaTime);
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

    public void Attack()
    {
        if (InputHelper.GetKeyDown(KeyCode.Mouse0))
        {
            if (weapon != null)
            {
                weapon.gameObject.GetComponent<Weapon>().Attack();
            }
        }
    }

    public Weapon GetWeapon()
    {
        return this.weapon;
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        weaponRenderer = weapon.GetComponent<SpriteRenderer>();
    }
}