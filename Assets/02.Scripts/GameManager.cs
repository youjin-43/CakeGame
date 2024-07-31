using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager 스크립트는 UI를 관리하고 게임 재시작을 해야하기 때문에 관련 라이브러리를 using으로 가져와야 함
using UnityEngine.UI;//UI 관련 라이브러리
using UnityEngine.SceneManagement;//씬 관련 라이브러리


public class GameManager : MonoBehaviour
{
    //싱글턴으로
    public static GameManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("게임매니저가 생성됐습니다");
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("게임매니저를 죽입니다");
        }
    }


    [Header("Managers")]
    public DataManager dataManager = new DataManager();
    private QuestManager questManager;
    private UIManager uiManager;
    private ExpManager expManager;

    public enum Seasons
    {
        spring,
        summer, 
        fall,
        winter
    }

    [Header("BasicData")]
    public Seasons season;
    public int date;
    public int money;
    public int popularity;

    [Header("About Exp")]
    //[SerializeField] public float exp = 0f; // 이거 getset으로 하면 인스펙터 창에 안뜨던데 우선 이렇게 해놓겟음 
    //[SerializeField] public float exp_max = 100f;

    [Header("About Running")]
    public float runTime; // 가게 runTime시간
    public bool isRunning; //가게가 운영중인지 표시하는 변수

    public GameObject QuestBoard;
    public GameObject QuestUIprefab;


    void Start()
    {


        questManager = GetComponent<QuestManager>();
        questManager.QuestDT = dataManager.tableDic[DataManager.CSVDatas.QuestTable]; //여기서 이렇게 할당을 해줘야 돌아감. 퀘스트매니저 안에서 할당받으면 안돌아감; 이유는 몰루,,
        Debug.Log(questManager.QuestDT.Columns[0] + "게임매니저에서 퀘스트 데이터를 할당받음 ");

        uiManager = GetComponent<UIManager>();
        expManager = GetComponent<ExpManager>();

        //이전 데이터 가져오기
        season = (Seasons)PlayerPrefs.GetInt("season");
        date = PlayerPrefs.GetInt("date");
        money = PlayerPrefs.GetInt("money"); //주어진 키로 저장된 값이 없으면 기본값을 반환
        popularity = PlayerPrefs.GetInt("popularity");

        expManager.level = PlayerPrefs.GetInt("level");
        expManager.exp = PlayerPrefs.GetFloat("exp");

        uiManager.SetDatainUI();//UI 데이터 표시
        uiManager.setExpUI();//UI에 경험치 표시 

    }

    void Update()
    {

        //가게가 운영되는 동안 운영 시간 표시 
        if (isRunning)
        {
            runTime += Time.deltaTime;
            uiManager.runningTimeText.text = "Time :" + (int)runTime;

            if (runTime >= 5.0f) // 우선 5로 해놓음 
            {
                EndRunning();
            }
        }

        //경험치 실험 중
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExpManager.instance.getExp(10);
        }

    }


    //가게 운영 시작하는 함수 
    public void StartRunning()
    {
        uiManager.runningOverBoard.SetActive(false); // 정산 화면 끄기

        date++;
        runTime = 0;

        

        isRunning = true;

        //새로운 퀘스트 부여
        questManager.GenQuest();

    }

    //가게 운영이 끝났을때 호출 할 함수
    private void EndRunning()
    {
        isRunning = false; //운영 끝! 
        uiManager.runningOverBoard.SetActive(true); // 정산 화면 뜨기
        uiManager.RunStartButton.SetActive(true);

        //정산 결과 저장 -> 돈, 인지도 등 데이터 갱신
        PlayerPrefs.SetInt("date", date);
        PlayerPrefs.SetInt("season", (int)season);

        //PlayerPrefs.SetFloat(string key, float value); //float 값을 저장할때 : 키와 키와 대응하는 값을 입력값으로 받음
        //PlayerPrefs.GetFloat(string key); float 값을 불러올 때
        //int 와 string도 가능

        PlayerPrefs.SetFloat("money", money);


    }

    public void getMoney()
    {
        Debug.Log("getmoeny 함수 실행");
        money += 100;
        uiManager.moneyText.text = "Money : " + money;

        PlayerPrefs.SetInt("money", money); //이걸 endRunning 함수에 넣어야 하나 고민중 
    }


}
