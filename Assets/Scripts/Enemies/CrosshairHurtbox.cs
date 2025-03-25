using UnityEngine;

public class CrosshairHurtbox : MonoBehaviour
{
    private int damageAmount = 1;
    
    public void SetDamage(int damage)
    {
        damageAmount = damage;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}