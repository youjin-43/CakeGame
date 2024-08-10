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

    public Text dateText;
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트
    public Text levelText;

    public GameObject RunStartButton; //가게 운영이 끝났을때 활성화 할 오브젝트

    [Header("About EXP")]
    public GameObject ExpBar;


    
    private void Start()
    {

        
    }

    public void setUIObjects()
    {
        Debug.Log("SetDatainUI 실행됨 ");

        runningOverBoard = GameObject.Find("EndBoard");
        if(runningOverBoard != null) runningOverBoard.gameObject.SetActive(false);

        dateText = GameObject.Find("DateText").GetComponent<Text>();
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        RunStartButton = GameObject.Find("RunStartButoon");

        ExpBar = GameObject.Find("ExpBar");
    }

    public void SetDatainUI()
    {
        //seasonText.text = GameManager.instance.season.ToString();
        dateText.text = GameManager.instance.date.ToString();
        moneyText.text = GameManager.instance.money.ToString();
        levelText.text = ExpManager.instance.level.ToString();
    }

    public void SetExpBarUI()
    {
        ExpBar.GetComponent<Image>().fillAmount = ExpManager.instance.exp / ExpManager.instance.exp_max;
    }
}
