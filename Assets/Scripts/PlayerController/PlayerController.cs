using UnityEngine;

public class PlayerController : MonoBehaviour{
    public float moveSpeed = 5f; 

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    public void MoveObject(Vector2 input){
        rb.linearVelocity = input * moveSpeed;
    }
}