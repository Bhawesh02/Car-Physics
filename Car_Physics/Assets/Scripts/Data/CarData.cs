using UnityEngine;

    [CreateAssetMenu(fileName = "New_Car_Data", menuName = "creative/NewCarData")]
    public class CarData : ScriptableObject
    {
        public CarDataType CarDataType;
        public DriveType CarDriveType;
        public float WheelRotationSpeed = 1f;
        public float MaxTyreRotation = 45f;
        [Header("Suspension")]
        public float SpringStrength = 100f;
        public float SpringDamper = 10f;
        public float SuspensionRestDistance = 1f;
        public LayerMask LayersToIgnore;
        [Header("Steering")]
        [Range(0,1)]
        public float TyreGripFactor;
        public float TyreMass;

        [Header("Acceleration")] 
        public float TorqueToApply;
        public float MaxCarSpeed;
        public AnimationCurve SpeedXTorqueCurve;
        [Range(0,1)]
        public float ReverseAccelerationMultiplier;

        [Header("Brakes")] 
        [Range(0, 1)] 
        public float StaticBraceValue = 0.1f;     
        [Range(0,1)]
        public float FrontWheelBrakeFactor;
        [Range(0,1)]
        public float BackWheelBrakeFactor;
    }
