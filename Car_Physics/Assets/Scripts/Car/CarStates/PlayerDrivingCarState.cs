using UnityEngine;
    public class PlayerDrivingCarState : CarState
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const float ROTATION_BACK_TO_ZERO_MULTIPLIER = 10f;

    private readonly Tyre[] m_frontTyres;
    
    private float m_verticalAxisInput;
    private float m_horizontalAxisInput;
    private float m_carSpeed;
    private Vector3 m_wheelTurnAngle;
    private float m_initialTyreRotation;
    public PlayerDrivingCarState(CarController carController, Tyre[] frontTyres) : base(carController)
    {
        m_frontTyres = frontTyres;
    }
    public override void OnEnter()
    {
        m_wheelTurnAngle = m_frontTyres[0].transform.localEulerAngles;
        m_initialTyreRotation = m_wheelTurnAngle.y;
    }
    public override void OnUpdate()
    {
        m_verticalAxisInput = Input.GetAxis(VERTICAL_AXIS);
        m_horizontalAxisInput = Input.GetAxis(HORIZONTAL_AXIS);
        RotateTires();
    }
    public override void OnFixedUpdate()
    {
        MoveCar();
    }
    private void MoveCar()
    {
        if (m_verticalAxisInput > 0)
        {
            if (m_carSpeed >= CarController.CurrentCarData.MaxCarSpeed)
            {
                return;
            }
            CarController.AccelerateCar(true);
        }
        else if (m_verticalAxisInput < 0)
        {
            if (m_carSpeed > 0)
            {
                CarController.ApplyBrakes();
            }
            else
            {
                Debug.Log("Reverse");
                CarController.AccelerateCar(false);
            }
        }
    }
    private void RotateTires()
    {
        Transform tyreTransform;
        float wheelRotationSpeed = CarController.CurrentCarData.WheelRotationSpeed;
        if (Mathf.Approximately(m_horizontalAxisInput,m_initialTyreRotation))
        {
            wheelRotationSpeed *= ROTATION_BACK_TO_ZERO_MULTIPLIER;
        }
        foreach (Tyre frontTyre in m_frontTyres)
        {
            tyreTransform = frontTyre.transform ;
            m_wheelTurnAngle.y = Mathf.Lerp(m_wheelTurnAngle.y, 
                (CarController.CurrentCarData.MaxTyreRotation * m_horizontalAxisInput) + m_initialTyreRotation, 
                wheelRotationSpeed * Time.deltaTime);
            tyreTransform.localEulerAngles = m_wheelTurnAngle;
        }
    }
}

