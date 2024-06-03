using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    public QuestLog questLog;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private RectTransform questItemContainer;

    private void OnEnable()
    {
        questLog.onQuestActivated.AddListener(OnQuestActivated);
        questLog.onQuestCompleted.AddListener(OnQuestCompleted);
        OnQuestActivated(questLog.GetActiveQuest());
    }

    private void OnDisable()
    {
        questLog.onQuestActivated.RemoveListener(OnQuestActivated);
        questLog.onQuestCompleted.RemoveListener(OnQuestCompleted);
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
        questLog.ActivateNextQuest();
    }
}
