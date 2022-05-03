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
    private HingeJoint joint;
    private JointMotor legMotor;

    private void Start()
    {
        JointSetup();
    }

    private void JointSetup()
    {
        legMotor.force = HexapodController.DefaultMotorForce;
        joint = GetComponent<HingeJoint>();
        joint.motor = legMotor;
        joint.useMotor = true;
        transform.localRotation = group switch
        {
            InversionGroup.B => Quaternion.Euler(HexapodController.AngleTouchDown, 0, 0),
            InversionGroup.A => Quaternion.Euler(HexapodController.AngleLiftOff, 0, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void ContinuousRotation(float velocity)
    {
        legMotor.targetVelocity = velocity;
        legMotor.force = HexapodController.Pid.GetOutput(
            Math.Abs(TargetVelocity() - Velocity()), 
            Time.deltaTime
            );
        legMotor.freeSpin = false;
        joint.motor = legMotor;
        joint.useLimits = false;
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
        legMotor.targetVelocity = 0;
        legMotor.force = HexapodController.DefaultMotorForce;
        joint.motor = legMotor;
        joint.limits = SetLimit();
        joint.useLimits = true;
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
        return legMotor.targetVelocity;
    }

    public float Velocity()
    {
        return joint.velocity;
    }

    public float Torque()
    {
        return legMotor.force;
    }

    private float Angle()
    {
        return joint.angle;
    }
}
