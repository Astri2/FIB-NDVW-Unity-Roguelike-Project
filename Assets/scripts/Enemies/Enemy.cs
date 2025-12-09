using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    ///////////////////////////
    // Common to all enemies //
    ///////////////////////////
    [SerializeField] protected int cost;

    [SerializeField] protected float hp;

    [SerializeField] protected List<Chest> chests = new();

    [SerializeField] protected List<Vector2Int> roomPoints;

    [SerializeField] protected Vector3 dungeonCenteringShift;

    [SerializeField] protected Transform player;

    public int GetCost() => cost;
    public float GetHP() => hp;
    public void SetHP(float hp) => this.hp = hp;
    public void AddChest(Chest chest) => chests.Add(chest);

    protected List<Chest> GetChests() => chests;

    public void SetRoomPoints(List<Vector2Int> roomPoints) => this.roomPoints = roomPoints;

    public List<Vector2Int> GetRoomPoints() => roomPoints;

    public void SetDungeonCenteringShift(Vector3 dungeonCenteringShift) => this.dungeonCenteringShift = dungeonCenteringShift;

    public Vector3 GetDungeonCenteringShift() => dungeonCenteringShift;

    public void SetPlayerTransform(Transform player) => this.player = player;

    public Transform GetPlayerTransform() => player;
}