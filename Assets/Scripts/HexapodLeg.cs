using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HexapodLeg : MonoBehaviour
{
    private enum InversionGroup
    {
        A,B
    }

    [SerializeField] private float angleTouchDown;
    [SerializeField] private float angleLiftOff;
    [SerializeField] private float motorForce = 1000f;
    [SerializeField] private InversionGroup group;
    [SerializeField] private float limitRes = 2.5f;
    private HingeJoint _joint;
    private JointMotor _legMotor;

    private const float Kp = 1000f;
    private const float Ki = 10f;
    private const float Kd = 10f;
    private readonly PID _pid = new PID(Kp, Ki, Kd);

    private void Start()
    {
        JointSetup();
    }

    private void JointSetup()
    {
        _legMotor.force = motorForce;
        _joint = GetComponent<HingeJoint>();
        _joint.motor = _legMotor;
        _joint.useMotor = true;
        switch (group)
        {
            case InversionGroup.B:
                transform.parent.Rotate(180, 0, 0);
                break;
            case InversionGroup.A:
                transform.parent.Rotate(0, 0, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ContinuousRotation(float velocity)
    {
        _legMotor.targetVelocity = velocity;
        _legMotor.force = _pid.GetOutput(Math.Abs(TargetVelocity() - Velocity()), Time.deltaTime);
        _legMotor.freeSpin = false;
        _joint.motor = _legMotor;
        _joint.useLimits = false;
    }

    public void StopRotation()
    {
        _legMotor.targetVelocity = 0;
        _legMotor.force = _pid.GetOutput(Math.Abs(TargetVelocity() - Velocity()), Time.deltaTime);
        _joint.motor = _legMotor;
        _joint.limits = SetLimit();
        _joint.useLimits = true;
    }

    private JointLimits SetLimit()
    {
        var targetAngle = _joint.angle;
        var limits = new JointLimits
        {
            max = targetAngle + limitRes,
            min = targetAngle - limitRes
        };
        return limits;
    }

    public float TargetVelocity()
    {
        return _legMotor.targetVelocity;
    }

    public float Velocity()
    {
        return _joint.velocity;
    }
}
