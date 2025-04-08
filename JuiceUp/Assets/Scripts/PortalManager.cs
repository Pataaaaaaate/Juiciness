using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float EnemyPeriod;
    public GameObject ennemiApparition;
    private float timeApparition = 1f;
    void Start()
    {
        
        InvokeRepeating("InvokeEnemy", 0, EnemyPeriod);
    }

    void InvokeEnemy()
    {

        
        Instantiate(EnemyPrefab, transform.position, transform.rotation);
        Instantiate(ennemiApparition, transform.position, transform.rotation);


        
    }


   
}