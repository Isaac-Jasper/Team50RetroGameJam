using System.Collections;
using UnityEngine;

public class StationaryCrosshair : EnemyCrosshairBase
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float safeTimeLength;
    [Header("Spawn area specifications")]
    [SerializeField]
    private Vector2 bottomLeftCorner, topRightCorner;

    protected void Start() {
        OnSpawn();
    }

    protected override void OnMove() {
        //no movement
    }

    protected override void OnFire() {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, hurtboxSizeReference.radius, LayerMask.GetMask("Player"));
        //play damage animation
        if (hitPlayer != null) {
            PlayerController player = hitPlayer.GetComponent<PlayerController>();
            player.TakeDamage(damage);
        }
        //Debug.Log("Fired");
        OnDeath();
    }

    override protected IEnumerator SafeFrames() {
        Color startColor = sr.color;
        sr.color = new Color(startColor.r,startColor.g,startColor.b,ALPHA_LEVEL);
        playerCollisionCollider.enabled = false;
        yield return new WaitForSeconds(safeTimeLength);
        sr.color = startColor;
        playerCollisionCollider.enabled = true;
    }

    protected override void OnSpawn() {
        StartCoroutine(SafeFrames());

        float randX = Random.Range(bottomLeftCorner.x,topRightCorner.x);
        float randY = Random.Range(bottomLeftCorner.y,topRightCorner.y);
        transform.position = new Vector3(randX,randY);
        sr.enabled = true;
        
        StartCoroutine(LifeTimer());
    }
}
