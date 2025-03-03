using UnityEngine;

public static class MathUtils
{
    public static Vector3 ProjectForward(Vector3 forward, Vector3 up)
    {
        return Vector3.Normalize(Vector3.Cross(up, Vector3.Normalize(Vector3.Cross(forward, up))));
    }
}
