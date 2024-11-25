using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;

namespace GameDevForBeginners
{
    public enum StateResultType
    {
        False,
        True,
        Error
    }

    public struct StateResult
    {
        public StateResultType resultType;
        public string errorMessage;

        public StateResult(StateResultType resultType, string errorMessage)
        {
            this.resultType = resultType;
            this.errorMessage = errorMessage;
        }
    }

    [System.Serializable]
    public struct StateConditionDescriptor
    {
        [SerializeField] private State[] _variables;

        public void RegisterVariables(UnityAction<string> onStateChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged.AddListener(onStateChanged);
            }
        }

        public void UnregisterVariables(UnityAction<string> onStateChanged)
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged.RemoveListener(onStateChanged);
            }
        }

        public void UnregisterAllVariables()
        {
            foreach (var variable in _variables)
            {
                if (variable == null)
                    continue;
                variable.onStateChanged.RemoveAllListeners();
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

    [CreateAssetMenu(fileName = "State Condition", menuName = "GMD/State/Condition", order = 1)]
    public class StateCondition : ScriptableObject
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private StateConditionDescriptor conditionDescriptor;

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
            conditionDescriptor.RegisterVariables(OnStateChanged);
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            conditionDescriptor.UnregisterVariables(OnStateChanged);
        }

        void OnStateChanged(string value)
        {
            Execute();
        }

        public bool Execute(bool invokeEvents = true)
        {
            if (!conditionDescriptor.Validate(out string variableName))
            {
                Debug.LogError($"{name}, variable: {variableName} already exists!", this);
                return false;
            }

            if (_detectInfiniteLoop.Detect(this))
                return false;

            ConditionResult conditionResult = conditionDescriptor.TryParse();
            if (invokeEvents)
            {
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
            }
#if UNITY_EDITOR
            _parsedResult = conditionDescriptor.parsedString;
            _conditionResult = $"{conditionResult.resultType.ToString()} {conditionResult.errorMessage}";
#endif
            return conditionResult.resultType == ContitionResultType.True;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            Execute(false);
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