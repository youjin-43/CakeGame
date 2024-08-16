using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AnimalInteractionManager : MonoBehaviour
{
    [SerializeField]
    public GameObject UICanvas; // 게임 시작할 때만 켜도록..

    [SerializeField]
    public Camera mainCamera;

    [SerializeField]
    public int targetAnimalCount; // 총 잡아야하는 너구리 수

    [SerializeField]
    public int curAnimalCount; // 현재까지 소환된 너구리 수

    [SerializeField]
    public int catchAnimalCount; // 현재 잡은 너구리 수 

    [SerializeField]
    public int playTime = 10; // 너구리 잡기 게임 시간

    [SerializeField]
    public float curTime; // 현재 흐른 시간

    [SerializeField]
    public float screenWidth; // 스크린 너비

    [SerializeField]
    public float screenHeight; // 스크린 높이

    [SerializeField]
    public float gameSpeed; // 너구리 나오는 속도

    [SerializeField]
    public float animalEmergeTime;

    [SerializeField]
    public GameObject animalParent; // 너구리가 생성될 부모

    [SerializeField]
    public GameObject animalPrefab;

    [SerializeField]
    public Button gameStartButton;

    [SerializeField]
    public bool gameStart = false;

    [SerializeField]
    public Action<int> OnFailRequested; // 농사 망치기 함수 연결해줄것..


    // UI 관련
    public Button backgroundButton; // 게임 끝나고 누르면 진짜 팜씬 시작.. 
    public Text gameText;

    [Header("Close Farm Interaction Button")]
    public GameObject buttonParentGameObject; // 너구리 게임 띄우면 버튼 안 보이도록 하기 위함..


    private void Awake()
    {
        backgroundButton.onClick.AddListener(CloseAnimalGame);
        gameStartButton.onClick.AddListener(() => GetAnimalGameRun(10));
    }

    private void Start()
    {
        // 카메라 렉트의 width 와 height 을 가져온 후, 카메라 사이즈만큼 곱해주기..
        screenWidth = mainCamera.rect.width * mainCamera.orthographicSize;
        screenHeight = mainCamera.rect.height * mainCamera.orthographicSize;

        Debug.Log(screenWidth);
        Debug.Log(screenHeight);
    }



    private void Update()
    {
        if (gameStart)
        {
            curTime += Time.deltaTime;
            animalEmergeTime += Time.deltaTime;

            gameSpeed = playTime / (float)targetAnimalCount;

            if (curAnimalCount < targetAnimalCount && animalEmergeTime >= gameSpeed)
            {
                float randomX = Random.Range(-screenWidth, screenWidth);
                float randomY = Random.Range(-screenHeight, screenHeight);

                Vector3 animalPos = new Vector3(randomX, randomY, 0);

                Animal animal = Instantiate(animalPrefab, animalParent.transform).GetComponent<Animal>();
                animal.transform.position = animalPos;
                curAnimalCount++;
                animalEmergeTime = 0;
            }

            if (curTime >= playTime + 2)
            {
                // 게임 종료 시 로직
                curTime = 0;
                curAnimalCount = 0;
                gameStart = false;

                if (catchAnimalCount >= targetAnimalCount)
                {
                    gameText.text = "게임 클리어! 농장을 무사히 지켜냈다!";
                } 
                else
                {
                    gameText.text = "농사땅 " + (targetAnimalCount - catchAnimalCount) + "개가 망했어요..";
                }

                // 2초 뒤에 게임 끄기 버튼 활성화..
                Invoke("SetBackgroundButton", 2);

                backgroundButton.enabled = true;

                Debug.Log("게임 끝!");
            }
        }
    }


    public void SetAnimalCount(int count)
    {
        gameStartButton.onClick.RemoveAllListeners(); // 일단 시작 버튼에 연결된 모든 함수 삭제한 후..
        gameStartButton.onClick.AddListener(() => GetAnimalGameRun(count)); // 함수 다시 연결해주기..
    }

    void CloseFarmButton()
    {
        if (UICanvas.activeSelf)
        {
            if (UIInventoryManager.instance.buttonParentGameObject != null) 
                buttonParentGameObject.SetActive(false);
        }
    }

    void SetBackgroundButton()
    {
        backgroundButton.enabled = true;
    }

    public void GetAnimal()
    {
        catchAnimalCount++;
    }

    public void ResetGameData()
    {
        targetAnimalCount = 0;
        curAnimalCount = 0;
        catchAnimalCount = 0;
        animalEmergeTime = 0;
        gameSpeed = 0;
        curTime = 0;
        buttonParentGameObject.SetActive(false);
    }

    // 미니게임 로직..
    public void GetAnimalGameRun(int targetCount)
    {
        ResetGameData();
        UICanvas.SetActive(true);

        gameStart = true;
        targetAnimalCount = targetCount;
        gameStartButton.gameObject.SetActive(false);
    }


    public void CloseAnimalGame()
    {
        backgroundButton.enabled = false;
        UICanvas.SetActive(false);
        UIInventoryManager.instance.buttonParentGameObject.SetActive(true); // 농사땅 버튼 이제 켜주기..
        gameStartButton.gameObject.SetActive(true);

        // 못잡은 너구리 수만큼 농사땅 망치기 위한 로직..
        // 연결해놓은 함수에 매개변수 전달하는 것..
        OnFailRequested?.Invoke(targetAnimalCount - catchAnimalCount);
    }
}
