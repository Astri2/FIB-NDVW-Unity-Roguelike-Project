using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

class HeadlessDungeonGeneratorGrid2D: DungeonGeneratorBaseGrid2D
{
    private HeadlessGraph headlessGraph;

    public RoomTemplatesSet DefaultRoomsSet;
    public GameObject BossRoom; // for testing purpose only. We should use sets everywhere

    public RoomTemplatesSet CorridorRoomTemplateSets;


    new void Start()
    {
        // do nothing. I simply wanted to override the base one
    }

    new void Awake()
    {
        headlessGraph = new HeadlessGraph();
        this.FixedLevelGraphConfig.LevelGraph = headlessGraph;

        // set the default individual room template of the whole graph
        // headlessGraph.DefaultIndividualRoomTemplates = new List<GameObject> { BossRoom };
        // set the default room templates set of the whole graph
        headlessGraph.DefaultRoomTemplateSets = new List<RoomTemplatesSet> { DefaultRoomsSet };

        // set the default connection template set of the whole graph 
        headlessGraph.CorridorRoomTemplateSets = new List<RoomTemplatesSet> { CorridorRoomTemplateSets };

        Room room1 = (Room)headlessGraph.CreateRoom();
        Room room2 = (Room)headlessGraph.CreateRoom();
        Room room3 = (Room)headlessGraph.CreateRoom();

        // set the default individual room template of room1
        room2.IndividualRoomTemplates = new List<GameObject> { BossRoom };
        // set the default room templates set of room2
        //room2.RoomTemplateSets = new List<RoomTemplatesSet> { DefaultRoomsSet };
        
        ConnectionBase connection1 = headlessGraph.CreateConnection(room1, room2);
        ConnectionBase connection2 = headlessGraph.CreateConnection(room2, room3);
    }
}

