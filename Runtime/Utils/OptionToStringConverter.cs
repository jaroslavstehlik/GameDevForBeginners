using GameDevForBeginners;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OptionToStringConverter : MonoBehaviour
{
    public UnityEvent<string> onOptionConverted;
    
    public void ConvertStateToString(Option option)
    {
        if (option == null)
        {
            onOptionConverted?.Invoke("Null");
            return;
        }

        onOptionConverted?.Invoke(option.name);   
    }
}
