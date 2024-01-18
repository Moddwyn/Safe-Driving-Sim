using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public float maxLaneAngle;
    public float motorForce;
    public float breakForce;
    public float maxSteerAngle;

    [HorizontalLine]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    WheelCollider[] wheelColliders;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [HorizontalLine]
    [ReadOnly] public float horizontalInput;
    [ReadOnly] public float verticalInput;
    [ReadOnly] public bool isBreaking;
    [ReadOnly] public bool isCentered;
    [ReadOnly] public bool facingLane;
    [ReadOnly] public float currentSpeed;
    [ReadOnly] public float currentSteerAngle;
    [ReadOnly] public float currentbreakForce;
    [ReadOnly] public List<float> speedList = new List<float>();

    bool canDrive;

    public static PlayerCar Instance;

    private void Awake()
    {
        Instance = this;
        
        canDrive = true;

        wheelColliders = new WheelCollider[4] { frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider };
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        currentSpeed = CalculateSpeed();
        if (canDrive)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
        }
        else
        {
            horizontalInput = 0;
            verticalInput = 0;

            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;

            frontRightWheelCollider.brakeTorque = float.MaxValue;
            frontLeftWheelCollider.brakeTorque = float.MaxValue;
            rearLeftWheelCollider.brakeTorque = float.MaxValue;
            rearRightWheelCollider.brakeTorque = float.MaxValue;
        }
        UpdateWheels();
        UpdateAverageSpeed();
    }

    void UpdateAverageSpeed()
    {
        if (Mathf.Abs(verticalInput) == 1)
            speedList.Add(currentSpeed);

        if (speedList.Count > 10)
        {
            int elementsToRemove = speedList.Count - 10;
            speedList.RemoveRange(0, elementsToRemove);
        }
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    float CalculateSpeed()
    {
        // Get the velocity of the car in meters per second.
        Vector3 velocity = GetComponent<Rigidbody>().velocity;

        // Calculate the magnitude of the velocity vector (speed in meters per second).
        float speedMS = velocity.magnitude;

        // Convert meters per second to miles per hour.
        float speedMPH = speedMS * 2.23694f;

        return Mathf.Abs(speedMPH);
    }

    public void StopDriving()
    {
        canDrive = false;
    }
}