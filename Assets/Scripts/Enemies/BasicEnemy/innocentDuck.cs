using Unity.Collections;
using UnityEngine;

public class InnocentDuck : Enemy
{
    [SerializeField]
    private Rigidbody2D rb;

    [Range(0.0f, 50.0f)]
    [SerializeField]
    private float angleRange;
    private Vector3 moveDirection;
    private bool inArena;

    private Vector2 hackyVelocity;

    override protected void Start() {
        base.Start();
        OnSpawn();
    }
    void Update() {
        if (rb.linearVelocity.magnitude >= enemyMoveSpeed) {
            hackyVelocity = rb.linearVelocity;
        }
        //moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*moveAngle), Mathf.Sin(2*Mathf.PI*moveAngle));
        //OnMove();
    }
    protected override void OnAim() {
        //no aim
    }

    protected override void OnDeath() {
        throw new System.NotImplementedException();
    }

    protected override void OnMove() {
        rb.linearVelocity = moveDirection.normalized*enemyMoveSpeed;
    }

    protected override void OnSpawn() {
        //spawn on right or left side and fly toward middle
        moveDirection = Vector2.left*transform.position.x;
        OnMove();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (!inArena) return;

        if (col.collider.CompareTag("East Wall")) {
            //find angle after bounce
            float bounceAngle = Mathf.Atan2(hackyVelocity.y, -hackyVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            
            Debug.Log(bounceAngle + " y = " + hackyVelocity.y + " x = " + (-hackyVelocity.x));

            //max min currently breaking. need to have the value be able to wrap from 1 to 0
            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange/100, 0.25f), Mathf.Min(bounceAngle+angleRange/100, 0.75f));
            Debug.Log("New angle:" + newAngle);
            //0.25 to 0.75
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("West Wall")) {//find angle after bounce
            float bounceAngle = Mathf.Atan2(hackyVelocity.y, -hackyVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            Debug.Log(bounceAngle + " y = " + hackyVelocity.y + " x = " + (-hackyVelocity.x));

            //max min currently breaking. need to have the value be able to wrap from 1 to 0
            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange/100, -0.25f), Mathf.Min(bounceAngle+angleRange/100, 0.25f));
            Debug.Log("New angle:" + newAngle);
            //-0.25 to 0.25
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("North Wall")) {
            float bounceAngle = Mathf.Atan2(-hackyVelocity.y, hackyVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            Debug.Log(bounceAngle + " y = " + (-hackyVelocity.y) + " x = " + hackyVelocity.x);

            //max min currently breaking. need to have the value be able to wrap from 1 to 0
            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange/100, 0.5f), Mathf.Min(bounceAngle+angleRange/100, 1));
            Debug.Log("New angle:" + newAngle);
            //0.5 to 1
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("South Wall")) {
            float bounceAngle = Mathf.Atan2(-hackyVelocity.y, hackyVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            Debug.Log(bounceAngle + " y = " + (-hackyVelocity.y) + " x = " + hackyVelocity.x);

            //max min currently breaking. need to have the value be able to wrap from 1 to 0
            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange/100, 0), Mathf.Min(bounceAngle+angleRange/100, 0.5f));
            Debug.Log("New angle:" + newAngle);
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            //0 to 0.5
            OnMove();
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Arena")) {
            inArena = true;
        }
    }
}
