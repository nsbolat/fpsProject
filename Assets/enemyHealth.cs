using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{

    public float maxHealth;
    
    public float currentHealth;
    Ragdoll ragdoll;
    private void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        currentHealth = maxHealth;
        var _rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in _rigidBodies)
        {
            enemyHitbox eHitbox = rigidBody.gameObject.AddComponent<enemyHitbox>();
            eHitbox.enemyHealth = this;
        }
    }

    public void takeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount; // gelen hasarı mevcut candan çıkar
        
        
        if (currentHealth<=0.0f) //canı 0 sa öldür
        {
            Die();
        }
    }

    void Die()
    {
        ragdoll.ActiveRagdoll();
    }
}
