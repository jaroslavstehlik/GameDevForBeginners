using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestListUI : MonoBehaviour
{
    [FormerlySerializedAs("questLog")] public LinearQuestLog linearQuestLog;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private RectTransform questItemContainer;

    private void OnEnable()
    {
        OnQuestActivated(linearQuestLog.GetActiveQuest());
    }

    private void OnDisable()
    {
    }

    private void ClearItems()
    {
        // do while child count is above zero
        while (questItemContainer.childCount > 0)
        {
            Transform child = questItemContainer.GetChild(0);
            // remove child from container first
            child.SetParent(null);
            // destroy child from scene
            Destroy(child.gameObject);
        }
    }
    
    private void OnQuestActivated(Quest quest)
    {
        QuestItemUI questItemUI = Instantiate(questItemPrefab, questItemContainer).GetComponent<QuestItemUI>();
        questItemUI.Quest = quest;
    }
    
    private void OnQuestCompleted(Quest quest)
    {
        ClearItems();
        linearQuestLog.ActivateNextQuest();
    }
}
