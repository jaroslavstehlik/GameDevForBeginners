using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// We just listen for the quest state changes
// and propagate those changes to our scene
public class LinearQuestLogListener : MonoBehaviour
{
    [FormerlySerializedAs("questLog")] public LinearQuestLog linearQuestLog;
    public UnityEvent<LinearQuestLog> onQuestLogStarted;
    public UnityEvent<LinearQuestLog> onQuestLogCompleted;
    public UnityEvent<LinearQuestLog> onQuestLogReset;
    
    private void OnEnable()
    {
        linearQuestLog.onQuestLogStarted.AddListener(OnQuestLogStarted);
        linearQuestLog.onQuestLogCompleted.AddListener(OnQuestLogCompleted);
        linearQuestLog.onQuestLogReset.AddListener(OnQuestLogReset);
        if (linearQuestLog.isCompleted)
            OnQuestLogCompleted(linearQuestLog);
    }
    
    private void OnDisable()
    {
        linearQuestLog.onQuestLogStarted.RemoveListener(OnQuestLogStarted);
        linearQuestLog.onQuestLogCompleted.RemoveListener(OnQuestLogCompleted);
        linearQuestLog.onQuestLogReset.RemoveListener(OnQuestLogReset);
    }

    void OnQuestLogStarted(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogStarted != null)
            onQuestLogStarted.Invoke(linearQuestLog);
    }

    void OnQuestLogCompleted(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogCompleted != null)
            onQuestLogCompleted.Invoke(linearQuestLog);
    }
    
    void OnQuestLogReset(LinearQuestLog linearQuestLog)
    {
        if(onQuestLogReset != null)
            onQuestLogReset.Invoke(linearQuestLog);
    }
}
