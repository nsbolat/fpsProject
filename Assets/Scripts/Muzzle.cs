using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    public AudioClip muzzleSound;
    private Transform firePoint;
    
    //ateş patlama particles
    private GameObject prefabFlashParticles;
    
    //Ateş Işık
    private GameObject prefabFlashLight;
    private float flashLightDuration; //ışık açık kalma süresi
    private Vector3 flashLightOffset;

    void muzzleEffect()
    {
        
    }


}
