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
        private Dictionary<string, State> _variables;

        public StateConditionDescriptorCache(string condition, Dictionary<string, State> variables)
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
                if (_variables.TryGetValue(variable, out State state) && state != null)
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
        [HideInInspector] public UnityEvent<string> onStateConditionChanged;
        private Dictionary<string, State> _runtimeVariables;
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
        
        public bool AddRuntimeVariable(State state)
        {
            if (_runtimeVariables == null)
                _runtimeVariables = new Dictionary<string, State>();
            
            if (!_runtimeVariables.TryAdd(state.name, state))
                return false;
            
            state.onStateChanged.AddListener(OnStateChanged);
            state.onDestroy.AddListener(OnStateDestroyed);
            return true;
        }

        public bool RemoveRuntimeVariable(State state)
        {
            if (_runtimeVariables == null)
                return false;

            if (!_runtimeVariables.Remove(state.name))
                return false;
            
            state.onStateChanged.RemoveListener(OnStateChanged);
            state.onDestroy.RemoveListener(OnStateDestroyed);
            return true;
        }
        
        public ConditionResult TryParse()
        {
            AddVariablesToRuntimeVariables();
            StateConditionDescriptorCache stateConditionDescriptorCache = new StateConditionDescriptorCache(_condition, _runtimeVariables);
            stateConditionDescriptorCache.ReplaceVariablesWithValues(out _parsedString);
            
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
        
        private void OnStateChanged(string value)
        {
            onStateConditionChanged?.Invoke(value);
        }

        private void OnStateDestroyed(State state)
        {
            RemoveRuntimeVariable(state);
        }
    }
}