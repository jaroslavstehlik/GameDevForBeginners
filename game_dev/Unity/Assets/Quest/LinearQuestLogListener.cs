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
