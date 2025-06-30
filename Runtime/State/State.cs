using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct StateDescriptor
    {
        public string name;
        public Option defaultOption;
        public Options options;
        public string saveKey;
    }
    
    [CreateAssetMenu(fileName = "State", menuName = "GMD/State/State", order = 1)]
    public class State : ScriptableObject, IState
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [ShowInInspectorAttribute(false)] private Option _activeOption;

        [SerializeField] private Options _options;
        public Options options => _options;
        
        [OptionAttribute(nameof(_options))]
        [SerializeField] private Option _defaultOption = null;
        public Option defaultOption => _defaultOption;
        
        // The key to our state, it has to be unique per whole game.
        [SerializeField] private string _saveKey = string.Empty;
        [HideInInspector] [SerializeField] private UnityEvent<Option> _onStateChanged;
        public UnityEvent<Option> onStateChanged => _onStateChanged;

        private Option _lastOption;
        public Option lastActiveOption => _lastOption;

        [Space]
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

        public static void OnEnable(IState state, string saveKey)
        {
            if (isPlayingOrWillChangePlaymode &&
                !string.IsNullOrEmpty(saveKey) &&
                PlayerPrefs.HasKey(saveKey))
            {
                string optionName = PlayerPrefs.GetString(saveKey);
                foreach (var option in state.options.options)
                {
                    if (optionName == option.name)
                    {
                        state.activeOption = option;
                        break;
                    }
                }
                state.activeOption = null;
            }
            else
            {
                state.activeOption = state.defaultOption;
            }
        }

        public static void OnDisable(IState state)
        {
            state.activeOption = state.defaultOption;
        }
        

        public static void OnValidate(IState state)
        {
            if (!isPlayingOrWillChangePlaymode)
            {
                state.activeOption = state.defaultOption;
            }
        }

        private void Awake()
        {
            _onCreate?.Invoke(this);
        }

        private void OnEnable()
        {
            OnEnable(this, _saveKey);
        }

        private void OnDisable()
        {
            OnDisable(this);
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
#if UNITY_EDITOR
            get
            {
                if (isPlayingOrWillChangePlaymode)
                    return _activeOption;

                return _defaultOption;
            }
#else
        get => _activeState;
#endif
            set
            {
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

                if (!string.IsNullOrEmpty(_saveKey))
                    PlayerPrefs.SetString(_saveKey, _activeOption.name);

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