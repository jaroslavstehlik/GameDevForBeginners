using UnityEngine;
using UnityEngine.Events;

// We just listen for the quest state changes
// and propagate those changes to our scene
public class QuestLogListener : MonoBehaviour
{
    public QuestLog questLog;
    public UnityEvent<QuestLog> onQuestLogCompleted;
    public UnityEvent<QuestLog> onQuestLogReset;
    
    private void OnEnable()
    {
        questLog.onQuestLogCompleted.AddListener(OnQuestLogCompleted);
        questLog.onQuestLogReset.AddListener(OnQuestLogReset);
        if (questLog.isCompleted)
            OnQuestLogCompleted(questLog);
    }
    
    private void OnDisable()
    {
        questLog.onQuestLogCompleted.RemoveListener(OnQuestLogCompleted);
        questLog.onQuestLogReset.RemoveListener(OnQuestLogReset);
    }

    void OnQuestLogCompleted(QuestLog questLog)
    {
        if(onQuestLogCompleted != null)
            onQuestLogCompleted.Invoke(questLog);
    }
    
    void OnQuestLogReset(QuestLog questLog)
    {
        if(onQuestLogReset != null)
            onQuestLogReset.Invoke(questLog);
    }
}
