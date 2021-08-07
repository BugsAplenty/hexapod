using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodController : MonoBehaviour
{
    // Controller parameters.
    private static float motorForce = 1000;
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
    private HexapodLeg leftFront;
    private HexapodLeg rightMiddle;
    private HexapodLeg leftMiddle; 
    private HexapodLeg rightFront; 
    private HexapodLeg leftBack;
    private HexapodLeg rightBack;
    void Start()
    {
        Debug.Log("wtf");
        leftFront = new HexapodLeg("HexapodLegLeftFront", angleLiftOffLeft, angleTouchDownLeft, motorForce);
        rightMiddle = new HexapodLeg("HexapodLegRightMiddle", angleLiftOffRight, angleTouchDownRight, motorForce);
        leftMiddle = new HexapodLeg("HexapodLegLeftMiddle", angleLiftOffLeft, angleTouchDownLeft, motorForce);
        rightFront = new HexapodLeg("HexapodLegRightFront", angleLiftOffRight, angleTouchDownRight, motorForce);
        leftBack = new HexapodLeg("HexapodLegLeftBack", angleLiftOffLeft, angleTouchDownLeft, motorForce);
        rightBack = new HexapodLeg("HexapodLegRightBack", angleLiftOffRight, angleTouchDownRight, motorForce);
        Debug.Log("All legs located successfully.");
    }
    void Update()
    {
        leftBack.MoveTo(500, 90);
        leftFront.MoveTo(500, 90);
        leftMiddle.MoveTo(500, 90);
        rightFront.MoveTo(500, 90);
        rightMiddle.MoveTo(500, 90);
        rightBack.MoveTo(500, 90);
    }
    void Walk(float velForward)
    {
        
    }
    void Stop()
    {
        
    }
    void TurnLeft(float velTurn)
    {
        
    }
    void TurnRight(float velTurn)
    {

    }
}