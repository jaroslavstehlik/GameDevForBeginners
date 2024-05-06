using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Quest Log", menuName = "GMD/Quest Log", order = 1)]
public class QuestLog : ScriptableObject
{
    public UnityEvent<QuestLog> onQuestLogCompleted;
    
    [SerializeField] Quest[] quests;

    [SerializeField] private int activeQuestIndex = 0;

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
    
    public Quest GetActiveQuest()
    {
        return quests[activeQuestIndex];
    }
    
    public void CompleteActiveQuest()
    {
        quests[activeQuestIndex].Complete();
    }

    public Quest ActivateNextQuest()
    {
        // Deactivate last quest
        quests[activeQuestIndex].Deactivate();
        
        // Check if we are out of quests
        if (activeQuestIndex >= quests.Length)
        {
            if(onQuestLogCompleted != null)
                onQuestLogCompleted.Invoke(this);
            // return nothing when we run out of quests
            return null;
        }

        // increment quest index
        activeQuestIndex++;
        
        // Activate current quest
        quests[activeQuestIndex].Activate();
        
        // return current active quest
        return quests[activeQuestIndex];
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

        // Reset active quest index
        activeQuestIndex = 0;
    }
}