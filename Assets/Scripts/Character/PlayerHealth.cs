using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public float currentPlayerHealth = 100f;
    [SerializeField] private float maxPlayerHealth = 100f;

    [Header("UI Elements")]
    [SerializeField] private Image redImage;
    [SerializeField] private Image hurtImage;
    
    [Header("Damage Feedback")]
    [SerializeField] private float hurtTimer = 0.1f;
    [SerializeField] private AudioClip hurtAudio;
    private AudioSource healthAudioSource;
    [Header("Regen")]
    [SerializeField] private float regenTimer;
    private void Start()
    {
        healthAudioSource = GetComponent<AudioSource>();
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        Color redImageColor = redImage.color;
        redImageColor.a = 1 - (currentPlayerHealth / maxPlayerHealth);
        
        redImage.color = redImageColor;
    }

    private IEnumerator HurtFlash()
    {
        hurtImage.enabled = true;
        healthAudioSource.PlayOneShot(hurtAudio);
        yield return new WaitForSeconds(hurtTimer);
        hurtImage.enabled = false;
    }

    public void TakePlayerDamage(int damageAmount)
    {
        if (currentPlayerHealth > 0)
        {
            currentPlayerHealth -= damageAmount;
            StartCoroutine(HurtFlash());
            UpdateHealthUI();

            // Check if player health drops to zero or below
            if (currentPlayerHealth <= 0)
            {
                // Call a method for player death or game over
                PlayerDeath();
            }
        }
    }

    private void PlayerDeath()
    {
        // Implement player death logic here
        Debug.Log("Player has died.");
        // Example: Restart the level, show game over screen, etc.
    }
}