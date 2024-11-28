using System;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
    public class Tyre : MonoBehaviour
{
    private const float MAX_SUSPENSION_OFFSET = 1f;
    private const float MIN_SUSPENSION_OFFSET = -1f;
    private const float SUSPENSION_THRESOLD = -0.2f;
    private const float MAX_TYRE_ROTATION_SPEED = 1000f;

    [SerializeField] private CarController m_carController;
    [SerializeField] private Transform m_wheel;
    [SerializeField] private Vector3 m_wheelRotationAxis = -Vector3.up;

    private CarData m_carData;
    private Transform m_tyreTransform;
    private RaycastHit m_raycastHit;
    private Vector3 m_tyreVelocity;
    private Vector3 m_initialWheelPosition;
    private Vector3 m_springDirection;
    private float m_velocityAlongSpringDirection;
    private Vector3 m_steeringDirection;
    private float m_steeringVelocity;
    private Vector3 m_accelerationDirection;
    private float m_forwardVelocity;
    private bool m_isTyreOnGround;
    private float m_tyreSpeed;

    public bool IsTyreOnGround => m_isTyreOnGround;
    
    private void Start()
    {
        m_tyreTransform = transform;
        m_carData = m_carController.CurrentCarData;
        if (m_wheel)
        {
            m_initialWheelPosition = m_wheel.localPosition;
        }
    }

    private void Update()
    {
        ConfigTyre();
        if (!m_wheel)
        {
            return;
        }
        m_wheel.Rotate(m_wheelRotationAxis,
            MAX_TYRE_ROTATION_SPEED * (m_tyreSpeed / m_carData.MaxCarSpeed ) * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!Physics.Raycast(m_tyreTransform.position, -m_tyreTransform.up, out m_raycastHit, m_carData.SuspensionRestDistance,layerMask: ~m_carData.LayersToIgnore))
        {
            m_isTyreOnGround = false;
            return;
        }
        m_isTyreOnGround = true;
        ApplySuspensionForce();
        ApplySteeringForce();
    }

    private void ConfigTyre()
    {
        m_tyreVelocity = m_carController.CarRigidbody.GetPointVelocity(m_tyreTransform.position);
        m_springDirection = m_tyreTransform.up;
        if (m_springDirection.z < 0f && m_springDirection.z > SUSPENSION_THRESOLD)
        {
            m_springDirection.z = 0f;
        }
        m_velocityAlongSpringDirection = Vector3.Dot(m_springDirection, m_tyreVelocity);
        m_steeringDirection = m_tyreTransform.right;
        m_steeringVelocity = Vector3.Dot(m_steeringDirection,m_tyreVelocity);
        m_accelerationDirection = m_tyreTransform.forward;
        m_forwardVelocity = Vector3.Dot(m_accelerationDirection, m_tyreVelocity);
        m_tyreSpeed = Vector3.Dot(m_accelerationDirection, m_tyreVelocity);
       
    }
    private void ApplySuspensionForce()
    {
        if (Mathf.Approximately(m_carData.SuspensionRestDistance,m_raycastHit.distance))
        {
            return;
        }
        float suspensionOffset = m_carData.SuspensionRestDistance - m_raycastHit.distance;
        suspensionOffset = Mathf.Clamp(suspensionOffset, MIN_SUSPENSION_OFFSET,MAX_SUSPENSION_OFFSET);
        float suspensionForceMagnitude = (suspensionOffset * m_carData.SpringStrength) - (m_velocityAlongSpringDirection * m_carData.SpringDamper);
        Vector3 suspensionForceToApply = suspensionForceMagnitude * m_springDirection;
        SetForceAtPosition(suspensionForceToApply,m_tyreTransform);
        if (m_wheel)
        {
            m_wheel.localPosition = m_initialWheelPosition + new Vector3(0,suspensionOffset,0);
        }
    }

    private void ApplySteeringForce()
    {
        float velocityChange = -m_steeringVelocity * m_carData.TyreGripFactor;
        float acceleration = velocityChange / Time.fixedDeltaTime;
        Vector3 steeringForce = m_steeringDirection * (m_carData.TyreMass * acceleration);
        SetForceAtPosition(steeringForce,m_tyreTransform);
    }
    
    private void SetForceAtPosition(Vector3 forceToAdd, Transform tyreTransform)
    {
        m_carController.CarRigidbody.AddForceAtPosition(forceToAdd, tyreTransform.position);
    }
    
    public void ApplyTorque(bool isForward, float carSpeed, float torqueMultiplier)
    {
        if (!m_isTyreOnGround)
        {
            return;
        }
        float normalizeSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed / m_carData.MaxCarSpeed));
        float torqueMagnitude = m_carData.SpeedXTorqueCurve.Evaluate(normalizeSpeed) * m_carData.TorqueToApply;
        if (!isForward)
        {
            torqueMagnitude = -torqueMagnitude * m_carData.ReverseAccelerationMultiplier;
        }
        Vector3 torque = torqueMagnitude * m_accelerationDirection * torqueMultiplier;
        SetForceAtPosition(torque,m_tyreTransform);
    }

    public void ApplyBrakeForce(float brakeFactor)
    {
        if (!m_isTyreOnGround)
        {
            return;
        }
        float velocityChange = -m_forwardVelocity * brakeFactor;
        float brakePower = velocityChange / Time.fixedDeltaTime;
        Vector3 brakeForce = m_accelerationDirection * (m_carData.TyreMass * brakePower);
        SetForceAtPosition(brakeForce,m_tyreTransform);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int thickness = 5;
        m_tyreTransform = transform;
        Vector3 currPos = m_tyreTransform.position;
        Vector3 veloccity = m_carController.CarRigidbody.GetPointVelocity(currPos);
        Handles.DrawBezier(currPos, (currPos + (m_tyreTransform.right) * veloccity.x), currPos, (currPos + m_tyreTransform.right), Color.red, null, thickness);
        Handles.DrawBezier(currPos, (currPos + (m_tyreTransform.up) * veloccity.y), currPos, (currPos + m_tyreTransform.up), Color.green, null, thickness);
        Handles.DrawBezier(currPos, (currPos + (m_tyreTransform.forward) * -veloccity.z), currPos, (currPos + m_tyreTransform.forward), Color.blue, null, thickness);
    }
#endif
}
    
