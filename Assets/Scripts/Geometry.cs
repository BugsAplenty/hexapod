public class Geometry
{
    public static float AngleModDeg(float angle)
    {
        switch (angle)
        {
            case >= 180:
                angle += -360;
                break;
            case <= -180:
                angle += 360;
                break;
        }

        return angle;
    }
}