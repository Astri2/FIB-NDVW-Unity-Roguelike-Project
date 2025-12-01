using Edgar.Unity.Examples;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBarUpgrade : Upgrade
{
    [SerializeField]
    private List<Weapon> WeaponItem;
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
            bool cont = true;
            int index = 0;
            while (cont)
            {
                cont = false;
                index = UnityEngine.Random.Range(0, WeaponItem.Count);
                //disallow instantiation of the same type of upgrade for both right click and spacebar
                if (Player.GetRightWeapon() != null && Player.GetRightWeapon().GetType() == WeaponItem[index].GetType())
                {
                    cont = true;
                }
            }
            
            Weapon weapon = GameObject.Instantiate(WeaponItem[index]);
            weapon.transform.SetParent(Player.transform);
            Player.SetSpaceWeapon(weapon);
            Destroy(this.gameObject);
        }
    }

    public override void EndInteract()
    {
        HideText();
    }
}
