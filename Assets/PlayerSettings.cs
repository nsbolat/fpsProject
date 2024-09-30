using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings Instance { get; private set; }
    public fps_Models.PlayerSettingsModel playerSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // PlayerSettingsModel nesnesini başlat
        if (playerSettings == null)
        {
            playerSettings = new fps_Models.PlayerSettingsModel();
        }
    }
    
    public void OnViewXsensChanged(float value)
    {
        playerSettings.ViewXsens = value;
        PlayerPrefs.SetFloat("ViewXsens", value);
    }

    public void OnViewYsensChanged(float value)
    {
        playerSettings.ViewYsens = value;
        PlayerPrefs.SetFloat("ViewYsens", value);
    }
}
