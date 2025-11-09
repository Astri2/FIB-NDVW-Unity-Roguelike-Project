using Edgar.Unity.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class LeftClickUpgrade : Upgrade
{
    [SerializeField]
    private Weapon WeaponItem;
    [SerializeField]
    private PlayerManager Player;

    public bool AlreadyCollected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override bool IsInteractionAllowed()
    {
        return !AlreadyCollected;
    }

    public override void BeginInteract()
    {
        ShowText("Press E to equip weapon");
    }

    public override void Interact()
    {
        if (InputHelper.GetKey(KeyCode.E) && !AlreadyCollected)
        {
            //instanciate new upgrade
            AlreadyCollected = true;
            Weapon weapon = GameObject.Instantiate(WeaponItem);
            weapon.transform.position = Player.transform.position + new Vector3(0, -0.2f,0);
            weapon.transform.SetParent(Player.transform);
            Player.SetWeapon(weapon);
            Destroy(this.gameObject);
        }
    }

    public override void EndInteract()
    {
        HideText();
    }
}
