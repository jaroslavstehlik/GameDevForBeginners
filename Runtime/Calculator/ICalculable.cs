using GameDevForBeginners;
using UnityEngine.Events;

public interface ICalculable : IScriptableValue
{
    public void AddRuntimeVariable(IScriptableValue variable);
    public void RemoveRuntimeVariable(IScriptableValue variable);
    public float Execute();
    public UnityEvent<float> OnResultChanged { get; }
}
