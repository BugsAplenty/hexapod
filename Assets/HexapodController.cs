using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodController : MonoBehaviour
{
    // Rear right leg.
    private GameObject hexapodLegRightBackObj;
    private HingeJoint hexapodLegRightBackHinge;
    private JointMotor hexapodLegRightBackMotor;
    // Rear left leg.
    private GameObject hexapodLegLeftBackObj;
    private HingeJoint hexapodLegLeftBackHinge;
    private JointMotor hexapodLegLeftBackMotor;
    // Front right leg.
    private GameObject hexapodLegRightFrontObj;
    private HingeJoint hexapodLegRightFrontHinge;
    private JointMotor hexapodLegRightFrontMotor;
    // Front left leg.
    private GameObject hexapodLegLeftFrontObj;
    private HingeJoint hexapodLegLeftFrontHinge;
    private JointMotor hexapodLegLeftFrontMotor;
    // Middle right leg.
    private GameObject hexapodLegRightMiddleObj;
    private HingeJoint hexapodLegRightMiddleHinge;
    private JointMotor hexapodLegRightMiddleMotor;
    // Middle left leg.
    private GameObject hexapodLegLeftMiddleObj;
    private HingeJoint hexapodLegLeftMiddleHinge;
    private JointMotor hexapodLegLeftMiddleMotor;
    // Controller parameters.
    private static float velForward = 200; // TODO: Temporary value.
    private static float velTurn = 100; // TODO: Temporary value.
    private static float velReverse = 100; // TODO: Temporary value.
    private static float velControl = 1000; // TODO: Temporary value.
    private static float angleLiftOffLeft = 0; // TODO: Temporary value.
    private static float angleLiftOffRight = 0; // TODO: Temporary value.
    private static float angleTouchDownLeft = 90; // TODO: Temporary value.
    private static float angleTouchDownRight = -90; // TODO: Temporary value.
    private static float dTouchDown = 90;
    private static float velBoost = (360 - Mathf.Abs(angleLiftOffLeft - angleTouchDownLeft)) / (Mathf.Abs(angleLiftOffLeft - angleTouchDownLeft));
    void Start()
    {
        LoadLegs();
        Debug.Log("All legs located successfully.");


    }
    void LoadLegs()
    {
        hexapodLegRightBackObj = GameObject.Find("HexapodLegRightBack");
        hexapodLegRightBackHinge = hexapodLegRightBackObj.GetComponent<HingeJoint>();
        hexapodLegRightBackMotor = hexapodLegRightBackObj.GetComponent<JointMotor>();
        Debug.Log("Found right rear leg.");
        hexapodLegLeftBackObj = GameObject.Find("HexapodLegLeftBack");
        hexapodLegLeftBackHinge = hexapodLegLeftBackObj.GetComponent<HingeJoint>();
        hexapodLegLeftBackMotor = hexapodLegLeftBackObj.GetComponent<JointMotor>();
        Debug.Log("Found left rear leg.");
        hexapodLegRightFrontObj = GameObject.Find("HexapodLegRightFront");
        hexapodLegRightFrontHinge = hexapodLegRightFrontObj.GetComponent<HingeJoint>();
        hexapodLegRightFrontMotor = hexapodLegRightFrontObj.GetComponent<JointMotor>();
        Debug.Log("Found right front leg.");
        hexapodLegLeftFrontObj = GameObject.Find("HexapodLegLeftFront");
        hexapodLegLeftFrontHinge = hexapodLegLeftFrontObj.GetComponent<HingeJoint>();
        hexapodLegLeftFrontMotor = hexapodLegLeftFrontObj.GetComponent<JointMotor>();
        Debug.Log("Found left front leg.");
        hexapodLegRightMiddleObj = GameObject.Find("HexapodLegRightMiddle");
        hexapodLegRightMiddleHinge = hexapodLegRightMiddleObj.GetComponent<HingeJoint>();
        hexapodLegRightMiddleMotor = hexapodLegRightMiddleObj.GetComponent<JointMotor>();
        Debug.Log("Found right middle leg.");
        hexapodLegLeftMiddleObj = GameObject.Find("HexapodLegLeftMiddle");
        hexapodLegLeftMiddleHinge = hexapodLegLeftMiddleObj.GetComponent<HingeJoint>();
        hexapodLegLeftMiddleMotor = hexapodLegLeftMiddleObj.GetComponent<JointMotor>();
        Debug.Log("Found left middle leg.");
        
    }
    void Update()
    {
        
        if (Input.GetKey(KeyCode.W))
        {
            Walk(velForward);
        } else if (Input.GetKey(KeyCode.A)) {
            TurnLeft(velTurn);
        } else if (Input.GetKey(KeyCode.S)) {
            Walk(velReverse);
        } else if (Input.GetKey(KeyCode.D)) {
            TurnRight(velTurn);
        } else {
            Stop();
        }
    }
    void Walk(float vel)
    {
       if (!CheckAlign("walk", "right"))
       {
           Align(hexapodLegRightBackHinge, hexapodLegLeftMiddleHinge, hexapodLegRightFrontHinge);
       } else if (!CheckAlign("walk", "left")) {
           Align(hexapodLegLeftBackHinge, hexapodLegRightMiddleHinge, hexapodLegLeftFrontHinge);
       } else if (!CheckSync()) {
           Sync(hexapodLegLeftFrontHinge, hexapodLegRightFrontHinge);
       } else {
           
       }
    }
    void MoveLegLeft(HingeJoint hexapodLegLeftFrontHinge, float vel)
    {
        if(IsInTouchDownLeft(hexapodLegLeftFrontHinge))
        {
            Rotate(hexapodLegLeftFrontHinge, -vel);
        } else {
            Rotate(hexapodLegLeftFrontHinge, -vel * velBoost);
        }
    }
    void MoveLegRight(HingeJoint hexapodLegRightFrontHinge, float vel)
    {
        if (IsInTouchDownRight(hexapodLegRightFrontHinge))
        {
            Rotate(hexapodLegRightFrontHinge, vel);
        } else {
            Rotate(hexapodLegRightFrontHinge, vel*velBoost);
        }
    }

    private void Sync(HingeJoint hexapodLegLeftFrontHinge, HingeJoint hexapodLegRightFrontHinge)
    {
        float error = hexapodLegLeftFrontHinge.angle + hexapodLegRightFrontHinge.angle;
                                                                                                                        
    }

    private void Align(HingeJoint hexapodLegLeftBackHinge, HingeJoint hexapodLegRightMiddleHinge, HingeJoint hexapodLegLeftFrontHinge)
    {
        throw new NotImplementedException();
    }
    void Rotate(HingeJoint legJoint, float vel)
    {
        JointMotor legMotor = legJoint.motor;
        legMotor.targetVelocity = vel;
        legJoint.motor = legMotor;
    }
    void Stop()
    {
        GaitControl("stand");
    }
    void TurnLeft(float vel)
    {

    }
    void TurnRight(float vel)
    {

    }
    void GaitControl(string gait)
    {

    }
    bool CheckSync()
    {
        if (IsInTouchDownLeft(hexapodLegLeftFrontHinge) && IsInTouchDownRight(hexapodLegRightFrontHinge))
        {
            return Mathf.Abs(hexapodLegLeftFrontHinge.angle - hexapodLegRightFrontHinge.angle) == dTouchDown;
        } else if (!IsInTouchDownLeft(hexapodLegLeftFrontHinge) && !IsInTouchDownRight(hexapodLegRightFrontHinge)) {
            return Mathf.Abs(hexapodLegLeftFrontHinge.angle - hexapodLegRightFrontHinge.angle) == dTouchDown * velBoost;
        } else if (!IsInTouchDownLeft(hexapodLegLeftFrontHinge) && IsInTouchDownRight(hexapodLegRightFrontHinge)) {
            return velBoost * Mathf.Abs(hexapodLegRightFrontHinge.angle - angleTouchDownRight) == Mathf.Abs(hexapodLegLeftFrontHinge.angle - angleLiftOffLeft);
        } else {
            return velBoost * Mathf.Abs(hexapodLegLeftFrontHinge.angle - angleTouchDownLeft) == Mathf.Abs(hexapodLegRightFrontHinge.angle - angleLiftOffRight);
        }
    }
    bool CheckAlign(string mode, string side)
    {
        if (mode == "walk")
        {
            if (side == "left")
            {
                return (hexapodLegLeftBackHinge.angle == hexapodLegLeftFrontHinge.angle && hexapodLegLeftFrontHinge.angle == - hexapodLegRightMiddleHinge.angle);
            } else {
                return (hexapodLegRightBackHinge.angle == hexapodLegRightFrontHinge.angle && hexapodLegRightFrontHinge.angle == -hexapodLegLeftMiddleHinge.angle);
            }
        } else {
            if (side == "left")
            {
                return (hexapodLegLeftFrontHinge.angle == hexapodLegLeftMiddleHinge.angle && hexapodLegLeftMiddleHinge.angle == hexapodLegLeftBackHinge.angle);
            } else {
                return (hexapodLegRightFrontHinge.angle == hexapodLegRightMiddleHinge.angle && hexapodLegRightMiddleHinge.angle == hexapodLegRightBackHinge.angle);
            }
        }
    }
    bool IsInTouchDownLeft(HingeJoint hexapodLegLeftFrontHinge)
    {
        return (hexapodLegLeftFrontHinge.angle > 0 && hexapodLegLeftFrontHinge.angle < 90);
    }
    bool IsInTouchDownRight(HingeJoint hexapodLegRightFrontHinge)
    {
        return (hexapodLegRightFrontHinge.angle < 0 && hexapodLegRightFrontHinge.angle > -90);
    }
}
