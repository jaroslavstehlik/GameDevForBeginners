using UnityEngine;
using UnityEngine.Events;

// We just listen for the quest state changes
// and propagate those changes to our scene
public class QuestListener : MonoBehaviour
{
    public Quest quest;

    public UnityEvent<Quest> onQuestActivated;
    public UnityEvent<Quest> onQuestDeactivated;
    
    public UnityEvent<Quest> onQuestReset;
    public UnityEvent<Quest> onQuestComplete;

    private void OnEnable()
    {
        // Register all events when listener enables
        quest.onQuestActivated.AddListener(OnQuestActivated);
        quest.onQuestDeactivated.AddListener(OnQuestDeactivated);
        quest.onQuestReset.AddListener(OnQuestReset);
        quest.onQuestComplete.AddListener(OnQuestComplete);
        
        // Activate immediately if quest is already active
        if (quest.IsActive())
        {
            OnQuestActivated(quest);
        }
        else
        {
            // Deactive immediately if quest is already inactive
            OnQuestDeactivated(quest);
        }

        // Complete immediately if quest is already completed
        if (quest.IsCompleted())
        {
            OnQuestComplete(quest);
        }
        else
        {
            // Reset immediately if quest is already reset
            OnQuestReset(quest);
        }
    }

    private void OnDisable()
    {
        // Unregister all events when listener disables
        quest.onQuestActivated.RemoveListener(OnQuestActivated);
        quest.onQuestDeactivated.RemoveListener(OnQuestDeactivated);
        quest.onQuestReset.RemoveListener(OnQuestReset);
        quest.onQuestComplete.RemoveListener(OnQuestComplete);
    }

    void OnQuestActivated(Quest quest)
    {
        Debug.Log($"OnQuestActivated: {quest.name}");
        if(onQuestActivated != null)
            onQuestActivated.Invoke(quest);
    }
    
    void OnQuestDeactivated(Quest quest)
    {
        Debug.Log($"OnQuestDeactivated: {quest.name}");
        if(onQuestDeactivated != null)
            onQuestDeactivated.Invoke(quest);
    }
    
    void OnQuestReset(Quest quest)
    {
        if(onQuestReset != null)
            onQuestReset.Invoke(quest);
    }
    
    void OnQuestComplete(Quest quest)
    {
        if(onQuestComplete != null)
            onQuestComplete.Invoke(quest);
    }
}
