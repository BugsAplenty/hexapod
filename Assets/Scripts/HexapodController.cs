using System;
using System.Collections;
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
    public State state;
    
    private void Awake()
    {
        LegSetup();      
    }

    public enum State
    {
        Walk,
        Stand
    }

    public void Stand()
    {
        foreach (var leg in legs)
        {
            switch (leg.tripod)
            {
                case HexapodLeg.Tripod.A:
                    leg.MoveToOptimal(VelForward, AngleTouchDown);
                    break;
                case HexapodLeg.Tripod.B:
                    leg.MoveToOptimal(VelForward, AngleLiftOff);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        state = State.Stand;
    }
    
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (state == State.Stand)
            {
                MoveForward();
            }

            state = State.Walk;
            return;
        }
        Stand();
    }

    private void StopMovement()
    {
        foreach(var leg in legs)
        {
            leg.StopRotation();
        }
        
    }

    private void MoveForward()
    { 
        foreach (var leg in legs)
        {
            // switch (leg.tripod)
            // {
            //     case HexapodLeg.Tripod.A:
            //         leg.MoveToForward(VelForward, AngleTouchDown);
            //         break;
            //     case HexapodLeg.Tripod.B:
            //         leg.MoveToForward(VelForward, AngleLiftOff);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            leg.ContinuousRotation(200);
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
