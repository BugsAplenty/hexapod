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
    public void MoveTo(float vel, float angle)
    {
        this.SetGoalAngle(angle);
        this.Rotate(vel);
    }
    public void MoveToOptimal(float vel, float angle)
    {
        this.SetGoalAngle(angle);
        if (Mathf.Abs(angle - this.joint.angle) < 180) 
        {
            this.Rotate(vel);
        } else {
            this.Rotate(-vel);
        }
    }

    public void SetGoalAngle(float angle)
    {
        JointLimits limits = this.joint.limits;
        if (angle < this.joint.angle) 
        {
            limits.min = angle;
        } else {
            limits.max = angle;
        }
        this.joint.limits = limits;
    }
    public void Walk(float velTouchDown, float velLiftOff)
    {
        if (this.IsTouchDown())
        {
            this.MoveTo(velTouchDown, angleLiftOff);
        } else {
            this.MoveTo(velLiftOff, angleTouchDown);
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



