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
    public float touchDownSpeed = 100f;
    private IEnumerable<LegMotor> Tripod1 => new[] {frontLeft, backLeft, centerRight};
    private IEnumerable<LegMotor> Tripod2 => new[] {frontRight, backRight, centerLeft};
    public IEnumerable<LegMotor> LegMotors => new[] {frontLeft, frontRight, backLeft, backRight, centerLeft, centerRight};
    private const float AngleLiftOff = 140f;
    private const float AngleTouchDown = 30f;
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
                Debug.Log("Stopping rest coroutine");
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
            if (isWalking) {
                isWalking = false;
                if (walkCycleCoroutine != null)
                {
                    StopCoroutine(walkCycleCoroutine);
                }
                StartCoroutine(MoveToRest());
            } 
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
        var coroutines = LegMotors.Select(leg => StartCoroutine(leg.MoveToAngleShortestDistance(AngleLiftOff, ToRestSpeed))).ToList();

        // Wait for all legs to complete their movements to the rest position
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        isWalking = false;
    }

    private IEnumerator LeftTurnCycle()
    {
        isWalking = true;
        while (isWalking)
        {
            // Step 1: Move tripod1 to AngleTouchDown, tripod2 to AngleLiftOff
            yield return StartCoroutine(TurnTripods(AngleTouchDown, -touchDownSpeed, AngleLiftOff, -liftOffSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));

            // Step 2: Move tripod1 to AngleLiftOff, tripod2 to AngleTouchDown
            yield return StartCoroutine(TurnTripods(AngleLiftOff, -liftOffSpeed, AngleTouchDown, -touchDownSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }   

    private IEnumerator RightTurnCycle()
    {
        isWalking = true;
        while (isWalking)
        {
            // Similar approach as LeftTurnCycle but with adjusted angles and speeds for a right turn
            yield return StartCoroutine(TurnTripods(AngleLiftOff, touchDownSpeed, AngleTouchDown, liftOffSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
            yield return StartCoroutine(TurnTripods(AngleTouchDown, liftOffSpeed, AngleLiftOff, touchDownSpeed));
            yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
        }
    }

    private IEnumerator ReverseWalkCycle()
    {
        isWalking = true;
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

    private IEnumerator MoveTripods(float angleForTripod1, float speedForTripod1, float angleForTripod2, float speedForTripod2)
    {
        {
            // For forward or reverse walking, move all legs normally
            foreach (var leg in Tripod1)
            {
                StartCoroutine(leg.MoveToAngleAt(angleForTripod1, speedForTripod1));
            }

            foreach (var leg in Tripod2)
            {
                StartCoroutine(leg.MoveToAngleAt(angleForTripod2, speedForTripod2));
            }
        }
        yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
    }
    private IEnumerator TurnTripods(float angleForTripod1, float speedForTripod1, float angleForTripod2, float speedForTripod2)
    {
        {
            // For forward or reverse walking, move all legs normally
            foreach (var leg in Tripod1)
            {
                if (leg == centerRight) {
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, -speedForTripod1));
                } else {
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, speedForTripod1));
                }
            }

            foreach (var leg in Tripod2)
            {
                if (leg == centerLeft) {
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod2, speedForTripod2));
                } else {
                    StartCoroutine(leg.MoveToAngleAt(angleForTripod1, -speedForTripod2));
                }
            }
        }
        yield return new WaitUntil(() => LegMotors.All(leg => leg.HasReachedTarget));
    }
}
