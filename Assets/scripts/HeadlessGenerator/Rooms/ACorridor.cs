using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using UnityEngine;

public abstract class ACorridor : Connection
{

    public List<GameObject> IndividualRoomTemplates = new List<GameObject>();
    public List<RoomTemplatesSet> RoomTemplateSets = new List<RoomTemplatesSet>();

    public override List<GameObject> GetRoomTemplates()
    {
        return IndividualRoomTemplates
                .Union(RoomTemplateSets
                    .Where(x => x != null)
                    .SelectMany(x => x.RoomTemplates)
                )
                .Distinct()
                .ToList();
    }
}
