using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_UI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TextMeshProUGUI interactTextUI;

    private void Awake()
    {
        interactTextUI = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string promptMessage)
    {
        interactTextUI.text = promptMessage;
    }
}
