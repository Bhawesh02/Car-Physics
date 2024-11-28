using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;
    public class BotDrivingCarState : CarState
{
    private readonly Transform m_carTransform;
    private readonly Tyre[] m_frontTyres;
    private readonly float m_initialTyreRotation;
    
    private float m_tyreRotateAngle;
    private Vector3 m_wheelTurnAngle;
    private bool m_isCarOnRamp;
    private SplineComputer m_splineComputer;
    private float m_currentSplinePoint;
    private Vector3 m_targetPosition;
    private Vector3 m_vectorToTarget;
    private Vector3 m_carMoveDirection;
    private bool m_botDrivingEnabled = true;
    private float m_splinePercentageIncrement;
    
    public BotDrivingCarState(CarController carController, Tyre[] frontTyres, SplineComputer splineComputer, float splinePercentageIncrement) : base(carController)
    {
        m_carTransform = carController.transform;
        m_frontTyres = frontTyres;
        m_wheelTurnAngle = m_frontTyres[0].transform.localEulerAngles;
        m_initialTyreRotation = m_wheelTurnAngle.y;
        if (!splineComputer)
        {
            return;
        }
        m_splineComputer = splineComputer;
        m_splinePercentageIncrement = splinePercentageIncrement;
        m_currentSplinePoint = m_splinePercentageIncrement;
    }
    
    public override void OnUpdate()
    {
        if (!m_botDrivingEnabled || !CarController.IsBotDriving)
        {
            return;
        }
        UpdatePointToReach();
        RotateCar();
    }

    public override void OnFixedUpdate()
    {
        if (!m_botDrivingEnabled)
        {
            return;
        }
        m_carMoveDirection = m_carTransform.forward;
        CarController.AccelerateCar(true);
    }
    
    private void UpdatePointToReach()
    {
        m_targetPosition = m_splineComputer.EvaluatePosition(m_currentSplinePoint);
        m_vectorToTarget = m_targetPosition - m_carTransform.position;
        m_vectorToTarget.y = 0f;
        if (m_vectorToTarget.magnitude > CarController.PointCheckDistance)
        {
            return;
        }
        m_currentSplinePoint += m_splinePercentageIncrement;
        if (m_currentSplinePoint < 1)
        {
            return;
        }
        if (m_splineComputer.isClosed)
        {
            m_currentSplinePoint = 0;
        }
        else
        {
            m_botDrivingEnabled = false;
        }
    }
    
    private void RotateCar()
    {
        m_tyreRotateAngle = Vector3.SignedAngle(m_carMoveDirection, m_vectorToTarget, Vector3.up);
        float wheelRotationSpeed = CarController.CurrentCarData.WheelRotationSpeed;
        Transform tyreTransform;
        m_tyreRotateAngle = Mathf.Clamp(m_tyreRotateAngle,
            -CarController.CurrentCarData.MaxTyreRotation,
            CarController.CurrentCarData.MaxTyreRotation);
        m_tyreRotateAngle += m_initialTyreRotation;
        Quaternion finalRotation;
        Quaternion tyreRotation;
        foreach (Tyre frontTyre in m_frontTyres)
        {
            tyreTransform = frontTyre.transform;
            finalRotation = Quaternion.AngleAxis(m_tyreRotateAngle,Vector3.up);
            tyreRotation = tyreTransform.localRotation;
            Debug.Log($"Current Rotaiton: {tyreRotation}, finalRotation : {finalRotation}");
            tyreTransform.localRotation = Quaternion.Lerp(tyreRotation, finalRotation, wheelRotationSpeed * Time.deltaTime);
        }
    }
    
    public override void OnDrawGizmos()
    {
        int thickness = 2;
        float lineLenght = 10f;
        Vector3 currPos = m_carTransform.position;
        Vector3 endPos = m_targetPosition;
        endPos.y = currPos.y;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(currPos, endPos);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_targetPosition, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currPos, CarController.PointCheckDistance);
    }
    
}

