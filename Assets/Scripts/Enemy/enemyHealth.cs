using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class enemyHealth : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;

    private AiAgent agent;

    private enemyUIHealthBar _healthBar;

    [Header("Hit Effects")] 
    public float blinkDuration;
    private float blinkTimer;
    public Material damageMaterial;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private Dictionary<SkinnedMeshRenderer, Material[]> originalMaterials = new Dictionary<SkinnedMeshRenderer, Material[]>(); // Original materials dictionary
    
    private bool isBlinking = false;
    public bool isDead = false;

    private void Start()
    {
        agent = GetComponent<AiAgent>();
        currentHealth = maxHealth;
        
        var _rigidBodies = GetComponentsInChildren<Rigidbody>(); // Get all child rigidbodies
        foreach (var rigidBody in _rigidBodies)
        {
            enemyHitbox eHitbox = rigidBody.gameObject.AddComponent<enemyHitbox>(); // Add enemyHitbox script to them
            eHitbox.enemyHealth = this;

            if (rigidBody.gameObject.name.Equals("Head", StringComparison.OrdinalIgnoreCase)) // Eğer obje "head" ise
            {
                eHitbox.isHead = true; // isHead özelliğini true yap
            }
        }

        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); // Get all child skinned mesh renderers
        foreach (var skinnedMesh in _skinnedMeshRenderers)
        {
            originalMaterials[skinnedMesh] = skinnedMesh.materials; // Store original materials
        }

        _healthBar = GetComponentInChildren<enemyUIHealthBar>();
    }

    public void takeDamage(float amount, Vector3 direction, GameObject hitObject,bool isHeadshot = false) // Take damage
    {
        if (!isDead)
        {
            float finalDamage = isHeadshot ? amount * 2 : amount; // Eğer headshot ise hasarı 2 kat yap
        
            currentHealth -= finalDamage; // Subtract the incoming damage from current health
            _healthBar.setHealthBarPercentage(currentHealth / maxHealth);
        
            agent.stateMachine.ChangeState(AiStateId.Damage);

            blinkTimer = blinkDuration;
            isBlinking = true;
        
            ApplyDamageMaterial(); // Apply damage material
            if(currentHealth <= 0.0f)
            {
                Die(direction,hitObject);
                
            }
        }
    }

    void Die(Vector3 direction, GameObject hitObject)
    {
        AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        deathState.direction = direction;
        agent.stateMachine.ChangeState(AiStateId.Death);
        agent.ragdoll.ApplyForce(direction*agent.config.dieForce,hitObject.GetComponent<Rigidbody>());
        isDead = true;
    }

    private void Update()
    {
        if (isBlinking)
        {
            blinkTimer -= Time.deltaTime;
            
            if (blinkTimer <= 0)
            {
                ResetMaterials(); // Revert to original materials
                isBlinking = false;
            }
        }
    }

    private void ApplyDamageMaterial()
    {
        foreach (var skinnedMesh in _skinnedMeshRenderers)
        {
            Material[] damageMaterials = new Material[skinnedMesh.materials.Length];
            for (int i = 0; i < damageMaterials.Length; i++)
            {
                damageMaterials[i] = damageMaterial; // Set damage material to all slots
            }
            skinnedMesh.materials = damageMaterials;
        }
    }

    private void ResetMaterials()
    {
        foreach (var skinnedMesh in _skinnedMeshRenderers)
        {
            if (originalMaterials.ContainsKey(skinnedMesh))
            {
                skinnedMesh.materials = originalMaterials[skinnedMesh]; // Revert to original materials
            }
        }
    }
}
