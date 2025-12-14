using Edgar.Unity.Examples;
using System.Collections.Generic;
using UnityEngine;

public class RightClickUpgrade : Upgrade
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
            if(Player.GetRightWeapon() == null)
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
                Player.SetRightWeapon(weapon);
            }
            else
            {
                if(Player.GetRightWeapon() is RangedWeapon) 
                {
                    //need explicit call of RangedWeapon Scale method for projectile scaling
                    ((RangedWeapon)Player.GetRightWeapon()).Scale();
                }
                else
                {
                    Player.GetRightWeapon().Scale();
                }
            }
            Player.SetHP(Player.GetHP() + Player.GetMaxHP() * 0.1f);
            if (Player.GetHP() > Player.GetMaxHP())
            {
                Player.SetHP(Player.GetMaxHP());
            }
            Destroy(this.gameObject);
        }
    }

    public override void EndInteract()
    {
        HideText();
    }
}
