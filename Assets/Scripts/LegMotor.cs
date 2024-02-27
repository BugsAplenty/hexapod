using System.Collections;
using UnityEngine;

public class LegMotor : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public float targetAngle; // The target angle for the leg to rotate to
    private PID pid;
    private Rigidbody rb;
    private float desiredVelocity = 100f; // Desired constant velocity in degrees per second
    private float maxForce = 100f; // Maximum force the PID controller can apply
    public float CurrentAngle => hingeJoint.transform.localEulerAngles.z;
    // PID coefficients
    private float kp = 10f; // Proportional gain
    private float ki = 0.1f; // Integral gain
    private float kd = 5f; // Derivative gain

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pid = new PID(kp, ki, kd);
    }
// Updated RotateToAngle method to include a clockwise parameter
    public void RotateToAngle(float angle, bool clockwise, float speed)
    {
        targetAngle = angle;
        // The 'clockwise' parameter is not used in the current implementation
        // as the direction is determined by the PID controller based on the angle error.
    }
    // Method to stop the leg movement and hold its current position
    public void StopAndHoldPosition()
    {
        targetAngle = CurrentAngle; // Set the target angle to the current angle to minimize error
        // This will cause the PID controller to apply minimal force to maintain the current position
    }
    void FixedUpdate()
    {
        float currentAngle = hingeJoint.transform.localEulerAngles.z;
        float angleError = Mathf.DeltaAngle(currentAngle, targetAngle);

        // If we're close to the target angle, switch to holding position
        if (Mathf.Abs(angleError) < 1f)
        {
            HoldPositionWithDynamicForce(angleError);
        }
        else
        {
            MoveWithConstantVelocity(angleError);
        }
    }

    private void HoldPositionWithDynamicForce(float angleError)
    {
        // Compute the PID output for torque to apply
        float force = pid.Compute(angleError, 0, Time.fixedDeltaTime);
        force = Mathf.Clamp(force, -maxForce, maxForce); // Clamp force to max limits

        // Apply torque around Z-axis to hold position
        rb.AddTorque(Vector3.forward * force);
    }

    private void MoveWithConstantVelocity(float angleError)
    {
        // Determine direction to rotate based on angle difference
        float direction = angleError > 0 ? 1f : -1f;
        // Convert desired velocity to radians per second, then to angular velocity in the rigidbody's scale
        rb.angularVelocity = Vector3.forward * (desiredVelocity * Mathf.Deg2Rad * direction);
    }

    // Call this method to rotate the leg to a new angle
    
}
