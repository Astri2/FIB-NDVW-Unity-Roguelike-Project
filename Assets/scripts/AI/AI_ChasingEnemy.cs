using System.Security.Cryptography.X509Certificates;
using Unity.Jobs;
using UnityEngine;

public class AI_ChasingEnemy : MonoBehaviour
{
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

    public void Start()
    {
        hp = 20;
    }

    void Update()
    {
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
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    speed * Time.deltaTime
                );
            }
            else {
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

        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
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

    public Weapon GetWeapon() { return weapon;}

    public float GetHP()
    {
        return hp;
    }

    public void SetHP(float value)
    {
        hp = value;
    }
}
