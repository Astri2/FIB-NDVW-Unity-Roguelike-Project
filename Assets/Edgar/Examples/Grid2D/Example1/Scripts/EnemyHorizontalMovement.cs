using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

public class EnemyHorizontalMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;          // Movement speed
    public float moveDistance = 3f;   // How far the enemy moves from the start position

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        // Save the initial position
        startPos = transform.position;
    }

    void Update()
    {
        // Move horizontally
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        /*
        // Check distance from start
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            // Reverse direction
            direction *= -1;

            // Clamp position to avoid drifting
            float clampedX = Mathf.Clamp(transform.position.x, startPos.x - moveDistance, startPos.x + moveDistance);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hi!");
        if (collision.gameObject.layer != LayerMask.NameToLayer("Walls")) return;
        Debug.Log("Hi2!");
        direction *= -1;
    }
}
