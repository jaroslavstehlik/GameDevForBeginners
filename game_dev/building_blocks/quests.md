# Quests
Many RPG games have quests or objectives. Most modern games even if they are casual and don't represent them selves as RPG games have some sort of objectives.

The Elder Scrolls V: Skyrim quests
<img src="../../img/skyrim_quests.png" alt="skyrim_quests" height="400"/>

A quest is basically a task and a quest log is just a Todo list.
Quest needs a unique name so it can be easily understood by the player but also remembered by the game it self that it has been already completed. 

## Quest

```csharp
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
```

This quest lives inside the project, therefore is unique per project.
In order to react to changes to the quest we need a listener which lives in the scene.

## Quest Listener

```csharp
using UnityEngine;
using UnityEngine.Events;

// We just listen for the quest state changes
// and propagate those changes to our scene
public class QuestListener : MonoBehaviour
{
    public Quest quest;

    public UnityEvent<Quest> onQuestActivated;
    public UnityEvent<Quest> onQuestComplete;
    public UnityEvent<Quest> onQuestDeactivated;

    private void OnEnable()
    {
        // Register all events when listener enables
        quest.onQuestActivated.AddListener(OnQuestActivated);
        quest.onQuestComplete.AddListener(OnQuestComplete);
        quest.onQuestDeactivated.AddListener(OnQuestDeactivated);

        switch (quest.state)
        {
            case QuestState.active:
                OnQuestActivated(quest);
                break;
            case QuestState.complete:
                OnQuestComplete(quest);
                break;
            case QuestState.inactive:
                OnQuestDeactivated(quest);
                break;
        }
    }

    private void OnDisable()
    {
        // Unregister all events when listener disables
        quest.onQuestActivated.RemoveListener(OnQuestActivated);
        quest.onQuestComplete.RemoveListener(OnQuestComplete);
        quest.onQuestDeactivated.RemoveListener(OnQuestDeactivated);
    }

    void OnQuestActivated(Quest quest)
    {
        if(onQuestActivated != null)
            onQuestActivated.Invoke(quest);
    }
    
    void OnQuestComplete(Quest quest)
    {
        if(onQuestComplete != null)
            onQuestComplete.Invoke(quest);
    }
    
    void OnQuestDeactivated(Quest quest)
    {
        if(onQuestDeactivated != null)
            onQuestDeactivated.Invoke(quest);
    }
}
```

## Linear quests

First quest must finish in order so we can advance to the next quest.
The benefit of linear quests is that there is a clear progression, the player is not overwhelmed with decisions on which quest to start.  
This is usually the main storyline.

## Linear Quest Log

