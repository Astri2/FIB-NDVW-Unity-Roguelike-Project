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

    public int minPathLength;
    public int maxPathLength;
    public int numberOfFeatures;
    public int LoopWeight;
    public int BranchWeight;
    public int DeadEndWeight;
    public bool disableRight;

    public new void Start()
    {
        // do nothing. I simply wanted to override the base one
    }

    public new void Awake()
    {
        headlessGraph = ScriptableObject.CreateInstance<HeadlessGraph>();
        this.FixedLevelGraphConfig.LevelGraph = headlessGraph;

        // set the default connection template set of the whole graph 
        // headlessGraph.CorridorRoomTemplateSets = new List<RoomTemplatesSet> { CorridorRoomTemplateSets };

        /*
        ARoom room1 = headlessGraph.CreateRoom<SpawnRoom>();
        room1.distanceToSpawn = 0;
        ARoom room2 = headlessGraph.CreateRoom<BasicRoom>();
        room2.distanceToSpawn = 1;
        ARoom room3 = headlessGraph.CreateRoom<BossRoom>();
        room3.distanceToSpawn = 2;
        
        ACorridor connection1 = headlessGraph.CreateConnection<BasicCorridor>(room1, room2);
        ACorridor connection2 = headlessGraph.CreateConnection<FancyCorridor>(room2, room3);
        */

        var weights = new List<int> { LoopWeight, BranchWeight, DeadEndWeight };
        
        HeadlessGraphGenerator gen = new HeadlessGraphGenerator(headlessGraph);
        gen.GenerateDungeon(minPathLength, maxPathLength, numberOfFeatures, weights, disableRight);
    }
}

