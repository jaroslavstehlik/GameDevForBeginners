using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MathParserTK;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CalculatorDescriptor
    {
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IScriptableValue>[] _variables;
        
        [HideInInspector] public UnityEvent<float> onValueChanged;
        private Dictionary<string, IScriptableValue> _runtimeVariables;

        [SerializeField] private string _expression;
        private string _parsedString;
        public string parsedString => _parsedString;

        private void AddVariablesToRuntimeVariables()
        {
            foreach (var variable in _variables)
            {
                AddRuntimeVariable(variable.value);
            }
        }
        
        public bool AddRuntimeVariable(IScriptableValue scriptableValue)
        {
            if (scriptableValue == null)
                return false;
            
            if (_runtimeVariables == null)
                _runtimeVariables = new Dictionary<string, IScriptableValue>();
            
            if (!_runtimeVariables.TryAdd(scriptableValue.name, scriptableValue))
                return false;
            
            scriptableValue.onValueChanged.AddListener(OnValueChanged);
            scriptableValue.onDestroy.AddListener(OnDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(IScriptableValue scriptableValue)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.Remove(scriptableValue.name))
                return false;
            
            scriptableValue.onValueChanged.RemoveListener(OnValueChanged);
            scriptableValue.onDestroy.RemoveListener(OnDestroyed);
            return true;
        }

        public CalculatorResult TryParse()
        {
            AddVariablesToRuntimeVariables();
            _parsedString = _expression;
            
            foreach (var variable in _runtimeVariables)
            {
                if (variable.Value == null)
                    continue;
                _parsedString = _parsedString.Replace(variable.Key, variable.Value.GetValue());
            }

            float result = 0;
            try
            {
                MathParser mathParser = new MathParser();
                result = (float)mathParser.Parse(_parsedString);
            }
            catch (Exception e)
            {
                return new CalculatorResult(CalculatorResultType.Error, 0, e.Message);
            }

            return new CalculatorResult(CalculatorResultType.Value, result, string.Empty);
        }
        
        private void OnValueChanged(IScriptableValue scriptableValue)
        {
            if(scriptableValue.GetValueType() != ScriptableValueType.Number)
                return;
            if(!float.TryParse(scriptableValue.GetValue(), out float value))
                return;
            onValueChanged?.Invoke(value);
        }
        
        private void OnDestroyed(IScriptableValue scriptableValue)
        {
            RemoveRuntimeVariable(scriptableValue);
        }
    }

}
