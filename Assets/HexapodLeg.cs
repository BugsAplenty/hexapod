using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodLeg
{
    private GameObject _obj;
    private float _angleTouchDown;
    private float _angleLiftOff;
    public HingeJoint joint;
    public JointMotor motor;
    private float resAngle =100;
    
    public HexapodLeg(string name, float angleLiftOff, float angleTouchDown, float force)
    {
        _obj = GameObject.Find(name);
        Debug.Log($"{name} found.");
        joint = _obj.GetComponent<HingeJoint>();
        joint.useMotor = true;
        SetMotorParams();
    }
    private void SetMotorParams()
    {
        JointMotor legMotor = joint.motor;
        legMotor.force = 1000;
        joint.motor = legMotor;
    }
    public void Rotate(float vel)
    {
        JointMotor legMotor = joint.motor;
        legMotor.targetVelocity = vel;
        joint.motor = legMotor;
        Debug.Log(joint.angle);
    }
    public void Stop()
    {
        Rotate(0);
    }
    private bool IsCounterClockWise()
    {
        Vector3 vecCounterClockWise = new Vector3(1,1,1);
        return (_obj.transform.localScale == vecCounterClockWise);
    }
    public void MoveTo(float vel, float angle)
    {
        float trueVel;
        if (IsCounterClockWise())
        {
            trueVel = vel;
        } else {
            trueVel = -vel;
        }
        if (Mathf.Abs(joint.angle - angle) > resAngle) 
        {
            // Debug.Log(joint.angle-angle);
            Rotate(trueVel);
        } else {
            Stop();
        }
    }
    public void MoveToOptimal(float vel, float angle)
    {
        float dAngle = joint.angle - angle;
        Debug.Log(dAngle);
        if (dAngle > 180)
        {
            MoveTo(vel, angle);
        } else if (dAngle < -180) {
            MoveTo(vel, angle);
        } else {
            MoveTo(-vel, angle);
        }
    }
    public bool IsTouchDown()
    {
        if (_angleLiftOff > _angleTouchDown)
        {
            return joint.angle > _angleTouchDown && joint.angle < _angleLiftOff;
        } else {
            return joint.angle > _angleLiftOff && joint.angle < _angleTouchDown;
        }
    }
}

