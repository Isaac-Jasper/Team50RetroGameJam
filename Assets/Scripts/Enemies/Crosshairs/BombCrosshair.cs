using UnityEngine;

public class BombCrosshair : EnemyCrosshairBase
{
    [SerializeField]
    private Rigidbody2D rb;

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

    protected override void OnSpawn() {
        StartCoroutine(SafeFrames());
        StartCoroutine(LifeTimer());
    }
}
