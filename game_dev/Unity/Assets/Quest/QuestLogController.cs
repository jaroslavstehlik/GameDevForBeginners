using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Make public methods available in the scene
// for controlling our Quest Log.
public class QuestLogController : MonoBehaviour
{
    // reference to our quest log project asset
    public QuestLog questLog;

    public void ActivateNextQuest()
    {
        questLog.ActivateNextQuest();
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
        questLog.CompleteFocusedQuest();
    }
    
    public void Reset()
    {
        questLog.Reset();
    }
}
