using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class Pid
{
    private float p, i, d;
    private float kp, ki, kd;
    private float prevError;

    /// <summary>
    /// Constant proportion
    /// </summary>
    public float Kp
    {
        get => kp;
        set => kp = value;
    }

    /// <summary>
    /// Constant integral
    /// </summary>
    public float Ki
    {
        get => ki;
        set => ki = value;
    }

    /// <summary>
    /// Constant derivative
    /// </summary>
    public float Kd
    {
        get => kd;
        set => kd = value;
    }

    public Pid(float p, float i, float d)
    {
        kp = p;
        ki = i;
        kd = d;
    }

    /// <summary>
    /// Based on the code from Brian-Stone on the Unity forums
    /// https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645
    /// </summary>
    /// <param name="currentError"></param>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public float GetOutput(float currentError, float deltaTime)
    {
        p = currentError;
        i += p * deltaTime;
        d = (p - prevError) / deltaTime;
        prevError = currentError;
        
        return p * Kp + i * Ki + d * Kd;
    }
}