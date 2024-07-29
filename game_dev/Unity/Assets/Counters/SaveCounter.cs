using UnityEngine;
using UnityEngine.Events;

public class SaveCounter : MonoBehaviour
{
    // public event
    public UnityEvent<int> onCountChanged;
    
    // counter variable
    [SerializeField] int count = 0;

    // The key to our counter, it has to be unique per whole game.
    public string counterKey = "myCounterKey";
    
    // Load counter when our component is enabled
    void OnEnable()
    {
        // Check if any counter has been saved before
        if (PlayerPrefs.HasKey(counterKey))
        {
            // Load the counter in to our variable
            Set(PlayerPrefs.GetInt(counterKey));
        }
    }

    // Save counter when our component is disabled
    private void OnDisable()
    {
        PlayerPrefs.SetInt(counterKey, count);
    }

    // Method for reading counter
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
