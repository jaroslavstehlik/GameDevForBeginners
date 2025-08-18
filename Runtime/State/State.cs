using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct StateDescriptor
    {
        public string name;
        public Option defaultOption;
        public Options options;
    }
    
    [CreateAssetMenu(fileName = "State", menuName = "GMD/State/State", order = 1)]
    public class State : ScriptableObject, IState
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        private bool _inited = false;
        private Option _activeOption;

        [SerializeField] private Options _options;
        public Options options => _options;
        
        [OptionAttribute(nameof(_options))]
        [SerializeField] private Option _defaultOption = null;
        public Option defaultOption => _defaultOption;
        
        [HideInInspector] 
        [SerializeField] private UnityEvent<Option> _onStateChanged = new UnityEvent<Option>();
        public UnityEvent<Option> onStateChanged => _onStateChanged;

        private Option _lastOption;
        public Option lastActiveOption => _lastOption;

        [Space]
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onCreate = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onValueChanged = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onDestroy = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        public static void OnValidate(IState state)
        {
            if (!isPlayingOrWillChangePlaymode)
                state.activeOption = state.defaultOption;
        }

        void Init(bool force = false)
        {
            if(_inited && !force)
                return;

            _activeOption = _defaultOption;
            _inited = true;
        }

        private void Awake()
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
            OnValidate(this);
        }
#endif
        public ScriptableValueType GetValueType()
        {
            return ScriptableValueType.String;
        }

        public string GetValue()
        {
            if (activeOption == null) 
                return string.Empty;
            return activeOption.name.ToString();
        }

        public Option activeOption
        {
            get
            {
                Init();
                return _activeOption;
            }
            set
            {
                Init();
                if (_options == null)
                    return;

                if (!_options.options.Contains(value))
                {
                    Debug.LogError($"Option: {value.name} is invalid! It must be present in: {_options.name}");
                    return;
                }
                
                if (_activeOption == value)
                    return;

                _lastOption = _activeOption;
                _activeOption = value;

                if (!isPlayingOrWillChangePlaymode)
                    return;

                if (!_detectInfiniteLoop.Detect(this))
                {
                    _onValueChanged?.Invoke(this);
                    _onStateChanged?.Invoke(_activeOption);
                }
            }
        }

        public void Reset()
        {
            activeOption = _defaultOption;
        }

        public int activeOptionIndex
        {
            get
            {
                if (_options == null) return -1;
                return _options.GetOptionIndex(_activeOption);
            }
        }
        
        public void SetPreviousOption(bool cycle = false)
        {
            int index = cycle ? activeOptionIndex - 1 % _options.Length() : Math.Min(activeOptionIndex - 1, 0); 
            activeOption = _options[index];
        }
        
        public void SetNextOption(bool cycle = false)
        {
            int index = cycle ? activeOptionIndex + 1 % _options.Length() : Math.Max(activeOptionIndex - 1, _options.Length() - 1);
            activeOption = _options[index];
        }
        public static bool isPlayingOrWillChangePlaymode
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