using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //DataTable 사용을 위해 추가
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //싱글턴으로
    public static QuestManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("QuestManager가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 QuestManagerr가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("QuestManager를 죽입니다");
        }
    }


    public DataTable QuestDT;

    public GameObject QuestBoard; // 정확히는 새로운 퀘스트 UI 프리팹이 들어갈 content
    public GameObject QuestUIprefab;

    private void Start()
    {
        QuestDT = DataManager.instance.tableDic[DataManager.CSVDatas.QuestTable];
        Debug.Log("QuestDT.Rows.Count :" + QuestDT.Rows.Count + " - 퀘스트 매니저에서 성공적으로 데이터 테이블을 할당받음!");
    }


    public void GenQuest()
    {

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

        Debug.Log("퀘스트 객체 생성 완료 questID : " + questID + ", explaneText : " + explaneText);

        //퀘스트창에 퀘스트 추가 
        GameObject temp = Instantiate(QuestUIprefab);
        temp.transform.SetParent(QuestBoard.transform);

        SetQuestUItext(temp, newQuest);
    }

    public void SetQuestUItext(GameObject questUI, Quest quest)
    {
        //Debug.Log(questUI.transform.GetChild(0).name);
        questUI.transform.Find("Title").GetComponent<Text>().text = quest.ExplaneText;
        //다른건 나중에 ..

    }
}