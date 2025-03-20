using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Weapon Settings")]
    public float shootCooldown = 0.5f; // Time between shots
    private float nextFireTime = 0f;
    
    [Header("Health Settings")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;
    [SerializeField] private float invincibilityDuration = 1.5f;
    private bool isInvincible = false;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitEffectDuration = 0.5f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null && (hitSound != null || deathSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        InputManager.Instance.OnFire.AddListener(Fire);
        InputManager.Instance.OnKBMove.AddListener(MoveObject);
        
        // Initialize player state
        currentLives = maxLives;
        isDead = false;
        
        // Update UI
        GameManager.Instance.UpdateLivesDisplay(currentLives);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Crosshair"), true);

    }
    
    private void MoveObject(Vector2 input)
    {
        if (isDead) return;
        rb.linearVelocity = input * moveSpeed;
    }   

    private void Fire()
    {
        if (isDead || Time.time < nextFireTime) return;
    
        // Set next fire time
        nextFireTime = Time.time + shootCooldown;
    
        // Flash effect
        StartCoroutine(FlashSprite(spriteRenderer, Color.yellow, 0.1f));
    
        // TODO: Implement actual weapon firing here
    }
    private IEnumerator FlashSprite(SpriteRenderer renderer, Color flashColor, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }

    public void TakeDamage(int damageAmount = 1)
    {
        if (isInvincible || isDead) return;
        
        currentLives -= damageAmount;
        GameManager.Instance.UpdateLivesDisplay(currentLives);
        
        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, hitEffectDuration);
        }
        
        if (currentLives <= 0)
        {
            Die();
            return;
        }
        
        // Temporary invincibility
        StartCoroutine(InvincibilityFrames());
    }
    
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        
        // Visual feedback that player is hit
        if (spriteRenderer != null)
        {
            float blinkInterval = 0.15f;
            for (float i = 0; i < invincibilityDuration; i += blinkInterval)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }
        
        isInvincible = false;
    }
    
    private void Die()
    {
        isDead = true;
        
        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // Disable player controls
        rb.linearVelocity = Vector2.zero;
        
        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        // Optional death animation before disabling
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Simple death animation
        if (spriteRenderer != null)
        {
            // Fade out or other visual effect
            Color originalColor = spriteRenderer.color;
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        
        // Disable player GameObject
        gameObject.SetActive(false);
    }
}