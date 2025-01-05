using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MathParserTK;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterCalculatorDescriptor
    {
        [SerializeField] private Counter[] _variables;
        [HideInInspector] public UnityEvent<float> onCounterCalculatorChanged;
        private Dictionary<string, Counter> _runtimeVariables;

        [SerializeField] private string _expression;
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

        public CalculatorResult TryParse()
        {
            AddVariablesToRuntimeVariables();
            _parsedString = _expression;
            
            foreach (var variable in _runtimeVariables)
            {
                if (variable.Value == null)
                    continue;
                _parsedString = _parsedString.Replace(variable.Key, variable.Value.count.ToString());
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
        
        private void OnCounterChanged(float value)
        {
            onCounterCalculatorChanged?.Invoke(value);
        }
        
        private void OnCounterDestroyed(Counter counter)
        {
            RemoveRuntimeVariable(counter);
        }
    }

}
