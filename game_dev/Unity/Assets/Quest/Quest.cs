using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Quest", menuName = "GMD/Quest", order = 1)]
public class Quest : ScriptableObject
{
    public UnityEvent<Quest> onQuestActivated;
    public UnityEvent<Quest> onQuestDeactivated;
    
    public UnityEvent<Quest> onQuestReset;
    public UnityEvent<Quest> onQuestComplete;
    
    // Each quest needs to identify if it is currently active
    [SerializeField] private bool active = false;
    // Each quest needs to identify if it has been completed
    [SerializeField] bool completed = false;
    // The description of our quest to the player
    public string description = string.Empty;
    // The unique identifier to enable quest progress saving
    public string saveKey = string.Empty;

    // Is current quest active?
    public bool IsActive()
    {
        return active;
    }
    
    // Is current quest completed?
    public bool IsCompleted()
    {
        return completed;
    }
    
    // Mark this quest as completed
    public void Complete()
    {
        completed = true;
        if (onQuestComplete != null)
            onQuestComplete.Invoke(this);
    }
    
    // Reset the state of the quest
    public void Reset()
    {
        completed = false;
        if (onQuestReset != null)
            onQuestReset.Invoke(this);
    }

    // Activate the quest
    public void Activate()
    {
        if (!active)
        {
            active = true;
            if (onQuestActivated != null)
                onQuestActivated.Invoke(this);
        }
    }

    // Deactivate the quest
    public void Deactivate()
    {
        if (active)
        {
            active = false;
            if (onQuestDeactivated != null)
                onQuestDeactivated.Invoke(this);
        }
    }

    // Save the quest to save data
    public void Save()
    {
        PlayerPrefs.SetInt(saveKey, completed ? 0 : 1);
    }

    // Load the quest from save data
    public void Load()
    {
        // Check if save already exists
        if (PlayerPrefs.HasKey(saveKey))
        {
            // Read the save
            if (PlayerPrefs.GetInt(saveKey) == 1)
            {
                Complete();
            }
            else
            {
                Reset();
            }
        }
    }
}
