using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterConditionBehaviourDescriptor
    {
        [SerializeField] private CounterBehaviour[] _sceneVariables;
        [SerializeField] private Counter[] _projectVariables;

        List<ICountable> GetAllVariables()
        {
            List<ICountable> variables = new List<ICountable>();
            if (_sceneVariables != null)
            {
                foreach (var sceneVariable in _sceneVariables)
                {
                    if (sceneVariable == null)
                        continue;
                    variables.Add(sceneVariable);
                }
            }

            if (_projectVariables != null)
            {
                foreach (var projectVariable in _projectVariables)
                {
                    if (projectVariable == null)
                        continue;
                    variables.Add(projectVariable);
                }
            }

            return variables;
        }
        
        public void RegisterVariables(UnityAction<float> onCounterChanged)
        {
            List<ICountable> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                variable.onCountChanged?.AddListener(onCounterChanged);
            }
        }

        public void UnregisterVariables(UnityAction<float> onCounterChanged)
        {
            List<ICountable> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                variable.onCountChanged?.RemoveListener(onCounterChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            List<ICountable> variables = GetAllVariables();
            foreach (var variable in variables)
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
            HashSet<string> encounteredVariables = new HashSet<string>();
            List<ICountable> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                if (variable == null)
                    continue;

                if (!encounteredVariables.Add(variable.name))
                {
                    variableName = variable.name;
                    return false;
                }
            }
            
            variableName = string.Empty;
            return true;
        }

        public ConditionResult TryParse()
        {
            List<ICountable> variables = GetAllVariables();
            _parsedString = _condition;
            foreach (var variable in variables)
            {
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