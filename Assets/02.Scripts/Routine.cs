using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Routine : MonoBehaviour
{
    private float timer;
    public float prepareTime;
    public float openTime;
    public RoutineState routineState;
    public static Routine instance;


    // 배경 UI 관련
    public Sprite[] backgroundImages; // [0]: 아침, [1]: 밤
    public Image backgroundImage; // 현재 무슨 상태인지에 따라 사진이 변경될 것..


    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            routineState = RoutineState.Prepare;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록

            // 중복으로 함수 연결되지 않도록 인스턴스 만들어질 때 한 번만 연결해주기..
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Update()
    {
        //timer += Time.deltaTime;
        //if (timer > prepareTime)
        //{
        //    routineState = RoutineState.Open;
        //}
        //if (timer > prepareTime + openTime)
        //{
        //    routineState = RoutineState.Close;
        //}


        // 배경 사진 바꾸는 로직
        switch (routineState)
        {
            case RoutineState.None:
            case RoutineState.Prepare:
            case RoutineState.Open:
                backgroundImage.sprite = backgroundImages[0]; // 이미지 아침으로 변경..
                break;
            case RoutineState.Close:
                backgroundImage.sprite = backgroundImages[1]; // 이미지 밤으로 변경..
                break;
        }
    }
    public void SetPrepare()
    {
        routineState = RoutineState.Prepare;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 완전히 로드될 때까지 기다린 후 코루틴 시작..
        StartCoroutine(InitializeAfterSceneLoad());
    }

    private IEnumerator InitializeAfterSceneLoad()
    {
        // 다음 프레임에 실행 되도록 하는 구문..
        yield return null;


        // 씬이 로드될 때 참조 변수 설정
        backgroundImage = GameObject.Find("BackgroundImage").GetComponent<Image>();
    }
}
public enum RoutineState
{
    None,
    Prepare,
    Open,
    Close
}
