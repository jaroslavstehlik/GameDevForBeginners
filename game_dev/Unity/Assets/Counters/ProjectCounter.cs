using UnityEngine;
using UnityEngine.Events;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Counter", menuName = "GMD/Counter", order = 1)]

// Scriptable object can be stored only in project
// it can be referenced in scene
// it is used mostly for holding game data
public class ProjectCounter : ScriptableObject
{
    // public event
    public UnityEvent<int> onCounterChanged;
    
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
        if (onCounterChanged != null)
            onCounterChanged.Invoke(count);
    }
    
    // Method for adding count
    public void Add(int value)
    {
        count += value;
        if (onCounterChanged != null)
            onCounterChanged.Invoke(count);
    }

    // Method for subtracting count
    public void Subtract(int value)
    {
        count -= value;
        if (onCounterChanged != null)
            onCounterChanged.Invoke(count);
    }
}