using System;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IState> _state;
        
        [StateAttribute(nameof(_state))] [SerializeField] private Option _targetOption;
        [SerializeField] private bool _activateOnEnable = true;

        public UnityEvent onStateActivate;
        public UnityEvent onStateDeactivate;
        public UnityEvent<Option> onStateChanged;

        private StateActive _isActive = StateActive.NotInitialized;

        private void OnEnable()
        {
            if(_state.value == null)
                return;

            _state.value.onStateChanged?.AddListener(OnStateChanged);
            _state.value.onDestroy?.AddListener(OnStateDestroyed);
            if (_activateOnEnable)
                OnStateChanged(_state.value.activeOption);
        }

        private void OnDisable()
        {
            if(_state.value == null)
                return;

            _state.value.onStateChanged?.RemoveListener(OnStateChanged);
            _state.value.onDestroy?.RemoveListener(OnStateDestroyed);
        }

        public IState state
        {
            get
            {
                return _state.value;
            }
            set
            {
                if (_state.value != null)
                {
                    _state.value.onStateChanged?.RemoveListener(OnStateChanged);
                    _state.value.onDestroy?.RemoveListener(OnStateDestroyed);
                }

                _state.value = value;

                if (isActiveAndEnabled && _state.value != null)
                {
                    _state.value.onStateChanged?.AddListener(OnStateChanged);
                    _state.value.onDestroy?.AddListener(OnStateDestroyed);
                    OnStateChanged(_state.value.activeOption);
                }
            }
        }

        void OnStateChanged(Option option)
        {
            SetActive(_targetOption == option);
            onStateChanged?.Invoke(option);
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
        private void OnStateDestroyed(IScriptableValue scriptableValue)
        {
            IState destroyedState = scriptableValue as IState;
            if(destroyedState == null)
                return;
            
            destroyedState.onStateChanged?.RemoveListener(OnStateChanged);
            destroyedState.onDestroy?.RemoveListener(OnStateDestroyed);

            if (_state.value == destroyedState)
            {
                _state.value = null;
            }
        }
    }
}