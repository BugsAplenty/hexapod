using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignLegProperties : MonoBehaviour
{
    private GameObject hexapodLegRightBackObj;
    private Rigidbody hexapodLegRightBack;
    private GameObject hexapodLegLeftBackObj;
    private Rigidbody hexapodLegLeftBack;
    private GameObject hexapodLegRightFrontObj;
    private Rigidbody hexapodLegRightFront;
    private GameObject hexapodLegLeftFrontObj;
    private Rigidbody hexapodLegLeftFront;
    private GameObject hexapodLegRightMiddleObj;
    private Rigidbody hexapodLegRightMiddle;
    private GameObject hexapodLegLeftMiddleObj;
    private Rigidbody hexapodLegLeftMiddle;
    private static int legMass = 5;
    // Start is called before the first frame update
    void Start()
    {
        hexapodLegRightBackObj = GameObject.Find("HexapodLegRightBack");
        hexapodLegRightBack = hexapodLegRightBack.GetComponent<Rigidbody>();
        hexapodLegRightBack.mass = legMass;
        hexapodLegLeftBackObj = GameObject.Find("HexapodLegLeftBack");
        hexapodLegLeftBack = hexapodLegLeftBack.GetComponent<Rigidbody>();
        hexapodLegLeftBack.mass = legMass;
        hexapodLegRightFrontObj = GameObject.Find("HexapodLegRightFront");
        hexapodLegRightFront = hexapodLegRightFrontObj.GetComponent<Rigidbody>();
        hexapodLegRightFront.mass = legMass;
        hexapodLegLeftFrontObj = GameObject.Find("HexapodLegLeftFront");
        hexapodLegLeftFront = hexapodLegLeftFrontObj.GetComponent<Rigidbody>();
        hexapodLegLeftFront.mass = legMass;
        hexapodLegRightMiddleObj = GameObject.Find("HexapodLegRightMiddle");
        hexapodLegRightMiddle = hexapodLegRightMiddleObj.GetComponent<Rigidbody>();
        hexapodLegRightMiddle.mass = legMass;
        hexapodLegLeftMiddleObj = GameObject.Find("HexapodLegLeftMiddle");
        hexapodLegLeftMiddle = hexapodLegLeftMiddleObj.GetComponent<Rigidbody>();
        hexapodLegLeftMiddle.mass = legMass;
    }
}
