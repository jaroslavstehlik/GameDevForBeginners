using UnityEngine.Events;

namespace GameDevForBeginners
{
    public interface ICountable
    {
        public string name { get; }
        public float defaultCount { get; }
        public bool wholeNumber { get; }
        public float count { get; set; }
        public UnityEvent<float> onCountChanged { get; }

        // Method for reading count
        public float Get();

        // Method for writing count
        public void Set(float value);
        public void Set(Counter counter);

        // Method for adding count
        public void Add(float value);
        public void Add(Counter counter);

        // Method for subtracting count
        public void Subtract(float value);
        public void Subtract(Counter counter);

        // Method for multiplying count
        public void Multiply(float value);
        public void Multiply(Counter counter);

        // Method for dividing count
        public void Divide(float value);
        public void Divide(Counter counter);
        public void Reset();
    }
}