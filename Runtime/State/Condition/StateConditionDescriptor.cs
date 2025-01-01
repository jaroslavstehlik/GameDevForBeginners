using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    /*
    todo: find strings
    "this is a string"
    todo: find variables
    variable
    todo: find operators
    && || ! == !=
    */
    
    struct StateConditionDescriptorCache
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
        private Dictionary<string, State> _cachedVariables;

        public StateConditionDescriptorCache(string condition, State[] variables)
        {
            _cachedCondition = new List<string>();
            _cachedVariables = new Dictionary<string, State>();
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
                if (_cachedVariables.TryGetValue(variable, out State state))
                {
                    replacedString += state.activeState;
                }
                else
                {
                    replacedString += variable;
                }
            }
        }
    }
    
    // TODO: support comparing state names
    [System.Serializable]
    public struct StateConditionDescriptor
    {
        [SerializeField] private State[] _variables;
        private StateConditionDescriptorCache _stateConditionDescriptorCache;
        
        public void RegisterVariables(UnityAction<string> onStateChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged?.AddListener(onStateChanged);
            }
        }

        public void UnregisterVariables(UnityAction<string> onStateChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged?.RemoveListener(onStateChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged?.RemoveAllListeners();
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
            _stateConditionDescriptorCache = new StateConditionDescriptorCache(_condition, _variables);
            _stateConditionDescriptorCache.ReplaceVariablesWithValues(out _parsedString);
            
            Parser parser = new Parser();
            LogicExpression logicExpression = null;
            try
            {
                ExpressionContext expressionContext = new ExpressionContext();
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