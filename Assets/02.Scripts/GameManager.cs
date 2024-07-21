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
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }
    }


    [Header("Managers")]
    public DataManager dataManager = new DataManager();
    private QuestManager questManager = new QuestManager();


    //이거 아래 스토어 매니저로 옮기는게 좋을것 같은데 

    enum Seasons
    {
        spring,
        summer,
        fall,
        winter
    }

    [Header("Data")]
    [SerializeField] private Seasons season;
    [SerializeField] private int date;
    [SerializeField] public int money;
    [SerializeField] private int popularity;


    [Header("About Running")]
    [SerializeField] private float runTime; // 가게 runTime시간
    [SerializeField] public bool isRunning; //가게가 운영중인지 표시하는 변수

    [Header("About UI")]
    public GameObject runningOverBoard; //가게 운영이 끝났을때 활성화 할 오브젝트 
    public Text runningTimeText;//현재 시간을 표시할 텍스트 컴포넌트
    public Text seasonText;
    public Text dateText;
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트
    public Text popularityText;//현재 돈을 표시할 텍스트 컴포넌트

    //public GameObject QuestBoard;
    public Text QuestText;//임시 퀘스트 관련 텍스트 컴포넌트


    void Start()
    {
        questManager.QuestDT = dataManager.tableDic[DataManager.CSVDatas.QuestTable];
        //Debug.Log(questManager.QuestDT.Columns[0]);

        //이전 데이터 가져오기
        season = (Seasons)PlayerPrefs.GetInt("season");
        date = PlayerPrefs.GetInt("date");
        money = PlayerPrefs.GetInt("money"); //주어진 키로 저장된 값이 없으면 기본값을 반환
        popularity = PlayerPrefs.GetInt("popularity");



        //가게 운영 시작 -> start는 게임 처음 실행될때 실행되는거라 가게 운영 시작하는 기능은 따로 빼야할 듯
        StartRunning();

    }

    void Update()
    {
        //우선 임시로 R 누르면 넘어가도록 해놓음 
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CakeStorePractice")){
                SceneManager.LoadScene("Farm");
            }
            else
            {
                SceneManager.LoadScene("CakeStorePractice");
            }
            
        }

        //혹시 개발 과정에서 필요할까 싶어 
        if (Input.GetKeyDown(KeyCode.X))
        {
            money = 0;
            PlayerPrefs.SetInt("money", money); //이걸 endRunning 함수에 넣어야 하나 고민중
            moneyText.text = "Money : " + money;
        }

        //임시 퀘스트 창
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    if(QuestBoard.activeSelf == true)
        //    {
        //        QuestBoard.SetActive(false);
        //    }
        //    else
        //    {
        //        QuestBoard.SetActive(true);
        //    }
        //}

        //가게가 운영되는 동안 운영 시간 표시 
        if (isRunning)
        {
            runTime += Time.deltaTime;
            runningTimeText.text = "Time :" + (int)runTime;

            if (runTime>=5.0f) // 우선 5로 해놓음 
            {
                EndRunning();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StartRunning();
            }
        }
    }


    //가게 운영 시작하는 함수 
    private void StartRunning()
    {
        runningOverBoard.SetActive(false); // 정산 화면 끄기

        date++;
        runTime = 0;

        //새로운 퀘스트 부여
        Quest quest = questManager.GenQuest();
        QuestText.text = "ID : " + quest.QuestID + " || getMony :" + quest.Reward1Amount + "\n 목표 :" + quest.ExplaneText;

        //데이터 표시
        seasonText.text = season.ToString();
        dateText.text = date.ToString();
        moneyText.text = "Money : " + money;
        popularityText.text = "Popularity : " + popularity;

        isRunning = true;

    }

    //가게 운영이 끝났을때 호출 할 함수
    private void EndRunning()
    {
        isRunning = false; //운영 끝! 
        runningOverBoard.SetActive(true); // 정산 화면 뜨기

        //정산 결과 저장 -> 돈, 인지도 등 데이터 갱신
        PlayerPrefs.SetInt("date", date);
        PlayerPrefs.SetInt("season", (int)season);

        //PlayerPrefs.SetFloat(string key, float value); //float 값을 저장할때 : 키와 키와 대응하는 값을 입력값으로 받음
        //PlayerPrefs.GetFloat(string key); float 값을 불러올 때
        //int 와 string도 가능

        //PlayerPrefs.SetFloat("money", money);


    }

    public void getMoney()
    {
        Debug.Log("getmoeny 함수 실행");
        money += 100;
        moneyText.text = "Money : " + money;

        PlayerPrefs.SetInt("money", money); //이걸 endRunning 함수에 넣어야 하나 고민중 
    }
}
