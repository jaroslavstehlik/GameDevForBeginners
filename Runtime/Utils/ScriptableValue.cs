using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    public enum ScriptableValueType
    {
        Unknown,
        Counter,
        State
    }
    
    public abstract class ScriptableValue : ScriptableObject
    {
        public abstract ScriptableValueType GetValueType();
        public abstract string GetValue();
        
        [HideInInspector] [SerializeField] protected UnityEvent<ScriptableValue> _onValueChanged;
        public virtual UnityEvent<ScriptableValue> onValueChanged => _onValueChanged;
        
        [HideInInspector] [SerializeField] protected UnityEvent<ScriptableValue> _onDestroy;
        public virtual UnityEvent<ScriptableValue> onDestroy => _onDestroy;
        
        private void OnDestroy()
        {
            _onDestroy?.Invoke(this);
        }
    }
}