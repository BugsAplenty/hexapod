using System;
using System.Collections;
using UnityEngine;

public class LegMotor : MonoBehaviour
{
    public new HingeJoint hingeJoint;
    private float targetAngleDeg;
    public Rigidbody rb;

    public bool HasReachedTarget { get; private set; }

    public IEnumerator MoveToAngleAt(float targetAngle, float rotationSpeed)
    {
        targetAngleDeg = targetAngle;
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) &&
            !Input.GetKey(KeyCode.D))
        {
            yield break;
        }
        HasReachedTarget = false;
        StartCoroutine(StartMotor(rotationSpeed));
        while (!HasReachedTarget)
        {
            if (Math.Abs(targetAngle - AngleDeg()) < 5f)
            {
                HasReachedTarget = true; // Set this before stopping the motor
                StartCoroutine(StopMotor());
            }
            yield return null;
        }
    }

    public IEnumerator MoveToAngleShortestDistance(float targetAngle, float rotationSpeed)
    {
        var angleDiff = Geometry.AngleModDiff(targetAngle, AngleDeg());
        StartCoroutine(Math.Abs(angleDiff) < 180
            ? MoveToAngleAt(targetAngle, -rotationSpeed)
            : MoveToAngleAt(targetAngle, rotationSpeed));
        yield return null;
    }

    private IEnumerator StartMotor(float rotationSpeed)
    {
        hingeJoint.useLimits = false;
        hingeJoint.useMotor = true;
        var motor = hingeJoint.motor;
        motor.targetVelocity = -rotationSpeed;
        motor.force = 100f;
        hingeJoint.motor = motor;
        yield return null;
    }
    
    public string GetDebugInfo()
    {
        return $"Current angle: {AngleDeg():F2}°\n" +
               $"Current velocity: {RotationSpeed():F2}°/s\n" +
               $"Current motor force: {hingeJoint.motor.force:F2}\n" +
               $"Target angle: {targetAngleDeg:F2}°\n" +
               $"Target velocity: {hingeJoint.motor.targetVelocity:F2}°/s\n" +
               $"Is motor enabled: {hingeJoint.useMotor}\n" +
               $"Has Leg Reached Target: {HasReachedTarget}";
    }
    
    private IEnumerator StopMotor()
    {
        // hingeJoint.useLimits = true;
        var motor = hingeJoint.motor;
        motor.targetVelocity = 0f;
        hingeJoint.motor = motor;
        // var limits = hingeJoint.limits;
        // var currentAngle = hingeJoint.angle;
        // limits.min = currentAngle - 5f;
        // limits.max = currentAngle + 5f;
        // hingeJoint.limits = limits;
        yield return null;
    }

    private float AngleDeg()
    {
        return 180f - rb.transform.localEulerAngles.z;
    }

    private float RotationSpeed() {
        return rb.angularVelocity.z;
    }
}


