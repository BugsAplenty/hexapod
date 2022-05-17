using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;


public class HexapodLeg : MonoBehaviour
{
    public enum Tripod
    {
        A,B
    }

    private enum State
    {
        Stop, Go
    }

    private State state;
    
    [SerializeField] public Tripod tripod;
    private HingeJoint joint;
    private JointMotor legMotor;

    private void Start()
    {
        JointSetup();
        state = State.Stop;
    }

    private void JointSetup()
    {
        legMotor.force = HexapodController.DefaultMotorForce;
        joint = GetComponent<HingeJoint>();
        joint.motor = legMotor;
        joint.useMotor = true;

        //GetComponentInParent<Transform>().localRotation = tripod switch
        // transform.localRotation = tripod switch
        //  {
        //     Tripod.B => Quaternion.Euler(HexapodController.AngleTouchDown, 0, 0),
        //     Tripod.A => Quaternion.Euler(HexapodController.AngleLiftOff, 0, 0),
        //     _ => throw new ArgumentOutOfRangeException()
        // };
    }

    public void ContinuousRotation(float velocity)
    {
        state = State.Go;
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
        var angleDiff = Angle() - targetAngle;
        angleDiff = Geometry.AngleModDeg(angleDiff);
        if (angleDiff > HexapodController.LimitRes * 3 | angleDiff < -HexapodController.LimitRes * 3)
        {
            ContinuousRotation(velocity);
        }
        else
        {
            StopRotation();
        }
    }
    public void MoveToOptimal(float velocity, float targetAngle)
    {
        var angleDiff = Angle() - targetAngle;
        angleDiff = Geometry.AngleModDeg(angleDiff);
        if (angleDiff < HexapodController.LimitRes * 3 & angleDiff > -HexapodController.LimitRes * 3)
        {
            StopRotation();
        }
        else if (angleDiff > 180)
        {
            ContinuousRotation(-velocity);
        }
        else
        {
            ContinuousRotation(velocity);
        }
    }

    public void StopRotation()
    {
        if (state != State.Go) return;
        state = State.Stop;
        legMotor.targetVelocity = 0;
        legMotor.force = HexapodController.DefaultMotorForce;
        joint.motor = legMotor;
        SetLimit();
        joint.useLimits = true;

        // UnityEngine.Debug.Log("Current Angle Limits for " + name + " are: " + joint.limits.min + " | " + joint.limits.max);
        // UnityEngine.Debug.Log("Current Angle is for " + name + " is: " + Angle());
    }

    private void SetLimit()
    {
        var targetAngle = Angle();
        var limits = new JointLimits
        {
            max = targetAngle + HexapodController.LimitRes,
            min = targetAngle - HexapodController.LimitRes,
            // bounciness = 0.2f,
            // bounceMinVelocity = 0.2f
        };
        limits.max = Geometry.AngleModDeg(limits.max);
        limits.min = Geometry.AngleModDeg(limits.min); 
        joint.limits = limits;
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

    public float Angle()
    {
        return joint.angle;
    }
}
