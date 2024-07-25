using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public GameObject QuestBoard;

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
