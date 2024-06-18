using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Lerp
{
    public static float Smooth(float time, float precision = 0.1f)
    {
        return 1f - Mathf.Pow(precision, Time.deltaTime / time);
    }

}
