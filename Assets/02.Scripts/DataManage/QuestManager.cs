using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //DataTable 사용을 위해 추가

public class QuestManager 
{
    public DataTable QuestDT;

    //public QuestManager()
    //{
    //    Debug.Log("generate QuestManager");
    //    QuestDT = GameManager.instance.dataManager.tableDic[DataManager.CSVDatas.QuestTable];
    //}

    public Quest GenQuest()
    {
        //Debug.Log(QuestDT.Rows.Count);
        //Debug.Log(QuestDT.Rows[10][0]);
        //Debug.Log(QuestDT.Rows[10][1]);

        int randNum = Random.Range(0, QuestDT.Rows.Count);
        Debug.Log("randNum : "+randNum);

        //Debug.Log(QuestDT.Rows[randNum][0].GetType().Name);

        int questID = int.Parse(QuestDT.Rows[randNum][0].ToString());
        int questCategory= int.Parse(QuestDT.Rows[randNum][1].ToString());
        string explaneText= QuestDT.Rows[randNum][2].ToString();
        int deadline= int.Parse(QuestDT.Rows[randNum][3].ToString());
        int clearValue= int.Parse(QuestDT.Rows[randNum][4].ToString());
        int reward1= int.Parse(QuestDT.Rows[randNum][5].ToString());
        int reward1Amount= int.Parse(QuestDT.Rows[randNum][6].ToString());

        Quest newQuest = new Quest(questID, questCategory, explaneText, deadline, clearValue, reward1, reward1Amount);
        //Quest newQuest = new Quest(1, 1, explaneText, 1, 1, 1, 100);
        return newQuest;

    }
}