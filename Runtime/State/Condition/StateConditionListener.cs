using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    public class StateConditionListener : MonoBehaviour
    {
        [SerializeField] private StateCondition stateCondition;
        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent onTrue;
        public UnityEvent onFalse;
        public UnityEvent onError;

        private void OnEnable()
        {
            stateCondition.onTrue.AddListener(OnTrue);
            stateCondition.onFalse.AddListener(OnFalse);
            stateCondition.onError.AddListener(OnError);
            if (_executeOnEnable)
                stateCondition.Execute();
        }

        private void OnDisable()
        {
            stateCondition.onTrue.RemoveListener(OnTrue);
            stateCondition.onFalse.RemoveListener(OnFalse);
            stateCondition.onError.RemoveListener(OnError);
        }

        void OnTrue()
        {
            onTrue?.Invoke();
        }

        void OnFalse()
        {
            onFalse?.Invoke();
        }

        void OnError()
        {
            onError?.Invoke();
        }
    }
}