using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    public enum ContitionResultType
    {
        False,
        True,
        Error
    }

    public struct ConditionResult
    {
        public ContitionResultType resultType;
        public string errorMessage;

        public ConditionResult(ContitionResultType resultType, string errorMessage)
        {
            this.resultType = resultType;
            this.errorMessage = errorMessage;
        }
    }

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
                variable.onCountChanged.AddListener(onCounterChanged);
            }
        }

        public void UnregisterVariables(UnityAction<float> onCounterChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onCountChanged.RemoveListener(onCounterChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onCountChanged.RemoveAllListeners();
            }
        }

        [SerializeField] private string _condition;
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

        public ConditionResult TryParse()
        {
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

    [CreateAssetMenu(fileName = "Counter Condition", menuName = "GMD/Counter/Condition", order = 1)]
    public class CounterCondition : ScriptableObject
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private CounterConditionDescriptor conditionDescriptor;

        [ShowInInspectorAttribute(false)] private string _parsedResult = String.Empty;

        [ShowInInspectorAttribute(false)] private string _conditionResult = String.Empty;

        [HideInInspector] public UnityEvent onTrue;
        [HideInInspector] public UnityEvent onFalse;
        [HideInInspector] public UnityEvent onError;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        private void OnEnable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            conditionDescriptor.RegisterVariables(OnCounterChanged);
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            conditionDescriptor.UnregisterVariables(OnCounterChanged);
        }

        void OnCounterChanged(float value)
        {
            Execute();
        }

        public bool Execute()
        {
            if (_detectInfiniteLoop.Detect(this))
                return false;

            ConditionResult conditionResult = conditionDescriptor.TryParse();
            switch (conditionResult.resultType)
            {
                case ContitionResultType.True:
                    onTrue?.Invoke();
                    break;
                case ContitionResultType.False:
                    onFalse?.Invoke();
                    break;
                case ContitionResultType.Error:
                    onError?.Invoke();
                    break;
            }

            return conditionResult.resultType == ContitionResultType.True;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!conditionDescriptor.Validate(out string variableName))
            {
                Debug.LogError($"{name}, variable: {variableName} already exists!", this);
            }

            ConditionResult conditionResult = conditionDescriptor.TryParse();
            _parsedResult = conditionDescriptor.parsedString;
            _conditionResult = $"{conditionResult.resultType.ToString()} {conditionResult.errorMessage}";
        }
#endif

        public static bool isPlayingOrWillChangePlaymode
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
            }
        }
    }
}