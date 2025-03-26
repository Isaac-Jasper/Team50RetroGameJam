using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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
    [SerializeField] private GameObject flareEffect;
    [SerializeField] private float hitEffectDuration = 0.5f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private GameObject dashEffectPrefab;
    private bool canDash = false;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null && (hitSound != null || deathSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Initialize player state
        currentLives = maxLives;
        isDead = false;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Crosshair"), true);
    }

    private void Start()
    {
        InputManager.Instance.OnFire.AddListener(Fire);
        InputManager.Instance.OnDash.AddListener(DoDash);
        InputManager.Instance.OnKBMove.AddListener(MoveObject);

        GameManager.Instance.UpdateLivesDisplay(currentLives);
    }
    
    private void Update()
    {
        // Handle dash cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        
        // Check for dash input
        if (canDash && GlobalUpgradeSettings.dashUnlocked && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.U))
    {
        UpgradeManager.Instance.ShowUpgradeSelection();
    }
    }
    
    private void MoveObject(Vector2 input)
    {
        if (isDead || isDashing) return;
        rb.linearVelocity = input * moveSpeed;
        movement = input; // Store for dash direction
    }   

    private void DoDash() {
        if (canDash && GlobalUpgradeSettings.dashUnlocked && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }
    }

    private void Fire()
    {
        if (isDead || Time.time < nextFireTime) return;

        // Set next fire time
        nextFireTime = Time.time + shootCooldown;

        // Flash effect for visual feedback
        float randRotation = Random.Range(0,360);
        Instantiate(flareEffect, transform.position, Quaternion.Euler(Vector3.forward*randRotation));

        // Perform a raycast directly at the player's position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            // Check if the hit object has an Enemy component
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply damage to the enemy; apply critical hits if enabled
                int damage = 1;
                if (UnityEngine.Random.value < GlobalUpgradeSettings.criticalHitChance)
                {
                    damage = 2;
                    // Optional: Visual feedback for critical hit
                    StartCoroutine(FlashSprite(spriteRenderer, Color.red, 0.1f));
                }
                enemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator Dash()
    {
        if (movement.magnitude < 0.1f) yield break;
        
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        
        // Store current velocity
        Vector2 dashVelocity = movement.normalized * (moveSpeed * 3);
        Vector2 originalVelocity = rb.linearVelocity;
        
        // Disable collisions during dash
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        
        // Visual effect
        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, dashDuration);
        }
        
        // Adjust alpha for visual feedback
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        
        // Perform dash
        rb.linearVelocity = dashVelocity;
        float dashTimer = 0;
        
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            yield return null;
        }
        
        // Restore state
        rb.linearVelocity = originalVelocity;
        isDashing = false;
        spriteRenderer.color = originalColor;
        
        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    }

    // New method to unlock dash ability
    public void UnlockDash()
    {
        canDash = true;
        GlobalUpgradeSettings.dashUnlocked = true;
        Debug.Log("Dash ability unlocked!");
    }

    public void getSmaller()
    {
        transform.localScale = new Vector3(.75f, .75f, .75f); // Double the size
        Debug.Log("Smol!");
    }


    // New method to add a life
    public void AddLife()
    {
        currentLives++;
        GameManager.Instance.UpdateLivesDisplay(currentLives);
        Debug.Log("Extra life added! Current lives: " + currentLives);
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