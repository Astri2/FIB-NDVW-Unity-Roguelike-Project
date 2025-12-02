using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class HeadlessGraphGenerator
{
    private readonly HeadlessGraph headlessGraph;
    private readonly System.Random rng = new System.Random();

    public HeadlessGraphGenerator(HeadlessGraph graph)
    {
        headlessGraph = graph;
    }

    public void GenerateDungeon(int minPathLength, int maxPathLength, int numberOfFeatures, List<int> featureWeights, bool disableRight)
    {
        int mainPathLength = rng.Next(minPathLength, maxPathLength);

        HashSet<int> left = new HashSet<int>(), right = new HashSet<int>();

        // --- Create main rooms ---
        SpawnRoom spawnRoom = headlessGraph.CreateRoom<SpawnRoom>();
        BossRoom bossRoom = headlessGraph.CreateRoom<BossRoom>();

        List<ARoom> mainPath = new List<ARoom>();
        mainPath.Add(spawnRoom);

        // Create the linear main path
        // remove 1 before & after because spawn/boss rooms
        for (int i = 1; i < mainPathLength - 1; i++)
        {
            BasicRoom newRoom = headlessGraph.CreateRoom<BasicRoom>();
            newRoom.name = "Main: " + i.ToString();
            headlessGraph.CreateConnection<BasicCorridor>(mainPath[i - 1], newRoom);
            mainPath.Add(newRoom);

        }

        if(disableRight)
        {
            // fill right to make it impossible to use
            for(int i = 0; i < mainPath.Count; i++)
            {
                right.Add(i);
            }
        }


        var featureEntries = new List<(Func<int, bool>, float)>
        {
            (roomNb => CreateLoop(mainPath, roomNb, ref left, ref right), featureWeights[0]),
            (roomNb => CreateBranch(mainPath, roomNb, ref left, ref right), featureWeights[1]),
            (roomNb => CreateDeadEnd(mainPath, roomNb, ref left, ref right), featureWeights[2])
        };

        for (int i = 0; i < numberOfFeatures; i++)
        {
            // First, choose the feature type
            var featureEntry = WeightedRandom.Choose(featureEntries);


            for (int _ = 0; _ < 5; _++)
            {
                int roomNumber = rng.Next(mainPath.Count);
                bool success = featureEntry(roomNumber);
                if (success) break;
            }
        }

        // Final connection to boss
        headlessGraph.CreateConnection<BasicCorridor>(mainPath[mainPath.Count - 1], bossRoom);

        string dot = GraphVizExporter.ToDot(headlessGraph);
        File.WriteAllText("dungeon.dot", dot);
    }

    private bool CreateDeadEnd(List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            Debug.Log("Failed early to generate dead end on node: " + rootNumber.ToString());
            return false;
        }

        Debug.Log("Dead end on node: " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int branchSize = rng.Next(1, 6);

        ARoom current = origin;
        for (int i = 0; i < branchSize; i++)
        {
            BasicRoom room = headlessGraph.CreateRoom<BasicRoom>();

            // TODO: make this cleaner or remove. this is debug 
            if (i == 0) headlessGraph.CreateConnection<FancyCorridor>(current, room);
            else headlessGraph.CreateConnection<BasicCorridor>(current, room);
            current = room;
        }

        if (left.Contains(rootNumber)) right.Add(rootNumber);
        else left.Add(rootNumber);
        return true;
    }

    private bool CreateBranch(List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            Debug.Log("Failed early to generate branch on node: " + rootNumber.ToString());
            return false;
        }

        Debug.Log("Initiating branch from node : " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int branchSize = rng.Next(2, 6);

        ARoom current = origin;
        for (int i = 0; i < branchSize; i++)
        {
            BasicRoom room = headlessGraph.CreateRoom<BasicRoom>();

            // TODO: make this cleaner or remove. this is debug 
            if(i == 0) headlessGraph.CreateConnection<FancyCorridor>(current, room);
            else headlessGraph.CreateConnection<BasicCorridor>(current, room);
            current = room;
        }

        int tries = 0;
        do
        {
            int closeNumber = rng.Next(mainPath.Count);
            if(closeNumber == rootNumber)
            {
                continue;
            }

            int minID = Math.Min(rootNumber, closeNumber);
            int maxID = Math.Max(rootNumber, closeNumber);

            bool useLeft;
            // if the left side is free between min and max, you can put the branch here
            if (!left.Any(x => x >= minID && x <= maxID))
            {
                useLeft = true;
            }
            // otherwise, try the right brancg
            else if(!right.Any(x => x >= minID && x <= maxID))
            {
                useLeft = false;
                
            } 
            // otherwise consider as invalid
            else continue;

            // This is a valid node to reattach the node
            headlessGraph.CreateConnection<FancyCorridor>(current, mainPath[closeNumber]);

            // mark the side as used for the whole branch
            for(int i = minID; i < maxID; i++)
            {
                if(useLeft) left.Add(i);
                else right.Add(i);
            }
            Debug.Log("Branch from node : " + minID.ToString() + " to node " + maxID.ToString());
            return true;

        } while (tries++ < 5);

        Debug.Log("Failed to close branch" + rootNumber.ToString());
        return true; // true because it still generated a dead end
    }

    private bool CreateLoop(List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            Debug.Log("Failed early to generate loop on node: " + rootNumber.ToString());
            return false;
        }

        Debug.Log("Cycle on node : " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int loopSize = rng.Next(4, 6);

        // store the first room of the branch for future looping
        BasicRoom firstRoom = headlessGraph.CreateRoom<BasicRoom>();
        headlessGraph.CreateConnection<FancyCorridor>(origin, firstRoom);

        ARoom previous = firstRoom;
        for (int i = 0; i < loopSize-1; i++)
        {
            BasicRoom room = headlessGraph.CreateRoom<BasicRoom>();
            headlessGraph.CreateConnection<BasicCorridor>(previous, room);
            previous = room;
        }

        // close the loop
        headlessGraph.CreateConnection<BasicCorridor>(previous, firstRoom);

        if (left.Contains(rootNumber)) return right.Add(rootNumber);
        else left.Add(rootNumber);

        return true;

    }
}
