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
        if (_fpsCharacterController.isSprint && _fpsCharacterController.input_Movement.magnitude!=0 && _fpsCharacterController.isGrounded)
        {
            _Animator.SetBool("isSprint",true);
        }
        else
        {
            _Animator.SetBool("isSprint",false);
        }

        if (_fpsCharacterController.input_Movement.magnitude!=0 && !_fpsCharacterController.isSprint && _fpsCharacterController.isGrounded)
        {
            _Animator.SetBool("isWalk",true);
        }
        else
        {
            _Animator.SetBool("isWalk",false);

        }
    }
}
