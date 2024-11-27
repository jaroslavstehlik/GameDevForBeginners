using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    // TODO: when calculator is not referenced in scene, it is never enabled
    // therefore it does not react to changes
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
            calculatorDescriptor.RegisterVariables(OnCounterChanged);
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            calculatorDescriptor.UnregisterVariables(OnCounterChanged);
        }

        void OnCounterChanged(float value)
        {
            Execute();
        }

        public bool Execute(bool invokeEvents = true)
        {
            if (!calculatorDescriptor.Validate(out string variableName))
            {
                Debug.LogError($"{name}, variable: {variableName} already exists!", this);
                return false;
            }
            
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