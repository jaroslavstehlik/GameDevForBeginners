using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestLogController : MonoBehaviour
{
    public QuestLog questLog;

    public Quest ActivateNextQuest()
    {
        return questLog.ActivateNextQuest();
    }

    public Quest GetActiveQuest()
    {
        return questLog.GetActiveQuest();
    }

    public float GetQuestLogProgress()
    {
        return questLog.GetQuestLogProgress();
    }

    public void CompleteActiveQuest()
    {
        questLog.CompleteActiveQuest();
    }
    
    public void Reset()
    {
        questLog.Reset();
    }
}
