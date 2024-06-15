using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    #region Variables
    [Header("SWAY")]

    [Header("Position")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;
    public fps_CharacterController playerCotroller;
    private Vector3 originalPosition;
    
    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotationAmount=12f;
    private Quaternion originalRotation;

    [Header("Space")] 
    public bool rotatitionX = true;
    public bool rotatitionY = true;
    public bool rotatitionZ = true;


    private float t_x_mouse;
    private float t_y_mouse;

    [Header("Bobbing")] 
    public float speedCurve;

    public float curveSin
    {
        get => Mathf.Sin(speedCurve);
    }
    public float curveCos
    {
        get => Mathf.Cos(speedCurve);
    }

    [Header("Bobbing")] 
    private Vector3 bobEulerRotation;
    
    
    public Vector3 travelLimit = Vector2.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.1f;
    [Space]
    [Header("Aiming")] 
    public Vector3 travelLimitAim = Vector2.one * 0.025f;
    public Vector3 bobLimitAim = Vector3.one * 0.1f;
    public AdvancedWeaponRecoil AdvancedWeaponRecoil;
    public Vector3 bobPosition;


    #endregion

    #region MonoBehavior Callbacks

    private void Start()
    {
        playerCotroller = GameObject.FindGameObjectWithTag("Player").GetComponent<fps_CharacterController>();
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        calculateSway();
        MoveSway();
        TiltSway();
        bobOffset();
    }

    #endregion

    #region Private Methods

    
    void bobOffset()
    {
        if (AdvancedWeaponRecoil.aiming)
        {
            bobPosition.x= curveCos*bobLimitAim.x * (playerCotroller.isGrounded?0: 0) - (playerCotroller.input_Movement.x * travelLimitAim.x);

            bobPosition.y = (curveSin * bobLimitAim.y) - (playerCotroller.rb.velocity.y * travelLimitAim.y);
            bobPosition.z = -(playerCotroller.input_Movement.y * travelLimitAim.z);
        }
        else
        {
            bobPosition.x= curveCos*bobLimit.x * (playerCotroller.isGrounded?0: 0) - (playerCotroller.input_Movement.x * travelLimit.x);

            bobPosition.y = (curveSin * bobLimit.y) - (playerCotroller.rb.velocity.y * travelLimit.y);
            bobPosition.z = -(playerCotroller.input_Movement.y * travelLimit.z);
        }

    }
    
    private void calculateSway()
    {
        //controls
        t_x_mouse = Input.GetAxis("Mouse X");
        t_y_mouse = Input.GetAxis("Mouse Y");
    }
    private void MoveSway()
    {
        //hedef rotasyonu hesapla
        float moveX = Mathf.Clamp(t_x_mouse * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(t_y_mouse * amount, -maxAmount, maxAmount);

        Vector3 target_position = new Vector3(-moveX, -moveY, 0);

        //hedef rotasyona çevir
        transform.localPosition = Vector3.Lerp(transform.localPosition, target_position + originalPosition + bobPosition, smoothAmount * Time.deltaTime);
    }

    private void TiltSway()
    {       
        //hedef rotasyonu hesapla
        
        float tiltY = Mathf.Clamp(t_x_mouse * rotationAmount, -maxRotationAmount, rotationAmount);
        float tiltX = Mathf.Clamp(t_y_mouse * rotationAmount, -maxRotationAmount, rotationAmount);

        Quaternion target_rotation = Quaternion.Euler(new Vector3(
            rotatitionX ? -tiltY : 0f, 
            rotatitionX ? tiltX : 0f,  
            rotatitionZ ? tiltY: 0f));

        //hedef rotasyona çevir
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target_rotation * originalRotation, smoothRotationAmount * Time.deltaTime);

    }


    #endregion
}
