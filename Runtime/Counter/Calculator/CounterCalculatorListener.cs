using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/Calculator/CounterCalculatorListener")]
    public class CounterCalculatorListener : MonoBehaviour
    {
        [SerializeField] private CounterCalculator counterCalculator;
        [SerializeField] private bool _executeOnEnable = true;

        public UnityEvent<float> onResultChanged;
        private void OnEnable()
        {
            counterCalculator.OnResultChanged.AddListener(OnResultChanged);
            if (_executeOnEnable)
                counterCalculator.Execute();
        }

        private void OnDisable()
        {
            counterCalculator.OnResultChanged.RemoveListener(OnResultChanged);
        }

        void OnResultChanged(float value)
        {
            onResultChanged?.Invoke(value);
        }
    }
}