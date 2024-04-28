using System;
using System.Collections;
using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;

public class LegMotor : MonoBehaviour
{
    public new HingeJoint hingeJoint;
    public new Rigidbody rigidbody;
    private float targetAngleDeg;
    public bool HasReachedTarget { get; private set; }
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Calculate the normal force
            Vector3 normalForce = contact.normal * (rigidbody.mass * Physics.gravity.magnitude);
            
            // Determine the direction of friction (opposite to the velocity)
            Vector3 frictionDirection = -rigidbody.linearVelocity.normalized;
            float frictionMagnitude = 1f * normalForce.magnitude; // Assume a friction coefficient
            Vector3 frictionForce = frictionDirection * frictionMagnitude;
            
            // Apply the friction force at the point of contact
            rigidbody.AddForceAtPosition(frictionForce, contact.point);
        }
    }
    public IEnumerator MoveToAngleAt(float targetAngle, float rotationSpeed)
    {
        targetAngleDeg = targetAngle;
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) &&
            !Input.GetKey(KeyCode.D))
        {
            yield break;
        }
        HasReachedTarget = false;
        while (!HasReachedTarget)
        {
            var angleToRotate = Math.Abs(targetAngle - AngleDeg());
            var step = -rotationSpeed * Time.deltaTime;
            rigidbody.MoveRotation(Quaternion.Euler(0, 0, rigidbody.rotation.eulerAngles.z + step));
            if (angleToRotate < 2f)
            {
                HasReachedTarget = true; // Set this before stopping the motor
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator MoveToAngleShortestDistance(float targetAngle, float rotationSpeed)
    {
        var angleDiff = Geometry.AngleModDiff(targetAngle, AngleDeg());
        StartCoroutine(MoveToAngleAt(targetAngle, Math.Abs(angleDiff) < 180 ? rotationSpeed : -rotationSpeed));
        yield return null;
    }

    private IEnumerator StartMotor(float rotationSpeed)
    {
        hingeJoint.useLimits = false;
        hingeJoint.useMotor = true;
        var motor = hingeJoint.motor;
        motor.targetVelocity = -rotationSpeed;
        motor.force = 50f;
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
        // hingeJoint.useLimits = true;
        hingeJoint.useMotor = false;
        // var limits = hingeJoint.limits;
        // var currentAngle = hingeJoint.angle;
        // limits.min = currentAngle - 5f;
        // limits.max = currentAngle + 5f;
        // hingeJoint.limits = limits;
        yield return null;
    }

    private float AngleDeg()
    {
        return 180f - rigidbody.rotation.eulerAngles.z;
    }
}


