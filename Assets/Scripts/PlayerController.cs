using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;         // Speed at which the player moves.
    public float jumpForce = 5f;         // The force applied when the player jumps.
    public float gravity = -9.81f;       // Gravity value affecting the player.

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        // Cache the CharacterController component attached to this GameObject.
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get horizontal and vertical input (by default mapped to WASD or Arrow Keys).
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate the movement direction relative to the player's current facing direction.
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Check if the player is on the ground; if so, reset downward velocity.
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Jumping logic: When the Jump button is pressed and the player is grounded.
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            // Calculate the velocity required to reach the desired jump height.
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity over time.
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
