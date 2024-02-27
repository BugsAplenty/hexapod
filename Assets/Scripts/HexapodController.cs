using UnityEngine;
using System.Collections;
using System.Linq;

public class HexapodController : MonoBehaviour
{
    public LegMotor[] tripod1; // left back, right center, left front
    public LegMotor[] tripod2; // right back, left center, right front
    public const float AngleLiftOff = -45;
    public const float AngleTouchDown = 45;
    public const float DefaultMotorForce = 10f;
    public static readonly Pid Pid = new Pid(Kp, Ki, Kd);
    public const float LimitRes = 3f;
    private const float Kp = 10f;
    private const float Ki = 1f;
    private const float Kd = 1f;
    public float liftOffAngle = 30f; // Angle for lifting off the ground
    public float touchDownAngle = -30f; // Angle for touching down
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second
    private bool isWalking = false; // Tracks if the hexapod is walking
    private int activeTripodGroup = 0; // 0: None, 1: Tripod1, 2: Tripod2
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(WalkCycle());
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            MoveToRestingPositions();
        }
    }
    public string GetDebugInfo()
    {
        var walkingState = isWalking ? "Walking" : "Idle";
        var activeGroup = activeTripodGroup switch
        {
            0 => "None",
            1 => "Tripod 1",
            _ => "Tripod 2"
        };
    
        return $"Walking State: {walkingState}\n" +
               $"Active Tripod Group: {activeGroup}";
    }

    private void MoveToRestingPositions()
    {
        StopAllCoroutines();
        isWalking = false; // Mark as not walking
        activeTripodGroup = 0; // No active group
        foreach (var leg in tripod1.Concat(tripod2))
        {
            leg.StopRotation();
        }
    }

    private IEnumerator WalkCycle()
    {
        isWalking = true; // Mark as walking
        while (true)
        {
            activeTripodGroup = 1; // Tripod 1 is active
            yield return StartCoroutine(MoveLegsGroupAndHold(tripod1, liftOffAngle, rotationSpeed));
        
            activeTripodGroup = 2; // Tripod 2 is active
            yield return StartCoroutine(MoveLegsGroupAndHold(tripod2, touchDownAngle, rotationSpeed));

            // Wait and check for continuous walking
            yield return new WaitForSeconds(1);

            if (Input.GetKey(KeyCode.W)) continue;
            MoveToRestingPositions();
            break;
        }
    }
    private static IEnumerator MoveLegsGroupAndHold(LegMotor[] legs, float targetAngle, float speed)
    {
        foreach (var leg in legs)
        {
            // Determine whether to rotate clockwise or counterclockwise based on shortest path
            var clockWise = DetermineShortestPath(leg.Angle(), targetAngle);
            leg.MoveToForward(speed, targetAngle);
        }

        // Wait for all legs in the group to reach the target angle
        bool allReached;
        do
        {
            allReached = legs.All(leg => Mathf.Abs(Mathf.DeltaAngle(leg.Angle(), targetAngle)) < 1f);
            yield return null;
        } while (!allReached);

        // Once all legs have reached the target, hold their position
        foreach (var leg in legs)
        {
            leg.StopRotation();
        }
    }

    // Determines the shortest path to the target angle (clockwise or counterclockwise)
    private static bool DetermineShortestPath(float currentAngle, float targetAngle)
    {
        var clockwiseDistance = Mathf.DeltaAngle(currentAngle, targetAngle);
        return clockwiseDistance < 0;
    }
}
