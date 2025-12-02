using System.Text;

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
                room is SpawnRoom ? "Spawn" :
                room is BossRoom ? "Boss" :
                room.name;

            sb.AppendLine($"    {id} [label=\"{label}\"];");
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
