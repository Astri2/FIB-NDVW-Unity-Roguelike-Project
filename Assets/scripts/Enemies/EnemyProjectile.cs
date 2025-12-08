using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;         // Bullet speed
    public float lifetime = 5f;      // Auto-destroy after X seconds
    public int damage = 1;           // How much damage it does (for later use)

    private Vector2 _direction;      // Movement direction (set by enemy when fired)

    void Start()
    {
        // If direction wasn't set by enemy (e.g. testing manually), aim at player
        if (_direction == Vector2.zero)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                Vector3 targetPos = playerObj.transform.position;
                _direction = (targetPos - transform.position).normalized * speed;
            }
        }

        // Auto-destroy after a few seconds so old bullets disappear
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the projectile each frame
        transform.Translate(_direction * Time.deltaTime, Space.World);
    }

    // Called by enemy to set bullet direction when spawned
    public void SetDirection(Vector2 dir)
    {
        _direction = dir.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If bullet hits the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy bullet!");

            // Example: apply damage if player has a script with a Damage() method
            // var player = other.GetComponent<Player>();
            // if (player != null)
            //     player.Damage(damage);

            Destroy(gameObject); // remove bullet after hit
        }

        // Optional: destroy if it hits walls or other obstacles
        // else if (other.CompareTag("Wall"))
        // {
        //     Destroy(gameObject);
        // }
    }
}
