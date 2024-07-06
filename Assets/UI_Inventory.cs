using System.Collections;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    public GameObject inventoryBar;
    public float fadeDuration = 2.0f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine; // Coroutine'u saklamak için bir değişken

    private void Start()
    {
        canvasGroup = inventoryBar.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on inventoryBar.");
        }
    }

    public void ShowInventoryBar()
    {
        // Eğer devam eden bir coroutine varsa durdur
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Set initial state
        inventoryBar.SetActive(true);
        canvasGroup.alpha = 1f;

        // Start fade out coroutine after a delay
        fadeCoroutine = StartCoroutine(FadeOutAfterDelay(fadeDuration));
    }

    private IEnumerator FadeOutAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // Ensure final alpha is set to 0
        canvasGroup.alpha = 0f;
        inventoryBar.SetActive(false);
    }
}