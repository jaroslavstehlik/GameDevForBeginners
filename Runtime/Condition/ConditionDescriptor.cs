using System;
using System.Collections.Generic;
using UnityEngine;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    [System.Serializable]
    public class ConditionDescriptor
    {
        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour), typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IScriptableValue>[] _variables = new SerializedInterface<IScriptableValue>[]{};
        
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
            
            scriptableValue.onValueChanged.AddListener(OnValueChangedHandler);
            scriptableValue.onDestroy.AddListener(OnDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(IScriptableValue scriptableValue)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.Remove(scriptableValue.name))
                return false;
            
            scriptableValue.onValueChanged.RemoveListener(OnValueChangedHandler);
            scriptableValue.onDestroy.RemoveListener(OnDestroyed);
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
            RemoveRuntimeVariable(scriptableValue);
        }
    }
}