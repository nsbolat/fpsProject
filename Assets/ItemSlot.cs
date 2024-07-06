using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI label, slotText;

    public void SetIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(icon != null);
        }
    }

    public void SetLabel(string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    public void SetSlotIndex(int text)
    {
        if (slotText!=null)
        {
            slotText.text = text.ToString();
        }
    }
    
}