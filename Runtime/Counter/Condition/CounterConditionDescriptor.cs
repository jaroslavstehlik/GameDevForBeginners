using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterConditionDescriptor
    {
        [SerializeField] private Counter[] _variables;

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
            if (_variables == null)
            {
                return new ConditionResult(ContitionResultType.Error, "Variables is null!");
            }
            
            _parsedString = _condition;
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                _parsedString = _parsedString.Replace(variable.name, variable.count.ToString());
            }

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