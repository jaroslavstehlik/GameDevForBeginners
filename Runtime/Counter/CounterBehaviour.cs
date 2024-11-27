using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterBehaviour")]
    public class CounterBehaviour : MonoBehaviour, ICountable
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [ShowInInspectorAttribute(false)] private float _count = 0;

        [SerializeField] private float _defaultCount = 0;
        public float defaultCount { get; }
        
        [SerializeField] private bool _wholeNumber = true;
        public bool wholeNumber { get; }
        
        [SerializeField] private UnityEvent<float> _onCountChanged;
        public UnityEvent<float> onCountChanged => _onCountChanged;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        private void Awake()
        {
            count = _defaultCount;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            _defaultCount = ValidateNumber(_defaultCount);
            if (!isPlayingOrWillChangePlaymode)
            {
                count = _defaultCount;
            }
        }
#endif
        float ValidateNumber(float value)
        {
            return _wholeNumber ? (int)value : value;
        }

        public float count
        {
            get => _count;
            set
            {
                float candidate = ValidateNumber(value);
                if (_count == candidate)
                    return;

                _count = candidate;

                if (!isPlayingOrWillChangePlaymode)
                    return;

                if (!_detectInfiniteLoop.Detect(this))
                    onCountChanged?.Invoke(_count);
            }
        }

        // Method for reading count
        public float Get()
        {
            return count;
        }

        // Method for writing count
        public void Set(float value)
        {
            count = value;
        }

        public void Set(Counter counter)
        {
            count = counter.count;
        }

        // Method for adding count
        public void Add(float value)
        {
            count += value;
        }

        public void Add(Counter counter)
        {
            count += counter.count;
        }

        // Method for subtracting count
        public void Subtract(float value)
        {
            count -= value;
        }

        public void Subtract(Counter counter)
        {
            count -= counter.count;
        }

        // Method for multiplying count
        public void Multiply(float value)
        {
            count *= value;
        }

        public void Multiply(Counter counter)
        {
            count *= counter.count;
        }

        // Method for dividing count
        public void Divide(float value)
        {
            count /= value;
        }

        public void Divide(Counter counter)
        {
            count /= counter.count;
        }

        public void Reset()
        {
            count = _defaultCount;
        }

        static bool isPlayingOrWillChangePlaymode
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
            }
        }
    }
}