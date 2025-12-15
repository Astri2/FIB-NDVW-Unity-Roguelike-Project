using System.Collections.Generic;
using Edgar.Unity;
using UnityEngine;

public class HeadlessDungeonGeneratorGrid2D: DungeonGeneratorBaseGrid2D
{
    private HeadlessGraph headlessGraph;

    public int minPathLength;
    public int maxPathLength;
    public int numberOfFeatures;
    public int LoopWeight;
    public int BranchWeight;
    public int DeadEndWeight;
    public bool disableRight;

    // Override Start & Awake behavior
    public new void Start() { }
    public new void Awake() { }

    public void GenerateGraph()
    {
        // create the graph object
        if(headlessGraph != null) ScriptableObject.Destroy(headlessGraph);
        headlessGraph = ScriptableObject.CreateInstance<HeadlessGraph>();
        this.FixedLevelGraphConfig.LevelGraph = headlessGraph;

        // update weights
        var weights = new List<int> { LoopWeight, BranchWeight, DeadEndWeight };

        // generate the graph
        HeadlessGraphGenerator.GenerateDungeon(headlessGraph, minPathLength, maxPathLength, numberOfFeatures, weights, disableRight);
    }
}

