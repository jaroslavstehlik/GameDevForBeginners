using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/State/StateBehaviourListener")]
    public class StateBehaviourListener : MonoBehaviour
    {
        enum StateActive
        {
            NotInitialized = -1,
            False = 0,
            True = 1
        }

        [SerializeField] private StateBehaviour _stateBehaviour;
        [StateAttribute] [SerializeField] private string _targetState;
        [SerializeField] private bool _activateOnEnable = true;

        public UnityEvent onStateActivate;
        public UnityEvent onStateDeactivate;
        public UnityEvent<string> onStateChanged;

        private StateActive _isActive = StateActive.NotInitialized;

        private void OnEnable()
        {
            _stateBehaviour.onStateChanged?.AddListener(OnStateChanged);
            if (_activateOnEnable)
                OnStateChanged(_stateBehaviour.activeState);
        }

        private void OnDisable()
        {
            _stateBehaviour.onStateChanged?.RemoveListener(OnStateChanged);
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
    }
}