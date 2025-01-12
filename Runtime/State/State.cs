using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// TODO: Add hash optimisation to prevent bruteforce string checks
namespace GameDevForBeginners
{
    [System.Serializable]
    public struct StateDescriptor
    {
        public string name;
        [State] public string defaultState;
        public string[] states;
        public string saveKey;
    }
    
    [CreateAssetMenu(fileName = "State", menuName = "GMD/State/State", order = 1)]
    public class State : ScriptableValue, IState
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [ShowInInspectorAttribute(false)] private string _activeState;

        [FormerlySerializedAs("states")] [SerializeField] private string[] _states = new[]
        {
            "default"
        };
        public string[] states => _states;

        [State] [SerializeField] private string _defaultState = "default";
        public string defaultState => _defaultState;
        
        // The key to our counter, it has to be unique per whole game.
        [SerializeField] private string _saveKey = string.Empty;
        [HideInInspector] [SerializeField] private UnityEvent<string> _onStateChanged;
        public UnityEvent<string> onStateChanged => _onStateChanged;
        
        private string _lastState;
        public string lastState => _lastState;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        public static State CreateState(StateDescriptor stateDescriptor)
        {
            State state = CreateInstance<State>();
            state._onValueChanged = new UnityEvent<ScriptableValue>();
            state._onStateChanged = new UnityEvent<string>();
            state._onDestroy = new UnityEvent<ScriptableValue>();
            state._states = stateDescriptor.states;
            state._defaultState = stateDescriptor.defaultState;
            state._activeState = stateDescriptor.defaultState;
            state._saveKey = stateDescriptor.saveKey;
            state.name = stateDescriptor.name;
            state.OnEnable();
            return state;
        }

        private void OnEnable()
        {
            if (isPlayingOrWillChangePlaymode &&
                !string.IsNullOrEmpty(_saveKey) &&
                PlayerPrefs.HasKey(_saveKey))
            {
                // Load the counter in to our variable
                activeState = PlayerPrefs.GetString(_saveKey);
            }
            else
            {
                activeState = _defaultState;
            }
        }

        private void OnDisable()
        {
            activeState = _defaultState;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (_states != null)
            {
                HashSet<string> encounteredStates = new HashSet<string>();
                foreach (var state in _states)
                {
                    if (!encounteredStates.Add(state))
                    {
                        Debug.LogError($"{name}, state: {state} already exists!", this);
                    }
                }
            }

            if (!isPlayingOrWillChangePlaymode)
            {
                activeState = _defaultState;
            }
        }
#endif

        public override ScriptableValueType GetValueType()
        {
            return ScriptableValueType.State;
        }

        public override string GetValue()
        {
            return activeState.ToString();
        }

        public string activeState
        {
#if UNITY_EDITOR
            get
            {
                if (isPlayingOrWillChangePlaymode)
                    return _activeState;

                return _defaultState;
            }
#else
        get => _activeState;
#endif
            set
            {
                if (_states == null)
                    return;
                
                string stateCandidate = null;
                foreach (var state in _states)
                {
                    if (state != value)
                        continue;

                    stateCandidate = state;
                    break;
                }

                if (stateCandidate == null)
                {
                    Debug.LogError($"{name}, Unable to find state: {value}", this);
                    return;
                }

                if (_activeState == stateCandidate)
                    return;

                _lastState = _activeState;
                _activeState = stateCandidate;

                if (!isPlayingOrWillChangePlaymode)
                    return;

                if (!string.IsNullOrEmpty(_saveKey))
                    PlayerPrefs.SetString(_saveKey, _activeState);

                if (!_detectInfiniteLoop.Detect(this))
                {
                    _onValueChanged?.Invoke(this);
                    _onStateChanged?.Invoke(_activeState);
                }
            }
        }

        public string GetActiveState()
        {
            return activeState;
        }

        public void SetActiveState(string stateName)
        {
            activeState = stateName;
        }

        public void Reset()
        {
            activeState = _defaultState;
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