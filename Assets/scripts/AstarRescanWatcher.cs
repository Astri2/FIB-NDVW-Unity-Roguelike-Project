using System.Collections;
using UnityEngine;
using Pathfinding;
using Edgar.Unity;
using UnityEngine.Tilemaps;

/// <summary>
/// Watches the scene for the presence (or replacement) of a GameObject named "Generated Level"
/// and triggers an A* scan when it's created/changed. This avoids modifying Edgar's code.
/// Attach this component to any persistent GameObject (e.g. GameManager).
/// </summary>
public class AstarRescanWatcher : DungeonGeneratorPostProcessingComponentGrid2D
{

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        
        StartCoroutine(WaitOneFrame(level));
    }

    IEnumerator WaitOneFrame(DungeonGeneratorLevelGrid2D level)
    {
        // This skips one frame to allow the dungeon to finish generating
        yield return null;

        ScanAstar(level);
    }
    private void ScanAstar(DungeonGeneratorLevelGrid2D level)
    {
        if (AstarPath.active == null)
        {
            Debug.LogWarning("A* object (AstarPath) not found in scene — cannot scan.");
            return;
        }


        // 1 is walls, 2 is collidable, It shoud be used as well for detecting navigable zones
        level.GetSharedTilemaps()[1].gameObject.layer = LayerMask.NameToLayer("Walls");

        Debug.Log("Detected Generated Level -> rescanning A* pathfinding");
        AstarPath.active.Scan();
    }

}
