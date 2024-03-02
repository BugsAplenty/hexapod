public static class Geometry
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

    public static float AngleModDiff(float angle, float otherAngle)
    {
        return AngleModDeg(angle - otherAngle);
    }
}