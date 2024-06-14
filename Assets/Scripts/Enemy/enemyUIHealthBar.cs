using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyUIHealthBar : MonoBehaviour
{
    public Transform target;
    public Image frontgroundImage,backgroundImage;
    public Vector3 offestBar;
    

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = (target.position - Camera.main.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction, Camera.main.transform.forward) <= 0.0f;
        frontgroundImage.enabled = !isBehind;
        backgroundImage.enabled = !isBehind;
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position + offestBar);
    }

    public void setHealthBarPercentage(float percentage)
    {
        float parentWith = GetComponent<RectTransform>().rect.width;
        float witdh = parentWith * percentage;
        frontgroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,witdh);
    }
}
