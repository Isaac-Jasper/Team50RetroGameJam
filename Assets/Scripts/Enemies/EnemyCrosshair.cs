using UnityEngine;

abstract public class EnemyCrosshair : MonoBehaviour
{
    [SerializeField]
    Enemy enemy;

    public int ammo;
    public float crosshairMoveSpeed, damage, fireRate; 

    abstract protected void OnFire();
    abstract protected void OnMove();
    abstract protected void OnSpawn();
    //public so enemy can destroy all crosshairs on death
    virtual public void OnDeath() { //maybe on death should handle removing crosshair from activeCrosshairs?
        Destroy(gameObject); //when overriding call super OnDeath at end to destroy gameobject
    } 
}
