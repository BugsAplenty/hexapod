using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexapodController : MonoBehaviour
{
    public LegMotor[] tripod1; // left back, right center, left front
    public LegMotor[] tripod2; // right back, left center, right front
    private const float AngleLiftOff = -45;
    private const float AngleTouchDown = 45;
    private const float TouchDownSpeed = 100;
    private const float RangeTouchDown = AngleTouchDown - AngleLiftOff;
    private const float RangeLiftOff = 360 - RangeTouchDown;
    private const float LiftOffSpeed = TouchDownSpeed * (RangeLiftOff / RangeTouchDown);
    public static readonly Pid Pid = new Pid(Kp, Ki, Kd);
    public const float LimitRes = 3f;
    private const float Kp = 10f;
    private const float Ki = 1f;
    private const float Kd = 1f;
    public float liftOffAngle = 30f; // Angle for lifting off the ground
    public float touchDownAngle = -30f; // Angle for touching down
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second
    private bool shouldBeWalking = false; // Tracks if the hexapod is walking
    private int activeTripodGroup = 0; // 0: None, 1: Tripod1, 2: Tripod2
    // State enumerator containing states - stop, go
    private enum State
    {
        Stop,
        Go
    }

    private State state = State.Stop;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            state = State.Go;
            StartCoroutine(WalkCycle());
        } else if (Input.GetKeyDown(KeyCode.A))
        {
        } else if (Input.GetKeyDown(KeyCode.S))
        {
        } else if (Input.GetKeyDown(KeyCode.D))
        {
        }
        else
        {
            // if (state == State.Stop)
            // {
            //     StartCoroutine(MoveLegsToRestingPositions());
            // }
        }
    }

    private IEnumerator MoveLegsToRestingPositions()
    {
        var moveTripod1 = StartCoroutine(MoveTripodToRestingPosition(tripod1, AngleLiftOff));
        var moveTripod2 = StartCoroutine(MoveTripodToRestingPosition(tripod2, AngleTouchDown));

        yield return moveTripod1;
        yield return moveTripod2;
    }

    private IEnumerator MoveTripodToRestingPosition(IEnumerable<LegMotor> tripod, float restAngle)
    {
        foreach (var leg in tripod)
        {
            StartCoroutine(leg.MoveToAngleShortestDistance(restAngle, rotationSpeed));
        }
        yield return null;
    }

    
    private IEnumerator WalkCycle()
    {
        while (state==State.Go)
        {
            yield return StartCoroutine(Step1());
            yield return new WaitUntil(() => tripod1.Concat(tripod2).All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(Step2());
            yield return new WaitUntil(() => tripod1.Concat(tripod2).All(leg => leg.HasReachedTarget));
            Debug.Log(state);
            if (Input.GetKeyUp(KeyCode.W))
            {
                state = State.Stop;
            }
        }

        yield break;

        // Create internal coroutine to handle step 1
        IEnumerator Step1()
        {
            // Start moving both tripods in parallel
            var moveTripod1 = StartCoroutine(TripodStepForward(tripod1, AngleLiftOff, TouchDownSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(tripod2, AngleTouchDown, LiftOffSpeed));

            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }

        IEnumerator Step2()
        {
            var moveTripod1 = StartCoroutine(TripodStepForward(tripod1, AngleTouchDown, LiftOffSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(tripod2, AngleLiftOff, TouchDownSpeed));

            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }
    }

    private IEnumerator TripodStepForward(IEnumerable<LegMotor> tripod, float targetAngle, float targetVelocity)
    {
        // Start movement coroutine for each leg using a for loop and await completion after the for loop
        foreach(var leg in tripod)
        {
            // Debug.Log(leg);
            StartCoroutine(leg.MoveToAngleAt(targetAngle, targetVelocity, true));
        }
        yield return null;
    }

}
