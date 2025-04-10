using System.Collections;
using UnityEngine;

public class DistanceEnemy : EnemyBase
{
    [SerializeField]
    private float aimPauseTimeUntilAim, aimPauseTimeAfterAim;
    [SerializeField]
    private float distance, rotateSpeed;
    private Transform playerTransform;

    void Update() {
        if (doMove) {
            OnMove();
        }
        if (rb.linearVelocity.x > 0) {
            transform.rotation = Quaternion.Euler(0,180,0);
        } else if (rb.linearVelocity.x < 0) {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }

    protected override void OnDeath() {
        //add death logic
        base.OnDeath();
    }

    protected override void OnMove() {
        //rb.linearVelocity = Vector2.Lerp(transform.position, playerTransform.position, enemyMoveSpeed)*distance;
        rb.linearVelocity = Vector2.Perpendicular(transform.position - playerTransform.position).normalized*rotateSpeed + (Vector2) (-playerTransform.position + transform.position)*enemyMoveSpeed*(distance/(playerTransform.position - transform.position).magnitude-1);
    }

    protected override IEnumerator OnAim() {
        doMove = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(aimPauseTimeUntilAim);
    
        //aiming animation here
        GameObject spawnedCrosshair = SpawnCrosshair();  // Use our new method instead of Instantiate
    
        yield return new WaitForSeconds(aimPauseTimeAfterAim);
        doMove = true;
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
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            playerTransform = player.transform;
        }
        doMove = true;
        StartCoroutine(AimTimer());
    }
}
