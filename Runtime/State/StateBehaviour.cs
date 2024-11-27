using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/State/StateBehaviour")]
    public class StateBehaviour : MonoBehaviour, IState
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
        
        [SerializeField] private UnityEvent<string> _onStateChanged;
        public UnityEvent<string> onStateChanged => _onStateChanged;

        private string _lastState;
        public string lastState => _lastState;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        private void Awake()
        {
            activeState = _defaultState;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (states != null)
            {
                HashSet<string> encounteredStates = new HashSet<string>();
                foreach (var state in states)
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
                if (states == null)
                    return;

                string stateCandidate = null;
                foreach (var state in states)
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

                if (!_detectInfiniteLoop.Detect(this))
                {
                    onStateChanged?.Invoke(_activeState);
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