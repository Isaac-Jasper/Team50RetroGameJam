using System.Collections;
using UnityEngine;

public class SmoughScript : EnemyBase
{
    [SerializeField]
    GameObject crosshairPrefabBigAttack;

    [SerializeField]
    private float bigAttackPauseTimeUntilAim, bigAttackPauseTimeAfterAim, normalAttackPauseTimeUntilAim, normalAttackPauseTimeAfterAim;
    [SerializeField]
    private int bigAttackWeight, normalAttackWeight, shotsUntilBigAttack; 
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

    //attack 1
    protected override IEnumerator OnAim() {
        if (normalAttackPauseTimeUntilAim == 0) {
            SpawnCrosshair();  
        } else {
            doMove = false;
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(normalAttackPauseTimeUntilAim);
        
            GameObject spawnedCrosshair = SpawnCrosshair();  
        
            yield return new WaitForSeconds(normalAttackPauseTimeAfterAim);
            doMove = true;
        }
    }

    //attack 2
    private IEnumerator OnAim2() {
        doMove = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(bigAttackPauseTimeUntilAim);
    
        //aiming animation here
        GameObject spawnedCrosshair = SpawnCrosshair2(); 
    
        yield return new WaitForSeconds(bigAttackPauseTimeAfterAim);
        doMove = true;
    }

    private GameObject SpawnCrosshair2() {
        if (crosshairPrefab != null)
        {
            // Instantiate the crosshair at the enemy's position
            GameObject newCrosshair = Instantiate(crosshairPrefabBigAttack, transform.position, Quaternion.identity);
            currentCrossHairs.Add(newCrosshair.GetComponent<EnemyCrosshairBase>());
            // Pass any necessary data to the crosshair
            EnemyCrosshairBase crosshairComponent = newCrosshair.GetComponent<EnemyCrosshairBase>();
            if (crosshairComponent != null)
            {
                crosshairComponent.sourceEnemy = this;
                
                // Apply crosshair size reduction from upgrades
                if (GlobalUpgradeSettings.crosshairSizeMultiplier != 1.0f)
                {
                    newCrosshair.transform.localScale *= GlobalUpgradeSettings.crosshairSizeMultiplier;
                }
            }
            
            return newCrosshair;
        }
        return null;
    }

    private IEnumerator AimTimer() {
        int shotsSinceBigAttack = 0;
        float rand = Random.Range(0, aimRate/2);
        yield return new WaitForSeconds(aimRate + rand);
        StartCoroutine(OnAim2());
        while (true) {
            rand = Random.Range(0, aimRate/2);

            if (shotsSinceBigAttack == 0) {
                yield return new WaitForSeconds(aimRate+ normalAttackPauseTimeUntilAim + normalAttackPauseTimeAfterAim + rand);
            } else {
                yield return new WaitForSeconds(aimRate+ bigAttackPauseTimeUntilAim + bigAttackPauseTimeAfterAim + rand);
            }

            if (shotsSinceBigAttack <= shotsUntilBigAttack) {
                StartCoroutine(OnAim());
                shotsSinceBigAttack++;
            } else {
                int randAtk = Random.Range(1, bigAttackWeight + normalAttackWeight + 1);
                if (randAtk <= bigAttackWeight) {
                    StartCoroutine(OnAim());
                    shotsSinceBigAttack++;
                } else {
                    StartCoroutine(OnAim2());
                    shotsSinceBigAttack = 0;
                }
            }

        }
    }

    protected override void OnSpawn() {
        //spawn on right or left side and fly toward middle
        playerTransform = GameObject.FindWithTag("Player").transform;
        doMove = true;
        StartCoroutine(AimTimer());
    }
}
