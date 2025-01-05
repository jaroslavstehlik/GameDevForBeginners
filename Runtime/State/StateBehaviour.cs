using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/State/StateBehaviour")]
    public class StateBehaviour : MonoBehaviour, IState
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private StateDescriptor _stateDescriptor = new StateDescriptor()
        {
            name = string.Empty,
            defaultState = "default",
            states = new string[]{"default"},
            saveKey = string.Empty
        };

        [SerializeField] private UnityEvent<State> _onStateCreated;
        public UnityEvent<State> onStateCreated => _onStateCreated;
        
        [SerializeField] private UnityEvent<string> _onStateChanged;
        public UnityEvent<string> onStateChanged => _onStateChanged;

        public string[] states
        {
            get
            {
                if (_state == null)
                    return Array.Empty<string>();

                return _state.states;
            }
        }

        public string defaultState
        {
            get
            {
                if (_state == null)
                    return string.Empty;

                return _state.defaultState;
            }
        }

        public string lastState
        {
            get
            {
                if (_state == null)
                    return string.Empty;

                return _state.lastState;
            }
        }

        private State _state;
        public State state => _state;

        private State GetOrCreateState()
        {
            if (_state == null)
            {
                _state = State.CreateState(_stateDescriptor);
                _onStateCreated?.Invoke(_state);
            }

            return _state;
        }

        private void Awake()
        {
            _ = GetOrCreateState();
        }

        private void OnDestroy()
        {
            Destroy(_state);
            _state = null;
        }

        public string activeState
        {
            get
            {
                if (_state == null)
                    return string.Empty;

                return _state.activeState;
            }
            set
            {
                if (_state == null)
                    return;

                _state.activeState = value;
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
            if(_state == null)
                return;
            
            activeState = _state.defaultState;
        }
    }
}