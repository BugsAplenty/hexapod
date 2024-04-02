using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexapodController : MonoBehaviour
{   
    public LegMotor frontLeft;
    public LegMotor frontRight;
    public LegMotor backLeft;
    public LegMotor backRight;
    public LegMotor centerLeft;
    public LegMotor centerRight;
    private IEnumerable<LegMotor> Tripod1 => new[] {frontLeft, backLeft, centerRight};
    private IEnumerable<LegMotor> Tripod2 => new[] {frontRight, backRight, centerLeft};
    public IEnumerable<LegMotor> LegMotors => new[] {frontLeft, frontRight, backLeft, backRight, centerLeft, centerRight};
    private const float AngleLiftOff = -45;
    private const float AngleTouchDown = 45;
    private const float TouchDownSpeed = 100;
    private const float RangeTouchDown = AngleTouchDown - AngleLiftOff;
    private const float RangeLiftOff = 360 - RangeTouchDown;
    private const float LiftOffSpeed = TouchDownSpeed * (RangeLiftOff / RangeTouchDown);
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
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(LeftWalkCycle());
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            state = State.Go;
            StartCoroutine(ReverseWalkCycle());
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(RightWalkCycle());
        }
    }

    private IEnumerator RightWalkCycle()
    {
        while (state==State.Go)
        {
            yield return StartCoroutine(Step1());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(Step2());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }

        yield break;

        // Create internal coroutine to handle step 1
        IEnumerator Step1()
        {
            // Start moving both tripods in parallel
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleTouchDown, TouchDownSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleLiftOff, -LiftOffSpeed));
            
            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }

        IEnumerator Step2()
        {
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleLiftOff, LiftOffSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleTouchDown, -TouchDownSpeed));

            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }
    }

    private IEnumerator LeftWalkCycle()
    {
        while (state==State.Go)
        {
            yield return StartCoroutine(Step1());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(Step2());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }

        yield break;

        // Create internal coroutine to handle step 1
        IEnumerator Step1()
        {
            // Start moving both tripods in parallel
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleTouchDown, -TouchDownSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleLiftOff, LiftOffSpeed));
            
            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }

        IEnumerator Step2()
        {
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleLiftOff, -LiftOffSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleTouchDown, TouchDownSpeed));

            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }
    }


    private IEnumerator WalkCycle()
    {
        var tripod1 = new LegMotor[3];
        var tripod2 = new LegMotor[3];
        tripod1[0] = frontLeft;
        tripod1[1] = backRight;
        tripod1[2] = centerLeft;
        tripod2[0] = frontRight;
        tripod2[1] = backLeft;
        tripod2[2] = centerRight;
        while (state==State.Go)
        {
            yield return StartCoroutine(Step1());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(Step2());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
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

    private IEnumerator ReverseWalkCycle()
    {
        while (state==State.Go)
        {
            yield return StartCoroutine(Step1());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(Step2());
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }

        yield break;

        // Create internal coroutine to handle step 1
        IEnumerator Step1()
        {
            // Start moving both tripods in parallel
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleTouchDown, -TouchDownSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleLiftOff, -LiftOffSpeed));
            
            // Wait for both tripods to finish their movements
            yield return moveTripod1;
            yield return moveTripod2;
        }

        IEnumerator Step2()
        {
            var moveTripod1 = StartCoroutine(TripodStepForward(Tripod1, AngleLiftOff, -LiftOffSpeed));
            var moveTripod2 = StartCoroutine(TripodStepForward(Tripod2, AngleTouchDown, -TouchDownSpeed));

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
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) &&
                !Input.GetKey(KeyCode.D))
            {
                yield break;
            }
            // Debug.Log(leg);
            StartCoroutine(leg.MoveToAngleAt(targetAngle, targetVelocity, true));
        }
        yield return null;
    }

}
