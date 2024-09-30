using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public float currentPlayerHealth = 100f;
    [SerializeField] private float maxPlayerHealth = 100f;

    [Header("Damage Feedback")]
    [SerializeField] private float voroIntensityStat = 2.55f;
    [SerializeField] private float vignetteIntensityStat = 1.25f;

    [Header("Intensity Scaling")]
    [SerializeField] private float voroScalingFactor = 1.0f;
    [SerializeField] private float vignetteScalingFactor = 1.0f;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashFadeOutTime = 0.5f;

    [Header("References")]
    [SerializeField] private ScriptableRendererFeature fullScreenDamage;
    [SerializeField] private Material damageMaterial;

    private int voroIntensityID = Shader.PropertyToID("_VoroIntensity");
    private int vignetteIntensityID = Shader.PropertyToID("_VignetIntensity");
    private int colorID = Shader.PropertyToID("_Color");

    [SerializeField] private AudioClip hurtAudio;
    private AudioSource healthAudioSource;

    [Header("Regen")]
    [SerializeField] private float regenTimer = 5f;
    [SerializeField] private float regenSpeed = 10f;
    private Coroutine regenCoroutine;

    private void Start()
    {
        healthAudioSource = GetComponent<AudioSource>();
        UpdateHealthUI();
        fullScreenDamage.SetActive(false);
    }

    private void UpdateHealthUI()
    {
        float healthRatio = 1 - (currentPlayerHealth / maxPlayerHealth);

        float voroIntensity = Mathf.Lerp(0, voroIntensityStat * voroScalingFactor, healthRatio);
        float vignetteIntensity = Mathf.Lerp(0, vignetteIntensityStat * vignetteScalingFactor, healthRatio);

        damageMaterial.SetFloat(voroIntensityID, voroIntensity);
        damageMaterial.SetFloat(vignetteIntensityID, vignetteIntensity);

        if (currentPlayerHealth < maxPlayerHealth)
        {
            fullScreenDamage.SetActive(true);
        }
        else
        {
            fullScreenDamage.SetActive(false);
        }
    }

    private IEnumerator HurtFlash()
    {
        damageMaterial.SetColor(colorID, Color.red);
        float healthRatio = 1 - (currentPlayerHealth / maxPlayerHealth);
        float initialVoroIntensity = Mathf.Lerp(0, voroIntensityStat * voroScalingFactor, healthRatio);
        float initialVignetteIntensity = Mathf.Lerp(0, vignetteIntensityStat * vignetteScalingFactor, healthRatio);

        // Set intensities to flash values
        damageMaterial.SetFloat(voroIntensityID, 2.0f);
        damageMaterial.SetFloat(vignetteIntensityID, 2.0f);

        healthAudioSource.PlayOneShot(hurtAudio);

        yield return new WaitForSeconds(flashDuration);

        // Lerp back to initial intensities
        float elapsedTime = 0f;
        while (elapsedTime < flashFadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / flashFadeOutTime;
            damageMaterial.SetFloat(voroIntensityID, Mathf.Lerp(2.0f, initialVoroIntensity, lerpFactor));
            damageMaterial.SetFloat(vignetteIntensityID, Mathf.Lerp(2.0f, initialVignetteIntensity, lerpFactor));
            yield return null;
        }

        damageMaterial.SetFloat(voroIntensityID, initialVoroIntensity);
        damageMaterial.SetFloat(vignetteIntensityID, initialVignetteIntensity);
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(regenTimer);

        // Change color to green
        damageMaterial.SetColor(colorID, Color.green);

        while (currentPlayerHealth < maxPlayerHealth)
        {
            currentPlayerHealth += regenSpeed * Time.deltaTime;
            currentPlayerHealth = Mathf.Min(currentPlayerHealth, maxPlayerHealth);
            UpdateHealthUI();
            yield return null;
        }

        // Revert color to original
        damageMaterial.SetColor(colorID, Color.red);

        regenCoroutine = null;
    }

    public void TakePlayerDamage(int damageAmount)
    {
        if (currentPlayerHealth > 0)
        {
            currentPlayerHealth -= damageAmount;
            UpdateHealthUI();

            StartCoroutine(HurtFlash());

            // Restart the regeneration coroutine
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(RegenerateHealth());

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
