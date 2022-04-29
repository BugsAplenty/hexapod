using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodLeg : MonoBehaviour
{
    enum InversionGroup
    {
        A,B
    }

    [SerializeField] float angleTouchDown;
    [SerializeField] float angleLiftOff;
    [SerializeField] float motorForce = 1000f;
    [SerializeField] InversionGroup Group;

    private HingeJoint joint;
    private JointMotor legMotor;


    private void Start()
    {
        JointSetup();
    }

    private void JointSetup()
    {
        joint = GetComponent<HingeJoint>();
        legMotor.force = motorForce;
        joint.motor = legMotor;
        joint.useMotor = true;
        if (Group == InversionGroup.B)
        {
            transform.parent.Rotate(180, 0, 0);
        }
    }

    public void ContinualRotation(float velocity)
    {
        legMotor.force = motorForce;
        legMotor.targetVelocity = velocity;
        legMotor.freeSpin = false;
        joint.motor = legMotor;
        joint.useLimits = false;
    }

    public void StopRotation()
    {   

        legMotor.targetVelocity = 0;
        joint.motor = legMotor;
        joint.limits = SetLimit();
        joint.useLimits = true;
    }

    private JointLimits SetLimit()
    {
        JointLimits limits = new JointLimits();
        limits.max = joint.angle + 10f;
        limits.min = joint.angle - 2f;
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

    //private void SetGoalAngle(float angle)
    //{
    //    JointLimits limits = joint.limits;
    //    if (angle < joint.angle)
    //    {
    //        limits.min = angle;
    //    }
    //    else
    //    {
    //        limits.max = angle;
    //    }
    //    joint.limits = limits;
    //}

    //public void Rotate(float velocity)
    //{
    //    legMotor.targetVelocity = velocity;
    //    joint.motor = legMotor;
    //}

    //public void MoveTo(float velocity, float angle)
    //{
    //    SetGoalAngle(angle);
    //    Rotate(velocity);
    //}

}
