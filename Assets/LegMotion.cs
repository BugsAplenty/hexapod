using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMotion : MonoBehaviour
{
    private HingeJoint legHinge;
    private JointMotor legMotor;
    private Rigidbody leg;
    private static int velForward;
    private static int velSide;
    private static int velReverse;
    private static float angleLiftOffLeft;
    private static float angleLiftOffRight;
    private static float angleTouchDownLeft;
    private static float angleTouchDownRight;
    private static float intervalOnGround;
    private static float velBoost;
    private static bool isLeft;
    private static bool isRight;
    private static bool isTriLeft;
    private static bool isTriRight;
    private static float kP;
    private static float kI;
    private static float kD;
    private static float errorIntegral;
    private static float errorPrevious;
    void Start()
    {
        legHinge = GetComponent<HingeJoint>();
        leg = GetComponent<Rigidbody>();
        leg.mass = 5;
        legMotor = legHinge.motor;
        legMotor = legHinge.motor;
        legMotor.freeSpin = false;
        legHinge.useMotor = true;
        legMotor.force = 1000;
        angleLiftOffLeft = 0;
        angleLiftOffRight = 0;
        angleTouchDownLeft = 90;
        angleTouchDownRight = -90;
        intervalOnGround = Mathf.Abs(angleLiftOffLeft - angleTouchDownLeft);
        velBoost = (360 - intervalOnGround) / intervalOnGround; 
        velForward = 200;
        velReverse = 100;
        velSide = 100;
        errorPrevious = 0;
        errorIntegral = 0;
        kP = 0.01f;
        kI = 0.01f;
        kD = 0.001f;
        InitializeLegs();
    }
    void InitializeLegs()
    {
        Stop();
    }
    void Update()
    {
        isLeft = (this.gameObject.name == "RhexLegLeftBack" || this.gameObject.name == "RhexLegLeftFront" || this.gameObject.name == "RhexLegLeftMiddle");
        isRight = (this.gameObject.name == "RhexLegRightBack" || this.gameObject.name == "RhexLegRightFront" || this.gameObject.name == "RhexLegRightMiddle");
        isTriLeft = (this.gameObject.name == "RhexLegLeftBack" || this.gameObject.name == "RhexLegRightFront" || this.gameObject.name == "RhexLegLeftMiddle");
        isTriRight = (this.gameObject.name == "RhexLegRightBack" || this.gameObject.name == "RhexLegLeftFront" || this.gameObject.name == "RhexLegRightMiddle");
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            ResetLegs();
        }
        if (Input.GetKey(KeyCode.W))
        {
            Forward();
        } else if (Input.GetKey(KeyCode.S)) {
            Reverse();
        } else if (Input.GetKey(KeyCode.A)) {
            Left();
        } else if (Input.GetKey(KeyCode.D)) {
            Right();
        } else {
            Stop();
        }
    }
    void Forward()
    {
        if (isLeft) 
        {
            Walk(-velForward);
        } else {
            Walk(velForward);
        }
    }
    void Reverse()
    {
        if (isLeft)
        {
            Walk(velReverse);
        } else {
            Walk(-velReverse);
        }
    }
    void Left()
    {
        if (isLeft)
        {
            Walk(velForward);
        } else {
            Walk(velForward);
        }
    }
    void Right()
    {
        if (isLeft) 
        {
            Walk(-velForward);
        } else {
            Walk(-velForward);
        }
    }
    void Stop()
    {
        Rotate(0);
    }
    void Rotate(float vel) 
    {
        float error = vel - legHinge.velocity;
        errorIntegral += error * Time.deltaTime;
        float errorDerivative = (error - errorPrevious) / Time.deltaTime;
        legMotor.targetVelocity = kP * error + kI * errorIntegral + kD * errorDerivative;
        errorPrevious = error;
        legHinge.motor = legMotor;
    } 
    void Walk(float vel)
    {
        if (isLeft) 
        {
            if (legHinge.angle < angleTouchDownLeft && legHinge.angle > angleLiftOffLeft) 
            {
                Rotate(vel);
            } else {
                Rotate(vel * velBoost);
            }
        } else {
            if (legHinge.angle > angleTouchDownRight && legHinge.angle < angleLiftOffRight) 
            {
                Rotate(vel);   
            } else {
                Rotate(vel * velBoost);
            }
        }
    }
    void ResetLegs() 
    {

    }
}
