using UnityEngine.Events;

namespace GameDevForBeginners
{
    public interface ICondition : IScriptableValue
    {
        public void Execute();
        public void AddRuntimeVariable(IScriptableValue scriptableValue);
        public void RemoveRuntimeVariable(IScriptableValue scriptableValue);

        public UnityEvent onTrue { get; }
        public UnityEvent onFalse { get; }
        public UnityEvent onError { get; }
    }
}