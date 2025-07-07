using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterBehaviour")]
    public class CounterBehaviour : MonoBehaviour, ICountable
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        private bool _inited = false;
        [ShowInInspectorAttribute(false)] private float _count = 0;

        [SerializeField] private string _name;
        public string name => _name;

        [SerializeField] private float _defaultCount = 0;
        public float defaultCount => _defaultCount;

        [SerializeField] private bool _wholeNumber = true;
        public bool wholeNumber => _wholeNumber;

        [HideInInspector] [SerializeField] private UnityEvent<float> _onCountChanged = new UnityEvent<float>();
        public UnityEvent<float> onCountChanged => _onCountChanged;
        
        [Space]
        [HideInInspector] [SerializeField] private UnityEvent<IScriptableValue> _onCreate = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector] [SerializeField] private UnityEvent<IScriptableValue> _onValueChanged = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector] [SerializeField] private UnityEvent<IScriptableValue> _onDestroy = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        void Init()
        {
            if(_inited)
                return;

            _count = _defaultCount;
            _inited = true;
        }
        
        private void Awake()
        {
            Init();
            _onCreate?.Invoke(this);
        }

        private void OnEnable()
        {
            count = _defaultCount;
        }

        private void OnDisable()
        {
            count = _defaultCount;
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