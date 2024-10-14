using UnityEngine;

public class DetectInfiniteLoop 
{
    private int _maxChangesPerFrame = 0;
    private int _currentFrameCounterChanges = 0;
    private int _lastCountChangeFrameIndex = 0;

    public DetectInfiniteLoop(int maxChangesPerFrame = 1000)
    {
        _maxChangesPerFrame = maxChangesPerFrame;
    }
    
    public bool Detect(UnityEngine.Object target)
    {
        if (_lastCountChangeFrameIndex != Time.frameCount)
        {
            _lastCountChangeFrameIndex = Time.frameCount;
            _currentFrameCounterChanges = 0;
            return false;
        }
        
        if (_currentFrameCounterChanges >= _maxChangesPerFrame)
        {
            Debug.LogError("Detected possible infinite loop!", target);
            return true;
        }
        
        _currentFrameCounterChanges++;
        return false;
    }
}
