using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager 스크립트는 UI를 관리하고 게임 재시작을 해야하기 때문에 관련 라이브러리를 using으로 가져와야 함
using UnityEngine.UI;//UI 관련 라이브러리
using UnityEngine.SceneManagement;//씬 관련 라이브러리


public class GameManager : MonoBehaviour
{
    public GameObject runningOverBoard; //가게 운영이 끝났을때 활성화 할 오브젝트 

    public Text runningTimeText;//현재 시간을 표시할 텍스트 컴포넌트 
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트

    private float runTime; // 가게 운runTime시간
    private bool isRunning; //가게가 운영중인지 표시하는 변수 

    void Start()
    {
        runTime = 0;
        isRunning = true;
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
                isRunning = false;
            }
        }
        else
        {
            //가게 운영이 끝났을때 처리할것들

            // 정산 화면 뜨고 -> runningOverBoard 활성화

            //우선 임시로 R 누르면 넘어가도록 해놓음 
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    public void EndRunning()
    {
        //가게 운영이 끝났을때 호출 할 함수 구현은 안하고 우선 선언만 해둠
    }
}
