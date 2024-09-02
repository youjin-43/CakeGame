using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;//씬 관련 라이브러리
using System.IO; // 파일, 폴더 생성을 위해 

public class EventSceneManager : MonoBehaviour
{
    //싱글턴으로
    public static EventSceneManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        if (instance == null)
        {
            instance = this; // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            Debug.Log("EventSceneManager 생성");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록

            //데이터 패스 설정
            SavePath = Application.persistentDataPath + "/Event/";
            saveFilePath = SavePath + "ToShowEvnetList.json";

            LoadEventListFromJson(); // 보여줘야하는 이벤트 씬 로드 
        }
        else
        {
            Debug.LogWarning("씬에 두개 이상의 EventSceneManager 존재합니다!");
            Destroy(gameObject);
            Debug.Log("EventSceneManager 죽입니다");
        }
    }

    [Header("For EventScene")]
    public ToShowEvnetList toShowEvnetList; ////해금해야하는 레시피 인덱스들을 저장해놓음 
    private string SavePath; //저장된 폴더
    string saveFilePath; //dataPath

    [Header("For Dialog")]
    public EventDialogs eventDialogs; 
    private string DialogjsonPath = Application.dataPath + "/02.Scripts/Event/DialogDB.json"; //dataPath

    public GameObject dialogBox;
    public Text dialogText;
    public GameObject BackgroundImage;
    public string[] currntDialog;
    public int idx=0;


    //이벤트 씬을 보여주기위한 환경 세팅 
    public void SettingForEvent()
    {
        LoadEventDialogJson(); //대사 정보 가져오기 

        //대사 출력을 위한 오브젝트 셋팅
        BackgroundImage = GameObject.Find("BackgroundImages");
        dialogBox = GameObject.Find("DialogButton");
        dialogBox.GetComponent<Button>().onClick.AddListener(EventSceneManager.instance.PassDialog);
        //dialogBox.onClick.AddListener(PassDialog);
        dialogText = GameObject.Find("DialogText").GetComponent<Text>();

        Debug.Log(toShowEvnetList.events[0]+"-----------------");
        int i = toShowEvnetList.events[0]; //실행해야하는 이벤트 변수에 할당

        toShowEvnetList.events.RemoveAt(0);//그 이벤트 리스트에서 삭제 
        SaveEventList(); //현재 상태 업데이트하여 json에 저장

        CakeManager.instance.UnlockCake(i); //케이크 레시피  해금
        QuestManager.instance.GenMainQuest(i);//퀘스틑 추가 

        //실행해야하는 이벤트에 따라 다이얼로그 할당 
        if (i == 1)
        {
            currntDialog = eventDialogs.Level2;
            BackgroundImage.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (i == 2)
        {
            currntDialog = eventDialogs.Level4;
            BackgroundImage.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (i == 3)
        {
            currntDialog = eventDialogs.Level6;
            BackgroundImage.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (i == 4)
        {
            currntDialog = eventDialogs.Level8;
            BackgroundImage.transform.GetChild(3).gameObject.SetActive(true);
        }
        else if(i == 5)
        {
            currntDialog = eventDialogs.Level10;
            BackgroundImage.transform.GetChild(4).gameObject.SetActive(true);
        }
        idx = 0;
        dialogText.text = currntDialog[idx];
        
    }

    public void StartEventScene()
    {
        Debug.Log("------StartEventScene() 실행------");
        SceneManager.LoadScene("Event");
    }

    public void PassDialog()
    {
        idx++;

        if (idx >= currntDialog.Length)
        {
            SceneManager.LoadScene("CakeStore 1");
            return;
        }
        dialogText.text = currntDialog[idx];
    }

    public void LoadEventDialogJson()
    {
        string DialogJsonText = File.ReadAllText(DialogjsonPath);
        //Debug.Log("DialogJsonText : " + DialogJsonText);
        eventDialogs = JsonUtility.FromJson<EventDialogs>(DialogJsonText);
        //Debug.Log("이벤트 대사 json에서 불러오기 완료");
    }

    //현재 가지고 있는 이벤트 정보를 json에서 로딩하는 함수 
    public void LoadEventListFromJson()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        //Debug.Log("현재 가지고 있는 퀘스트 데이터 불러오기 완료");
        toShowEvnetList = JsonUtility.FromJson<ToShowEvnetList>(saveFile);
        Debug.Log("toShowEvnetList size : " + toShowEvnetList.events.Count);   
    }

    //현재 실행되어야하는 이벤트 정보를 업데이트하여 새로 json에 저장하는 함수 
    public void SaveEventList()
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath); //폴더가 없으면 폴더 생성 

        string saveJson = JsonUtility.ToJson(toShowEvnetList);
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("EvnetList Save Success: " + saveFilePath);
    }

    [System.Serializable]
    public class ToShowEvnetList
    {
        public List<int> events;
    }

    [System.Serializable]
    public class EventDialogs
    {
        public string[] Level2;
        public string[] Level4;
        public string[] Level6;
        public string[] Level8;
        public string[] Level10;
    }
}

