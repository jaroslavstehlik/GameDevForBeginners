using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [CreateAssetMenu(fileName = "Counter Condition", menuName = "GMD/Counter/Condition", order = 1)]
    public class CounterCondition : ScriptableObject
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private CounterConditionDescriptor conditionDescriptor;

        [ShowInInspectorAttribute(false)] private string _parsedResult = String.Empty;

        [ShowInInspectorAttribute(false)] private string _conditionResult = String.Empty;

        [SerializeField] private bool _executeOnValueChanged = true;
        
        [HideInInspector] public UnityEvent onTrue;
        [HideInInspector] public UnityEvent onFalse;
        [HideInInspector] public UnityEvent onError;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        private void OnEnable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            
            conditionDescriptor.onConditionValueChanged?.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            
            conditionDescriptor.onConditionValueChanged?.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            if(_executeOnValueChanged)
                Execute();
        }

        public void AddRuntimeVariable(ScriptableValue scriptableValue)
        {
            conditionDescriptor.AddRuntimeVariable(scriptableValue);
        }

        public void RemoveRuntimeVariable(ScriptableValue scriptableValue)
        {
            conditionDescriptor.RemoveRuntimeVariable(scriptableValue);
        }

        public bool Execute(bool invokeEvents = true)
        {
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