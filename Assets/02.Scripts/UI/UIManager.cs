using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //씬 관련 라이브러리 

public class UIManager : MonoBehaviour
{
    //싱글턴으로
    public static UIManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("UIManager가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 UIManager가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("UIManager를 죽입니다");
        }
    }



    [Header("About UI")]
    public GameObject runningOverBoard; //가게 운영이 끝났을때 활성화 할 오브젝트
    public GameObject EndBoardContent;
    public int[] SelledCakeCnt; //해당 케이크가 얼마다 팔렸는지 갯수 저장 (배열 인덱스 == 케이크id idx)

    public Text dateText;
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트
    public Text levelText;

    public GameObject RunStartButton; //가게 운영을 시작시키는 버튼
    public GameObject GoFarmButton; //가게 운영이 끝나고 농장으로 가는 버튼 

    public GameObject EventButton;//이벤트가 있는경우 클릭해서 이벤트를 실행할 버튼

    [Header("About EXP")]
    public GameObject ExpBar;


    //오브젝트 변수에 오브젝트들 찾아서 할당 
    public void setUIObjects()
    {
        //Debug.Log("SetDatainUI 실행됨 ");

        //부모에 접근 
        GameObject tmp = GameObject.Find("SettingAnchors");

        //씬에 정산보드(EndBoard) UI 오브젝트가 있다면 
        if (tmp.transform.GetChild(1).name == "EndBoard")
        {
            runningOverBoard = tmp.transform.GetChild(1).gameObject;//EndBoard 오브젝트 셋팅하고
            runningOverBoard.gameObject.SetActive(false); //정산보드 꺼놓음
            EndBoardContent = runningOverBoard.transform.GetComponentInChildren<VerticalLayoutGroup>().gameObject;//정산보드 컨텐츠를 구성할 오브젝트도 할당 
        }
        
        //이벤트 버튼
        EventButton = GameObject.Find("EventButton");
        if(EventButton != null)
        {
            // 실행해야하는 이벤트 없으면 비활성화
            if (EventSceneManager.instance.toShowEvnetList.events.Count < 1) EventButton.gameObject.SetActive(false); 
            EventButton.GetComponent<Button>().onClick.AddListener(EventSceneManager.instance.StartEventScene);//버튼 기능 세팅 
        }

        //각종 데이터(텍스트)를 표시할 오브젝트들 
        dateText = GameObject.Find("DateText").GetComponent<Text>();
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        RunStartButton = GameObject.Find("RunStartButoon");//가게 운영 스타트 버튼
        if(RunStartButton != null)
        {
            RunStartButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.StartRunning);//버튼 기능 세팅 -> 이렇게 코드로 하면 인스펙터에서 안보이네..?
        }
        GoFarmButton = tmp.transform.GetChild(2).gameObject; // 농장으로 가는 버튼 
        ExpBar = GameObject.Find("ExpBar");
    }

    //각 UI오브젝트에 맞는 데이터들 할당 
    public void SetDatainUI()
    {
        //Debug.Log("SetDatainUI실행");
        //seasonText.text = GameManager.instance.season.ToString();
        dateText.text = GameManager.instance.date.ToString();
        moneyText.text = GameManager.instance.money.ToString();
        levelText.text = ExpManager.instance.level.ToString();
    }

    //경험치 데이터에 맞게 expbar 설정 
    public void SetExpBarUI()
    {
        ExpBar.GetComponent<Image>().fillAmount = ExpManager.instance.exp / ExpManager.instance.exp_max;
    }

    
    public void initSelledCakeCnt()
    {
        SelledCakeCnt = new int[6] { 0, 0, 0, 0, 0,0 };
    }

    public void RaiseUpCakeCntForEndBoard(int idx)
    {
        SelledCakeCnt[idx]++;
    }
    
    public void SetEndBoard()
    {
        int total = 0;
       
        for (int i = 0; i < EndBoardContent.transform.childCount; i++)
        {
            if (SelledCakeCnt[i] == 0)
            {
                EndBoardContent.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                Transform content = EndBoardContent.transform.GetChild(i);
                content.gameObject.SetActive(true);
                content.Find("ItemCnt").GetComponent<Text>().text = SelledCakeCnt[i].ToString();//팔린갯수 셋팅

                int cost = SelledCakeCnt[i] * 100; //100은 케이크 가격. 나중에 수정 해야함 
                content.Find("sum").GetComponent<Text>().text = cost.ToString();//팔린갯수 셋팅
                total += cost;
            }
            
        }
        runningOverBoard.transform.Find("totalMoneyText").GetComponent<Text>().text = total.ToString();//팔린갯수 셋팅

    }
}
