using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Input movimento da tastiera
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D or arrows sx/dx
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S or arrows up/down
        moveInput.Normalize(); 
    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}

