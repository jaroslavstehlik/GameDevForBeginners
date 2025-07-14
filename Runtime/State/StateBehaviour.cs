using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/State/StateBehaviour")]
    public class StateBehaviour : MonoBehaviour, IState
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;
        
        private bool _inited = false;
        [ShowInInspectorAttribute(false)] private Option _activeOption;
        
        [SerializeField] private Options _options;
        [OptionAttribute(nameof(_options))]
        [SerializeField] private Option _defaultOption;
        
        [HideInInspector]
        [SerializeField] private UnityEvent<Option> _onStateChanged;
        public UnityEvent<Option> onStateChanged => _onStateChanged;
        
        private Option _lastOption;
        public Option lastActiveOption => _lastOption;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();
        public Options options => _options;
        public Option defaultOption => _defaultOption;
        
        [Space]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onCreate; 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onValueChanged; 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [SerializeField]
        private UnityEvent<IScriptableValue> _onDestroy; 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        void Init()
        {
            if(_inited)
                return;

            _activeOption = _defaultOption;
            _inited = true;
        }

        private void Awake()
        {
            Init();
            _onCreate?.Invoke(this);
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            State.OnValidate(this);
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
                
                if (!State.isPlayingOrWillChangePlaymode)
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
            activeOption = defaultOption;
        }
    }
}