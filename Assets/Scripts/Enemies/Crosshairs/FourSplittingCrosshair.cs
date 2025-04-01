using UnityEngine;

public class FourSplittingCrosshair : EnemyCrosshairBase
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject splitCrosshair;
    [SerializeField]
    private int numberOfSplits;


    protected void Start() {
        OnSpawn();
    }

    protected override void OnMove() {
        //no movement
    }

    protected override void OnFire() {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, hurtboxSizeReference.radius*transform.localScale.x, LayerMask.GetMask("Player"));

        //play damage animation
        if (hitPlayer != null) {
            PlayerController player = hitPlayer.GetComponent<PlayerController>();
            player.TakeDamage(damage);
        }

        for (int i = 0; i < numberOfSplits; i ++) {
            EnemyCrosshairBase split = Instantiate(splitCrosshair, transform.position, Quaternion.Euler(0,0,360 * i / numberOfSplits)).GetComponent<EnemyCrosshairBase>();
            split.sourceEnemy = sourceEnemy;
        }
        
        //Debug.Log("Fired");
        OnDeath();
    }

    protected override void OnSpawn() {
        StartCoroutine(SafeFrames());
        StartCoroutine(LifeTimer());
        OnMove();
    }
}
