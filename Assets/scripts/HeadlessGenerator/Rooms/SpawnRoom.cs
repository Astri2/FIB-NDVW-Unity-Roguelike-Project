using Edgar.Unity;
using UnityEngine;

public class SpawnRoom : ARoom
{
    private void OnEnable()
    {
        RoomTemplatesSet set = Resources.Load<RoomTemplatesSet>("Sets/Spawn rooms");
        RoomTemplateSets.Add(set);
    }
}
