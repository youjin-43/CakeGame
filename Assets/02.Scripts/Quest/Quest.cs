using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public int QuestId;

    public void EraseQuest()
    {
        QuestManager.instance.EraseQuest(QuestId);
    }

    public void CompleteQuest()
    {
        QuestManager.instance.CompleteQuest(QuestId);
    }
}
