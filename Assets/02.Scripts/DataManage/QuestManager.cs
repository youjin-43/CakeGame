using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //DataTable 사용을 위해 추가
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public DataTable QuestDT;

    public GameObject QuestBoard;
    public GameObject QuestUIprefab;

    //public QuestManager()
    //{
    //    Debug.Log("generate QuestManager");
    //    QuestDT = GameManager.instance.dataManager.tableDic[DataManager.CSVDatas.QuestTable];
    //}

    //void Start()
    //{
    //    QuestDT = GameManager.instance.dataManager.tableDic[DataManager.CSVDatas.QuestTable]; //얘가 왜 
    //    Debug.Log(QuestDT.Rows.Count + "퀘스트 매니저에서 성공적으로 데이터 테이블을 할당받음!");
    //}
    //여긴 왤케 순서가 꼬이는지 아직도 이해얀됨; 


     public void GenQuest()
    {
        //Debug.Log(QuestDT.Rows.Count);
        //Debug.Log(QuestDT.Rows[10][0]);
        //Debug.Log(QuestDT.Rows[10][1]);

        int randNum = Random.Range(0, QuestDT.Rows.Count);
        Debug.Log("NewQuestNum : "+randNum);

        int questID = int.Parse(QuestDT.Rows[randNum][0].ToString());
        int questCategory= int.Parse(QuestDT.Rows[randNum][1].ToString());
        string explaneText= QuestDT.Rows[randNum][2].ToString();
        int deadline= int.Parse(QuestDT.Rows[randNum][3].ToString());
        int clearValue= int.Parse(QuestDT.Rows[randNum][4].ToString());
        int reward1= int.Parse(QuestDT.Rows[randNum][5].ToString());
        int reward1Amount= int.Parse(QuestDT.Rows[randNum][6].ToString());

        Quest newQuest = new Quest(questID, questCategory, explaneText, deadline, clearValue, reward1, reward1Amount);

        //퀘스트창에 퀘스트 추가 
        GameObject temp = Instantiate(QuestUIprefab);
        temp.transform.SetParent(QuestBoard.transform);

        SetQuestUItext(temp, newQuest);
    }

    public void SetQuestUItext(GameObject questUI, Quest quest)
    {
        Debug.Log(questUI.transform.GetChild(0).name);
        questUI.transform.Find("Title").GetComponent<Text>().text = quest.ExplaneText;
        //다른건 나중에 ..

    }
}