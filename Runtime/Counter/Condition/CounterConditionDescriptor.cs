using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    struct CounterConditionDescriptorCache
    {
        private static char[] operatorList = new char[]
        {
            '|',
            '^',
            '&',
            '=',
            '!',
            '>',
            '<',
            '(',
            ')',
            ' '
        };

        int GetOperatorIndex(char letter)
        {
            for (int i = 0; i < operatorList.Length; i++)
            {
                if(operatorList[i] != letter)
                    continue;
                return i;
            }

            return -1;
        }

        private List<string> _cachedCondition;
        private Dictionary<string, Counter> _variables;

        public CounterConditionDescriptorCache(string condition, Dictionary<string, Counter> variables)
        {
            _cachedCondition = new List<string>();
            _variables = variables;
            
            string buffer = string.Empty;
            string variableBuffer = string.Empty;
            for (int i = 0; i < condition.Length; i++)
            {
                if(GetOperatorIndex(condition[i]) == -1)
                {
                    variableBuffer += condition[i];
                    if (buffer != string.Empty)
                    {
                        _cachedCondition.Add(buffer);
                        buffer = string.Empty;
                    }
                }
                else
                {
                    buffer += condition[i];
                    if (variableBuffer != string.Empty)
                    {
                        _cachedCondition.Add(variableBuffer);
                        variableBuffer = string.Empty;
                    }
                }
            }
            
            if (buffer != string.Empty)
                _cachedCondition.Add(buffer);

            if (variableBuffer != string.Empty)
                _cachedCondition.Add(variableBuffer);
        }
        
        public void ReplaceVariablesWithValues(out string replacedString)
        {
            replacedString = string.Empty;
            if(_variables == null)
                return;
            
            foreach (var variable in _cachedCondition)
            {
                if (_variables.TryGetValue(variable, out Counter counter) && counter != null)
                {
                    replacedString += counter.count.ToString();
                }
                else
                {
                    replacedString += variable;
                }
            }
        }
    }
    
    [System.Serializable]
    public struct CounterConditionDescriptor
    {
        [SerializeField] private Counter[] _variables;
        [HideInInspector] public UnityEvent<float> onCounterConditionChanged;
        private Dictionary<string, Counter> _runtimeVariables;
        
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
        
        public bool AddRuntimeVariable(Counter counter)
        {
            if (_runtimeVariables == null)
                _runtimeVariables = new Dictionary<string, Counter>();
            
            if (!_runtimeVariables.TryAdd(counter.name, counter))
                return false;
            
            counter.onCountChanged.AddListener(OnCounterChanged);
            counter.onDestroy.AddListener(OnCounterDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(Counter counter)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.Remove(counter.name))
                return false;
            
            counter.onCountChanged.RemoveListener(OnCounterChanged);
            counter.onDestroy.RemoveListener(OnCounterDestroyed);
            return true;
        }

        public ConditionResult TryParse()
        {
            AddVariablesToRuntimeVariables();
            CounterConditionDescriptorCache counterConditionDescriptorCache = new CounterConditionDescriptorCache(_condition, _runtimeVariables);
            counterConditionDescriptorCache.ReplaceVariablesWithValues(out _parsedString);

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

        private void OnCounterChanged(float value)
        {
            onCounterConditionChanged?.Invoke(value);
        }

        private void OnCounterDestroyed(Counter counter)
        {
            RemoveRuntimeVariable(counter);
        }
    }
}