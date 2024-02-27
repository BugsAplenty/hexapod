using UnityEngine;
using System.Collections;
using System.Linq;

public class HexapodController : MonoBehaviour
{
    public LegMotor[] tripod1; // left back, right center, left front
    public LegMotor[] tripod2; // right back, left center, right front

    public float liftOffAngle = 30f; // Angle for lifting off the ground
    public float touchDownAngle = -30f; // Angle for touching down
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

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

    private void MoveToRestingPositions()
    {
        StopAllCoroutines(); // Stop the walk cycle
        foreach (var leg in tripod1.Concat(tripod2))
        {
            leg.StopAndHoldPosition();
        }
    }

    private IEnumerator WalkCycle()
    {
        while (true) // Continuously cycle the walk as long as 'W' is held down
        {
            // Lift off one tripod and touch down the other
            yield return StartCoroutine(MoveLegsGroupAndHold(tripod1, liftOffAngle, rotationSpeed));
            yield return StartCoroutine(MoveLegsGroupAndHold(tripod2, touchDownAngle, rotationSpeed));

            // Swap roles for the next cycle
            yield return new WaitForSeconds(1); // Adjust wait time as needed for synchronization

            if (!Input.GetKey(KeyCode.W))
            {
                MoveToRestingPositions();
                break;
            }
        }
    }

    private IEnumerator MoveLegsGroupAndHold(LegMotor[] legs, float targetAngle, float speed)
    {
        foreach (var leg in legs)
        {
            // Determine whether to rotate clockwise or counterclockwise based on shortest path
            bool clockWise = DetermineShortestPath(leg.CurrentAngle, targetAngle);
            leg.RotateToAngle(targetAngle, clockWise, speed);
        }

        // Wait for all legs in the group to reach the target angle
        bool allReached;
        do
        {
            allReached = legs.All(leg => Mathf.Abs(Mathf.DeltaAngle(leg.CurrentAngle, targetAngle)) < 1f);
            yield return null;
        } while (!allReached);

        // Once all legs have reached the target, hold their position
        foreach (var leg in legs)
        {
            leg.StopAndHoldPosition();
        }
    }

    // Determines the shortest path to the target angle (clockwise or counterclockwise)
    private static bool DetermineShortestPath(float currentAngle, float targetAngle)
    {
        var clockwiseDistance = Mathf.DeltaAngle(currentAngle, targetAngle);
        return clockwiseDistance < 0;
    }
}
