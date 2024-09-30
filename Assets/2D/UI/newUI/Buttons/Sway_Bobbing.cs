using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway_Bobbing : MonoBehaviour
{
    public fps_CharacterController mover;
    public GunSystem gunSystem; // GunSystem scriptine referans

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public float stepADS = 0.005f; // ADS için sway değeri
    public float maxStepDistanceADS = 0.03f; // ADS için max sway değeri
    public float stepAir = 0.02f; // Havadayken sway değeri
    public float maxStepDistanceAir = 0.1f; // Havadayken max sway değeri
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    public float rotationStepADS = 2f; // ADS için sway rotasyon değeri
    public float maxRotationStepADS = 2.5f; // ADS için max sway rotasyon değeri
    public float rotationStepAir = 6f; // Havadayken sway rotasyon değeri
    public float maxRotationStepAir = 7.5f; // Havadayken max sway rotasyon değeri
    Vector3 swayEulerRot;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.05f;
    public float sprintBobbingMultiplier = 1.5f;
    public float midpoint = 0.0f;
    private float timer = 0.0f;

    [Header("Idle Bobbing")]
    public float idleBobbingSpeed = 0.1f;
    public float idleBobbingAmount = 0.02f;
    public Vector3 bobLimitIdle = new Vector3(0.01f, 0.01f, 0.01f); // Idle bobbing limit değeri
    public Vector3 multiplierIdle = new Vector3(0.005f, 0.005f, 0.005f); // Idle bobbing multiplier değeri

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public Vector3 travelLimitADS = Vector3.one * 0.0125f; // ADS için travel limit değeri
    public Vector3 bobLimitADS = Vector3.one * 0.005f; // ADS için bob limit değeri
    public Vector3 travelLimitAir = Vector3.one * 0.05f; // Havadayken travel limit değeri
    public Vector3 bobLimitAir = Vector3.one * 0.02f; // Havadayken bob limit değeri
    Vector3 bobPosition;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    public Vector3 multiplierADS; // ADS için multiplier değeri
    public Vector3 multiplierAir; // Havadayken multiplier değeri
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
        Debug.Log(walkInput);
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
        float currentStep;
        float currentMaxStepDistance;

        if (mover.isGrounded)
        {
            currentStep = gunSystem.isAds ? stepADS : step;
            currentMaxStepDistance = gunSystem.isAds ? maxStepDistanceADS : maxStepDistance;
        }
        else
        {
            currentStep = stepAir;
            currentMaxStepDistance = maxStepDistanceAir;
        }

        Vector3 invertLook = lookInput * -currentStep * Time.deltaTime;
        invertLook.x = Mathf.Clamp(invertLook.x, -currentMaxStepDistance, currentMaxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -currentMaxStepDistance, currentMaxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        float currentRotationStep;
        float currentMaxRotationStep;

        if (mover.isGrounded)
        {
            currentRotationStep = gunSystem.isAds ? rotationStepADS : rotationStep;
            currentMaxRotationStep = gunSystem.isAds ? maxRotationStepADS : maxRotationStep;
        }
        else
        {
            currentRotationStep = rotationStepAir;
            currentMaxRotationStep = maxRotationStepAir;
        }

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
        float currentBobbingAmount = bobbingAmount;

        if (mover.isSprint)
        {
            currentBobbingSpeed *= sprintBobbingMultiplier;
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
                // Idle bobbing
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
            // Havadayken bobbing
            timer += currentBobbingSpeed / 2; // Havadayken bobbing daha yavaş
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
                currentMultiplier = multiplierIdle; // Idle bobbing multiplier
            }
            else
            {
                currentMultiplier = gunSystem.isAds ? multiplierADS : multiplier;
            }
        }
        else
        {
            currentMultiplier = multiplierAir;
        }

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
