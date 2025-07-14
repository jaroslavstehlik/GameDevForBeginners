using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Calculator/CalculatorListener")]
    public class CalculatorListener : MonoBehaviour
    {
        [FormerlySerializedAs("calculator")]
        [SerializedInterface(new [] {typeof(Calculator), typeof(CalculatorBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICalculable> _calculator;

        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent<float> onResultChanged;
        private void OnEnable()
        {
            if (_calculator.value != null)
            {
                _calculator.value.OnResultChanged.AddListener(OnResultChanged);
                if (_executeOnEnable)
                    _calculator.value.Execute();
            }
        }

        private void OnDisable()
        {
            if (_calculator.value != null)
                _calculator.value.OnResultChanged.RemoveListener(OnResultChanged);
        }

        void OnResultChanged(float value)
        {
            onResultChanged?.Invoke(value);
        }
    }
}