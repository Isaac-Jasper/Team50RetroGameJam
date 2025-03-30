using System.Collections;
using UnityEngine;

public class BouncingEnemy : EnemyBase
{
    [SerializeField]
    private float aimPauseTimeUntilAim, aimPauseTimeAfterAim;

    [Header("% of circle path can deviate")]
    [Range(0.0f, 0.25f)]
    [SerializeField]
    private float angleRange; //deviates on both sides of path, for a total of 50% circle coverage
    private Vector3 moveDirection; //direction duck is currently moving
    private Vector2 nonZeroVelocity; //velocity of duck right before colliding with wall

    void Update() {
        if (rb.linearVelocity.magnitude > 0) {
            nonZeroVelocity = rb.linearVelocity;
        }
        if (rb.linearVelocity.x > 0) {
            spriteObject.transform.rotation = Quaternion.Euler(0,180,0);
        } else if (rb.linearVelocity.x < 0) {
            spriteObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }

    protected override void OnDeath() {
        //add death logic
        //play death animation here
        
        base.OnDeath();
    }

    protected override void OnMove() {
        rb.linearVelocity = moveDirection.normalized*enemyMoveSpeed;
    }

    protected override IEnumerator OnAim() {
        Vector2 temp = moveDirection;
        moveDirection = Vector2.zero;
        OnMove();
        yield return new WaitForSeconds(aimPauseTimeUntilAim);
    
        //aiming animation here
        GameObject spawnedCrosshair = SpawnCrosshair();  // Use our new method instead of Instantiate
    
        yield return new WaitForSeconds(aimPauseTimeAfterAim);
        moveDirection = temp;
        OnMove();
    }

    private IEnumerator AimTimer() {
        float rand = Random.Range(0, aimRate/3);
        yield return new WaitForSeconds(aimRate + rand);
        StartCoroutine(OnAim());
        while (true) {
            rand = Random.Range(0, aimRate/5);
            yield return new WaitForSeconds(aimRate+ aimPauseTimeUntilAim + aimPauseTimeAfterAim + rand);
            StartCoroutine(OnAim());
        }
    }

    protected override void OnSpawn() {
        //spawn on right or left side and fly toward middle
        moveDirection = Vector2.left*transform.position.x;
        StartCoroutine(AimTimer());
        OnMove();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.collider.CompareTag("East Wall")) {
            if (rb.linearVelocity.x < 0) return;
            if (rb.linearVelocity.x == 0) {
                moveDirection = Vector2.left;
                OnMove();
            }
            //find angle after bounce
            float bounceAngle = Mathf.Atan2(nonZeroVelocity.y, -nonZeroVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            
            //Debug.Log(bounceAngle + " y = " + hackyVelocity.y + " x = " + (-hackyVelocity.x));

            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange, 0.25f), Mathf.Min(bounceAngle+angleRange, 0.75f));
            //Debug.Log("New angle:" + newAngle);
            //0.25 to 0.75
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("West Wall")) {
            if (rb.linearVelocity.x > 0) return;
            if (rb.linearVelocity.x == 0) {
                moveDirection = Vector2.right;
                OnMove();
            }
            //find angle after bounce
            float bounceAngle = Mathf.Atan2(nonZeroVelocity.y, -nonZeroVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            //Debug.Log(bounceAngle + " y = " + hackyVelocity.y + " x = " + (-hackyVelocity.x));

            //avoids dealing with the 1 not wrapping to 0
            float temp = bounceAngle + 0.25f;
            if (temp > 1) temp--;
            float newAngle = Random.Range(Mathf.Max(temp-angleRange, 0f), Mathf.Min(temp+angleRange, 0.5f));
            newAngle -= 0.25f;
            //Debug.Log("New angle:" + newAngle);
            //-0.25 to 0.25
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("North Wall")) {
            if (rb.linearVelocity.y < 0) return;
            if (rb.linearVelocity.y == 0) {
                moveDirection = Vector2.down;
                OnMove();
            }
            float bounceAngle = Mathf.Atan2(-nonZeroVelocity.y, nonZeroVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++; 
            //Debug.Log(bounceAngle + " y = " + (-hackyVelocity.y) + " x = " + hackyVelocity.x);

            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange, 0.5f), Mathf.Min(bounceAngle+angleRange, 1));
            //Debug.Log("New angle:" + newAngle);
            //0.5 to 1
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            OnMove();
            return;
        }
        if (col.collider.CompareTag("South Wall")) {
            if (rb.linearVelocity.y > 0) return;
            if (rb.linearVelocity.y == 0) {
                moveDirection = Vector2.up;
                OnMove();
            }
            float bounceAngle = Mathf.Atan2(-nonZeroVelocity.y, nonZeroVelocity.x)/(2*Mathf.PI);
            if (bounceAngle < 0) bounceAngle++;
            //Debug.Log(bounceAngle + " y = " + (-hackyVelocity.y) + " x = " + hackyVelocity.x);

            float newAngle = Random.Range(Mathf.Max(bounceAngle-angleRange, 0), Mathf.Min(bounceAngle+angleRange, 0.5f));
            //Debug.Log("New angle:" + newAngle);
            moveDirection = new Vector2(Mathf.Cos(2*Mathf.PI*newAngle), Mathf.Sin(2*Mathf.PI*newAngle));
            //0 to 0.5
            OnMove();
            return;
        }
    }
}
