using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public DefaultInput _defaultInput;
    public DefaultInput.CharacterActions _characterActions;
    public DefaultInput.UIActions _uiActions;

    private void Awake()
    {
        _defaultInput = new DefaultInput();
        _characterActions = _defaultInput.Character;
        _uiActions = _defaultInput.UI;
    }

    private void OnEnable()
    {
        _characterActions.Enable();
        _uiActions.Enable();
    }

    private void OnDisable()
    {
        _characterActions.Disable();
        _uiActions.Disable();
    }
}
