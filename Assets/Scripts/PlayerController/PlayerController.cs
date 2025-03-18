using UnityEngine;

public class PlayerController : MonoBehaviour{
    public float moveSpeed = 5f;
    public float reloadSpeed;
    public float fireRate;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxHealth;
    private int ammo;
    private int health;

    

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        InputManager.Instance.OnFire.AddListener(Fire);
        InputManager.Instance.OnKBMove.AddListener(MoveObject);
        ammo = maxAmmo;
        health = maxHealth;
        GameManager.Instance.UpdateHealth(health);
        GameManager.Instance.updateAmmoCount(ammo);
    }
    private void MoveObject(Vector2 input){
        rb.linearVelocity = input * moveSpeed;
    }

    private void Fire()
    {
        if(ammo <= 0) Reload();
        else ammo--;
        GameManager.Instance.updateAmmoCount(ammo);
    }

    private void Reload()
    {
        ammo = maxAmmo;
    }

}