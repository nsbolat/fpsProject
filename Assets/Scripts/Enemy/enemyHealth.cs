using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float dieForce;
    [HideInInspector]
    public float currentHealth;
    Ragdoll ragdoll;

    private enemyUIHealthBar _healthBar;

    [Header("Hit Effects")] 
    public float blinkIntensity;
    public float blinkDuration;
    private float blinkTimer;
    public Color blinkColor;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private Dictionary<SkinnedMeshRenderer, Color> originalColors = new Dictionary<SkinnedMeshRenderer, Color>(); //chatGPT yazdı ama işe yaradı orginal rengi tutuyor
    
    private bool isBlinking = false;

    private void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        currentHealth = maxHealth;
        
        var _rigidBodies = GetComponentsInChildren<Rigidbody>();//tüm children rigidbodyleri çağır
        foreach (var rigidBody in _rigidBodies)
        {
            enemyHitbox eHitbox = rigidBody.gameObject.AddComponent<enemyHitbox>();//ve onlara enemyhitbox scriptti ekle
            eHitbox.enemyHealth = this;
        }

        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();//tüm cocuk meshleri çağır
        foreach (var skinnedMesh in _skinnedMeshRenderers)
        {
            if (skinnedMesh.material.HasProperty("_Color"))
            {
                originalColors[skinnedMesh] = skinnedMesh.material.color; //chatGPT yaptı
            }
        }

        _healthBar = GetComponentInChildren<enemyUIHealthBar>();
    }

    public void takeDamage(float amount, Vector3 direction) //HASAR VERME
    {
        currentHealth -= amount; // gelen hasarı mevcut candan çıkar
        _healthBar.setHealthBarPercentage(currentHealth/maxHealth);

        blinkTimer = blinkDuration;
        isBlinking = true;

        if (currentHealth <= 0.0f) // canı 0 sa öldür
        {
            Die(direction);
        }
    }



    void Die( Vector3 direction)
    {
        ragdoll.ActiveRagdoll();
        direction.y = 1f;
        ragdoll.ApplyForce(direction*dieForce);
        _healthBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isBlinking)
        {
            blinkTimer -= Time.deltaTime;
            float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
            float intensity = (lerp * blinkIntensity) + 1.0f;
            
            foreach (var skinnedMesh in _skinnedMeshRenderers)
            {
                if (originalColors.ContainsKey(skinnedMesh))
                {
                    skinnedMesh.material.color = blinkColor * intensity;
                }
            }
            if (blinkTimer <= 0)
            {
                ResetColors();
                isBlinking = false;
            }
            
        }
    }

    private void ResetColors()
    {
        foreach (var skinnedMesh in _skinnedMeshRenderers)
        {
            if (originalColors.ContainsKey(skinnedMesh))
            {
                skinnedMesh.material.color = originalColors[skinnedMesh];
            }
        }
    }
}
