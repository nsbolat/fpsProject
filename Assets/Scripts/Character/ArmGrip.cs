using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmGrip : MonoBehaviour
{
    public TwoBoneIKConstraint rightHandIK, leftHandIK;
    public RigBuilder rigbuilder;

    private void Awake()
    {
        rightHandIK = rightHandIK.GetComponent<TwoBoneIKConstraint>();
        leftHandIK = leftHandIK.GetComponent<TwoBoneIKConstraint>();
    }

    public void UpdateIK(Transform rHandTransform, Transform lHandTransform)
    {

        rightHandIK.data.target = rHandTransform;
        leftHandIK.data.target = lHandTransform;
        rigbuilder.Build();
    }
}
            
