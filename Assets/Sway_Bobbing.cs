using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayNBobScript : MonoBehaviour
{
    public fps_CharacterController mover;
    public GunSystem gunSystem; // GunSystem scriptine referans

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public float stepADS = 0.005f; // ADS için sway değeri
    public float maxStepDistanceADS = 0.03f; // ADS için max sway değeri
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    public float rotationStepADS = 2f; // ADS için sway rotasyon değeri
    public float maxRotationStepADS = 2.5f; // ADS için max sway rotasyon değeri
    Vector3 swayEulerRot;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.05f;
    public float sprintBobbingMultiplier = 1.5f;
    public float midpoint = 0.0f;
    private float timer = 0.0f;

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public Vector3 travelLimitADS = Vector3.one * 0.0125f; // ADS için travel limit değeri
    public Vector3 bobLimitADS = Vector3.one * 0.005f; // ADS için bob limit değeri
    Vector3 bobPosition;
    
    [Header("Bob Rotation")]
    public Vector3 multiplier;
    public Vector3 multiplierADS; // ADS için multiplier değeri
    public Vector3 sprintMultiplier; // Sprint için multiplier değeri
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

        Sway();
        SwayRotation();
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

    void Sway()
    {
        float currentStep = gunSystem.isAds ? stepADS : step;
        float currentMaxStepDistance = gunSystem.isAds ? maxStepDistanceADS : maxStepDistance;

        Vector3 invertLook = lookInput * -currentStep * Time.deltaTime;
        invertLook.x = Mathf.Clamp(invertLook.x, -currentMaxStepDistance, currentMaxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -currentMaxStepDistance, currentMaxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        float currentRotationStep = gunSystem.isAds ? rotationStepADS : rotationStep;
        float currentMaxRotationStep = gunSystem.isAds ? maxRotationStepADS : maxRotationStep;

        Vector2 invertLook = lookInput * -currentRotationStep * Time.deltaTime;
        invertLook.x = Mathf.Clamp(invertLook.x, -currentMaxRotationStep, currentMaxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -currentMaxRotationStep, currentMaxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        float waveslice = 0.0f;
        float horizontal = walkInput.x;
        float vertical = walkInput.y;

        float currentBobbingSpeed = bobbingSpeed * Time.deltaTime;

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
                // Player just landed
                timer = 0.0f; // Reset timer to give a landing impact effect
            }

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                timer = 0.0f;
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer += currentBobbingSpeed;
                if (timer > Mathf.PI * 2)
                {
                    timer -= Mathf.PI * 2;
                }
            }

            if (waveslice != 0)
            {
                Vector3 currentBobLimit = gunSystem.isAds ? bobLimitADS : bobLimit;
                Vector3 currentTravelLimit = gunSystem.isAds ? travelLimitADS : travelLimit;

                // Bobbing hareketinde yön faktörünü ekliyoruz
                bobPosition.x = (Mathf.Cos(timer) * currentBobLimit.x * (horizontal > 0 ? 1 : -1)) - (horizontal * currentTravelLimit.x);
                bobPosition.y = (Mathf.Sin(timer * 2) * currentBobLimit.y * (vertical > 0 ? 1 : -1)) - (vertical * currentTravelLimit.y);
            }
            else
            {
                bobPosition.x = 0;
                bobPosition.y = midpoint;
            }

            wasGrounded = true;
        }
        else
        {
            // Apply bobbing effect while in the air
            if (wasGrounded)
            {
                // Player just jumped
                timer = 0.0f; // Reset timer to give an initial jump impact effect
            }

            // Add a small bobbing effect when in the air
            waveslice = Mathf.Sin(timer);
            timer += currentBobbingSpeed / 2; // Slower bobbing speed in the air
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }

            Vector3 currentBobLimit = gunSystem.isAds ? bobLimitADS : bobLimit;
            Vector3 currentTravelLimit = gunSystem.isAds ? travelLimitADS : travelLimit;

            bobPosition.x = (Mathf.Cos(timer) * currentBobLimit.x) - (horizontal * currentTravelLimit.x);
            bobPosition.y = (Mathf.Sin(timer * 2) * currentBobLimit.y) - (vertical * currentTravelLimit.y);

            wasGrounded = false;
        }
    }

    void BobRotation()
    {
        Vector3 currentMultiplier = gunSystem.isAds ? multiplierADS : multiplier;
        
        if (mover.isSprint)
        {
            currentMultiplier = Vector3.Scale(currentMultiplier, sprintMultiplier);
        }

        bobEulerRotation.x = (walkInput != Vector2.zero
            ? currentMultiplier.x * (Mathf.Sin(2 * timer))
            : currentMultiplier.x * (Mathf.Sin(2 * timer) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? currentMultiplier.y * Mathf.Cos(timer) : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? currentMultiplier.z * Mathf.Cos(timer) * walkInput.x : 0);
    }
}
