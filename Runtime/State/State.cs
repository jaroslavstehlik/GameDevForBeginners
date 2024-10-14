using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "State", menuName = "GMD/State", order = 1)]
public class State : ScriptableObject
{
    [DrawHiddenFieldsAttribute] [SerializeField] private bool _dummy;

    [ShowInInspectorAttribute(false)]
    private string _activeState = string.Empty;
    
    public string[] states = new []
    {
        "default"
    };
    
    [State]
    [SerializeField] 
    private string _defaultState = "default";
    
    // The key to our counter, it has to be unique per whole game.
    [SerializeField] private string _saveKey = string.Empty;
    public UnityEvent<string> onStateChanged;
    
    private DetectStackOverflow _detectStackOverflow = new DetectStackOverflow();

    private void OnEnable()
    {
        if (isPlayingOrWillChangePlaymode &&
            !string.IsNullOrEmpty(_saveKey) && 
            PlayerPrefs.HasKey(_saveKey))
        {
            // Load the counter in to our variable
            activeState = PlayerPrefs.GetString(_saveKey);
        }
        else
        {
            activeState = _defaultState;   
        }
    }

    private void OnDisable()
    {
        activeState = _defaultState;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        HashSet<string> encounteredStates = new HashSet<string>();
        foreach (var state in states)
        {
            if (!encounteredStates.Add(state))
            {
                Debug.LogError($"{name}, state: {state} already exists!", this);
            }
        }
    }
#endif

    public string activeState
    {
        get => _activeState;
        set
        {
            if(states == null)
                return;

            string stateCandidate = null;
            foreach (var state in states)
            {
                if(state != value)
                    continue;

                stateCandidate = state;
                break;
            }

            if (stateCandidate == null)
            {
                Debug.LogError($"{name}, Unable to find state: {value}", this);
                return;
            }
        
            _activeState = stateCandidate;
        
            if(!isPlayingOrWillChangePlaymode)
                return;
            
            if(!string.IsNullOrEmpty(_saveKey))
                PlayerPrefs.SetString(_saveKey, _activeState);

            Debug.Log($"{name}, SetActiveState: {_activeState}", this);
            if (!_detectStackOverflow.Detect())
                onStateChanged?.Invoke(_activeState);
        }
    } 
    
    public string GetActiveState()
    {
        return activeState;
    }

    public void SetActiveState(string stateName)
    {
        activeState = stateName;
    }
    
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
