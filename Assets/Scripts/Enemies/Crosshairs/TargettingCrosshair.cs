using System.Collections;
using UnityEngine;

public class TargettingCrosshair : EnemyCrosshairBase
{
    [SerializeField]
    private Rigidbody2D rb;

    private bool doMove = true;
    private Transform playerTransform; 

    protected void Start() {
        OnSpawn();
    }
    void Update() {
        if (doMove) {
            OnMove();
        }
    }

    protected override void OnMove() {
        if (playerTransform != null) 
            transform.position = Vector2.Lerp(transform.position, playerTransform.position, Time.deltaTime * crosshairMoveSpeed); //position shouldnt be an issue here since this shouldnt every use physics to collide wit hanything
    }

    protected override void OnFire() {
        doMove = false;

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, hurtboxSizeReference.radius*transform.localScale.x, LayerMask.GetMask("Player"));
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
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            playerTransform = player.transform;
        } else {
            playerTransform= null;
        }
        doMove = true;
    }
}
