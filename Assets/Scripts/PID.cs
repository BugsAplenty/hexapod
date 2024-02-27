public class PID
{
    public float P, I, D;

    private float previousError, integral;

    public PID(float p, float i, float d)
    {
        P = p;
        I = i;
        D = d;
    }

    public float Compute(float setPoint, float actual, float timeFrame)
    {
        var error = setPoint - actual;
        integral += error * timeFrame;
        var derivative = (error - previousError) / timeFrame;
        previousError = error;
        return P * error + I * integral + D * derivative;
    }
}