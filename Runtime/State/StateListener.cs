using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class StateListener : MonoBehaviour
{
    [SerializeField] private State state;
    [SerializeField] private string _targetState;
    [SerializeField] private bool _activateOnEnable = true;

    public UnityEvent onStateActivate;
    public UnityEvent onStateDeactivate;

    private string _lastState = null;
    
    private void OnEnable()
    {
        state.onStateChanged.AddListener(OnStateChanged);
        if(_activateOnEnable)
            OnStateChanged(state.GetActiveState());
    }

    private void OnDisable()
    {
        state.onStateChanged.RemoveListener(OnStateChanged);
    }

    void OnStateChanged(string stateName)
    {
        if (_lastState != stateName)
        {
            _lastState = stateName;
            if (stateName != _targetState)
            {
                onStateDeactivate?.Invoke();
            }
            else
            {
                onStateActivate?.Invoke();
            }
        }
    }
}
