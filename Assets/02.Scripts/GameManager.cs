using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager 스크립트는 UI를 관리하고 게임 재시작을 해야하기 때문에 관련 라이브러리를 using으로 가져와야 함
using UnityEngine.UI;//UI 관련 라이브러리
using UnityEngine.SceneManagement;//씬 관련 라이브러리


public class GameManager : MonoBehaviour
{
    //싱글턴으로
    public static GameManager instance; // 싱글톤을 할당할 전역 변수 -> 이 instance 자체는 게임 오브젝트를 얘기하는것 같고 

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("게임매니저가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록 s

            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로딩될 때마다 함수를 호출하기위해 

            //이전 데이터 가져오기
            Debug.Log("게임매니저에서 기본 데이터를 로드함");
            season = (Seasons)PlayerPrefs.GetInt("season");
            date = PlayerPrefs.GetInt("date");
            money = PlayerPrefs.GetInt("money"); //주어진 키로 저장된 값이 없으면 기본값을 반환

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

    [Header("About Running")]
    public float runTime; // 현재 시간
    public float MaxRunTime = 60f;
    //public bool isRunning; //가게가 운영중인지 표시하는 변수

    public GameObject RuntimeBar;

    void Update()
    {

        //가게가 운영되는 동안 운영 시간 표시 
        if (Routine.instance.routineState == RoutineState.Open)
        {
            runTime += Time.deltaTime;
            RuntimeBar.GetComponent<Image>().fillAmount = runTime / MaxRunTime;

            if (runTime >= MaxRunTime) 
            {
                EndRunning();
            }
        }

    }

    // 이 함수는 매 씬마다 호출됨.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded : " + scene.name);
        Debug.Log("mode : " + mode);

        UIManager.instance.setUIObjects(); //오브젝트 변수에 오브젝트들 찾아서 할당 
        UIManager.instance.SetDatainUI();//각 UI오브젝트에 맞는 데이터들 할당 
        UIManager.instance.SetExpBarUI(); //경험치 데이터에 맞게 expbar 설정 

        QuestManager.instance.setBasicQuestUIs();//퀘스트 UI 세팅

        RuntimeBar = GameObject.Find("Background").gameObject;
    }


    //가게 운영 시작하는 함수 
    public void StartRunning()
    {
        Routine.instance.routineState = RoutineState.Open;

        UIManager.instance.runningOverBoard.SetActive(false); //정산화면 끄기

        date++;
        UIManager.instance.dateText.text = date.ToString();//UI 적용 

        runTime = 0;
        //isRunning = true;
    }

    //가게 운영이 끝났을때 호출 할 함수
    private void EndRunning()
    {
        //isRunning = false; //운영 끝!
        Routine.instance.routineState = RoutineState.Close;

        UIManager.instance.runningOverBoard.SetActive(true); // 정산 화면 뜨기
        //UIManager.instance.RunStartButton.SetActive(true); // 가게 운영이 끝나면 농장으로 가도록 

        //정산 결과 저장 -> 돈, 인지도 등 데이터 갱신
        PlayerPrefs.SetInt("date", date);
        PlayerPrefs.SetInt("season", (int)season);
        PlayerPrefs.SetFloat("money", money);
    }

    public void getMoney(int amount)
    {
        Debug.Log("getmoeny 함수 실행");

        money += amount;
        UIManager.instance.moneyText.text = money.ToString();
        PlayerPrefs.SetInt("money", money); 
    }
}
