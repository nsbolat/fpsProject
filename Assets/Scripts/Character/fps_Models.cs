using System;
using System.Collections.Generic;
using UnityEngine;

public static class fps_Models
{
    #region - Player -
    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone,
    }
    [Serializable] 
    public class PlayerSettingsModel
    {
        [Header("View Sens Settings")]
        public float ViewXsens;
        public float ViewYsens;
        public float CameraFov;
        
        public bool ViewXinverted;
        public bool ViewYinverted;
        
        [Header("Movement - Walking")] 
        public float walkFowardSpeed;
        public float walkingStrafeSpeed;

        [Header("Movement - Running")]
        public float runFowardSpeed;
        public float runStrafeSpeed;

        [Header("Movement Smooth")] 
        public float movementSmoothing;
        
        [Header("Run Hold")] 
        public bool sprintHold;
        
        [Header("Jumping")] 
        public float jumpingHeight;
        public float jumpingFalloff;
        public float jumpingFallSmooth;

        [Header("Speed Effectors")] 
        public float speedEffector = 1f;
        public float crouchSpeedEffector;
        public float proneSpeedEffector;
        public float fallingSpeedEffector;



    }
    [Serializable]
    public class CharacterStance
    {
        public float cameraHeight;
        public CapsuleCollider StanceCollider;
    }
    #endregion
    
}
