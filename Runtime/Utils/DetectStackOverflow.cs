using UnityEngine;

public class DetectStackOverflow 
{
    private int _maxChangesPerFrame = 0;
    private int _currentFrameCounterChanges = 0;
    private int _lastCountChangeFrameIndex = 0;
    
    public DetectStackOverflow(int maxChangesPerFrame = 1000000)
    {
        _maxChangesPerFrame = maxChangesPerFrame;
    }
    
    public bool Detect()
    {
        if (_lastCountChangeFrameIndex != Time.frameCount)
        {
            _lastCountChangeFrameIndex = Time.frameCount;
            _currentFrameCounterChanges = 0;
            return false;
        }
        
        if (_currentFrameCounterChanges >= _maxChangesPerFrame)
        {
            Debug.LogError("Detected possible stack overflow!");
            return true;
        }

        _currentFrameCounterChanges++;
        return false;
    }
}
