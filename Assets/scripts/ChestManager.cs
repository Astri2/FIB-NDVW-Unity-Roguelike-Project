using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public List<Chest> ChestList;
    public int index = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
