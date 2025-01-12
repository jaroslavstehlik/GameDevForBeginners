using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/State/StateListener")]
    public class StateListener : MonoBehaviour
    {
        enum StateActive
        {
            NotInitialized = -1,
            False = 0,
            True = 1
        }

        [FormerlySerializedAs("state")] [SerializeField] private State _state;
        [StateAttribute] [SerializeField] private string _targetState;
        [SerializeField] private bool _activateOnEnable = true;

        public UnityEvent onStateActivate;
        public UnityEvent onStateDeactivate;
        public UnityEvent<string> onStateChanged;

        private StateActive _isActive = StateActive.NotInitialized;

        private void OnEnable()
        {
            if(_state == null)
                return;

            _state.onStateChanged?.AddListener(OnStateChanged);
            _state.onDestroy?.AddListener(OnStateDestroyed);
            if (_activateOnEnable)
                OnStateChanged(_state.activeState);
        }

        private void OnDisable()
        {
            if(_state == null)
                return;

            _state.onStateChanged?.RemoveListener(OnStateChanged);
            _state.onDestroy?.RemoveListener(OnStateDestroyed);
        }

        public State state
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != null)
                {
                    _state.onStateChanged?.RemoveListener(OnStateChanged);
                    _state.onDestroy?.RemoveListener(OnStateDestroyed);
                }

                _state = value;

                if (isActiveAndEnabled && _state != null)
                {
                    _state.onStateChanged?.AddListener(OnStateChanged);
                    _state.onDestroy?.AddListener(OnStateDestroyed);
                    OnStateChanged(_state.activeState);
                }
            }
        }

        void OnStateChanged(string stateName)
        {
            SetActive(_targetState == stateName);
            onStateChanged?.Invoke(stateName);
        }

        void SetActive(bool active)
        {
            StateActive activeCandidate = active ? StateActive.True : StateActive.False;
            if (_isActive != StateActive.NotInitialized && _isActive == activeCandidate)
                return;

            _isActive = activeCandidate;
            if (!active)
            {
                onStateDeactivate?.Invoke();
                return;
            }

            onStateActivate?.Invoke();
        }
        private void OnStateDestroyed(ScriptableValue scriptableValue)
        {
            State destroyedState = scriptableValue as State;
            if(destroyedState == null)
                return;
            
            destroyedState.onStateChanged?.RemoveListener(OnStateChanged);
            destroyedState.onDestroy?.RemoveListener(OnStateDestroyed);

            if (_state == destroyedState)
            {
                _state = null;
            }
        }
    }
}