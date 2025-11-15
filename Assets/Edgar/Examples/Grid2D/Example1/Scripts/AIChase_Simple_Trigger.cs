using UnityEngine;

public class AIChase_Simple_Trigger : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Assign the player's Transform in the Inspector
    
    [Header("Settings")]
    public float speed = 3f; // Movement speed

    // This bool controls whether the enemy is allowed to chase or not
    private bool canChase = false; 

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
            Vector2 direction = (player.position - transform.position).normalized;

            // Move the enemy
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
            
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
}
