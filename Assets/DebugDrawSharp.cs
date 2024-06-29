using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawSharp : MonoBehaviour
{
   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(transform.position,0.05f);
   }
}
