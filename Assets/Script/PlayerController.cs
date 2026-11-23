using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Prevent the player from spinning.
        rb.freezeRotation = true;

        // Prevent gravity from affecting the player.
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + movement.normalized * speed * Time.fixedDeltaTime
        );
    }
}

