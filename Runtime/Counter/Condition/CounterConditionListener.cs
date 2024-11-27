using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/Condition/CounterConditionListener")]
    public class CounterConditionListener : MonoBehaviour
    {
        [SerializeField] private CounterCondition counterCondition;
        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent onTrue;
        public UnityEvent onFalse;
        public UnityEvent onError;

        private void OnEnable()
        {
            counterCondition.onTrue.AddListener(OnTrue);
            counterCondition.onFalse.AddListener(OnFalse);
            counterCondition.onError.AddListener(OnError);
            if (_executeOnEnable)
                counterCondition.Execute();
        }

        private void OnDisable()
        {
            counterCondition.onTrue.RemoveListener(OnTrue);
            counterCondition.onFalse.RemoveListener(OnFalse);
            counterCondition.onError.RemoveListener(OnError);
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