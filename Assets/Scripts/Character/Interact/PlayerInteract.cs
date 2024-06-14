using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;

    [SerializeField] private LayerMask mask;
    private Player_UI _playerUI;
    private InputManager inputManager;
    private void Awake()
    {
        
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        _playerUI = GetComponent<Player_UI>();
        inputManager = GetComponent<InputManager>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin,ray.direction*distance);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() !=null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                _playerUI.UpdateText(interactable.promptMessage);
                
                if (inputManager._characterActions.Interact.triggered)//Etkileşimlinin içindeki fonksiyonu çalıştır.
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}