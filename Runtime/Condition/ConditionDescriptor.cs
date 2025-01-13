using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct ConditionDescriptor
    {
        [SerializeField] private ScriptableValue[] _variables;
        [HideInInspector] public UnityEvent<string> onConditionValueChanged;
        private Dictionary<string, ScriptableValue> _runtimeVariables;
        [SerializeField] private string _condition;
        private string _parsedString;
        public string parsedString => _parsedString;

        private void AddVariablesToRuntimeVariables()
        {
            foreach (var variable in _variables)
            {
                AddRuntimeVariable(variable);
            }
        }
        
        public bool AddRuntimeVariable(ScriptableValue scriptableValue)
        {
            if (_runtimeVariables == null)
                _runtimeVariables = new Dictionary<string, ScriptableValue>();
            
            if (!_runtimeVariables.TryAdd(scriptableValue.name, scriptableValue))
                return false;
            
            scriptableValue.onValueChanged.AddListener(OnValueChanged);
            scriptableValue.onDestroy.AddListener(OnDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(ScriptableValue scriptableValue)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.Remove(scriptableValue.name))
                return false;
            
            scriptableValue.onValueChanged.RemoveListener(OnValueChanged);
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

        private void OnValueChanged(ScriptableValue scriptableValue)
        {
            onConditionValueChanged?.Invoke(scriptableValue.GetValue());
        }

        private void OnDestroyed(ScriptableValue scriptableValue)
        {
            RemoveRuntimeVariable(scriptableValue);
        }
    }
}