using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    public int frameRate = 60;

    private void Update()
    {
        if(frameRate <= 0)
            return;
        
        long ticks = DateTime.UtcNow.Ticks;
        float frameDuration = 1f / (float)frameRate;
        while (TimeSpan.FromTicks(DateTime.UtcNow.Ticks - ticks).TotalSeconds < frameDuration)
        {
            
        }
    }
}
