using UnityEngine.Events;

namespace GameDevForBeginners
{
    public enum ScriptableValueType
    {
        Unknown,
        Number,
        String,
        Bool
    }
    
    public interface IScriptableValue
    {
        public string name { get; }
        ScriptableValueType GetValueType();
        string GetValue();
        
        UnityEvent<IScriptableValue> onCreate { get; }
        UnityEvent<IScriptableValue> onValueChanged { get; }
        UnityEvent<IScriptableValue> onDestroy { get; }
    }
}