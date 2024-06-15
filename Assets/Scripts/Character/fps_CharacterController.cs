using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static fps_Models;

public class fps_CharacterController : MonoBehaviour
{
    private CharacterController _characterController;
    private DefaultInput defaultInput;
    public Vector2 input_Movement; //denetleyiciden gelen ham girdi
    public Vector2 input_View; //denetleyiciden gelen ham girdi
    public float currentViewXsens;
    public float currentViewYsens;


    public Vector2 newCameraRotasyon;
    private Vector3 newCharacterRotasyon;
    
    [Header("References")] 
    public Transform cameraHolder;
    public Transform feetTransform;
    public Rigidbody rb;

    [Header("Settings")] 
    public PlayerSettingsModel playerSettings;
    public float ViewClampYmin = -70;
    public float ViewClampYmax = 80;
    public LayerMask playerMask;
    
    [Header("Gravity")] 
    public float gravityAmount = -9.81f;
    public float gravityMin;
    public Vector3 velocity;
    public bool isGrounded;
    public float groundDistance = 0.4f;

    [Header("Jump")] 
    public Vector3 jumpingForce;
    public Vector3 jumpingForceVelocity;

    [Header("Stance")] 
    public PlayerStance _playerStance;
    public float playerStanceSmooth;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    private float stanceCheckErrorMargin = 0.05f;
    
    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenter;
    private Vector3 stanceCapsuleCenterVelocity;

    private float stanceCapsuleHeight;
    private float stanceCapsuleHeightVelocity;

    private bool isSprint;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;

    //[Header("Anim")] 
   // public Animator fpsAnim;
   public Camera cam;


    private void Awake()
    {
        defaultInput = new DefaultInput();
        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crouch.performed += e => Crouch();
        defaultInput.Character.Prone.performed += e => Prone();
        defaultInput.Character.Run.performed += e => ToggleSprint();
        defaultInput.Character.RunReleased.performed += e => stopSprint();


        cameraHolder = GameObject.Find("CameraHolder").transform;
        feetTransform = GameObject.Find("feetTransform").transform;
        rb = GetComponent<Rigidbody>();
        
        defaultInput.Enable();//denetleyici etkinleştirme

        newCameraRotasyon = cameraHolder.localRotation.eulerAngles;
        newCharacterRotasyon = transform.localRotation.eulerAngles;
        _characterController = GetComponent<CharacterController>();
        
        cameraHeight = cameraHolder.localPosition.y;
        cam.fieldOfView = playerSettings.CameraFov;

        currentViewYsens = playerSettings.ViewYsens;
        currentViewXsens = playerSettings.ViewXsens;
    }

    private void Update()
    {
        calculateView();
        calculateMovement();
        calculateJump();
        calculateStance();
        isGrounded = Physics.CheckSphere(feetTransform.position, groundDistance,playerMask);
        if (isGrounded && velocity.y<0)
        {
            velocity.y = -2f;
        }
    }
    
    private void calculateView()
    {
        newCharacterRotasyon.y += currentViewXsens * (playerSettings.ViewXinverted ? -input_View.x: input_View.x ) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotasyon);
        
        newCameraRotasyon.x += currentViewYsens * (playerSettings.ViewYinverted ? input_View.y: -input_View.y ) * Time.deltaTime;
        newCameraRotasyon.x = Mathf.Clamp(newCameraRotasyon.x, ViewClampYmin, ViewClampYmax); // X rotasyonu limitleme
        
