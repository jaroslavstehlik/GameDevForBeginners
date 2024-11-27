using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MathParserTK;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterCalculatorDescriptor
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

        [SerializeField] private string _expression;
        private string _parsedString;
        public string parsedString => _parsedString;

        public bool Validate(out string variableName)
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

            variableName = string.Empty;
            return true;
        }

        public CalculatorResult TryParse()
        {
            _parsedString = _expression;
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                _parsedString = _parsedString.Replace(variable.name, variable.count.ToString());
            }

            float result = 0;
            try
            {
                MathParser mathParser = new MathParser();
                result = (float)mathParser.Parse(_parsedString);
            }
            catch (Exception e)
            {
                return new CalculatorResult(CalculatorResultType.Error, 0, e.Message);
            }

            return new CalculatorResult(CalculatorResultType.Value, result, string.Empty);
        }
    }

}
