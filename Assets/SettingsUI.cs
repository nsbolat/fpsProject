using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public Slider viewXsensSlider;
    public Slider viewYsensSlider;
    public Toggle viewXinvertedToggle;
    public Toggle viewYinvertedToggle;

    // Diğer ayarları buraya ekleyin

    private void Start()
    {
        // Slider ve Toggle değerlerini başlangıçta yükleyin
        LoadSettings();

        // Slider ve Toggle değişikliklerine event ekleyin
        viewXsensSlider.onValueChanged.AddListener(delegate { OnViewXsensChanged(); });
        viewYsensSlider.onValueChanged.AddListener(delegate { OnViewYsensChanged(); });
        viewXinvertedToggle.onValueChanged.AddListener(delegate { OnViewXinvertedChanged(); });
        viewYinvertedToggle.onValueChanged.AddListener(delegate { OnViewYinvertedChanged(); });
    }

    private void LoadSettings()
    {
        // Ayarları `PlayerSettingsModel`'dan yükleyin
        viewXsensSlider.value = fps_Models.playerSettings.ViewXsens;
        viewYsensSlider.value = fps_Models.playerSettings.ViewYsens;
        viewXinvertedToggle.isOn = fps_Models.playerSettings.ViewXinverted;
        viewYinvertedToggle.isOn = fps_Models.playerSettings.ViewYinverted;

        // Diğer ayarları buraya ekleyin
    }

    public void OnViewXsensChanged()
    {
        fps_Models.playerSettings.ViewXsens = viewXsensSlider.value;
        PlayerPrefs.SetFloat("ViewXsens", viewXsensSlider.value);
    }

    public void OnViewYsensChanged()
    {
        fps_Models.playerSettings.ViewYsens = viewYsensSlider.value;
        PlayerPrefs.SetFloat("ViewYsens", viewYsensSlider.value);
    }

    public void OnViewXinvertedChanged()
    {
        fps_Models.playerSettings.ViewXinverted = viewXinvertedToggle.isOn;
        PlayerPrefs.SetInt("ViewXinverted", viewXinvertedToggle.isOn ? 1 : 0);
    }

    public void OnViewYinvertedChanged()
    {
        fps_Models.playerSettings.ViewYinverted = viewYinvertedToggle.isOn;
        PlayerPrefs.SetInt("ViewYinverted", viewYinvertedToggle.isOn ? 1 : 0);
    }

    // Diğer ayar değişiklik fonksiyonlarını buraya ekleyin
}
