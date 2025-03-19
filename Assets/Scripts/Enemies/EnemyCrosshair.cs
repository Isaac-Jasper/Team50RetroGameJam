// EnemyCrosshair.cs modifications
using System.Collections;
using UnityEngine;

abstract public class EnemyCrosshair : MonoBehaviour
{
    [SerializeField]
    protected GameObject hurtbox;

    public Enemy sourceEnemy { get; set; }  // Reference to the enemy that created this crosshair
    
    public int ammo;
    public float crosshairMoveSpeed, damage, fireRate, hurtboxDuration; 
    
    protected bool isFiring = false;  // Track if the crosshair is in firing state

    abstract protected IEnumerator OnFire();
    abstract protected void OnMove();
    abstract protected void OnSpawn();
    
    virtual protected void OnDeath() {
        Destroy(gameObject);
    }
    
    // Method to activate the hurtbox when firing
    protected void ActivateHurtbox()
    {
        if (hurtbox != null)
        {
            hurtbox.SetActive(true);
            isFiring = true;
            
            // Ensure the hurtbox is configured with the correct damage
            CrosshairHurtbox hurtboxComponent = hurtbox.GetComponent<CrosshairHurtbox>();
            if (hurtboxComponent != null)
            {
                hurtboxComponent.SetDamage((int)damage);
            }
        }
    }
    
    // Method to deactivate the hurtbox after firing
    protected void DeactivateHurtbox()
    {
        if (hurtbox != null)
        {
            hurtbox.SetActive(false);
            isFiring = false;
        }
    }
}