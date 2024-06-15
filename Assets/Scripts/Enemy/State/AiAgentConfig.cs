using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime =1f, maxDistance = 1f;
    public float dieForce=10f;
    public float maxSightDistance = 5.0f;
}
