using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HealerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator; 
    
    Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        HandleInput();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(moveX, moveY).normalized;
    }

    void HandleMovement()
    {
        rb.linearVelocity = movementInput * moveSpeed;

        if (animator != null)
        {
            bool isMoving = movementInput.magnitude > 0;
            animator.SetBool("IsWalking", isMoving);

            if (movementInput.x != 0)
            {
                float direction = Mathf.Sign(movementInput.x);
                transform.localScale = new Vector3(direction, 1f, 1f);
            }
        }
    }
}
