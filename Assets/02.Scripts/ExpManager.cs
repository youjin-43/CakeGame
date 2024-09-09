using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//씬 관련 라이브러리
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    //싱글턴으로
    public static ExpManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("ExpManager가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록

            //이전 데이터 가져오기
            LoadExpData();

            //주어진 키로 저장된 값이 없으면 기본값을 반환 -> 게임을 처음 시작해서 데이터가 0인경우 초기값으로 설정 후 다시 저장 
            if (level == 0)
            {
                level = 1;
                PlayerPrefs.SetInt("level", level);
            }
            if(exp_max == 0)
            {
                exp_max = 2000;
                PlayerPrefs.SetFloat("exp_max", exp_max);
            }

        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 ExpManager가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("ExpManager를 죽입니다");
        }
    }

    public int level = 1;
    public float exp; 
    public float exp_max=2000;

    //오브젝트가 
    private void Start()
    {
        LoadExpData();
    }

    private void Update()
    {
        //E를 누르면 경험치 10 씩 증가
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExpManager.instance.getExp(100);
            UIManager.instance.SetExpBarUI();
        }

        //경험치 및 레벨 초기화
        else if (Input.GetKeyDown(KeyCode.C))
        {
            level = 1;
            exp_max = 2000;
            exp = 0;
            SetExpData(); //데이터 갱신

            //UI 업데이트
            UIManager.instance.SetExpBarUI();
            UIManager.instance.levelText.text = ExpManager.instance.level.ToString();

            CakeManager.instance.ResetUnlockCake(); //케이크 레시피 상태도 초기화 

            Debug.Log("경험치 초기화시킴 ");
            QuestManager.instance.EraseAllQuest();
        }
    }

    private void LoadExpData()
    {
        level = PlayerPrefs.GetInt("level");
        exp = PlayerPrefs.GetFloat("exp");
        exp_max = PlayerPrefs.GetFloat("exp_max");
    }

    private void SetExpData()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("exp", exp);
        PlayerPrefs.SetFloat("exp_max", exp_max);
    }

    public void getExp(float delta)
    {
        exp += delta;
        PlayerPrefs.SetFloat("exp", exp);//exp데이터저장

        //레벨업 
        if (exp >= exp_max)
        {
            level++;
            UIManager.instance.levelText.text = level.ToString();//UI 업데이트 

            exp = exp - exp_max;
            exp_max += 2000; //다음 레벨업까지 얻어야하는 양 증가 
            UIManager.instance.SetExpBarUI();//UI 업데이트

            SetExpData();//데이터 저장 

            if (level == 2)
            {
                EventSceneManager.instance.toShowEvnetList.events.Add(1); //해금해야하는 레시피 인덱스를 받음 
            }
            else if(level == 4)
            {
                EventSceneManager.instance.toShowEvnetList.events.Add(2); 
            }
            else if (level == 6)
            {
                EventSceneManager.instance.toShowEvnetList.events.Add(3);
            }
            else if (level == 8)
            {
                EventSceneManager.instance.toShowEvnetList.events.Add(4);
            }
            else if (level == 10)
            {
                EventSceneManager.instance.toShowEvnetList.events.Add(5);
            }

            EventSceneManager.instance.SaveEventList();//추가된 이벤트 정보 json으로 저장 

        }
    }
}
