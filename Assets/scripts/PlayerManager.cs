using Edgar.Unity.Examples;
using UnityEngine;

/// <summary>
/// Very simple implementation of a player that can interact with objects.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private IInteractable interactableInFocus;
    [SerializeField]
    private Weapon weapon;

    /// <summary>
    /// If an interactable object is in focus and is allowed to interact, call its Interact() method.
    /// </summary>
    public void Update()
    {
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
        if (InputHelper.GetKey(KeyCode.Mouse0))
        {
            Debug.Log("hiyah");
        }
    }

    public Weapon GetWeapon()
    {
        return this.weapon;
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }
}