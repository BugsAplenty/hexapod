using System;
using System.Collections;
using UnityEngine;

public class LegMotor : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public float targetAngle; // The target angle for the leg to rotate to
    private HingeJoint joint;
    private JointMotor legMotor;
    private bool isStopping = false;
    private State state;

    private enum StopState
    {
        Stopping, Idle
    }
    private enum State
    {
        Stop, Go
    }
    private StopState stopState;
    private void Start()
    {
        JointSetup();
        state = State.Stop;
    }
    private void Update()
    {
        if (stopState == StopState.Stopping)
        {
            if (Mathf.Abs(joint.velocity) < 0.1f)
            {
                legMotor.targetVelocity = 0;
                legMotor.force = HexapodController.DefaultMotorForce;
                joint.motor = legMotor;
                SetLimit();
                joint.useLimits = true;
                state = State.Stop;
                stopState = StopState.Idle;
            }
            else
            {
                legMotor.targetVelocity = Mathf.Lerp(legMotor.targetVelocity, 0, Time.deltaTime * 5);
                legMotor.force = HexapodController.DefaultMotorForce;
                joint.motor = legMotor;
            }
        }
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
    public string GetDebugInfo()
    {
        return $"State: {state}\n" +
               $"Stop State: {stopState}\n" +
               $"Angle: {Angle():F2}\n" +
               $"Target Velocity: {TargetVelocity():F2}\n" +
               $"Current Velocity: {Velocity():F2}\n" +
               $"Torque: {Torque():F2}";
    }
    public bool IsAtAngle(float targetAngle)
    {
        var angleDiff = Geometry.AngleModDiff(Angle(), targetAngle);
        return angleDiff < HexapodController.LimitRes * 3 & angleDiff > -HexapodController.LimitRes * 3;
    }

    public void MoveToForward(float velocity, float targetAngle)
    {
        if (IsAtAngle(targetAngle))
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
        var angleDiff = Geometry.AngleModDiff(Angle(), targetAngle);
        if (IsAtAngle(targetAngle))
        {
            StopRotation();
        }
        else if (angleDiff > 180)
        {
            if (Angle() < targetAngle) 
            {
                StopRotation();
            }
            else 
            {
                ContinuousRotation(-velocity);
            }
        }
        else
        {
            if (Angle() > targetAngle)
            {
                StopRotation();
            }
            else 
            {
                ContinuousRotation(velocity);
            }
        }
    }

    public void StopRotation()
    {
        if (state != State.Go) return;
        stopState = StopState.Stopping;
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

    public bool IsAtLiftOff()
    {
        return IsAtAngle(HexapodController.AngleLiftOff);
    }

    public bool IsAtTouchDown()
    {
        return IsAtAngle(HexapodController.AngleTouchDown);
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
