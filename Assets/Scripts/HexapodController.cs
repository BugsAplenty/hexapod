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
    public float touchDownSpeed = 200f;
    private IEnumerable<LegMotor> Tripod1 => new[] {frontLeft, backLeft, centerRight};
    private IEnumerable<LegMotor> Tripod2 => new[] {frontRight, backRight, centerLeft};
    public IEnumerable<LegMotor> LegMotors => new[] {frontLeft, frontRight, backLeft, backRight, centerLeft, centerRight};
    private const float AngleLiftOff = 135f;
    private const float AngleTouchDown = 45f;
    private const float ToRestSpeed = 500;
    private float liftOffSpeed;
    private const float RangeTouchDown = AngleLiftOff - AngleTouchDown;
    private const float RangeLiftOff = 360 - RangeTouchDown;
    
    
    // State control for walk cycles
    private Coroutine walkCycleCoroutine;
    private Coroutine restCoroutine;
    private bool isWalking;
    // State enumerator containing states - stop, go

    private void Start()
    {
        liftOffSpeed = touchDownSpeed * (RangeLiftOff / RangeTouchDown);
    }

    private void Update()
    {
        // Start walking when W is pressed and the hexapod is not already walking
        if (Input.GetKeyDown(KeyCode.W) && !isWalking)
        {
            isWalking = true;
            if (restCoroutine != null)
            {
                StopCoroutine(restCoroutine);
            }
            walkCycleCoroutine = StartCoroutine(WalkCycle());
        }
        if (Input.GetKeyDown(KeyCode.A) && !isWalking)
        {
            isWalking = true;
            walkCycleCoroutine = StartCoroutine(LeftTurnCycle());
        }

        // Initiating right turn
        if (Input.GetKeyDown(KeyCode.D) && !isWalking)
        {
            isWalking = true;
            walkCycleCoroutine = StartCoroutine(RightTurnCycle());
        }

        // Initiating reverse movement
        if (Input.GetKeyDown(KeyCode.S) && !isWalking)
        {
            isWalking = true;
            walkCycleCoroutine = StartCoroutine(ReverseWalkCycle());
        }
        // Stop walking and move to rest when W is released or no keys are pressed
        else if (Input.GetKeyUp(KeyCode.W) || !IsAnyKeyHeldDown())
        {
            if (!isWalking) return;
            isWalking = false;
            if (walkCycleCoroutine != null)
            {
                StopCoroutine(walkCycleCoroutine);
            }
            restCoroutine = StartCoroutine(MoveToRest());
        }
    }

    private static bool IsAnyKeyHeldDown()
    {
        // Check if any movement key is held down
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
               Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
    
    private IEnumerator MoveToRest()
    {
        foreach (var leg in LegMotors)
        {
            StartCoroutine(leg.MoveToAngleShortestDistance(AngleLiftOff, ToRestSpeed));
        }
        isWalking = false;
        yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
    }


    private IEnumerator LeftTurnCycle()
    {
        while (isWalking)
        {
            // Step 1: Move tripod1 to AngleTouchDown, tripod2 to AngleLiftOff
            yield return StartCoroutine(MoveTripods(AngleTouchDown, touchDownSpeed, AngleLiftOff, liftOffSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));

            // Step 2: Move tripod1 to AngleLiftOff, tripod2 to AngleTouchDown
            yield return StartCoroutine(MoveTripods(AngleLiftOff, liftOffSpeed, AngleTouchDown, touchDownSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }

    private IEnumerator RightTurnCycle()
    {
        while (isWalking)
        {
            // Step 1: Move tripod1 to AngleTouchDown, tripod2 to AngleLiftOff
            yield return StartCoroutine(MoveTripods(AngleTouchDown, touchDownSpeed, AngleLiftOff, liftOffSpeed, true));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));

            // Step 2: Move tripod1 to AngleLiftOff, tripod2 to AngleTouchDown
            yield return StartCoroutine(MoveTripods(AngleLiftOff, liftOffSpeed, AngleTouchDown, touchDownSpeed, true));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }

    private IEnumerator ReverseWalkCycle()
    {
        while (isWalking)
        {
            // Step 1: Move tripod1 to AngleTouchDown, tripod2 to AngleLiftOff
            yield return StartCoroutine(MoveTripods(AngleTouchDown, -touchDownSpeed, AngleLiftOff, -liftOffSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));

            // Step 2: Move tripod1 to AngleLiftOff, tripod2 to AngleTouchDown
            yield return StartCoroutine(MoveTripods(AngleLiftOff, -liftOffSpeed, AngleTouchDown, -touchDownSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }

    private IEnumerator WalkCycle()
    {
        isWalking = true;

        while (isWalking)
        {
            yield return StartCoroutine(MoveTripods(AngleLiftOff, touchDownSpeed, AngleTouchDown, liftOffSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(MoveTripods(AngleTouchDown, liftOffSpeed, AngleLiftOff, touchDownSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }

    private IEnumerator MoveTripods(float angleForTripod1, float speedForTripod1, float angleForTripod2, float speedForTripod2, bool isTurning = false)
    {
        // Initiating movement for Tripod 1 legs
        foreach (var leg in Tripod1)
        {
            switch (isTurning)
            {
                case true when leg == centerRight:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, -speedForTripod1));
                    break;
                case true when !(leg == centerRight):
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, -speedForTripod1));
                    break;
                case false when leg == centerRight:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, speedForTripod1));
                    break;
                case false when !(leg == centerRight):
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, speedForTripod1));
                    break;
                default:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, speedForTripod1));
                    break;
            }
        }

        // Initiating movement for Tripod 2 legs
        foreach (var leg in Tripod2)
        {
            switch (isTurning)
            {
                case true when leg == centerLeft:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, -speedForTripod2));
                    break;
                case true when !(leg == centerLeft):
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, -speedForTripod2));
                    break;
                case false when leg == centerLeft:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, speedForTripod2));
                    break;
                case false when !(leg == centerLeft):
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, speedForTripod2));
                    break;
                default:
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, speedForTripod2));
                    break;
            }
        }

        // Wait for all movements to complete
        yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
    }
}
