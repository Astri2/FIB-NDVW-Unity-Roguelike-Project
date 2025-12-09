using System.Text;


// To export to png
// dot -Tpng dungeon.dot -o dungeon.png

public static class GraphVizExporter
{
    public static string ToDot(HeadlessGraph graph)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph Dungeon {");
        sb.AppendLine("    node [shape=circle, fontsize=12];");

        // --- Nodes ---
        foreach (var room in graph.Rooms)
        {
            string id = room.GetHashCode().ToString();
            string label =
                room is SpawnRoom ? "Spawn\n" :
                room is BossRoom ? "Boss\n" :
                room.name + "\n";
            label += "dist: " + ((ARoom)room).distanceToSpawn.ToString();
            label += "\nbudget: " + ((ARoom)room).enemiesBudget.ToString();

            string color =
                room is SpawnRoom ? "green" :
                room is BossRoom ? "red" :
                room.name != "" ? "orange" : // main path basic rooms (ugly check tbf)
                "black";

            sb.AppendLine($"    {id} [label=\"{label}\" color=\"{color}\"];");
        }

        // --- Edges ---
        foreach (var c in graph.Connections)
        {
            string a = c.From.GetHashCode().ToString();
            string b = c.To.GetHashCode().ToString();
            sb.AppendLine($"    {a} -- {b};");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}
