using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public List<Chest> ChestList;
    public List<Enemy> Enemies;
    public int index = 0;

    //healthbar
    public RectTransform healthBar;
    public float width, height;

    //melee stamina bar
    public RectTransform BarMelee;
    public RectTransform CooldownBarMelee;
    public float widthMelee, heightMelee;

    //ranged stamina bar
    public RectTransform BarRanged;
    public RectTransform CooldownBarRanged;
    public float widthRanged, heightRanged;

    //space stamina bar
    public RectTransform BarSpace;
    public RectTransform CooldownBarSpace;
    public float widthSpace, heightSpace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EnemyScaling());
    }

    // Update is called once per frame
    void Update()
    {
        int c = 0;
        foreach (Chest chest in ChestList)
        {
            if (chest.AlreadyOpened)
            {
                c++;
            }
        }
        index = c % 3;
        foreach (Chest chest in ChestList)
        {
            chest.SetIndex(index);
        }
    }

    public IEnumerator EnemyScaling()
    {
        yield return null;
        /*
        for(;;)
        {
            foreach (Enemies enemy in Enemies) {
                enemy.SetHP(enemy.GetHP() * 1.2f);
                enemy.GetWeapon().Scale();
            }
            yield return new WaitForSeconds(30);
        }
        */
    }
}
