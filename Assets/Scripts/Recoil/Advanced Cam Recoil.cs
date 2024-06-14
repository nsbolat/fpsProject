using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class AdvancedCamRecoil : MonoBehaviour
{
  [Header("Recoil Settings: ")] 
  public float rotationSpeed = 6f;
  public float returnSpeed = 25f;
  
  [Space()] 
  
  [Header("Hipfire: ")] 
  public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
  [Space()]
  
  [Header("Aiming: ")] 
  public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);
  [Space()] 
  
  [Header("State: ")] 
  public bool aiming;

  private Vector3 currentRotation;
  private Vector3 Rot;

  private void FixedUpdate()
  {
    currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
    Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.deltaTime);
    transform.localRotation=Quaternion.Euler(Rot);
  }

  public void Fire()
  {
    if (aiming)
    {
      currentRotation += new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
    }
    else
    {
      currentRotation += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
    }
    
  }

  private void Update()
  {
  }
}
