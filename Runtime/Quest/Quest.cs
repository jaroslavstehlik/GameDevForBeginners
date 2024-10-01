using System;
using UnityEngine;
using UnityEngine.Events;

public enum QuestState
{
    inactive = 0,
    active = 1,
    complete = 2
}

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Quest", menuName = "GMD/Quest/Quest", order = 1)]
public class Quest : ScriptableObject
{
    // Events for tracking our quest progress
    public UnityEvent<Quest> onQuestActivated;
    public UnityEvent<Quest> onQuestDeactivated;
    public UnityEvent<Quest> onQuestComplete;
    
    // attribute [NonSerialized] prevents saving value inside editor.
    // We need this in order that the state resets when the editor stops playing.
    [NonSerialized] private QuestState _state;

    public QuestState state
    {
        get
        {
            return _state;
        }
        set
        {
            if(_state == value)
                return;

            switch (value)
            {
                case QuestState.active:
                    Debug.Log($"Quest activated: {name}");
                    _state = QuestState.active;
                    if (onQuestActivated != null)
                        onQuestActivated.Invoke(this);
                    break;
                case QuestState.complete:
                    Debug.Log($"Quest completed: {name}");
                    _state = QuestState.complete;
                    if (onQuestComplete != null)
                        onQuestComplete.Invoke(this);
                    break;
                case QuestState.inactive:
                    Debug.Log($"Quest deactivated: {name}");
                    _state = QuestState.inactive;
                    if (onQuestDeactivated != null)
                        onQuestDeactivated.Invoke(this);
                    break;
            }
        }
    }
    
    // The description of our quest to the player
    public string description = string.Empty;
    // The unique identifier to enable quest progress saving
    public string saveKey = string.Empty;

    // Is current quest active?
    public bool IsActive()
    {
        return state == QuestState.active;
    }
    
    // Is current quest completed?
    public bool IsCompleted()
    {
        return state == QuestState.complete;
    }
    
    // Activate the quest
    public void Activate()
    {
        state = QuestState.active;
    }

    // Mark this quest as completed
    public void Complete()
    {
        state = QuestState.complete;
    }

    // Deactivate the quest
    public void Deactivate()
    {
        state = QuestState.inactive;
    }

    // Save the quest to save data
    public void Save()
    {
        PlayerPrefs.SetInt(saveKey, (int)state);
    }

    // Load the quest from save data
    public void Load()
    {
        // Check if save already exists
        if (PlayerPrefs.HasKey(saveKey))
        {
            state = (QuestState)PlayerPrefs.GetInt(saveKey);
        }
    }
}
