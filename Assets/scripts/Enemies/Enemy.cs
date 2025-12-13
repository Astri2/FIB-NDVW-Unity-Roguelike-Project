using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    ///////////////////////////
    // Common to all enemies //
    ///////////////////////////
    [SerializeField] protected int cost;

    [SerializeField] protected float hp;

    [SerializeField] private float maxHp;

    [SerializeField] private List<Chest> chests = new();

    [SerializeField] protected List<Vector2Int> roomPoints;

    [SerializeField] protected Vector3 dungeonCenteringShift;

    [SerializeField] protected Transform player;

    [SerializeField] private Slider HealthBar;

    public int GetCost() { return cost; }
    public float GetHP() => hp;
    public void SetHP(float hp) {
        this.hp = hp;
        HealthBar.value = hp;
    }

    public float GetMaxHP() => maxHp;

    public void SetMaxHP(float val)
    {
        this.maxHp = val;
    }

    public void AddChest(Chest chest) => chests.Add(chest);

    public virtual void Start()
    {
        HealthBar = GetComponentInChildren<Slider>();
        HealthBar.maxValue = hp;
        HealthBar.value = hp;
    }

    protected List<Chest> GetChests() => chests;

    public void SetRoomPoints(List<Vector2Int> roomPoints) => this.roomPoints = roomPoints;

    public List<Vector2Int> GetRoomPoints() => roomPoints;

    public void SetDungeonCenteringShift(Vector3 dungeonCenteringShift) => this.dungeonCenteringShift = dungeonCenteringShift;

    public Vector3 GetDungeonCenteringShift() => dungeonCenteringShift;

    public void SetPlayerTransform(Transform player) => this.player = player;

    public Transform GetPlayerTransform() => player;
}