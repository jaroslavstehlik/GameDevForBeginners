using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
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