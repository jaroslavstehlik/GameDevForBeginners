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
