using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Data; //DataTable 사용을 위해 추가
using UnityEngine.UI;
using System.IO; // 파일, 폴더 생성을 위해 
using Inventory.Model; // 파일, 폴더 생성을 위해

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

            //데이터 패스 설정
            SavePath = Application.persistentDataPath + "/Quest/";
            saveFilePath = SavePath + "HavingQuests.json";

            LoadQuestdataBase(); //전체 퀘스트 정보 로드 
            LoadHavingQuestsList(); // 현재 가지고 있는 퀘스트 정보 로드
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

    public QuestDB questDB; //생성될 수있는 모든 퀘스트 리스트 
    private string QuestjsonPath = Application.dataPath + "/02.Scripts/Quest/QuestDB.json"; //dataPath


    public HavingQuests havingQuests; // 현재 가지고 있는 퀘스트 
    private string SavePath; //저장된 폴더
    string saveFilePath; //dataPath

    [Header("Quest UI")]
    public GameObject QuestBoard;
    public GameObject QuestContent;
    public List<Transform> QuestUIs;

    public GameObject QuestUIprefab;

    private void Update()
    {
        //케이크 임시 추가 코드 
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    CakeManager.instance.cakeMakerController.CompleteCake(1);
        //    //setCurrentQuestUIs();
        //}

        //케이크 하나 없애는 임시코드 
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveCake(1, 1);
            setCurrentQuestUIs();
        }
    }


    //cnt만큼 가지고 있는 케이크의 갯수르 인벤토리에서 없애는 함수 
    void RemoveCake(int idx,int cnt)
    {
        InventoryItem tmpItem = new InventoryItem()
        {
            item = CakeManager.instance.cakeDataList[idx],
            quantity = cnt,
        };

        UIInventoryManager.instance.MinusItem(tmpItem);
    }

    //현재 가지고 있는 퀘스트 정보를 업데이트하여 새로 json에 저장하는 함수 
    public void UpdateCurrnetQuestList()
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath); //퀘스트 폴더가 없으면 폴더 생성 

        string saveJson = JsonUtility.ToJson(havingQuests);
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("HavingQuestsData 갱신됨 : " + saveFilePath);
    }

    //현재 가지고 있는 퀘스트 정보를 json에서 로딩하는 함수 
    public void LoadHavingQuestsList()
    {
        if (!File.Exists(saveFilePath))
        {
            //Debug.LogError("No such saveFile exists");
            Debug.Log("가지고 있는 퀘스트가 없습니다.");
            return;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        //Debug.Log("현재 가지고 있는 퀘스트 데이터 불러오기 완료");
        havingQuests = JsonUtility.FromJson<HavingQuests>(saveFile);
    }

    //생성될 수 있는 모든 퀘스트 정보를 json에서 로딩하는 함수 
    public void LoadQuestdataBase()
    {
        string QuestjsonText = File.ReadAllText(QuestjsonPath);
        //Debug.Log("QuestjsonText : "+QuestjsonText);
        questDB = JsonUtility.FromJson<QuestDB>(QuestjsonText);
        //Debug.Log("퀘스트 데이터 베이스 불러오기 완료");
    }

    //UI를 위한 오브젝트 변수들에 오브젝트들 셋팅 -> 게임매니저의 OnSceneLoaded 함수에서 호출됨 == 씬이 전환될때마다 
    public void setBasicQuestUIs()
    {
        //Debug.Log("setBasicQuestUIs 실행 ");
         
        QuestBoard = GameObject.Find("Quest").transform.GetChild(1).gameObject;
        QuestContent = QuestBoard.transform.GetComponentInChildren<VerticalLayoutGroup>().gameObject;

        QuestUIs = new List<Transform>();
        for(int i = 0; i < QuestContent.transform.childCount; i++)
        {
            QuestUIs.Add(QuestContent.transform.GetChild(i));
            QuestContent.transform.GetChild(i).gameObject.SetActive(false);
        }

        setCurrentQuestUIs();
    }

    //현재 가지고 있는 퀘스트 정보에 맞게 정보 셋팅 및 UI 활성화
    public void setCurrentQuestUIs()
    {
        int i = 0;
        for (; i < havingQuests.HavingQuestList.Count; i++)
        {
            QuestUIs[i].gameObject.SetActive(true); //활성화 시키고 
            QuestUIs[i].GetComponent<Quest>().QuestId = havingQuests.HavingQuestList[i].QuestID;//퀘스트컴포넌트에 가지고있는 퀘스트 할당

            //퀘스트에 따라 UI 이미지랑 텍스트 설정 
            QuestUIs[i].GetChild(0).Find("Title").GetComponent<Text>().text = havingQuests.HavingQuestList[i].ExplaneText; //퀘스트 제목 
            QuestUIs[i].GetChild(0).Find("MoneyText").GetComponent<Text>().text = havingQuests.HavingQuestList[i].MoneyAmount.ToString();//얻을수 있는돈 
            QuestUIs[i].GetChild(0).Find("PopularityText").GetComponent<Text>().text = havingQuests.HavingQuestList[i].ExpAmount.ToString();//얻을수 있는 명성

            //만들어야하는 케이크 인덱스 
            int cakeIdx = havingQuests.HavingQuestList[i].cakeToMakeIdx;

            //제작해야하는 케이크 이미지셋팅
            Sprite img = (Sprite)Resources.Load($"CakeImage/{cakeIdx}");
            QuestUIs[i].Find("CakeImage").GetComponent<Image>().sprite = img;

            //현재 가지고있는 양 / 퀘스트 완료를 위해 필요한 양 텍스트 셋팅 
            int toMakeCnt = havingQuests.HavingQuestList[i].ClearValue1; //만들어야하는 양
            int haveCnt= CakeManager.instance.cakeDataList[cakeIdx].cakeCount;  //현재 인벤토리에 가지고 있는 양
            QuestUIs[i].Find("needCnt").GetChild(0).GetComponent<Text>().text = haveCnt+"/"+toMakeCnt; //텍스트 설정 

            //퀘스트 완료 조건 확인
            if (toMakeCnt <= haveCnt)
            {
                //퀘스트 완료 조건이 충족된다면 
                QuestUIs[i].Find("completeButton").gameObject.SetActive(true); //완료 버튼 활성화
                //퀘스트 완료
            }
            else
            {
                //조건이 충족되지 않는다면 
                QuestUIs[i].Find("completeButton").gameObject.SetActive(false); //완료 버튼 비활성화
            }
        }

        for (; i < QuestUIs.Count; i++)
        {
            QuestUIs[i].gameObject.SetActive(false); //나머지는 비활성화 
        }
    }

    //퀘스트 생성 -> UI에 반영되지는 않음
    public void GenMainQuest(int idx)
    {
        //퀘스트 여분 자리가 있을때만 실행 
        if (havingQuests.HavingQuestList.Count >= QuestUIs.Count)
        {
            Debug.Log("퀘스트 여분 자리가 없습니다");
            return;
        }

        havingQuests.HavingQuestList.Add(questDB.QuestList[idx-1]); //현재 가지고 있는 퀘스트에 추가
        UpdateCurrnetQuestList(); // 현재 퀘스트 정보 저장
        //setCurrentQuestUIs();//UI 적용
    }

    //퀘스트 완료 함수
    public void CompleteQuest(int questId)
    {
        //퀘스트
        for (int i = havingQuests.HavingQuestList.Count - 1; i >= 0; i--)
        {
            if (havingQuests.HavingQuestList[i].QuestID == questId)
            {
                GameManager.instance.getMoney(havingQuests.HavingQuestList[i].MoneyAmount);//돈 증가
                ExpManager.instance.getExp(havingQuests.HavingQuestList[i].ExpAmount);//명성 증가

                //퀘스트에 사용된 아이템 인벤토리에서 삭제
                RemoveCake(havingQuests.HavingQuestList[i].cakeToMakeIdx, havingQuests.HavingQuestList[i].ClearValue1);

                havingQuests.HavingQuestList.Remove(havingQuests.HavingQuestList[i]);
                Debug.Log("Quest ID : " + i + " 를 완료했습니다.");
                UpdateCurrnetQuestList();//json에 반영 
                setCurrentQuestUIs();//UI반영
                break;
            }
        }
    }

    //해당 퀘스트를 가지고 있는 퀘스트리스트에서 삭제 (개발용)
    public void EraseQuest(int questId)
    {
        for(int i= havingQuests.HavingQuestList.Count-1; i >= 0; i--)
        {
            if (havingQuests.HavingQuestList[i].QuestID == questId)
            {
                havingQuests.HavingQuestList.Remove(havingQuests.HavingQuestList[i]);
                Debug.Log("Quest ID : " + i + " 를 현재 가지고 있는 퀘스트 리스트에서 삭제했습니다.");
                UpdateCurrnetQuestList();//json에 반영 
                setCurrentQuestUIs();//UI반영

                break;
            }
        }
    }

    //가지고 있는 퀘스트 전체 삭제(개발용)
    public void EraseAllQuest()
    {
        for(int i= havingQuests.HavingQuestList.Count-1;i>=0; i--)
        {
            havingQuests.HavingQuestList.Remove(havingQuests.HavingQuestList[i]);
        }

        UpdateCurrnetQuestList();//json에 반영 
        setCurrentQuestUIs();//UI반영
        Debug.Log("퀘스트를 모두 삭제했습니다.");
    }

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
        public int cakeToMakeIdx;
        public int CurrnetValue1;
        public int ClearValue1;
        public int MoneyAmount;
        public int ExpAmount;
    }
}