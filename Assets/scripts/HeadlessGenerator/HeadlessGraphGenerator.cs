using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public static class HeadlessGraphGenerator
{
    private static readonly System.Random rng = new System.Random();

    public static void GenerateDungeon(HeadlessGraph graph, int minPathLength, int maxPathLength, int numberOfFeatures, List<int> featureWeights, bool disableRight)
    {
        int mainPathLength = rng.Next(minPathLength, maxPathLength);

        HashSet<int> left = new HashSet<int>(), right = new HashSet<int>();

        // --- Create main rooms ---
        SpawnRoom spawnRoom = graph.CreateRoom<SpawnRoom>();
        BossRoom bossRoom = graph.CreateRoom<BossRoom>();

        List<ARoom> mainPath = new List<ARoom>();
        mainPath.Add(spawnRoom);

        // Create the linear main path
        // remove 1 before & after because spawn/boss rooms
        for (int i = 1; i < mainPathLength - 1; i++)
        {
            BasicRoom newRoom = graph.CreateRoom<BasicRoom>();
            newRoom.name = "Main: " + i.ToString();
            graph.CreateConnection<BasicCorridor>(mainPath[i - 1], newRoom);
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
            (roomNb => CreateLoop(graph, mainPath, roomNb, ref left, ref right), featureWeights[0]),
            (roomNb => CreateBranch(graph, mainPath, roomNb, ref left, ref right), featureWeights[1]),
            (roomNb => CreateDeadEnd(graph, mainPath, roomNb, ref left, ref right), featureWeights[2])
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
        graph.CreateConnection<BasicCorridor>(mainPath[mainPath.Count - 1], bossRoom);

        SetBudget(graph, spawnRoom, bossRoom);

        string dot = GraphVizExporter.ToDot(graph);
        File.WriteAllText("dungeon.dot", dot);
    }

    private static bool CreateDeadEnd(HeadlessGraph graph, List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            // Debug.Log("Failed early to generate dead end on node: " + rootNumber.ToString());
            return false;
        }

        // Debug.Log("Dead end on node: " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int branchSize = rng.Next(1, 6);

        ARoom current = origin;
        for (int i = 0; i < branchSize; i++)
        {
            BasicRoom room = graph.CreateRoom<BasicRoom>();

            // TODO: make this cleaner or remove. this is debug 
            if (i == 0) graph.CreateConnection<FancyCorridor>(current, room);
            else graph.CreateConnection<BasicCorridor>(current, room);
            current = room;
        }

        if (left.Contains(rootNumber)) right.Add(rootNumber);
        else left.Add(rootNumber);
        return true;
    }

    private static bool CreateBranch(HeadlessGraph graph, List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            //Debug.Log("Failed early to generate branch on node: " + rootNumber.ToString());
            return false;
        }

        //Debug.Log("Initiating branch from node : " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int branchSize = rng.Next(2, 6);

        ARoom current = origin;
        for (int i = 0; i < branchSize; i++)
        {
            BasicRoom room = graph.CreateRoom<BasicRoom>();

            // TODO: make this cleaner or remove. this is debug 
            if(i == 0) graph.CreateConnection<FancyCorridor>(current, room);
            else graph.CreateConnection<BasicCorridor>(current, room);
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
            graph.CreateConnection<FancyCorridor>(current, mainPath[closeNumber]);

            // mark the side as used for the whole branch
            for(int i = minID; i < maxID; i++)
            {
                if(useLeft) left.Add(i);
                else right.Add(i);
            }
            //Debug.Log("Branch from node : " + minID.ToString() + " to node " + maxID.ToString());
            return true;

        } while (tries++ < 5);

        //Debug.Log("Failed to close branch" + rootNumber.ToString());
        return true; // true because it still generated a dead end
    }

    private static bool CreateLoop(HeadlessGraph graph, List<ARoom> mainPath, int rootNumber, ref HashSet<int> left, ref HashSet<int> right)
    {
        // this cell is already taken on both sides
        if (left.Contains(rootNumber) && right.Contains(rootNumber))
        {
            //Debug.Log("Failed early to generate loop on node: " + rootNumber.ToString());
            return false;
        }

        //Debug.Log("Cycle on node : " + rootNumber.ToString());

        ARoom origin = mainPath[rootNumber];
        int loopSize = rng.Next(4, 6);

        // store the first room of the branch for future looping
        BasicRoom firstRoom = graph.CreateRoom<BasicRoom>();
        graph.CreateConnection<FancyCorridor>(origin, firstRoom);

        ARoom previous = firstRoom;
        for (int i = 0; i < loopSize-1; i++)
        {
            BasicRoom room = graph.CreateRoom<BasicRoom>();
            graph.CreateConnection<BasicCorridor>(previous, room);
            previous = room;
        }

        // close the loop
        graph.CreateConnection<BasicCorridor>(previous, firstRoom);

        if (left.Contains(rootNumber)) return right.Add(rootNumber);
        else left.Add(rootNumber);

        return true;

    }

    private static void SetBudget(HeadlessGraph graph, ARoom spawnRoom, ARoom bossRoom)
    {
        Queue<(ARoom, int)> toExplore = new Queue<(ARoom, int)>();
        HashSet<ARoom> visited = new HashSet<ARoom>();

        toExplore.Enqueue((spawnRoom, 0));
        visited.Add(spawnRoom);

        while (toExplore.Count != 0)
        {
            (ARoom,int) tuple = toExplore.Dequeue();
            ARoom room = tuple.Item1;
            int depth = tuple.Item2;
            
            room.distanceToSpawn = depth;
            room.enemiesBudget = 2 * depth;

            // +/- 10% difficulty
            double rngFactor = -0.2 + rng.NextDouble() * 0.4;
            room.enemiesBudget = (int)Math.Round(room.enemiesBudget * (1 + rngFactor));

            var neighbors =
                graph.Connections
                    .Where(connection => connection.From == room && !visited.Contains(connection.To))
                    .Select(connection => connection.To)
                .Union(graph.Connections
                    .Where(connection => connection.To == room && !visited.Contains(connection.From))
                    .Select(connection => connection.From)
                );

            foreach (ARoom neighbor in neighbors)
            {
                toExplore.Enqueue((neighbor, depth+1));
                visited.Add(neighbor);
            }
        }
    
        // burst of difficulty for boss room
        bossRoom.enemiesBudget *= 2;
    }
}
