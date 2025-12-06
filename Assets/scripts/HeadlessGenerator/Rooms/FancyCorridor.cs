using Edgar.Unity;
using UnityEngine;

public class FancyCorridor : ACorridor
{
    private void OnEnable()
    {
        RoomTemplatesSet set = Resources.Load<RoomTemplatesSet>("Sets/Fancy Corridors");
        RoomTemplateSets.Add(set);   
    }
}
