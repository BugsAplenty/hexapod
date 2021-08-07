using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodLeg
{
    public GameObject obj;
    public float angleTouchDown;
    public float angleLiftOff;
    public HingeJoint joint;
    
    public HexapodLeg(string name, float angleLiftOff, float angleTouchDown, float force)
    {
        this.obj = GameObject.Find(name);
        Debug.Log($"{name} found.");
        this.joint = this.obj.GetComponent<HingeJoint>();
        this.joint.useMotor = true;
        this.joint.useLimits = true;
        SetMotorParams();
    }
    private void SetMotorParams()
    {
        JointMotor legMotor = joint.motor;
        legMotor.force = 1000;
        this.joint.motor = legMotor;
    }
    public void Rotate(float vel)
    {
        JointMotor legMotor = this.joint.motor;
        legMotor.targetVelocity = vel;
        this.joint.motor = legMotor;
    }
    public void Stop()
    {
        Rotate(0);
    }

    public void MoveTo(float vel, float angle)
    {
        SetGoalAngle(angle);
        Rotate(vel);

    }
    public void SetGoalAngle(float angle)
    {
        if (this.IsReverse())
        {
            JointLimits limits = this.joint.limits;
            limits.max = angle;
            this.joint.limits = limits;
        } else {
            JointLimits limits = this.joint.limits;
            limits.min = angle;
            this.joint.limits = limits;
        }
    }

    public bool IsTouchDown()
    {
        if (this.angleLiftOff > this.angleTouchDown)
        {
            return this.joint.angle > this.angleTouchDown && this.joint.angle < this.angleLiftOff;
        } else {
            return this.joint.angle > this.angleLiftOff && this.joint.angle < this.angleTouchDown;
        }
    }
    private bool IsReverse()
    {
        if (this.joint.axis.z == -1)
        {
            return false;
        } else {
            return true;
        }
    }
}



