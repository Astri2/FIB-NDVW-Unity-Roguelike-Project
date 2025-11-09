using UnityEngine;
using Edgar.Unity.Examples;


/// <summary>
/// Example implementation of a chest that is opened (sprite change) when the players interacts with it.
/// </summary>
public class Example2Chest : InteractableBase
{
    public bool AlreadyOpened;

    [SerializeField]
    private Upgrade upgrade;

    /// <summary>
    /// Make sure to not make it possible to interact with the chest when it is already opened.
    /// </summary>
    /// <returns></returns>
    public override bool IsInteractionAllowed()
    {
        return !AlreadyOpened;
    }

    public override void BeginInteract()
    {
        ShowText("Press E to open chest");
    }

    public override void Interact()
    {
        if (InputHelper.GetKey(KeyCode.E))
        {
            gameObject.transform.Find("Closed").gameObject.SetActive(false);
            gameObject.transform.Find("Open").gameObject.SetActive(true);

            //instanciate new upgrade
            Upgrade up = GameObject.Instantiate(upgrade);
            up.transform.position = this.transform.position + new Vector3(0,-0.6f,0);
            AlreadyOpened = true;
        }
    }

    public override void EndInteract()
    {
        HideText();
    }
}