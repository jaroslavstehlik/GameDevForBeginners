using UnityEngine;
using UnityEngine.Events;

public class SaveCounter : MonoBehaviour
{
    // public event
    public UnityEvent<int> onCountChanged;
    
    // counter variable
    [SerializeField] int _count = 0;

    // The key to our counter, it has to be unique per whole game.
    public string counterKey = "myCounterKey";
    
    // Load counter when our component is enabled
    void OnEnable()
    {
        // Check if any counter has been saved before
        if (PlayerPrefs.HasKey(counterKey))
        {
            // Load the counter in to our variable
            count = PlayerPrefs.GetInt(counterKey);
        }
    }

    public int count
    {
        get
        {
            return _count;
        }
        set
        {
            if(_count != value)
                return;
            
            _count = value;
            PlayerPrefs.SetInt(counterKey, count);
            if (onCountChanged != null)
                onCountChanged.Invoke(_count);
        }
    }

    // Method for reading count
    public int Get()
    {
        return count;
    }

    // Method for writing count
    public void Set(int value)
    {
        count = value;
    }
    
    // Method for adding count
    public void Add(int value)
    {
        count += value;
    }

    // Method for subtracting count
    public void Subtract(int value)
    {
        count -= value;
    }
    
    // Method for multiplying count
    public void Multiply(int value)
    {
        count *= value;
    }

    // Method for dividing count
    public void Divide(int value)
    {
        count /= value;
    }
}
