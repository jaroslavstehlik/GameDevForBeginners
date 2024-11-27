using UnityEngine;
using UnityEngine.Events;

public interface IState 
{
    public string name { get; }
    public string[] states { get; }
    public string defaultState { get; }
    public UnityEvent<string> onStateChanged { get; }
    public string lastState { get; }
    public string activeState { get; set; }
    public string GetActiveState();
    public void SetActiveState(string stateName);
    public void Reset();
}