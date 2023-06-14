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
    private Coroutine walkingCoroutine;

    public void Stand()
    {
        if (state == State.Stand)
            return;
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
    private IEnumerator StopWalking()
    {
        foreach (var leg in legs)
        {
            leg.MoveToOptimal(VelForward, AngleTouchDown);
        }

        yield break;
    }
    
    private IEnumerator StartWalking()
    {
        foreach (var leg in legs)
        {
            switch (leg.tripod)
            {
                case HexapodLeg.Tripod.A:
                    leg.MoveToOptimal(VelForward, AngleLiftOff);
                    break;
                case HexapodLeg.Tripod.B:
                    leg.MoveToOptimal(VelForward, AngleTouchDown);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        yield break;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //If a coroutine is currently running, stop it
            if(walkingCoroutine != null)
            {
                StopCoroutine(walkingCoroutine);
                walkingCoroutine = null;
            }
            walkingCoroutine = StartCoroutine(StartWalking());
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if(walkingCoroutine != null)
            {
                StopCoroutine(walkingCoroutine);
                walkingCoroutine = null;
            }
            walkingCoroutine = StartCoroutine(StopWalking());
        }
    }



    private void StopMovement()
    {
        foreach (var leg in legs)
        {
            leg.StopRotation();
        }
    }

    private void MoveForward()
    { 
        foreach (var leg in legs)
        {
            switch (leg.tripod)
            {
                case HexapodLeg.Tripod.A:
                    // If the leg is at the liftoff angle, move to touchdown
                    if (leg.IsAtLiftOff())
                    {
                        leg.MoveToOptimal(VelForward, AngleTouchDown);
                    }
                    // If the leg is at the touchdown angle, move to liftoff
                    else if (leg.IsAtTouchDown())
                    {
                        leg.MoveToOptimal(VelForward, AngleLiftOff);
                    }
                    break;
                case HexapodLeg.Tripod.B:
                    // If the leg is at the liftoff angle, move to touchdown
                    if (leg.IsAtLiftOff())
                    {
                        leg.MoveToOptimal(VelForward, AngleTouchDown);
                    }
                    // If the leg is at the touchdown angle, move to liftoff
                    else if (leg.IsAtTouchDown())
                    {
                        leg.MoveToOptimal(VelForward, AngleLiftOff);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
