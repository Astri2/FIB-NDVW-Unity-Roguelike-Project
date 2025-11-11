using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edgar.Unity;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadlessGraph", menuName = "NDVW/Headless graph")]
class HeadlessGraph: LevelGraph
{
    private static Type FindType(string fullName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
         .Where(a => !a.IsDynamic)
         .SelectMany(a => a.GetTypes())
         .FirstOrDefault(t => t.FullName.Equals(fullName));
    }

    public RoomBase CreateRoom()
    {
        var type = FindType(this.RoomType);
        var roomType = type != null ? this.RoomType : typeof(Room).FullName;
        var room = (RoomBase)CreateInstance(roomType);

        // Add room to the level graph
        this.Rooms.Add(room);
        // AssetDatabase.AddObjectToAsset(room, this);

        return room;
    }

    public ConnectionBase CreateConnection(RoomBase from, RoomBase to)
    {
        /// most important lines:
        var type = FindType(this.RoomType);
        var connectionType = type != null ? this.ConnectionType : typeof(Connection).FullName;
        var connection = (ConnectionBase)CreateInstance(connectionType);

        // here we could most likely directly use Room instead of RoomControl
        connection.From = from;
        connection.To = to;

        this.Connections.Add(connection);
        // AssetDatabase.AddObjectToAsset(connection, this);

        return connection;
    }
}
