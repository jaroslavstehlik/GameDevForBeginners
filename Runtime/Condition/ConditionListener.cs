using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Condition/ConditionListener")]
    public class ConditionListener : MonoBehaviour
    {
        [SerializedInterface(new [] {typeof(Condition), typeof(ConditionBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICondition> _condition = new SerializedInterface<ICondition>();

        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent onTrue = new UnityEvent();
        public UnityEvent onFalse = new UnityEvent();
        public UnityEvent onError = new UnityEvent();

        private void OnEnable()
        {
            if (_condition.value != null)
            {
                _condition.value.onTrue.AddListener(OnTrue);
                _condition.value.onFalse.AddListener(OnFalse);
                _condition.value.onError.AddListener(OnError);
                if (_executeOnEnable)
                {
                    _condition.value.Execute();
                }
            }
        }

        private void OnDisable()
        {
            if (_condition.value != null)
            {
                _condition.value.onTrue.RemoveListener(OnTrue);
                _condition.value.onFalse.RemoveListener(OnFalse);
                _condition.value.onError.RemoveListener(OnError);
            }
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