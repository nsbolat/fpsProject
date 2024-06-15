using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyUIHealthBar : MonoBehaviour
{
    public Transform target;
    public Image frontgroundImage, backgroundImage;
    public Vector3 offsetBar;

    private void Awake()
    {
        frontgroundImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = (target.position - Camera.main.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction, Camera.main.transform.forward) <= 0.0f;

        // Eğer hedef kameranın arkasındaysa sağlık barlarını devre dışı bırak
        if (isBehind)
        {
            frontgroundImage.enabled = false;
            backgroundImage.enabled = false;
        }
        else
        {
            // Kameradan hedefe doğru bir ışın gönder
            Ray ray = new Ray(Camera.main.transform.position, target.position - Camera.main.transform.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Eğer ışın hedefe ulaştıysa, sağlık barlarını etkinleştir
                if (hit.transform == target)
                {
                    frontgroundImage.enabled = true;
                    backgroundImage.enabled = true;
                }
                else
                {
                    // Eğer ışın hedefe ulaşmadan başka bir nesneye çarptıysa, sağlık barlarını devre dışı bırak
                    frontgroundImage.enabled = false;
                    backgroundImage.enabled = false;
                }
            }
            else
            {
                // Eğer ışın hiçbir şeye çarpmadıysa (nadiren de olsa), sağlık barlarını devre dışı bırak
                frontgroundImage.enabled = false;
                backgroundImage.enabled = false;
            }
        }

        // Sağlık barını hedefin üzerinde konumlandır
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position + offsetBar);
    }

    public void setHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * percentage;
        frontgroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
