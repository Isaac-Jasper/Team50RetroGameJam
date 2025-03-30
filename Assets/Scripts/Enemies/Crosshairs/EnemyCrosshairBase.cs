// EnemyCrosshair.cs modifications
using System.Collections;
using UnityEngine;

abstract public class EnemyCrosshairBase : MonoBehaviour
{
    public EnemyBase sourceEnemy { get; set; }  // Reference to the enemy that created this crosshair
    [Header("Purely a size reference")]
    [SerializeField]
    protected CircleCollider2D hurtboxSizeReference;
    [SerializeField]
    GameObject onFireFlareEffect;
    public int damage;
    public float crosshairMoveSpeed, lifeTime; 
    
    [Header("Safe Frame Settings")]
    [SerializeField]
    protected Collider2D playerCollisionCollider;
    [SerializeField]
    protected SpriteRenderer sr;
    protected readonly float SAFE_TIMER = 0.5f, ALPHA_LEVEL = 0.5f;
    protected bool isFiring = false;  // Track if the crosshair is in firing state

    abstract protected void OnFire();
    abstract protected void OnMove();
    abstract protected void OnSpawn();
    virtual protected IEnumerator SafeFrames() {
        Color startColor = sr.color;
        sr.color = new Color(startColor.r,startColor.g,startColor.b,ALPHA_LEVEL);
        playerCollisionCollider.enabled = false;
        yield return new WaitForSeconds(SAFE_TIMER);
        sr.color = startColor;
        playerCollisionCollider.enabled = true;
    }
    protected virtual IEnumerator LifeTimer() {
        while (true) {
            yield return new WaitForSeconds(lifeTime);
            OnFire();
        }
    }
    
    virtual protected void OnDeath() {
        Instantiate(onFireFlareEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private IEnumerator FlashSprite(SpriteRenderer renderer, Color flashColor, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            StopCoroutine(LifeTimer());
            OnFire();
        }
    }
}