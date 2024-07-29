using UnityEngine;
using UnityEngine.Events;

public class GameObjectCounter : MonoBehaviour
{
    // public event
    public UnityEvent<int> onCountChanged;
    
    // counter variable
    [SerializeField] int count = 0;

    // Method for reading count
    public int Get()
    {
        return count;
    }

    // Method for writing count
    public void Set(int value)
    {
        count = value;
        if (onCountChanged != null)
            onCountChanged.Invoke(count);
    }
    
    // Method for adding count
    public void Add(int value)
    {
        count += value;
        if (onCountChanged != null)
            onCountChanged.Invoke(count);
    }

    // Method for subtracting count
    public void Subtract(int value)
    {
        count -= value;
        if (onCountChanged != null)
            onCountChanged.Invoke(count);
    }
}
