using UnityEngine.Events;

namespace GameDevForBeginners
{
    public interface ICondition : IScriptableValue
    {
        public void Execute();
        public void AddRuntimeVariable(string name, IScriptableValue scriptableValue);
        public void RemoveRuntimeVariable(string name);

        public UnityEvent onTrue { get; }
        public UnityEvent onFalse { get; }
        public UnityEvent onError { get; }
    }
}