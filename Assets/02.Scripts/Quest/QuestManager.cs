using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Data; //DataTable 사용을 위해 추가
using UnityEngine.UI;
using System.IO; // 파일, 폴더 생성을 위해 

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


    //public DataTable QuestDT;

    public GameObject QuestBoard; // 정확히는 새로운 퀘스트 UI 프리팹이 들어갈 content
    public GameObject QuestUIprefab;

    public QuestDB questDB; //생성될 수있는 모든 퀘스트 리스트 
    private string QuestjsonPath = Application.dataPath + "/02.Scripts/Quest/QuestDB.json"; //dataPath


    public HavingQuests havingQuests; // 현재 가지고 있는 퀘스트 
    private string SavePath; //저장된 폴더
    string saveFilePath; //dataPath

    private void Start()
    {
        //QuestDT = DataManager.instance.tableDic[DataManager.CSVDatas.QuestTable];
        //Debug.Log("QuestDT.Rows.Count :" + QuestDT.Rows.Count + " - 퀘스트 매니저에서 성공적으로 데이터 테이블을 할당받음!");

        //데이터 패스 설정
        SavePath = Application.persistentDataPath + "/Quest/";
        saveFilePath = SavePath + "HavingQuests.json";

        LoadQuestdataBase();
        LoadHavingQuestsList();
    }

    //현재 가지고 있는 퀘스트 정보를 업데이트하여 새로 저장하는 함수 
    public void UpdateCurrnetQuestList()
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath); //퀘스트 폴더가 없으면 폴더 생성 

        string saveJson = JsonUtility.ToJson(havingQuests);
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("HavingQuestsData Save Success: " + saveFilePath);
    }

    public void LoadHavingQuestsList()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        Debug.Log("현재 가지고 있는 퀘스트 데이터 불러오기 완료");
        havingQuests = JsonUtility.FromJson<HavingQuests>(saveFile);
    }

    public void LoadQuestdataBase()
    {
        string QuestjsonText = File.ReadAllText(QuestjsonPath);
        Debug.Log("QuestjsonText");
        questDB = JsonUtility.FromJson<QuestDB>(QuestjsonText);
        Debug.Log("퀘스트 데이터 베이스 불러오기 완료");
    }


    public void GenQuest()
    {

        //int randNum = Random.Range(0, QuestDT.Rows.Count);
        int randNum = Random.Range(0, questDB.QuestList.Count);
        Debug.Log("NewQuestNum : " + randNum);

        //int questID = int.Parse(QuestDT.Rows[randNum][0].ToString());
        //int questCategory= int.Parse(QuestDT.Rows[randNum][1].ToString());
        //string explaneText= QuestDT.Rows[randNum][2].ToString();
        //int deadline= int.Parse(QuestDT.Rows[randNum][3].ToString());
        //int clearValue= int.Parse(QuestDT.Rows[randNum][4].ToString());
        //int reward1= int.Parse(QuestDT.Rows[randNum][5].ToString());
        //int reward1Amount= int.Parse(QuestDT.Rows[randNum][6].ToString());

        //Quest newQuest = new Quest(questID, questCategory, explaneText, deadline, clearValue, reward1, reward1Amount);
        //Debug.Log("퀘스트 객체 생성 완료 questID : " + questID + ", explaneText : " + explaneText);

        havingQuests.HavingQuestList.Add(questDB.QuestList[randNum]);
        UpdateCurrnetQuestList();


        //퀘스트창에 퀘스트 추가 
        GameObject temp = Instantiate(QuestUIprefab);
        temp.transform.SetParent(QuestBoard.transform);

        //SetQuestUItext(temp, newQuest);
        temp.transform.Find("Title").GetComponent<Text>().text = questDB.QuestList[randNum].ExplaneText;

    }

    //public void SetQuestUItext(GameObject questUI, Quest quest)
    //{
    //    //Debug.Log(questUI.transform.GetChild(0).name);
    //    questUI.transform.Find("Title").GetComponent<Text>().text = quest.ExplaneText;
    //    //다른건 나중에 ..

    //}

    [System.Serializable]
    public class QuestDB
    {
        public List<QuestData> QuestList;
    }

    [System.Serializable]
    public class HavingQuests
    {
        public List<QuestData> HavingQuestList;
    }

    [System.Serializable]
    public class QuestData
    {
        public int QuestID;
        public int QuestCategory;
        public string ExplaneText;
        public int Deadline;
        public int CurrnetValue1;
        public int ClearValue1;
        public int reward1;
        public int reward1Amount;
    }
}