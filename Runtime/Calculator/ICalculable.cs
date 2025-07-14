using GameDevForBeginners;
using UnityEngine.Events;

public interface ICalculable : IScriptableValue
{
    public void AddRuntimeVariable(string name, IScriptableValue variable);
    public void RemoveRuntimeVariable(string name);
    public float Execute();
    public UnityEvent<float> OnResultChanged { get; }
}
