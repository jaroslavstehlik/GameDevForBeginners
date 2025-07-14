using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    [System.Serializable]
    public class ConditionVariable
    {
        [SerializeField] public string name = string.Empty;
        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour), typeof(Counter), typeof(CounterBehaviour)}, true)]
        public SerializedInterface<IScriptableValue> value = new SerializedInterface<IScriptableValue>{};
    }
    
    [System.Serializable]
    public class ConditionDescriptor
    {
        [SerializeField] private ConditionVariable[] _variables = Array.Empty<ConditionVariable>();
        
        public event Action<string> onValueChanged = null;
        private Dictionary<string, IScriptableValue> _runtimeVariables = new Dictionary<string, IScriptableValue>();
        [SerializeField] private string _condition = string.Empty;
        private string _parsedString = string.Empty;
        public string parsedString => _parsedString;

        private void AddVariablesToRuntimeVariables()
        {
            if(_variables == null)
                return;
            
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

        public ConditionResult TryParse()
        {
            AddVariablesToRuntimeVariables();
            ConditionDescriptorCache conditionDescriptorCache = new ConditionDescriptorCache(_condition, _runtimeVariables);
            conditionDescriptorCache.ReplaceVariablesWithValues(out _parsedString);

            Parser parser = new Parser();
            LogicExpression logicExpression = null;
            try
            {
                logicExpression = parser.Parse(_parsedString);
            }
            catch (Exception e)
            {
                return new ConditionResult(ContitionResultType.Error, e.Message);
            }

            return new ConditionResult(
                logicExpression.GetResult() ? ContitionResultType.True : ContitionResultType.False, string.Empty);
        }

        private void OnValueChangedHandler(IScriptableValue scriptableValue)
        {
            onValueChanged?.Invoke(scriptableValue.GetValue());
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