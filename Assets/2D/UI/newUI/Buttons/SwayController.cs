using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayController : MonoBehaviour
{
    public GunSystem gunSystem;

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public float stepADS = 0.005f;
    public float maxStepDistanceADS = 0.03f;
    public float stepAir = 0.02f;
    public float maxStepDistanceAir = 0.1f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    public float rotationStepADS = 2f;
    public float maxRotationStepADS = 2.5f;
    public float rotationStepAir = 6f;
    public float maxRotationStepAir = 7.5f;
    Vector3 swayEulerRot;

    public float smooth = 10f;
    public float smoothRot = 12f;
    public float returnSmooth = 5f;
    public float returnSmoothRot = 6f;

    void Start()
    {
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
        CompositePositionRotation();
    }

    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        walkInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void Sway()
    {
        float currentStep;
        float currentMaxStepDistance;

        if (IsGrounded()) // Replace mover.isGrounded
        {
            currentStep = gunSystem.isAds ? stepADS : step;
            currentMaxStepDistance = gunSystem.isAds ? maxStepDistanceADS : maxStepDistance;
        }
        else
        {
            currentStep = stepAir;
            currentMaxStepDistance = maxStepDistanceAir;
        }

        if (lookInput.sqrMagnitude > 0.1f) // Only apply sway if there's input
        {
            Vector3 invertLook = lookInput * -currentStep * Time.deltaTime;
            invertLook.x = Mathf.Clamp(invertLook.x, -currentMaxStepDistance, currentMaxStepDistance);
            invertLook.y = Mathf.Clamp(invertLook.y, -currentMaxStepDistance, currentMaxStepDistance);
            swayPos = invertLook;
        }
        else
        {
            // Return to original position
            swayPos = Vector3.Lerp(swayPos, Vector3.zero, returnSmooth * Time.deltaTime);
        }
    }

    void SwayRotation()
    {
        float currentRotationStep;
        float currentMaxRotationStep;

        if (IsGrounded()) // Replace mover.isGrounded
        {
            currentRotationStep = gunSystem.isAds ? rotationStepADS : rotationStep;
            currentMaxRotationStep = gunSystem.isAds ? maxRotationStepADS : maxRotationStep;
        }
        else
        {
            currentRotationStep = rotationStepAir;
            currentMaxRotationStep = maxRotationStepAir;
        }

        if (lookInput.sqrMagnitude > 0.1f) // Only apply sway if there's input
        {
            Vector2 invertLook = lookInput * -currentRotationStep * Time.deltaTime;
            invertLook.x = Mathf.Clamp(invertLook.x, -currentMaxRotationStep, currentMaxRotationStep);
            invertLook.y = Mathf.Clamp(invertLook.y, -currentMaxRotationStep, currentMaxRotationStep);
            swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
        }
        else
        {
            // Return to original rotation
            swayEulerRot = Vector3.Lerp(swayEulerRot, Vector3.zero, returnSmoothRot * Time.deltaTime);
        }
    }

    void CompositePositionRotation()
    {
        // Interpolating the position with a smooth factor
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos, smooth * Time.deltaTime);

        // Interpolating the rotation with a smooth rotation factor
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(swayEulerRot), smoothRot * Time.deltaTime);
    }

    bool IsGrounded()
    {
        // Implement a method to check if the player is grounded
        // This could be based on a raycast or other methods depending on your game
        return true;
    }
}
