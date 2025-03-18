using UnityEngine;

public class PlayerController : MonoBehaviour{
    public float moveSpeed = 5f; 

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //Removes gravity (Yes its stupid)
    }

    void Update()
    {
        //Gets the inputs (Default WASD/Arrow Keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        //Create movement vector to make speed consistent
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        //Updates the velocity
        rb.linearVelocity = movement * moveSpeed;
    }
}
