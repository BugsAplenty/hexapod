using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;


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

        //GetComponentInParent<Transform>().localRotation = group switch
        transform.localRotation = group switch
         {
            InversionGroup.B => Quaternion.Euler(HexapodController.AngleTouchDown, 0, 0),
            InversionGroup.A => Quaternion.Euler(HexapodController.AngleLiftOff, 0, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void ContinuousRotation(float velocity)
    {
        joint.useLimits = false;
        legMotor.targetVelocity = velocity;
        legMotor.force = HexapodController.Pid.GetOutput(
            Math.Abs(TargetVelocity() - Velocity()), 
            Time.deltaTime
            );
        legMotor.freeSpin = false;
        joint.motor = legMotor;
        
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
        UnityEngine.Debug.Log("Current Angle Limits for " + name + " are: " + joint.limits.min + " | " + joint.limits.max);
        UnityEngine.Debug.Log("Current Angle is for " + name + " is: " + Angle());
    }

    private JointLimits SetLimit()
    {
        var targetAngle = Angle();
        var limits = new JointLimits
        {
            max = targetAngle + HexapodController.LimitRes,
            min = targetAngle - HexapodController.LimitRes,
            bounciness = 0.2f,
            bounceMinVelocity = 0.2f
        };
        if(limits.max >= 180) limits.max += -360;
        if (limits.min <= -180) limits.min += 360;
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
