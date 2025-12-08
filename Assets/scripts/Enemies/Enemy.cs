using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    ///////////////////////////
    // Common to all enemies //
    ///////////////////////////
    [SerializeField] private int cost;

    [SerializeField] private float hp;

    [SerializeField] private List<Chest> chests = new();

    [SerializeField] private List<Vector2Int> roomPoints;

    [SerializeField] private Transform player;

    public int GetCost() => cost;
    public float GetHP() => hp;
    public void SetHP(float hp)
    {
        this.hp = hp;
        if (hp <= 0) death();
    }

    public void AddChest(Chest chest) => chests.Add(chest);

    public void SetRoomPoints(List<Vector2Int> roomPoints) => this.roomPoints = roomPoints;

    public List<Vector2Int> GetRoomPoints() => roomPoints;

    public void SetPlayerTransform(Transform player) => this.player = player;

    public Transform GetPlayerTransform() => player;

    public void death()
    {
        // allert the linked chests that the enemy is dead
        foreach (Chest chest in chests)
        {
            chest.RemoveEnemy(this);
        }
        Destroy(this.gameObject);
    }

}