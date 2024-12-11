using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

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
        private Dictionary<string, Counter> _cachedVariables;

        public CounterConditionDescriptorCache(string condition, Counter[] variables)
        {
            _cachedCondition = new List<string>();
            _cachedVariables = new Dictionary<string, Counter>();
            foreach (var variable in variables)
            {
                _cachedVariables.TryAdd(variable.name, variable);
            }
            
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
            foreach (var variable in _cachedCondition)
            {
                if (_cachedVariables.TryGetValue(variable, out Counter counter))
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
        private CounterConditionDescriptorCache _counterConditionDescriptorCache;
        
        public void RegisterVariables(UnityAction<float> onCounterChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onCountChanged?.AddListener(onCounterChanged);
            }
        }

        public void UnregisterVariables(UnityAction<float> onCounterChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onCountChanged?.RemoveListener(onCounterChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onCountChanged?.RemoveAllListeners();
            }
        }

        [SerializeField] private string _condition;
        private string _parsedString;
        public string parsedString => _parsedString;

        public bool Validate(out string variableName)
        {
            if (_variables != null)
            {
                HashSet<string> encounteredVariables = new HashSet<string>();
                foreach (var variable in _variables)
                {
                    if (variable == null)
                        continue;

                    if (!encounteredVariables.Add(variable.name))
                    {
                        variableName = variable.name;
                        return false;
                    }
                }
            }

            variableName = string.Empty;
            return true;
        }

        public ConditionResult TryParse()
        {
            _counterConditionDescriptorCache = new CounterConditionDescriptorCache(_condition, _variables);
            _counterConditionDescriptorCache.ReplaceVariablesWithValues(out _parsedString);

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
    }

}