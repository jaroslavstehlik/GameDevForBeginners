using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Condition/ConditionListener")]
    public class ConditionListener : MonoBehaviour
    {
        [SerializeField] private Condition condition;
        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent onTrue;
        public UnityEvent onFalse;
        public UnityEvent onError;

        private void OnEnable()
        {
            condition.onTrue.AddListener(OnTrue);
            condition.onFalse.AddListener(OnFalse);
            condition.onError.AddListener(OnError);
            if (_executeOnEnable)
                condition.Execute();
        }

        private void OnDisable()
        {
            condition.onTrue.RemoveListener(OnTrue);
            condition.onFalse.RemoveListener(OnFalse);
            condition.onError.RemoveListener(OnError);
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