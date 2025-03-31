using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb;
    [SerializeField]
    protected GameObject crosshairPrefab;  // Changed from crosshair to crosshairPrefab
    [SerializeField]
    protected GameObject spriteObject;
    [SerializeField]
    protected GameObject deathAnimationObject;

    [SerializeField]
    protected int health = 1;

    [SerializeField]
    protected float enemyMoveSpeed, aimRate; 

    [SerializeField]
    protected int damageOnCollision = 1;
    
    [SerializeField]
    protected int scoreValue = 10;

    public List<EnemyCrosshairBase> currentCrossHairs = new();
    private readonly float FLY_UP_SPEED_MULT = 8;
    protected bool doMove = false;

    protected virtual void Start() {
        doMove = true;
        if (transform.position.y <= -3) {
            StartCoroutine(FlyUpStart());
        } else {
            OnSpawn();
        }
    }
    protected abstract IEnumerator OnAim();
    protected virtual void OnMove() {
        if (doMove) return;
    }
    protected abstract void OnSpawn();

    protected IEnumerator FlyUpStart() {
        doMove = false;
        float randDir = Random.Range(-1f,1f);
        float randHeight = Random.Range(-2,3.5f);
        while (transform.position.y < randHeight) {
            rb.linearVelocity = FLY_UP_SPEED_MULT * Vector2.up + randDir*FLY_UP_SPEED_MULT * Vector2.right;
            yield return null;
        }

        OnSpawn();
    }
    
    // Method to spawn a crosshair
    protected GameObject SpawnCrosshair()
    {
        if (crosshairPrefab != null)
        {
            // Instantiate the crosshair at the enemy's position
            GameObject newCrosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
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
    
    public void TakeDamage(int damageAmount = 1)
    {
        health -= damageAmount;
        
        if (health <= 0) {
            OnDeath();
        }
    }
    
    protected virtual void OnDeath() {
        StopCoroutine(FlyUpStart());

        foreach (EnemyCrosshairBase cross in currentCrossHairs) {
            cross.RemoveCrosshair();
        }
        
        //play death animation
        if (deathAnimationObject != null) {
            Instantiate(deathAnimationObject, transform.position, deathAnimationObject.transform.rotation);
        }

        // Increase score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
        
        // Check for life steal chance
        if (GlobalUpgradeSettings.lifeStealChance > 0 && Random.value < GlobalUpgradeSettings.lifeStealChance)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.AddLife();
                
                // Visual feedback for life steal
                GameObject healEffect = new GameObject("HealEffect");
                healEffect.transform.position = transform.position;
                
                SpriteRenderer renderer = healEffect.AddComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>("HeartIcon"); // Ensure you have this sprite in Resources folder
                renderer.color = Color.green;
                
                // Animate the heal effect
                StartCoroutine(AnimateHealEffect(healEffect));
            }
        }
        
        Destroy(gameObject);
    }
    
    private IEnumerator AnimateHealEffect(GameObject effect)
    {
        float duration = 1.0f;
        float timer = 0;
        Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 endScale = new Vector3(1.5f, 1.5f, 1.5f);
        Vector3 moveDirection = Vector3.up;
        
        effect.transform.localScale = startScale;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            effect.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            effect.transform.position += moveDirection * Time.deltaTime;
            
            // Fade out
            SpriteRenderer renderer = effect.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                renderer.color = new Color(color.r, color.g, color.b, 1 - t);
            }
            
            yield return null;
        }
        
        Destroy(effect);
    }
}