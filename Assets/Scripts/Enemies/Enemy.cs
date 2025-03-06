using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected GameObject crosshair;

    [SerializeField]
    protected List<GameObject> activeCrosshairs; //may have issues with adding objects and then deleting them later

    [SerializeField]
    protected int health, ammo;

    //aimRate referces to enemy creating a crosshair, fireRate is the rate the crosshair shoots bullets (used in crosshair script)
    [SerializeField]
    protected float enemyMoveSpeed, crosshairMoveSpeed, damage, aimRate, fireRate; 

    protected virtual void Start() {
        activeCrosshairs = new List<GameObject>();
    }   
    protected abstract void OnMoves();
    protected abstract void OnAim();
    protected abstract void OnSpawn();
    protected abstract void OnDeath(); 

    protected void DestroyAllCrosshairs() {
        foreach (GameObject cross in activeCrosshairs) {
            cross.GetComponent<EnemyCrosshair>().OnDeath(); //may have issues with adding and destroying gameobjects in a list
        }
    }
    protected virtual void CreateCrosshair(GameObject crosshair, Vector3 position) {
        GameObject createdCrosshair = Instantiate(crosshair, position, crosshair.transform.rotation);
        activeCrosshairs.Add(createdCrosshair);
        EnemyCrosshair crosshairScript = createdCrosshair.GetComponent<EnemyCrosshair>();
        crosshairScript.ammo = ammo;
        crosshairScript.crosshairMoveSpeed = crosshairMoveSpeed;
        crosshairScript.damage = damage;
        crosshairScript.fireRate = fireRate;
    }
}
