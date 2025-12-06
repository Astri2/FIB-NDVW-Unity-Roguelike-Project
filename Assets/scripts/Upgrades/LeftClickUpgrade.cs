using Edgar.Unity.Examples;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeftClickUpgrade : Upgrade
{
    [SerializeField]
    private List<Weapon> WeaponItem;
    [SerializeField]
    private PlayerManager Player;

    public bool AlreadyCollected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
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
            if (Player.GetLeftWeapon() == null)
            {
                int index = UnityEngine.Random.Range(0, WeaponItem.Count);
                Weapon weapon = GameObject.Instantiate(WeaponItem[index]);
                if (index == 0)
                {
                    weapon.transform.position = Player.transform.position + new Vector3(0, -0.2f, 0);
                }
                else if (index == 1)
                {
                    weapon.transform.position = Player.transform.position + new Vector3(0, -0.35f, 0);
                }
                weapon.transform.SetParent(Player.transform);
                Player.SetLeftWeapon(weapon);
            }
            else
            {
                Player.GetLeftWeapon().Scale();
            }
            Destroy(this.gameObject);
        }
    }

    public override void EndInteract()
    {
        HideText();
    }
}
