using Edgar.Unity;
using UnityEngine;

public class BossRoom : ARoom
{
    private void OnEnable()
    {
        RoomTemplatesSet set = Resources.Load<RoomTemplatesSet>("Sets/Boss rooms");
        RoomTemplateSets.Add(set);
    }
}
