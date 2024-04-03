using System;
using System.Collections;
using UnityEngine;

public class LegMotor : MonoBehaviour
{
    public new HingeJoint hingeJoint;
    private float targetAngleDeg;

    public bool HasReachedTarget { get; private set; }

    public IEnumerator MoveToAngleAt(float targetAngle, float rotationSpeed, bool clockwise)
    {
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) &&
            !Input.GetKey(KeyCode.D))
        {
            yield break;
        }
        HasReachedTarget = false;
        targetAngleDeg = targetAngle;
        var angleDiff = targetAngle - AngleDeg();
        if (Math.Abs(angleDiff) > 5f)
        {
            StartCoroutine(StartMotor(rotationSpeed, clockwise));
        }
        while (!HasReachedTarget)
        {
            angleDiff = targetAngle - AngleDeg();
            if (Math.Abs(angleDiff) < 5f)
            {
                StartCoroutine(StopMotor());
                HasReachedTarget = true;
            }
            yield return null;
        }
    }

    public IEnumerator MoveToAngleShortestDistance(float targetAngle, float rotationSpeed)
    {
        var angleDiff = targetAngle - AngleDeg();
        StartCoroutine(Math.Abs(angleDiff) < 180
            ? MoveToAngleAt(targetAngle, rotationSpeed, true)
            : MoveToAngleAt(targetAngle, rotationSpeed, false));
        yield return null;
    }

    private IEnumerator StartMotor(float rotationSpeed, bool clockwise)
    {
        hingeJoint.useLimits = false;
        hingeJoint.useMotor = true;
        var motor = hingeJoint.motor;
        motor.targetVelocity = rotationSpeed * (clockwise ? -1 : 1);
        motor.force = 10f;
        hingeJoint.motor = motor;
        yield return null;
    }
    
    public string GetDebugInfo()
    {
        return $"Current angle: {AngleDeg():F2}°\n" +
               $"Current velocity: {hingeJoint.velocity:F2}°/s\n" +
               $"Target angle: {targetAngleDeg:F2}°\n" +
               $"Target velocity: {hingeJoint.motor.targetVelocity:F2}°/s\n" +
               $"Is motor enabled: {hingeJoint.useMotor}\n" +
               $"Has Leg Reached Target: {HasReachedTarget}";
    }
    
    private IEnumerator StopMotor()
    {
        hingeJoint.useLimits = true;
        hingeJoint.useMotor = false;
        var limits = hingeJoint.limits;
        var currentAngle = AngleDeg();
        limits.min = currentAngle - 5f;
        limits.max = currentAngle + 5f;
        hingeJoint.limits = limits;
        yield return null;
    }

    private float AngleDeg()
    {
        return transform.rotation.eulerAngles.z - 180f;
    }
}


