using Edgar.Unity;
using UnityEngine;

public class BasicCorridor : ACorridor
{
    private void OnEnable()
    {
        RoomTemplatesSet set = Resources.Load<RoomTemplatesSet>("Sets/Basic Corridors");
        RoomTemplateSets.Add(set);   
    }
}
