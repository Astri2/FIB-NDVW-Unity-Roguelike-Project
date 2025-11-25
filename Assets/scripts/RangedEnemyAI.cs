using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Player transform (found automatically if tagged)
    public GameObject projectilePrefab;     // Prefab for bullet (EnemyProjectile)

    [Header("Movement Settings")]
    public float moveSpeed = 2f;            // How fast the enemy moves
    public float detectionRange = 8f;       // Distance where enemy starts chasing
    public float attackRange = 5f;          // Distance where enemy stops and shoots

    [Header("Attack Settings")]
    public float fireInterval = 1.5f;       // Seconds between shots
    private float fireCooldown = 0f;        // Time left until next shot

    // Internal
    private Rigidbody2D rb;

    void Start()
    {
        // Get Rigidbody2D for movement
        rb = GetComponent<Rigidbody2D>();

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Update()
    {
        // Skip if no player yet
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;


        // Calculate distance to player
        float distance = Vector2.Distance(transform.position, player.position);

        // --- MOVEMENT ---
        if (distance > attackRange)
        {
            // Move toward player
            Vector2 direction = (player.position - transform.position).normalized;

            if (rb != null)
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            else
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // Stop moving while in attack range
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // --- SHOOTING ---
            fireCooldown -= Time.deltaTime;

            if (fireCooldown <= 0f)
            {
                ShootAtPlayer();
                fireCooldown = fireInterval;  // reset cooldown
            }
        }
    }

    private void ShootAtPlayer()
    {
        // Safety check
        if (projectilePrefab == null || player == null)
            return;

        // Calculate direction toward player
        Vector2 shootDirection = (player.position - transform.position).normalized;

        // Spawn projectile at current position (no rotation needed for 2D)
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Pass direction to projectile script
        EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.SetDirection(shootDirection);
        }
    }
}
