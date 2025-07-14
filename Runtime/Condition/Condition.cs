using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [CreateAssetMenu(fileName = "Condition", menuName = "GMD/Condition/Condition", order = 1)]
    public class Condition : ScriptableObject, ICondition
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private ConditionDescriptor conditionDescriptor;
        [ShowInInspectorAttribute(true, false, true, false)] private string _parsedResult = String.Empty;
        [ShowInInspectorAttribute(true, false, true, false)] private string _conditionResult = String.Empty;

        [SerializeField] private bool _executeOnValueChanged = true;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onCreate; 
        public UnityEvent<IScriptableValue> onCreate => _onCreate;

        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onValueChanged; 
        public UnityEvent<IScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector]
        [SerializeField]
        private UnityEvent<IScriptableValue> _onDestroy; 
        public UnityEvent<IScriptableValue> onDestroy => _onDestroy;

        private UnityEvent _onTrue = new UnityEvent();
        [HideInInspector] public UnityEvent onTrue => _onTrue;
        private UnityEvent _onFalse = new UnityEvent();
        [HideInInspector] public UnityEvent onFalse => _onFalse;
        private UnityEvent _onError = new UnityEvent();
        [HideInInspector] public UnityEvent onError => _onError;

        private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();

        public ScriptableValueType GetValueType()
        {
            return ScriptableValueType.Bool;
        }
        
        public string GetValue()
        {
            switch (Execute(false).resultType)
            {
                case ContitionResultType.True:
                    return true.ToString();
                case ContitionResultType.False:
                    return false.ToString();
                    default:
                    return "error";
            }
        }

        private void OnEnable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            
            conditionDescriptor.onValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            if (!isPlayingOrWillChangePlaymode)
                return;
            
            conditionDescriptor.onValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(string value)
        {
            if(_executeOnValueChanged)
                Execute();
        }

        public void AddRuntimeVariable(string name, IScriptableValue scriptableValue)
        {
            conditionDescriptor.AddRuntimeVariable(name, scriptableValue);
        }

        public void RemoveRuntimeVariable(string name)
        {
            conditionDescriptor.RemoveRuntimeVariable(name);
        }

        public void Execute()
        {
            Execute(true);
            _onValueChanged?.Invoke(this);
        }
        
        private ConditionResult Execute(bool invokeEvents)
        {
            if (_detectInfiniteLoop.Detect(this))
                return new ConditionResult(ContitionResultType.Error, "Infinite loop");

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
            return conditionResult;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if(!isPlayingOrWillChangePlaymode)
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