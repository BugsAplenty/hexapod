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
        joint.useMotor = true;
        joint.useLimits = true;
        legMotor.force = motorForce;
        joint.motor = legMotor;
    }

    public void Rotate(float velocity)
    {
        legMotor.targetVelocity = velocity;
        joint.motor = legMotor;
    }

    public void MoveTo(float velocity, float angle)
    {
        SetGoalAngle(angle);
        Rotate(velocity);
    }

    private void SetGoalAngle(float angle)
    {
        JointLimits limits = joint.limits;
        if (angle < joint.angle)
        {
            limits.min = angle;
        }
        else
        {
            limits.max = angle;
        }
        joint.limits = limits;
    }
}
