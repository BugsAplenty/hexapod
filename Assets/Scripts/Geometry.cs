public class Geometry
{
    public static float AngleModDeg(float angle)
    {
        if (angle >= 180) angle += -360;
        else if (angle <= -180) angle += 360;
        return angle;
    }
}