using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//씬 관련 라이브러리

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
    public float exp = 0f; // 이거 getset으로 하면 인스펙터 창에 안뜨던데 우선 이렇게 해놓겟음 
    public float exp_max = 100f;

    public void getExp(float delta)
    {
        exp += delta;
        if (exp > exp_max)
        {
            exp = exp - exp_max;
            level++;

            if(level == 3) SceneManager.LoadScene("Level5");

            UIManager.instance.levelText.text = level.ToString();

            exp_max += 50;
        }
        PlayerPrefs.SetFloat("exp", exp);
        UIManager.instance.setExpUI();
    }
}
