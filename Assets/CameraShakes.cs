using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakes : MonoBehaviour
{
    private fps_CharacterController _fpsCharacterController;
    private Animator _Animator;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
        _fpsCharacterController = GameObject.FindWithTag("Player").GetComponent<fps_CharacterController>();
    }

    private void FixedUpdate()
    {

    }
}
