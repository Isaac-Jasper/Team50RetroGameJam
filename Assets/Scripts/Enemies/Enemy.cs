using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected GameObject crosshairPrefab;  // Changed from crosshair to crosshairPrefab

    [SerializeField]
    protected int health = 1;

    [SerializeField]
    protected float enemyMoveSpeed, aimRate; 

    [SerializeField]
    protected int damageOnCollision = 1;
    
    [SerializeField]
    protected int scoreValue = 10;

    protected abstract void OnMove();
    protected abstract IEnumerator OnAim();
    protected abstract void OnSpawn();
    
    // Method to spawn a crosshair
    protected GameObject SpawnCrosshair()
    {
        if (crosshairPrefab != null)
        {
            // Instantiate the crosshair at the enemy's position
            GameObject newCrosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
            
            // Pass any necessary data to the crosshair
            EnemyCrosshair crosshairComponent = newCrosshair.GetComponent<EnemyCrosshair>();
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