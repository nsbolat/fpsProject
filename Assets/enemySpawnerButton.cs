using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawnerButton : Interactable
{
    public GameObject[] enemyPrefab; // Düşman prefabı
    public Transform[] spawnPoints; // Spawn noktaları
    public float spawnInterval = 3f; // Spawn aralığı (örneğin 3 saniyede bir)
    public bool buttonPress = false;
    private bool spawnedEnemies = false;

    private Renderer buttonRenderer; // Renderer bileşeni
    private MaterialPropertyBlock propertyBlock; // Materyal property bloğu
    private Color emissionColor = Color.black; // Varsayılan emission rengi
    private float emissionIntensity = 5f; // Emission yoğunluğu

    private void Start()
    {
        // Renderer bileşenini al
        buttonRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Materyaldeki varsayılan emission rengini ve intensitesini ayarla
        SetEmissionColor(emissionColor, emissionIntensity);
    }

    protected override void Interact()
    {
        if (!buttonPress)
        {
            InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
            SetMaterialColor(Color.green); // Materyal rengini yeşil yap
            SetEmissionColor(Color.green, emissionIntensity); // Emission rengini yeşil yap
            Debug.Log("Girdi");
            buttonPress = true;
        }
        else
        {
            buttonPress = false;
            SetMaterialColor(Color.red); // Materyal rengini kırmızı yap
            SetEmissionColor(Color.red, emissionIntensity); // Emission rengini siyah yap (kapalı), yoğunluğunu 0 yap

            CancelInvoke("SpawnEnemy");
            if (spawnedEnemies)
            {
                DestroySpawnedEnemies();
            }
        }
    }

    void SetMaterialColor(Color color)
    {
        // Materyal rengini ayarla
        buttonRenderer.material.color = color;
    }

    void SetEmissionColor(Color color, float intensity)
    {
        // Emission rengini ve intensitesini ayarla
        emissionColor = color * intensity;
        propertyBlock.SetColor("_EmissionColor", emissionColor); // Emission rengini ayarla
        buttonRenderer.SetPropertyBlock(propertyBlock);

        // Emission'ı etkinleştir
        buttonRenderer.material.EnableKeyword("_EMISSION");

        // Renderer'ın materyalini güncelle
        buttonRenderer.UpdateGIMaterials();
    }

    void SpawnEnemy()
    {
        // Rastgele bir spawn noktası seç
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Düşmanı spawn noktasında oluştur
        GameObject spawnedEnemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)], spawnPoint.position, spawnPoint.rotation);
        spawnedEnemies = true; // Set flag that enemies have been spawned
    }

    void DestroySpawnedEnemies()
    {
        // Tüm spawnlanmış düşmanları yok et
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies = false; // Reset flag after destroying enemies
    }
}
