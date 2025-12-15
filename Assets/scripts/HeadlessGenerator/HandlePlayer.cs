using UnityEngine;
using Edgar.Unity;

using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class HandlePlayer : DungeonGeneratorPostProcessingComponentGrid2D
{
    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        PlayerHandling(level);
    }

    public void PlayerHandling(DungeonGeneratorLevelGrid2D level)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerManager>().enabled = true;

        Camera.main.transform.SetParent(player.transform, false);

    }
}
