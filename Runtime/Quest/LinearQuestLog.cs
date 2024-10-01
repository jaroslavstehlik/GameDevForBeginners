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