using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct StateConditionBehaviourDescriptor
    {
        [SerializeField] private StateBehaviour[] _sceneVariables;
        [SerializeField] private State[] _projectVariables;

        List<IState> GetAllVariables()
        {
            List<IState> variables = new List<IState>();
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

        public void RegisterVariables(UnityAction<string> onStateChanged)
        {
            List<IState> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                variable.onStateChanged?.AddListener(onStateChanged);
            }
        }

        public void UnregisterVariables(UnityAction<string> onStateChanged)
        {
            List<IState> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                variable.onStateChanged?.RemoveListener(onStateChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            List<IState> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                variable.onStateChanged?.RemoveAllListeners();
            }
        }

        [SerializeField] private string _condition;
        private string _parsedString;
        public string parsedString => _parsedString;

        public bool Validate(out string variableName)
        {
            HashSet<string> encounteredVariables = new HashSet<string>();
            List<IState> variables = GetAllVariables();
            foreach (var variable in variables)
            {
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
            _parsedString = _condition;
            List<IState> variables = GetAllVariables();
            foreach (var variable in variables)
            {
                _parsedString = _parsedString.Replace(variable.name, variable.activeState);
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