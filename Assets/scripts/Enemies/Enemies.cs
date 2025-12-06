using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{

    public int cost;

    public List<Vector2Int> roomPoints;

    [Header("References")]
    public Transform player; // Assign the player's Transform in the Inspector

    [Header("Settings")]
    public float speed = 3f; // Movement speed

    [SerializeField]
    private Weapon weapon;

    [SerializeField]
    private float hp;

    // This bool controls whether the enemy is allowed to chase or not
    private bool canChase = false;

    public float scaling;

    public Rigidbody2D rb;

    [SerializeField]
    private List<Chest> chests = new();

    public void Start()
    {
        scaling = 1.0f;
    }

    public void Update()
    {
        rb.linearVelocity = new Vector3(0, 0, 0);
        // Only try to move if the 'canChase' switch is ON
        // and we have a valid player reference
        if (canChase && player != null)
        {
            // --- This is your original movement code ---
            // It moves in a straight line towards the player,
            // which works inside a single room with no obstacles.

            // Calculate direction to the player
            Vector2 dir = player.position - transform.position;
            Vector2 direction = dir.normalized;

            // Move the enemy
            if (dir.magnitude > 1.0)
            {
                rb.position = Vector2.MoveTowards(
                    rb.position,
                    player.position,
                    speed * Time.deltaTime
                );
            }
            else
            {
                weapon.Attack();
            }


            // --- This is your original rotation code ---
            // (I've uncommented it and fixed it slightly)
            if (direction != Vector2.zero)
            {
                // Flip the sprite based on direction (basic)
                if (direction.x > 0.01f)
                {
                    transform.localScale = new Vector3(1, 1, 1); // Facing right
                }
                else if (direction.x < -0.01f)
                {
                    transform.localScale = new Vector3(-1, 1, 1); // Facing left
                }
            }
            // ------------------------------------------
        }

        if (hp <= 0) { this.death(); }
    }

    /// <summary>
    /// This function is called by Unity when a Rigidbody2D ENTERS our trigger.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ENTERED the trigger zone!");
            // Turn the 'canChase' switch ON
            canChase = true;
        }
    }

    /// <summary>
    /// This function is called by Unity when a Rigidbody2D EXITS our trigger.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object that left is the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player LEFT the trigger zone!");
            // Turn the 'canChase' switch OFF
            canChase = false;
        }
    }

    public Weapon GetWeapon() { return weapon; }

    public float GetHP()
    {
        return hp;
    }

    public void SetHP(float value)
    {
        hp = value;
    }

    public float GetScaling()
    {
        return scaling;
    }

    public void SetScaling(float val)
    {
        scaling = val;
        weapon.Scale();
    }

    public void AddChest(Chest chest)
    {
        this.chests.Add(chest);
    }

    public void death()
    {
        // allert the linked chests that the enemy is dead
        foreach(Chest chest in chests)
        {
            chest.RemoveEnemy(this);
        }
        Destroy(this.gameObject);
    }
}
