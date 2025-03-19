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
        // Flash effect
        SpriteRenderer hurtboxRenderer = hurtbox.GetComponent<SpriteRenderer>();
        if (hurtboxRenderer != null)
        {
            StartCoroutine(FlashSprite(hurtboxRenderer, Color.red, 0.2f));
        }
        
        hurtbox.SetActive(true);
        isFiring = true;
        
        // Configure damage
        CrosshairHurtbox hurtboxComponent = hurtbox.GetComponent<CrosshairHurtbox>();
        if (hurtboxComponent != null)
        {
            hurtboxComponent.SetDamage((int)damage);
        }
    }
}

    private IEnumerator FlashSprite(SpriteRenderer renderer, Color flashColor, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
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