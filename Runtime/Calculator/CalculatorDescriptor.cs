using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MathParserTK;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public class CalculatorVariable
    {
        [SerializeField] public string name = string.Empty;
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        public SerializedInterface<IScriptableValue> value = new SerializedInterface<IScriptableValue>{};
    }

    [System.Serializable]
    public class CalculatorDescriptor
    {
        [SerializeField] private CalculatorVariable[] _variables;
        
        [HideInInspector] public event Action<float> onValueChanged;
        private Dictionary<string, IScriptableValue> _runtimeVariables;

        [SerializeField] private string _expression;
        private string _parsedString;
        public string parsedString => _parsedString;

        private void AddVariablesToRuntimeVariables()
        {
            foreach (var variable in _variables)
            {
                AddRuntimeVariable(variable.name, variable.value.value);
            }
        }
        
        public bool AddRuntimeVariable(string name, IScriptableValue scriptableValue)
        {
            if (scriptableValue == null)
                return false;
            
            if (_runtimeVariables == null)
                _runtimeVariables = new Dictionary<string, IScriptableValue>();
            
            if (!_runtimeVariables.TryAdd(name, scriptableValue))
                return false;
            
            scriptableValue.onValueChanged.AddListener(OnValueChangedHandler);
            scriptableValue.onDestroy.AddListener(OnDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(string name)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.TryGetValue(name, out IScriptableValue scriptableValue))
                return false;
            
            scriptableValue.onValueChanged.RemoveListener(OnValueChangedHandler);
            scriptableValue.onDestroy.RemoveListener(OnDestroyed);
            
            if (!_runtimeVariables.Remove(name))
                return false;

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
        
        private void OnValueChangedHandler(IScriptableValue scriptableValue)
        {
            if(scriptableValue.GetValueType() != ScriptableValueType.Number)
                return;
            if(!float.TryParse(scriptableValue.GetValue(), out float value))
                return;
            onValueChanged?.Invoke(value);
        }
        
        private void OnDestroyed(IScriptableValue scriptableValue)
        {
            KeyValuePair<string, IScriptableValue>[] keyValuePairs = _runtimeVariables.ToArray();
            foreach (var keyValuePair in keyValuePairs)
            {
                if (keyValuePair.Value != scriptableValue)
                    continue;
                
                _runtimeVariables.Remove(keyValuePair.Key);
            }
        }
    }

}
