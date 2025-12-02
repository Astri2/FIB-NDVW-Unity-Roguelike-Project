using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public List<Chest> ChestList;
    public List<AI_ChasingEnemy> Enemies;
    public int index = 0;
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
        for(;;)
        {
            foreach (AI_ChasingEnemy enemy in Enemies) {
                enemy.SetScaling(enemy.GetHP() * 1.2f);
                enemy.SetScaling(enemy.GetScaling()*1.2f);
            }
            yield return new WaitForSeconds(3);
        }
    }
}
