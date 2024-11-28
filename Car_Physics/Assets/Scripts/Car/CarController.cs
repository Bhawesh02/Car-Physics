using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
    public class CarController : MonoBehaviour
{
    [SerializeField] private Tyre[] m_frontTyres;
    [SerializeField] private Tyre[] m_backTyres;
    [SerializeField] private Rigidbody m_carRigidbody;
    [Tooltip("Car State to start the car with:")]
    [SerializeField] 
    private CarStates m_carDriveStartState;
    [SerializeField] private List<CarData> m_carDatas = new();
    [SerializeField] private CarDataType m_currentCarDataType;
    [SerializeField] private Vector3 m_centerOfMassOffset;
    [Header("Bot Control")] 
    [SerializeField]
    private bool m_isBotDriving;
    [Tooltip("Spline To be followed by the bot")] 
    [SerializeField] private SplineComputer m_splineComputer;
    [SerializeField] private float m_pointCheckDistance;
    [Range(0,1)]
    [SerializeField] private float m_splinePercentageIncrement;
    
    private float m_carSpeed;
    private CarData m_currentCarData;
    private StateMachine m_carStateMachine;
    private CarState m_playerDrivingState;
    private CarState m_botDrivingCarState;
    private CarStates m_currentCarState;
    private Vector3 m_carForwardVector;
    private float m_carXAngle;
    
    public CarData CurrentCarData => m_currentCarData;
    public Rigidbody CarRigidbody => m_carRigidbody;
    public bool IsBotDriving => m_isBotDriving;
    public float PointCheckDistance => m_pointCheckDistance;

    private void Awake()
    {
        InitCarStates();
        SwitchCarData();
    }

    private void OnValidate()
    {
        SwitchCarData();
    }

    private void InitCarStates()
    {
        m_carStateMachine = new StateMachine(CarStates.COUNT);
        m_playerDrivingState = new PlayerDrivingCarState(this,m_frontTyres);
        m_botDrivingCarState = new BotDrivingCarState(this,m_frontTyres,m_splineComputer,m_splinePercentageIncrement);
        m_carStateMachine.RegisterState(CarStates.INIT);
        m_carStateMachine.RegisterState(CarStates.PLAYER_DRIVING,m_playerDrivingState);
        m_carStateMachine.RegisterState(CarStates.BOT_DRIVING, m_botDrivingCarState);
        ChangeCarState(m_carDriveStartState);
    }

    private void Start()
    {
        m_carRigidbody.centerOfMass = m_centerOfMassOffset;
    }

    private void SwitchCarData()
    {
        if ( m_carDatas.Count == 0 ||(m_currentCarData && m_currentCarData.CarDataType == m_currentCarDataType))
        {
            return;
        }
        m_currentCarData = m_carDatas.Find(data => data.CarDataType == m_currentCarDataType);
    }
    
    private void Update()
    {
        m_carStateMachine.Update();
        m_carForwardVector = transform.forward;
        m_carSpeed = Vector3.Dot(m_carForwardVector, m_carRigidbody.velocity);
    }
    
    private void FixedUpdate()
    {
        m_carStateMachine.FixedUpdate();
        ApplyBrakes(m_currentCarData.StaticBraceValue);
    }

    private void ChangeCarState(CarStates stateToChangeCarInto)
    {
        if(stateToChangeCarInto == m_currentCarState)
        {
            return;
        }
        m_carStateMachine.SetState(stateToChangeCarInto);
        m_currentCarState = stateToChangeCarInto;
    }
    
    public void AccelerateCar(bool isForward, float accelerationMultiplier = 1f)
    {
        if (m_currentCarData.CarDriveType == DriveType.FORWARD_WHEEL_DRIVE)
        {
            foreach (Tyre frontTyre in m_frontTyres)
            {
                frontTyre.ApplyTorque(isForward, m_carSpeed, accelerationMultiplier);
            }
        }
        else
        {
            foreach (Tyre backTyre in m_backTyres)
            {
                backTyre.ApplyTorque(isForward, m_carSpeed, accelerationMultiplier);
            }
        }
    }
    
    public void ApplyBrakes(float brakeMultiplier = 1f)
    {
        foreach (Tyre frontTyre in m_frontTyres)
        {
            frontTyre.ApplyBrakeForce(m_currentCarData.FrontWheelBrakeFactor * brakeMultiplier);
        }

        foreach (Tyre backTyre in m_backTyres)
        {
            backTyre.ApplyBrakeForce(m_currentCarData.BackWheelBrakeFactor * brakeMultiplier);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_currentCarState == CarStates.BOT_DRIVING)
        {
            m_botDrivingCarState.OnDrawGizmos();
        }
        if (!m_carRigidbody)
        {
            return;
        }
        Vector3 com = m_centerOfMassOffset;
        com += transform.position;
        Gizmos.DrawSphere(com,0.1f);
    }
#endif
}

