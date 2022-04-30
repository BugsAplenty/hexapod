using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = System.Diagnostics.Debug;

public class HexapodLeg : MonoBehaviour
{
    public enum InversionGroup
    {
        A,B
    }
    
    [SerializeField] public InversionGroup group;
    private HingeJoint _joint;
    private JointMotor _legMotor;

    private void Start()
    {
        JointSetup();
    }

    private void JointSetup()
    {
        _legMotor.force = HexapodController.DefaultMotorForce;
        _joint = GetComponent<HingeJoint>();
        _joint.motor = _legMotor;
        _joint.useMotor = true;
        transform.localRotation = group switch
        {
            InversionGroup.B => Quaternion.Euler(HexapodController.AngleTouchDown, 0, 0),
            InversionGroup.A => Quaternion.Euler(HexapodController.AngleLiftOff, 0, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void ContinuousRotation(float velocity)
    {
        _legMotor.targetVelocity = velocity;
        _legMotor.force = HexapodController.Pid.GetOutput(
            Math.Abs(TargetVelocity() - Velocity()), 
            Time.deltaTime
            );
        _legMotor.freeSpin = false;
        _joint.motor = _legMotor;
        _joint.useLimits = false;
    }

    public void MoveToForward(float velocity, float targetAngle)
    {
        if (Math.Abs(Angle() - targetAngle) < 2)
        {
            StopRotation();
        }
        else
        {
            ContinuousRotation(velocity);
        }
    }

    public void StopRotation()
    {
        _legMotor.targetVelocity = 0;
        _legMotor.force = HexapodController.DefaultMotorForce;
        _joint.motor = _legMotor;
        _joint.limits = SetLimit();
        _joint.useLimits = true;
    }

    private JointLimits SetLimit()
    {
        var targetAngle = Angle();
        var limits = new JointLimits
        {
            max = targetAngle + HexapodController.LimitRes,
            min = targetAngle - HexapodController.LimitRes
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

    public float Torque()
    {
        return _legMotor.force;
    }

    private float Angle()
    {
        return _joint.angle;
    }
}
