using System.Collections;
using UnityEngine;

public class TargettingCrosshair : EnemyCrosshair
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private float pauseTimeUntilFire, pauseTimeAfterFire;
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
        transform.position = Vector2.Lerp(transform.position, playerTransform.position, Time.deltaTime * crosshairMoveSpeed); //position shouldnt be an issue here since this shouldnt every use physics to collide wit hanything
    }

    protected override IEnumerator OnFire() {
        doMove = false;
        OnMove();
        yield return new WaitForSeconds(pauseTimeUntilFire);
        hurtbox.SetActive(true);
        yield return new WaitForSeconds(hurtboxDuration);
        hurtbox.SetActive(false);
        //play damage animation
        //Debug.Log("Fired");
        yield return new WaitForSeconds(pauseTimeAfterFire);
        OnDeath();
    }

    protected override IEnumerator FireTimer() {
        while (true) {
            yield return new WaitForSeconds(fireRate);
            StartCoroutine(OnFire());
        }
    }

    protected override void OnSpawn() {
        StartCoroutine(FireTimer());
        playerTransform = GameObject.FindWithTag("Player").transform;
        doMove = true;
    }
}
