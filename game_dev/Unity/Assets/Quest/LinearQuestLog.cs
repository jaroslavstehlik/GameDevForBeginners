using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Quest Log", menuName = "GMD/Quest/Quest Log", order = 1)]
public class LinearQuestLog : ScriptableObject
{
    public UnityEvent<LinearQuestLog> onQuestLogStarted;
    public UnityEvent<LinearQuestLog> onQuestLogCompleted;
    public UnityEvent<LinearQuestLog> onQuestLogReset;
    
    public UnityEvent<Quest> onQuestActivated;
    public UnityEvent<Quest> onQuestCompleted;

    // when a quest in the queue is completed the next quest will be activated
    public bool activateNextQuestOnComplete = true;
    
    [SerializeField] private Quest[] quests;

    [NonSerialized] private int focusedQuestIndex = 0;

    private void OnEnable()
    {
        // start listening for quest activations and completions
        foreach (var quest in quests)
        {
            quest.onQuestActivated.AddListener(OnQuestActivated);
            quest.onQuestComplete.AddListener(OnQuestCompleted);
        }
    }

    private void OnDisable()
    {
        foreach (var quest in quests)
        {
            quest.onQuestActivated.RemoveListener(OnQuestActivated);
            quest.onQuestComplete.RemoveListener(OnQuestCompleted);
        }
    }

    private void OnQuestActivated(Quest quest)
    {
        if(onQuestActivated != null)
            onQuestActivated.Invoke(quest);

        if (quest == quests[0])
        {
            if (onQuestLogStarted != null)
                onQuestLogStarted.Invoke(this);
        }
    }

    private void OnQuestCompleted(Quest quest)
    {
        if(onQuestCompleted != null)
            onQuestCompleted.Invoke(quest);

        if (activateNextQuestOnComplete)
            ActivateNextQuest();
    }

    public Quest[] GetAllQuests()
    {
        return quests;
    }
    
    public int GetFocusedQuestIndex()
    {
        return focusedQuestIndex;
    }

    public float GetQuestLogProgress()
    {
        return Mathf.Clamp(focusedQuestIndex / (float)quests.Length, 0f, 1f);
    }

    public bool isCompleted
    {
        get
        {
            return focusedQuestIndex >= quests.Length;
        }
    }

    bool isFocusedQuestValid
    {
        get { return focusedQuestIndex < 0 || focusedQuestIndex >= quests.Length; }
    }

    public Quest GetFocusedQuest()
    {
        // Focused quest index is out of range
        if (isFocusedQuestValid)
            return null;

        return quests[focusedQuestIndex];
    }
    
    public Quest GetActiveQuest()
    {
        Quest focusedQuest = GetFocusedQuest();
        
        // Quest is not active
        if (!focusedQuest.IsActive())
            return null;
        
        return focusedQuest;
    }
    
    public bool CompleteFocusedQuest()
    {
        if (isFocusedQuestValid)
            return false;
        
        quests[focusedQuestIndex].Complete();
        return true;
    }

    public Quest ActivateNextQuest()
    {
        // Deactivate last quest
        quests[focusedQuestIndex].Deactivate();
        
        // increment quest index
        focusedQuestIndex++;

        // Check if we are out of quests
        if (focusedQuestIndex >= quests.Length)
        {
            // Complete log when we are out of quests
            if(onQuestLogCompleted != null)
                onQuestLogCompleted.Invoke(this);
            // return nothing when we run out of quests
            return null;
        }

        // Activate current quest
        quests[focusedQuestIndex].Activate();
        
        // return current active quest
        return quests[focusedQuestIndex];
    }

    // Reset all quests
    public void Reset()
    {
        // Iterate over each quest
        foreach (Quest quest in quests)
        {
            // Deactivate quest
            quest.Deactivate();
            
            // Reset its completion state
            quest.Reset();
        }

        // Reset focused quest index
        focusedQuestIndex = 0;
        
        // activate focused quest
        quests[focusedQuestIndex].Activate();
        
        if(onQuestLogReset != null)
            onQuestLogReset.Invoke(this);
    }

    public void StartFirstQuest()
    {
        if(quests.Length == 0)
            return;
        
        quests[0].Activate();
    }
}