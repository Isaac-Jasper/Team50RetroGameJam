using System.Collections;
using UnityEngine;

abstract public class EnemyCrosshair : MonoBehaviour
{
    [SerializeField]
    protected GameObject hurtbox;

    public int ammo;
    public float crosshairMoveSpeed, damage, fireRate, hurtboxDuration; 

    abstract protected IEnumerator OnFire();
    abstract protected void OnMove();
    abstract protected void OnSpawn();
    virtual protected void OnDeath() {
        Destroy(gameObject); //when overriding call super OnDeath at end to destroy gameobject
    } 
}
