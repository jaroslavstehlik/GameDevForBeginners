using UnityEngine;

public static class LerpUtils
{
    // This lerp utility is using exponential decay which is resilient towards low frame rates
    // It is based on Freya Holmer lerp video
    // https://www.youtube.com/watch?v=LSNQuFEDOyQ
    
    private const float DECAY = 16;
    
    public static float Lerp(float a, float b, float t)
    {
        return b + (a - b) * Mathf.Exp(-DECAY * t);
    }
    
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return b + (a - b) * Mathf.Exp(-DECAY * t);
    }
    
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return b + (a - b) * Mathf.Exp(-DECAY * t);
    }
    
    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        return b + (a - b) * Mathf.Exp(-DECAY * t);
    }
}
