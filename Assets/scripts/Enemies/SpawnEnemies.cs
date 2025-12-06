using UnityEngine;
using Edgar.Unity;

using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class SpawnEnemies : DungeonGeneratorPostProcessingComponentGrid2D
{
    // List of enemies prefabs
    public List<Enemies> enemies;

    // used for gizmos
    private List<GameObject> debugEnemies = new List<GameObject>();
    private DungeonGeneratorLevelGrid2D lvl;
    private Vector3 dungeonCenteringShift;

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        HandleEnemies(level);
    }

    private void HandleEnemies(DungeonGeneratorLevelGrid2D level)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        lvl = level;
        // every child of the rootGameObject is moved to center the dungeon.
        dungeonCenteringShift = level.RootGameObject.transform.GetChild(0).transform.position;

        // Remove any existing enemies holder
        GameObject enemiesGO = level.RootGameObject.transform.Find("Enemies")?.gameObject;
        if (enemiesGO != null) Destroy(enemiesGO);

        // GO at the root that will store all the ennemies
        enemiesGO = new GameObject("Enemies");
        enemiesGO.transform.position = dungeonCenteringShift;
        enemiesGO.transform.SetParent(level.RootGameObject.transform);

        Tilemap[] tilemapsWithCollider = { level.GetSharedTilemaps()[1], level.GetSharedTilemaps()[2] };

        // Iterate through all the rooms
        foreach (RoomInstanceGrid2D roomInstance in level.RoomInstances)
        {
            if (roomInstance.IsCorridor) continue;

            // retrieve all spawnable points of the room
            List<Vector2Int> outlinePts = roomInstance.OutlinePolygon.GetOutlinePoints();
            List<Vector2Int> allPts = roomInstance.OutlinePolygon.GetAllPoints();
            List<Vector2Int> pts = allPts
                .Except(outlinePts) // remove outside points
                .Where(pts => PositionLibre(pts, tilemapsWithCollider)) // filter inside walls/collidables
                .ToList(); // List of all available spawn points

            // decide on enemies to spawn              
            int budgetRoom = ((ARoom)roomInstance.Room).enemiesBudget;

            // apply a factor depending on the room size (30 is the smallest room)
            budgetRoom = (int)Mathf.Round(budgetRoom * (pts.Count / 30.0f));

            List<GameObject> choosenEnemies = new List<GameObject>();
            int nbCandidates;
            while (true)
            {
                List<Enemies> enemyCandidates = (from e in enemies where e.cost <= budgetRoom select e).ToList();
                nbCandidates = enemyCandidates.Count;
                
                if (nbCandidates == 0) break;

                Enemies enemy = enemyCandidates[Random.Next(0, nbCandidates)];
                choosenEnemies.Add(enemy.gameObject);
                budgetRoom -= enemy.cost;
            }

            // decide on spawn points
            
            // random position
            HashSet<Vector2Int> spawnPoints = new HashSet<Vector2Int>();
            foreach (var enemy in choosenEnemies)
            {
                Vector2Int position;
                
                for (int attempts = 0 ; attempts < 5; attempts++)
                {
                    position = pts[Random.Next(0, pts.Count)];
                    
                    if (spawnPoints.Contains(position)) continue; // an ennemy is already here
                    
                    spawnPoints.Add(position);

                    Vector3 actualSpawnPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0) + dungeonCenteringShift;
                    GameObject debugEnemy = Instantiate(enemy, actualSpawnPos, Quaternion.identity, enemiesGO.transform);
                    var temp = debugEnemy.GetComponent<Enemies>();
                    temp.player = player.transform;
                    temp.roomPoints = pts;
                    debugEnemies.Add(debugEnemy);
                    break;
                }
            }
        }
    }

    // check if the position is free or not
    bool PositionLibre(Vector2Int position, Tilemap[] tilemapColliders)
    {
        foreach(Tilemap tilemap in tilemapColliders)
        {
            Vector3Int pInt = new Vector3Int(position.x, position.y, 0);
            TileBase t = tilemap.GetTile(pInt);
            if (t != null) return false;
        }
        return true;
    }

    public void OnDrawGizmos()
    {
        
        if (lvl == null) return;

        Tilemap[] tilemapsWithCollider = { lvl.GetSharedTilemaps()[1], lvl.GetSharedTilemaps()[2] };

        foreach (var roomInstance in lvl.RoomInstances)
        {
            
            List<Vector2Int> pts = roomInstance.OutlinePolygon.GetAllPoints();
            for(int i = 0; i<pts.Count; i++)
            {
                Vector3 pos = new Vector3(pts[i].x + 0.5f, pts[i].y + 0.5f, 0) + dungeonCenteringShift;
                Gizmos.color = PositionLibre(pts[i], tilemapsWithCollider) ? Color.green : Color.red;
                Gizmos.DrawSphere(pos, 0.1f);
            }

            Gizmos.color = Color.red;
            List<Vector2Int> outlinePts = roomInstance.OutlinePolygon.GetOutlinePoints();
            for (int i = 0; i < outlinePts.Count; i++)
            {
                Vector3 pos = new Vector3(outlinePts[i].x + 0.5f, outlinePts[i].y + 0.5f, 0) + dungeonCenteringShift;
                Gizmos.DrawSphere(pos, 0.1f);
            }

        }
        Gizmos.color = Color.blue;
        foreach (var enemy in debugEnemies)
        {
            if (enemy == null) continue;

            BoxCollider2D col = enemy.GetComponent<BoxCollider2D>();
            Vector2 sizeCol = col.size;
            //float radius = Mathf.Max(sizeCol.x, sizeCol.y)/2.0f;
            Vector3 offset = new Vector3(col.offset.x, col.offset.y, 0);
            //Gizmos.DrawWireSphere(enemy.transform.position, radius);
            Gizmos.DrawWireCube(enemy.transform.position + offset, sizeCol);
        }
        
            
    }
}
