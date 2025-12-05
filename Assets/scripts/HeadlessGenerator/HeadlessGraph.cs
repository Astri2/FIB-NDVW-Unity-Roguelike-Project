using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edgar.Unity;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadlessGraph", menuName = "NDVW/Headless graph")]
public class HeadlessGraph: LevelGraph
{    public T CreateRoom<T>() where T : ARoom
    {
        T room = (T)CreateInstance(typeof(T).FullName);

        // Add room to the level graph
        this.Rooms.Add(room);
        // AssetDatabase.AddObjectToAsset(room, this);

        return room;
    }

    public T CreateConnection<T>(RoomBase from, RoomBase to) where T : ACorridor
    {
        T connection = (T)CreateInstance(typeof(T).FullName);

        // here we could most likely directly use Room instead of RoomControl
        connection.From = from;
        connection.To = to;

        this.Connections.Add(connection);
        // AssetDatabase.AddObjectToAsset(connection, this);

        return connection;
    }

    public new string RoomType = typeof(Room).FullName;

    public new string ConnectionType = typeof(Connection).FullName;
}
