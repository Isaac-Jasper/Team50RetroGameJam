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
        
        Destroy(gameObject);
    }
}