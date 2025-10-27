using UnityEngine;
using UnityEngine.Events;

public class Mountable : MonoBehaviour
{
    public UnityEvent onMounted;
    public UnityEvent onUnmounted;
    
    private bool _mounted = false;
    public bool mounted => _mounted;

    public void Mount()
    {
        if(_mounted)
            return;
        
        _mounted = true;
        onMounted?.Invoke();
    }

    public void Unmount()
    {
        if(!_mounted)
            return;

        _mounted = false;
        onUnmounted?.Invoke();
    }
    
}
