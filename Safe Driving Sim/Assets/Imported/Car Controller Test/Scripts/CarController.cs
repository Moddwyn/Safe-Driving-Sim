using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    public UnityEvent OnCollision;
    [Space(20)]
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public float horizontalInput;
    public float verticalInput;
    public bool isBreaking;

    public float currentSpeed;
    public List<float> speeds = new List<float>();
  

    private float currentSteerAngle;
    private float currentbreakForce;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    private WheelCollider[] wheelColliders;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void Awake()
    {
        wheelColliders = new WheelCollider[4]{ frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider };
    }

    private void FixedUpdate()
    {
        currentSpeed = CalculateSpeed();
        speeds.Add(currentSpeed);
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private float CalculateSpeed()
    {
        float totalSpeed = 0f;

        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            if (!wheelCollider.isGrounded) {
                continue;
            }
            float wheelAngularVelocity = wheelCollider.rpm * 2f * Mathf.PI / 60f;
            float wheelSpeed = wheelAngularVelocity * 0.35f;

            totalSpeed += wheelSpeed;
        }

        return Mathf.Abs(totalSpeed / wheelColliders.Length);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Collision") && other.transform != transform)
        {
            OnCollision?.Invoke();
        }
    }
}
