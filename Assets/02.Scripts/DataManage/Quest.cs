using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{

    public int QuestID { get; set; }
    public int QuestCategory { get; set; }
    public string ExplaneText { get; set; }
    public int Deadline { get; set; }
    public int ClearValue { get; set; }
    public int Reward1 { get; set; }
    public int Reward1Amount { get; set; }

    public Quest(int questID, int questCategory, string explaneText, int deadline, int clearValue, int reward1, int reward1Amount)
    {
        QuestID = questID;
        QuestCategory = questCategory;
        ExplaneText = explaneText;
        Deadline = deadline;
        ClearValue = clearValue;
        Reward1 = reward1;
        Reward1Amount = reward1Amount;
    }
}
