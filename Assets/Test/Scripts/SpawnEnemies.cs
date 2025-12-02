using UnityEngine;
using Edgar.Unity;
using UnityEngine.UIElements;

using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;
using System.Linq;
using Unity.VisualScripting;

public class SpawnEnemies : DungeonGeneratorPostProcessingComponentGrid2D
{
    

    public GameObject testPos;
    public List<Enemies> enemies;
    // private List<Vector2Int> positions = new List<Vector2Int>();
    private List<GameObject> debugEnemies = new List<GameObject>();

    private DungeonGeneratorLevelGrid2D lvl;

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        HandleEnemies(level);
    }

    private void HandleEnemies(DungeonGeneratorLevelGrid2D level)
    {
        GameObject ennemisGO = new GameObject("Ennemis");
        lvl = level;
        
        int budget = 2;
        // Iterate through all the rooms
        foreach (var roomInstance in level.RoomInstances)
        {

            if (roomInstance.IsCorridor) continue;

            // debug
            /*for(int i = 0; i< roomInstance.OutlinePolygon.GetCornerPoints().Count; i++)
            {
                Object.Instantiate(testPos, new Vector3(roomInstance.OutlinePolygon.GetCornerPoints()[i][0], roomInstance.OutlinePolygon.GetCornerPoints()[i][1], 0), Quaternion.identity);
            
            }
            Debug.Log(level.RootGameObject.transform.position);*/

            int budgetRoom = budget;
            List<GameObject> choosenEnemies = new List<GameObject>();
            int nbCandidates;
            while (true)
            {
                List<Enemies> enemyCandidates = (from e in enemies where e.cost <= budgetRoom select e).ToList();
                nbCandidates = enemyCandidates.Count;
                
                if (nbCandidates == 0) break;

                Enemies enemy = enemyCandidates[Random.Next(0, nbCandidates)];
                choosenEnemies.Add(enemy.GameObject());
                budgetRoom -= enemy.cost;
            }



            // random position
            List<Vector2Int> outlinePts = roomInstance.OutlinePolygon.GetOutlinePoints();
            List<Vector2Int> allPts = roomInstance.OutlinePolygon.GetAllPoints();
            List<Vector2Int> pts = allPts.Except(outlinePts).ToList();

            //positions.Clear();
           
            foreach (var enemy in choosenEnemies)
            {

                /*Vector2Int position;
                int attempts = 0;
                do
                {
                    position = pts[Random.Next(0, pts.Count)];
                    attempts++;
                    if (attempts == 5) break;
                } while (!FreePos(position));

                positions.Add(position);
                

                if(attempts < 5)
                {
                    Object.Instantiate(enemy, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity, ennemisGO.transform);
                }
*/
                Vector2 position;
                
                for (int attempts = 0 ; attempts < 5; attempts++)
                {
                    position = pts[Random.Next(0, pts.Count)];
                    
                    if (PositionLibre(position, enemy))
                    {
                       
                        GameObject debugEnemy = Object.Instantiate(enemy, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity, ennemisGO.transform);
                        debugEnemies.Add(debugEnemy);
                        break;
                    }
                }
               

            }


            budget +=2;
        }
    }

    // check if the position is free or not
    /*private bool FreePos(Vector2Int position)
    {
        foreach (var pos in positions)
        {
            if ((pos - position).magnitude < 2) return false;
        }
        return true;

    }*/
    
    bool PositionLibre(Vector2 position, GameObject enemy)
    {
        BoxCollider2D col = enemy.GetComponent<BoxCollider2D>();
        Vector2 sizeCol = col.size;
        //float radius = Mathf.Max(sizeCol.x, sizeCol.y)/2.0f;
        Vector2 offset = col.offset;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position + offset, sizeCol, 0, LayerMask.GetMask("Default"));
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Default"));
        
        int nbColliders = colliders.Length;
        Debug.Log("nbColliders = " + nbColliders);
        
        foreach (var c in colliders)
        {
            Debug.Log("collider name = " + c.name);
        }
        
       
        return nbColliders == 0;
    }

    private void OnDrawGizmos()
    {
        
        if (lvl == null) return;
        foreach (var roomInstance in lvl.RoomInstances)
        {
            Gizmos.color = Color.green;
            List<Vector2Int> pts = roomInstance.OutlinePolygon.GetAllPoints();
            for(int i = 0; i<pts.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(pts[i].x+0.5f, pts[i].y+0.5f, 0), 0.1f);
            }

            Gizmos.color = Color.red;
            List<Vector2Int> outlinePts = roomInstance.OutlinePolygon.GetOutlinePoints();
            for (int i = 0; i < outlinePts.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(outlinePts[i].x+0.5f, outlinePts[i].y+ 0.5f, 0), 0.1f);
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
