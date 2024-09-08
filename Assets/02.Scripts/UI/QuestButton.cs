using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public GameObject QuestBoard;

    private void Start()
    {
        QuestBoard = GameObject.Find("Quest").transform.GetChild(1).gameObject;
    }

    public void OpenQuestBorad()
    {
        if (QuestBoard.activeSelf == true)
        {
            QuestBoard.SetActive(false);
        }
        else
        {
            QuestBoard.SetActive(true);
        }
    }
}
