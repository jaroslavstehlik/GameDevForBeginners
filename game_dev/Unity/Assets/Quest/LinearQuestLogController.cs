using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Make public methods available in the scene
// for controlling our Quest Log.
public class LinearQuestLogController : MonoBehaviour
{
    // reference to our quest log project asset
    public LinearQuestLog linearQuestLog;

    public void ActivateNextQuest()
    {
        linearQuestLog.ActivateNextQuest();
    }

    public Quest GetActiveQuest()
    {
        return linearQuestLog.GetActiveQuest();
    }

    public float GetQuestLogProgress()
    {
        return linearQuestLog.GetQuestLogProgress();
    }

    public void CompleteActiveQuest()
    {
        linearQuestLog.CompleteFocusedQuest();
    }
    
    public void Reset()
    {
        linearQuestLog.Reset();
    }
}
