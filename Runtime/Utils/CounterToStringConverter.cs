using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    public class CounterToStringConverter : MonoBehaviour
    {
        public UnityEvent<string> onCounterConverted;
        public string format = "N1";

        public void ConvertCounterToString(float value)
        {
            onCounterConverted?.Invoke(value.ToString(format));
        }
    }
}