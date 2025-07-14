using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterDescriptor
    {
        public string name;
        public float defaultCount;
        public bool wholeNumber;
    }
    
// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
    [CreateAssetMenu(fileName = "Counter", menuName = "GMD/Counter/Counter", order = 1)]

// Scriptable object can be stored only in project
// it can be referenced in scene
// it is used mostly for holding game data
    public class Counter : ScriptableObject, ICountable
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        private bool _inited = false;
        private float _count = 0;

        [SerializeField] private float _defaultCount = 0;
        public float defaultCount => _defaultCount;

        [SerializeField] private bool _wholeNumber = true;
        public bool wholeNumber => _wholeNumber;

        [HideInInspector] [SerializeField] private UnityEvent<float> _onCountChanged;
        public UnityEvent<float> onCountChanged => _onCountChanged;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onCreate; 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onValueChanged; 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onDestroy; 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        void Init(bool force = false)
        {
            if(_inited && !force)
                return;

            _count = _defaultCount;
            _inited = true;
        }

        void Awake()
        {
            Init();
            _onCreate?.Invoke(this);
        }

        // For scriptable objects only!
        // Lifetime of SO is longer than scene objects! 
        private void OnEnable()
        {
            Init(true);
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke(this);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            _defaultCount = ValidateNumber(_defaultCount);
            if (!isPlayingOrWillChangePlaymode)
                count = _defaultCount;
        }
#endif

        public ScriptableValueType GetValueType()
        {
            return ScriptableValueType.Number;
        }

        public string GetValue()
        {
            return count.ToString();
        }

        float ValidateNumber(float value)
        {
            return _wholeNumber ? (int)value : value;
        }

        public float count
        {
            get
            {
                Init();
                return _count;
            }
            set
            {
                Init();
                float candidate = ValidateNumber(value);
                if (_count == candidate)
                    return;

                _count = candidate;

                if (!isPlayingOrWillChangePlaymode)
                    return;

                if (!_detectInfiniteLoop.Detect(this))
                {
                    _onValueChanged?.Invoke(this);
                    _onCountChanged?.Invoke(_count);
                }
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