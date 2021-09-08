using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexapodController : MonoBehaviour
{
    // Controller parameters.
    private static float motorForce = 1000;
    private static float velForward = 200f; // TODO: Temporary value.
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
        HexapodLeg[] inverseLegs = new HexapodLeg[] {leftFront, leftBack, leftMiddle};
        foreach (HexapodLeg leg in inverseLegs)
        {
            leg.obj.transform.forward = -leg.obj.transform.forward;
        }
        Debug.Log("All legs located successfully.");
    }
    void Update()
    {
        Stand();
    }
    void Walk(float velTouchDown)
    {
        leftBack.Walk(velTouchDown, velTouchDown * velBoost);
    }
    void Stand() 
    {
        leftBack.MoveToOptimal(500, 100);
        leftFront.MoveToOptimal(500, 100);
        leftMiddle.MoveToOptimal(500, 100);
        rightFront.MoveToOptimal(500, 100);
        rightMiddle.MoveToOptimal(500, 100);
        rightBack.MoveToOptimal(500,100);
    }
    void TurnLeft(float velTurn)
    {
        
    }
    void TurnRight(float velTurn)
    {

    }
}