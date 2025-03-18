using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected GameObject crosshair;

    [SerializeField]
    protected int health;

    //aimRate referces to enemy creating a crosshair, fireRate is the rate the crosshair shoots bullets (used in crosshair script)
    [SerializeField]
    protected float enemyMoveSpeed, aimRate; 

    protected abstract void OnMove();
    protected abstract IEnumerator OnAim();
    protected abstract void OnSpawn();
    protected abstract void OnDeath(); 
}
