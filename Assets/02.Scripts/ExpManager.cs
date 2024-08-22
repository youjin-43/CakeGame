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
            Debug.Log("exp매니저에서 데이터를 로드함");
            level = PlayerPrefs.GetInt("level");
            exp = PlayerPrefs.GetFloat("exp");
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

    //public GameObject ExpBar;

    public int level = 1;
    public float exp; // 이거 getset으로 하면 인스펙터 창에 안뜨던데 우선 이렇게 해놓겟음 
    public float exp_max=100;

    private void Start()
    {
        level = PlayerPrefs.GetInt("level");
        exp_max = PlayerPrefs.GetFloat("exp_max");
        exp = PlayerPrefs.GetFloat("exp");
    }

    public void getExp(float delta)
    {
        exp += delta;
        PlayerPrefs.SetFloat("exp", exp);//exp데이터저장

        //레벨업 
        if (exp >= exp_max)
        {
            level++;
            PlayerPrefs.SetInt("level", level);//레벨 데이터 업데이트
            UIManager.instance.levelText.text = level.ToString();//UI 업데이트 

            exp = exp - exp_max;
            PlayerPrefs.SetFloat("exp", exp);//exp데이터 업데이트

            exp_max += 100; //다음 레벨업까지 얻어야하는 양 증가 
            PlayerPrefs.SetFloat("exp_max", exp_max);

            UIManager.instance.SetExpBarUI();//UI 업데이트 

            //스토리 진행 
            if (level == 2)
            {
                //SceneManager.LoadScene("Level5");
            }else if (level == 4)
            {
                
            }else if (level == 6)
            {

            }else if(level == 8)
            {

            }else if (level == 10)
            {

            }          
        }
    }
}