        cameraHolder.localRotation = Quaternion.Euler(newCameraRotasyon);
    }

    private void calculateMovement()
    {

        
        //Movement atama
        var verticalSpeed = playerSettings.walkFowardSpeed;
        var horizontalSpeed =  playerSettings.walkingStrafeSpeed;
        
        if (isSprint) //koşuyorsa
        {
            verticalSpeed = playerSettings.runFowardSpeed;
            horizontalSpeed = playerSettings.runStrafeSpeed;
        }
        
        //koşma
        if (input_Movement.magnitude <= 0.2f && !playerSettings.sprintHold)
        {
            isSprint = false;
        }
        
        // Effectors
        if (!isGrounded)
        {
            playerSettings.speedEffector = playerSettings.fallingSpeedEffector;
        }else if (_playerStance == PlayerStance.Crouch)
        {
            playerSettings.speedEffector = playerSettings.crouchSpeedEffector;
        }else if (_playerStance == PlayerStance.Prone)
        {
            playerSettings.speedEffector = playerSettings.proneSpeedEffector;
        }
        else
        {
            playerSettings.speedEffector = 1;
        }
        verticalSpeed *= playerSettings.speedEffector;
        horizontalSpeed *= playerSettings.speedEffector;

        //newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0,verticalSpeed * input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity,_characterController.isGrounded ? playerSettings.movementSmoothing : playerSettings.jumpingFallSmooth);
        newMovementSpeed = new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
        //
        if (_characterController.isGrounded && velocity.y<0f)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravityAmount * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);
        _characterController.Move(movementSpeed);
    }

    /* private void handleAnimation() //test yürüme anim
    {
        if (newMovementSpeed == Vector3.zero)
        {
            fpsAnim.SetFloat("Speed",0f,0.2f,Time.deltaTime);
        }else if (newMovementSpeed != Vector3.zero && !isSprint)
        {
          fpsAnim.SetFloat("Speed",0.5f,0.2f,Time.deltaTime);
        }else if (newMovementSpeed != Vector3.zero && isSprint)
        {
            fpsAnim.SetFloat("Speed",1f,0.2f,Time.deltaTime);

        }
    }*/
    
    private void calculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.jumpingFalloff); //naptık aq
    }
    
    private void calculateStance()
    {
        var currentStance = playerStandStance;
        if (_playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (_playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }
        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y,currentStance.cameraHeight, ref cameraHeightVelocity,playerStanceSmooth);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        _characterController.height = Mathf.SmoothDamp(_characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity,playerStanceSmooth);
        _characterController.center = Vector3.SmoothDamp(_characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity,playerStanceSmooth);
    }
    
    private void Jump()
    {
        if (!isGrounded || _playerStance==PlayerStance.Prone)
        {
            return;
        }
        if (_playerStance==PlayerStance.Crouch)
        {
            if (stanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            _playerStance = PlayerStance.Stand;
            return;
        }
        
        //jump
        //jumpingForce = Vector3.up * playerSettings.jumpingHeight * -2f * gravityAmount;
        //velocity.y = 0f;
        velocity.y = Mathf.Sqrt(playerSettings.jumpingHeight * -2f * gravityAmount);
    }

    private void Crouch()
    {
        if (_playerStance == PlayerStance.Crouch)
        {
            if (stanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            _playerStance = PlayerStance.Stand;
            return;
        }
        
        if (stanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }
        
        _playerStance = PlayerStance.Crouch;
    }

    private void Prone()
    {
        if (_playerStance == PlayerStance.Prone)
        {
            if (stanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            
            _playerStance = PlayerStance.Stand;
            return;
        }
        
        if (stanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }
        
        _playerStance = PlayerStance.Prone;
        
    }

    private bool stanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x,feetTransform.position.y + _characterController.radius + stanceCheckErrorMargin,feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x,feetTransform.position.y - _characterController.radius - stanceCheckErrorMargin + stanceCheckHeight,feetTransform.position.z);

        return Physics.CheckCapsule(start,end,_characterController.radius,playerMask);
    }
    private void ToggleSprint()
    {
        if (input_Movement.magnitude <= 0.2f && !playerSettings.sprintHold)
        {
            isSprint = false;
            return;
        }
        isSprint = !isSprint;
    }  
    private void stopSprint()
    {
        if (playerSettings.sprintHold)
        {
            isSprint = false;
        }
    }
    
}
