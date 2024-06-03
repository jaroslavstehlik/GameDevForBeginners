using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private Quest quest;
    [SerializeField] private TextMeshProUGUI label;

    public Quest Quest
    {
        get
        {
            return quest;
        }
        set
        {
            quest = value;
            label.text = value.description;
        }
    }

    public void CompleteQuest()
    {
        quest.Complete();
    }
}