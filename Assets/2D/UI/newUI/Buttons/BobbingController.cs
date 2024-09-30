using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingController : MonoBehaviour
{
    public fps_CharacterController mover;
    public GunSystem gunSystem;

    [Header("Bobbing")]
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.05f;
    public float sprintBobbingMultiplier = 1.5f;
    public float midpoint = 0.0f;
    private float timer = 0.0f;

    [Header("Idle Bobbing")]
    public float idleBobbingSpeed = 0.1f;
    public float idleBobbingAmount = 0.02f;
    public Vector3 bobLimitIdle = new Vector3(0.01f, 0.01f, 0.01f);
    public Vector3 multiplierIdle = new Vector3(0.005f, 0.005f, 0.005f);

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public Vector3 travelLimitADS = Vector3.one * 0.0125f;
    public Vector3 bobLimitADS = Vector3.one * 0.005f;
    public Vector3 travelLimitAir = Vector3.one * 0.05f;
    public Vector3 bobLimitAir = Vector3.one * 0.02f;
    Vector3 bobPosition;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    public Vector3 multiplierADS;
    public Vector3 multiplierAir;
    public Vector3 sprintMultiplier;
    Vector3 bobEulerRotation;

    private bool wasGrounded;

    void Start()
    {
        if (mover == null)
        {
            mover = GetComponentInParent<fps_CharacterController>();
        }

        if (gunSystem == null)
        {
            gunSystem = GetComponentInParent<GunSystem>();
        }
    }

    void Update()
    {
        GetInput();

        BobOffset();
        BobRotation();
        CompositePositionRotation();
    }

    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        walkInput = mover.input_Movement;
        lookInput = mover.input_View;
    }

    void BobOffset()
    {
        float waveslice = 0.0f;
        float horizontal = walkInput.x;
        float vertical = walkInput.y;

        float currentBobbingSpeed = bobbingSpeed * Time.deltaTime;
        float currentBobbingAmount = bobbingAmount;

        if (mover.isSprint)
        {
            currentBobbingSpeed *= sprintBobbingMultiplier;
        }

        if (mover._playerStance == fps_Models.PlayerStance.Crouch)
        {
            currentBobbingSpeed *= mover.playerSettings.crouchSpeedEffector;
        }
        else if (mover._playerStance == fps_Models.PlayerStance.Prone)
        {
            currentBobbingSpeed *= mover.playerSettings.proneSpeedEffector;
        }
        else
        {
            // Default to standing speed if not crouching or prone
            currentBobbingSpeed *= mover.playerSettings.speedEffector;
        }
        
        
        if (mover.isGrounded)
        {
            if (!wasGrounded)
            {
                timer = 0.0f;
            }

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0 )
            {
                currentBobbingSpeed = idleBobbingSpeed * Time.deltaTime;
                currentBobbingAmount = idleBobbingAmount;

                waveslice = Mathf.Sin(timer);
                timer += currentBobbingSpeed;
                if (timer > Mathf.PI * 2)
                {
                    timer -= Mathf.PI * 2;
                }

                bobPosition.x = bobLimitIdle.x * Mathf.Cos(timer);
                bobPosition.y = bobLimitIdle.y * Mathf.Sin(timer * 2);
                bobPosition.z = bobLimitIdle.z * Mathf.Sin(timer);
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer += currentBobbingSpeed;
                if (timer > Mathf.PI * 2)
                {
                    timer -= Mathf.PI * 2;
                }

                Vector3 currentBobLimit = gunSystem.isAds ? bobLimitADS : bobLimit;
                Vector3 currentTravelLimit = gunSystem.isAds ? travelLimitADS : travelLimit;

                bobPosition.x = (Mathf.Cos(timer) * currentBobLimit.x * (horizontal > 0 ? 1 : -1)) - (horizontal * currentTravelLimit.x);
                bobPosition.y = (Mathf.Sin(timer * 2) * currentBobLimit.y * (vertical > 0 ? 1 : -1)) - (vertical * currentTravelLimit.y);
                bobPosition.z = Mathf.Sin(timer) * currentBobLimit.z;
            }

            wasGrounded = true;
        }
        else
        {
            timer += currentBobbingSpeed / 2;
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }

            Vector3 currentBobLimit = bobLimitAir;
            Vector3 currentTravelLimit = travelLimitAir;

            bobPosition.x = (Mathf.Cos(timer) * currentBobLimit.x) - (horizontal * currentTravelLimit.x);
            bobPosition.y = (Mathf.Sin(timer * 2) * currentBobLimit.y) - (vertical * currentTravelLimit.y);
            bobPosition.z = Mathf.Sin(timer) * currentBobLimit.z;

            wasGrounded = false;
        }
    }

    void BobRotation()
    {
        Vector3 currentMultiplier;

        if (mover.isGrounded)
        {
            if (Mathf.Abs(walkInput.x) == 0)
            {
                currentMultiplier = multiplierIdle;
                Debug.Log("İdle");
            }
            else
            {
                currentMultiplier = gunSystem.isAds ? multiplierADS : multiplier;
            }
        }
        else
        {
            currentMultiplier = multiplierAir;
            Debug.Log("havada");

        }

        if (mover.isSprint)
        {
            currentMultiplier = Vector3.Scale(currentMultiplier, sprintMultiplier);
            Debug.Log("koşuyor");

        }

        bobEulerRotation.x = (walkInput != Vector2.zero
            ? currentMultiplier.x * (Mathf.Sin(2 * timer))
            : currentMultiplier.x * (Mathf.Sin(2 * timer) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? currentMultiplier.y * Mathf.Cos(timer) : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? currentMultiplier.z * Mathf.Cos(timer) * walkInput.x : 0);

    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * 10f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(bobEulerRotation), Time.deltaTime * 12f);
    }
}

