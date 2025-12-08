using Edgar.Unity;
using UnityEngine;

public class BasicRoom : ARoom
{
    public BasicRoom()
    {
    }

    private void OnEnable()
    {
        RoomTemplatesSet set = Resources.Load<RoomTemplatesSet>("Sets/Basic rooms");
        RoomTemplateSets.Add(set);
    }
}
