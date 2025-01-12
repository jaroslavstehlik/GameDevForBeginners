using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [CreateAssetMenu(fileName = "Counter Calculator", menuName = "GMD/Counter/Calculator", order = 1)]
    public class CounterCalculator : ScriptableObject
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private CounterCalculatorDescriptor calculatorDescriptor;

        [ShowInInspectorAttribute(false)] private string _parsedResult = String.Empty;

        [ShowInInspectorAttribute(false)] private string _conditionResult = String.Empty;
        
        [HideInInspector] [SerializeField] private UnityEvent<float> _onResultChanged;
        public UnityEvent<float> OnResultChanged => _onResultChanged;

        private void OnEnable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            calculatorDescriptor.onCalculatorValueChanged?.AddListener(OnCounterChanged);
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            calculatorDescriptor.onCalculatorValueChanged?.RemoveListener(OnCounterChanged);
        }

        void OnCounterChanged(float value)
        {
            Execute();
        }

        public void AddRuntimeVariable(Counter counter)
        {
            calculatorDescriptor.AddRuntimeVariable(counter);
        }

        public void RemoveRuntimeVariable(Counter counter)
        {
            calculatorDescriptor.RemoveRuntimeVariable(counter);
        }

        public bool Execute(bool invokeEvents = true)
        {
            CalculatorResult calculatorResult = calculatorDescriptor.TryParse();
            if (invokeEvents)
            {
                switch (calculatorResult.resultType)
                {
                    case CalculatorResultType.Value:
                        _onResultChanged?.Invoke(calculatorResult.value);
                        break;
                    case CalculatorResultType.Error:
                        break;
                }
            }
#if UNITY_EDITOR
            _parsedResult = calculatorDescriptor.parsedString;
            if (calculatorResult.resultType == CalculatorResultType.Value)
            {
                _conditionResult = $"{calculatorResult.value.ToString()}";
            }
            else
            {
                _conditionResult = $"{calculatorResult.resultType.ToString()} {calculatorResult.errorMessage}";
            }
#endif
            
            return calculatorResult.resultType == CalculatorResultType.Value;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            Execute(false);
        }
#endif
        
        public static bool isPlayingOrWillChangePlaymode
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
            }
        }
    }
}