using System;
using System.Linq;
using UnityEngine;

public class HexapodController : MonoBehaviour
{
    

    public HexapodLeg[] legs;
    private const float VelForward = 200f;
    public const float AngleTouchDown = 45;
    public const float AngleLiftOff = -45;
    public const float LimitRes = 3f;
    private const float Kp = 10f;
    private const float Ki = 1f;
    private const float Kd = 1f;
    public const float DefaultMotorForce = 10f;
    public static readonly Pid Pid = new Pid(Kp, Ki, Kd);
    
    private void Awake()
    {
        LegSetup();      
    }
    

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
            return;
        }
        StopMovement();
    }

    private void StopMovement()
    {
        // Debug.Log("Trying to stop rotation");
        foreach(var leg in legs)
        {
            leg.StopRotation();
        }
        
    }

    private void MoveForward()
    { 
        foreach (var leg in legs)
        {
            leg.MoveToForward(VelForward, AngleTouchDown);
            //
            // switch (leg.group)
            // {
            //     case HexapodLeg.InversionGroup.A:
            //         leg.MoveToForward(VelForward, AngleTouchDown);
            //         break;
            //     case HexapodLeg.InversionGroup.B:
            //         leg.MoveToForward(VelForward, AngleLiftOff);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            // leg.ContinuousRotation(200);
        }
    }

    private void LegSetup()
    {
        legs = (
            from Transform child in transform 
            where child.CompareTag("LegAnchor") 
            select child.GetChild(0).GetComponent<HexapodLeg>()
            ).ToArray();
    }

    public HexapodLeg[] GetLegList()
    {
        return legs;
    }
}
