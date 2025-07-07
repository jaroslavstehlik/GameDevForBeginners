using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Calculator/CalculatorBehaviour")]
    public class CalculatorBehaviour : MonoBehaviour, ICalculable
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private CalculatorDescriptor calculatorDescriptor;

        [ShowInInspectorAttribute(false)] private string _parsedResult = String.Empty;

        [ShowInInspectorAttribute(false)] private string _conditionResult = String.Empty;
        
        [SerializeField] private bool _executeOnValueChanged = true;

        [HideInInspector] [SerializeField] private UnityEvent<float> _onResultChanged = new UnityEvent<float>();
        public UnityEvent<float> OnResultChanged => _onResultChanged;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onCreate = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onValueChanged = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onDestroy = new UnityEvent<IScriptableValue>(); 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        public ScriptableValueType GetValueType()
        {
            return ScriptableValueType.Number;
        }
        
        public string GetValue()
        {
            return Execute().ToString();
        }

        private void Awake()
        {
            _onCreate?.Invoke(this);
        }

        private void OnEnable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            calculatorDescriptor.onValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            calculatorDescriptor.onValueChanged -= OnValueChanged;
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke(this);
        }

        void OnValueChanged(float value)
        {
            if(_executeOnValueChanged)
                Execute();
        }

        public void AddRuntimeVariable(IScriptableValue variable)
        {
            calculatorDescriptor.AddRuntimeVariable(variable);
        }

        public void RemoveRuntimeVariable(IScriptableValue variable)
        {
            calculatorDescriptor.RemoveRuntimeVariable(variable);
        }

        public float Execute()
        {
            CalculatorResult calculatorResult = Execute(true);
            switch (calculatorResult.resultType)
            {
                case CalculatorResultType.Error:
                    return float.NaN;
                default:
                    return calculatorResult.value;
            }
        }
        
        public CalculatorResult Execute(bool invokeEvents)
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
            return calculatorResult;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if(!isPlayingOrWillChangePlaymode)
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