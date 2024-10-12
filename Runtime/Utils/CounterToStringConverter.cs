using UnityEngine;
using UnityEngine.Events;

public class CounterToStringConverter : MonoBehaviour
{
    public UnityEvent<string> onCounterConverted;
    
    public void ConvertCounterToString(float value)
    {
        onCounterConverted?.Invoke(value.ToString());   
    }
}
