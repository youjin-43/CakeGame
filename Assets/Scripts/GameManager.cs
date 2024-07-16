using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager 스크립트는 UI를 관리하고 게임 재시작을 해야하기 때문에 관련 라이브러리를 using으로 가져와야 함
using UnityEngine.UI;//UI 관련 라이브러리
using UnityEngine.SceneManagement;//씬 관련 라이브러리


public class GameManager : MonoBehaviour
{
    //싱글턴으로 만들어야 하는디... 우선 ㄱ


    [Header("Data")] 
    [SerializeField] private int money;
    [SerializeField] private int popularity;


    [Header("About Running")]
    [SerializeField] private float runTime; // 가게 runTime시간
    [SerializeField] private bool isRunning; //가게가 운영중인지 표시하는 변수

    [Header("About UI")]
    public GameObject runningOverBoard; //가게 운영이 끝났을때 활성화 할 오브젝트 
    public Text runningTimeText;//현재 시간을 표시할 텍스트 컴포넌트 
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트
    public Text popularityText;//현재 돈을 표시할 텍스트 컴포넌트

    void Start()
    {
        //이전 데이터 가져오기
        money = PlayerPrefs.GetInt("money"); //주어진 키로 저장된 값이 없으면 기본값을 반환
        popularity = PlayerPrefs.GetInt("popularity");

        //데이터 표시 
        moneyText.text = "Money : " + money;
        popularityText.text = "Popularity : " + popularity;

        //가게 운영 시작 -> start는 게임 처음 실행될때 실행되는거라 가게 운영 시작하는 기능은 따로 빼야할 듯
        StartRunning();

    }

    void Update()
    {
        //가게가 운영되는 동안 운영 시간 표시 
        if (isRunning)
        {
            runTime += Time.deltaTime;
            runningTimeText.text = "Time :" + (int)runTime;

            if (runTime>=10.0f) // 우선 10으로 해놓음 
            {
                EndRunning();
            }
        }
        else
        {
            //우선 임시로 R 누르면 넘어가도록 해놓음 
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Farm");
            }
        }
    }


    //가게 운영 시작하는 함수 
    private void StartRunning()
    {
        runTime = 0;
        isRunning = true;
    }

    //가게 운영이 끝났을때 호출 할 함수
    private void EndRunning()
    {
        isRunning = false; //운영 끝! 
        runningOverBoard.SetActive(true); // 정산 화면 뜨기

        //정산 결과 저장
        //PlayerPrefs.SetFloat(string key, float value); //float 값을 저장할때 : 키와 키와 대응하는 값을 입력값으로 받음
        //PlayerPrefs.GetFloat(string key); float 값을 불러올 때
        //int 와 string도 가능


    }
}