```csharp
using System;
using UnityEngine;
using UnityEngine.Events;

public enum LinearQuestLogState
{
    inactive = 0,
    active = 1,
    complete = 2
}

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Quest Log", menuName = "GMD/Quest/Quest Log", order = 1)]
public class LinearQuestLog : ScriptableObject
{
    public UnityEvent<LinearQuestLog> onQuestLogActivated;
    public UnityEvent<LinearQuestLog> onQuestLogCompleted;
    public UnityEvent<LinearQuestLog> onQuestLogDeactivated;
    
    // when a quest in the queue is completed the next quest will be activated
    public bool activateNextQuestOnComplete = true;
    
    [SerializeField] private Quest[] quests;

    [NonSerialized] private int activeQuestIndex = 0;

    [NonSerialized] private LinearQuestLogState _state;

    public LinearQuestLogState state
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
                case LinearQuestLogState.active:
                    Debug.Log($"Quest Log activated: {name}");
                    _state = LinearQuestLogState.active;
                    activeQuestIndex = 0;
                    SetAllQuests(QuestState.inactive);
                    GetActiveQuest().Activate();
                    if (onQuestLogActivated != null)
                        onQuestLogActivated.Invoke(this);
                    break;
                case LinearQuestLogState.complete:
                    Debug.Log($"Quest Log completed: {name}");
                    _state = LinearQuestLogState.complete;
                    SetAllQuests(QuestState.inactive);
                    if (onQuestLogCompleted != null)
                        onQuestLogCompleted.Invoke(this);
                    break;
                case LinearQuestLogState.inactive:
                    Debug.Log($"Quest Log deactivated: {name}");
                    _state = LinearQuestLogState.inactive;
                    SetAllQuests(QuestState.inactive);
                    if (onQuestLogDeactivated != null)
                        onQuestLogDeactivated.Invoke(this);
                    break;
            }
        }
    }
    
    private void OnEnable()
    {
        // start listening for quest activations and completions
        foreach (var quest in quests)
        {
            quest.onQuestComplete.AddListener(OnQuestCompleted);
        }
    }

    private void OnDisable()
    {
        foreach (var quest in quests)
        {
            quest.onQuestComplete.RemoveListener(OnQuestCompleted);
        }
    }

    private void OnQuestCompleted(Quest quest)
    {
        if (activateNextQuestOnComplete)
            ActivateNextQuest();
    }

    public Quest[] GetAllQuests()
    {
        return quests;
    }
    
    public int GetActiveQuestIndex()
    {
        return activeQuestIndex;
    }

    public float GetQuestLogProgress()
    {
        return Mathf.Clamp(activeQuestIndex / (float)quests.Length, 0f, 1f);
    }

    public bool isActive
    {
        get
        {
            return state == LinearQuestLogState.active;
        }
    }
    
    public bool isCompleted
    {
        get
        {
            return state == LinearQuestLogState.complete;
        }
    }
    
    public bool isDeactivated
    {
        get
        {
            return state == LinearQuestLogState.inactive;
        }
    }

    // Activate the quest
    public void Activate()
    {
        state = LinearQuestLogState.active;
    }

    // Mark this quest as completed
    public void Complete()
    {
        state = LinearQuestLogState.complete;
    }

    // Deactivate the quest
    public void Deactivate()
    {
        state = LinearQuestLogState.inactive;
    }

    bool isActiveQuestValid
    {
        get { return activeQuestIndex >= 0 && activeQuestIndex < quests.Length && quests[activeQuestIndex] != null; }
    }

    public Quest GetActiveQuest()
    {
        // Focused quest index is out of range
        if (!isActiveQuestValid)
        {
            Debug.LogError($"Active quest index is invalid! {activeQuestIndex}");
            return null;
        }

        return quests[activeQuestIndex];
    }
    
    public bool CompleteActiveQuest()
    {
        if (!isActiveQuestValid)
        {
            Debug.LogError($"Active quest index is invalid! {activeQuestIndex}");
            return false;
        }
        
        if (!GetActiveQuest().IsActive())
        {
            Debug.LogError($"Current quest: {GetActiveQuest().name} is not active, unable to complete!");
            return false;
        }

        GetActiveQuest().Complete();
        return true;
    }

    public Quest ActivateNextQuest()
    {
        // the linear quest log is inactive, activate it first
        if (state != LinearQuestLogState.active)
        {
            Activate();
            // return current active quest
            return GetActiveQuest();
        }

        // Deactivate last quest
        GetActiveQuest().Deactivate();

        // increment quest index
        activeQuestIndex++;

        // Did we finished our quest log?
        if (activeQuestIndex >= quests.Length)
        {
            Complete();
            return null;
        }

        // Activate quest
        GetActiveQuest().Activate();
        
        // return current active quest
        return GetActiveQuest();
    }

    void SetAllQuests(QuestState state)
    {
        // Iterate over each quest
        foreach (Quest quest in quests)
        {
            // Deactivate quest
            quest.state = state;
        }
    }
}
```

The linear quest log lives again in our project.
In order so we can react to changes to the quest log in our scene we
need to create a listener.

## Linear Quest Log Listener

```csharp
using UnityEngine;
using UnityEngine.Events;

// We just listen for the quest state changes
// and propagate those changes to our scene
public class LinearQuestLogListener : MonoBehaviour
{
    public LinearQuestLog linearQuestLog;
    public UnityEvent<LinearQuestLog> onQuestLogActivated;
    public UnityEvent<LinearQuestLog> onQuestLogCompleted;
    public UnityEvent<LinearQuestLog> onQuestLogDeactivated;
    
    private void OnEnable()
    {
        linearQuestLog.onQuestLogActivated.AddListener(OnQuestLogActivated);
        linearQuestLog.onQuestLogCompleted.AddListener(OnQuestLogCompleted);
        linearQuestLog.onQuestLogDeactivated.AddListener(OnQuestLogDeactivated);

        switch (linearQuestLog.state)
        {
            case LinearQuestLogState.active:
                OnQuestLogActivated(linearQuestLog);
                break;
            case LinearQuestLogState.complete:
                OnQuestLogCompleted(linearQuestLog);
                break;
            case LinearQuestLogState.inactive:
                OnQuestLogDeactivated(linearQuestLog);
                break;
        }
    }
    
    private void OnDisable()
    {
        linearQuestLog.onQuestLogActivated.RemoveListener(OnQuestLogActivated);
        linearQuestLog.onQuestLogCompleted.RemoveListener(OnQuestLogCompleted);
        linearQuestLog.onQuestLogDeactivated.RemoveListener(OnQuestLogDeactivated);
    }

    void OnQuestLogActivated(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogActivated != null)
            onQuestLogActivated.Invoke(linearQuestLog);
    }

    void OnQuestLogCompleted(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogCompleted != null)
            onQuestLogCompleted.Invoke(linearQuestLog);
    }
    
    void OnQuestLogDeactivated(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogDeactivated != null)
            onQuestLogDeactivated.Invoke(linearQuestLog);
    }
}
```

## Parallel quests

When navigating large scale world, we can encounter many NPCs which can give us small quests, we don't have to start or finish those tasks immediately. The benefit is that the player decides on which tasks to take and in which order, but it can also put pressure on the player to decide which task to start or that they have unfinished quests which they don't want to finish. This is usually kept for side missions which are not important for the main storyline.