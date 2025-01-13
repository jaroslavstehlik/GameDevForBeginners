using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [CreateAssetMenu(fileName = "Condition", menuName = "GMD/Condition/Condition", order = 1)]
    public class Condition : ScriptableObject
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private ConditionDescriptor conditionDescriptor;

        [ShowInInspectorAttribute(false)] private string _parsedResult = String.Empty;

        [ShowInInspectorAttribute(false)] private string _conditionResult = String.Empty;

        [FormerlySerializedAs("_executeOnStateChanged")] [SerializeField] private bool _executeOnValueChanged = true;
        
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

        public void Execute()
        {
            Execute(true);
        }
        
        private void Execute(bool invokeEvents)
        {
            if (_detectInfiniteLoop.Detect(this))
                return;

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