using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AnimalInteractionManager : MonoBehaviour
{
    // 게임 오브젝트 데이터
    [SerializeField]
    public FarmingManager farmingManager;

    [SerializeField]
    public AnimalSpawner animalSpawner;

    [SerializeField]
    public GameObject UICanvas; // 게임 시작할 때만 켜도록..

    [SerializeField]
    public Camera mainCamera;




    // 동물 게임 정보
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
    public float gameSpeed; // 너구리 나오는 속도

    [SerializeField]
    public float animalEmergeTime;

    [SerializeField]
    public int failFarmCount; // 망한 땅 개수

    [SerializeField]
    public int damageFarmCount; // 총 데미지 수(개구리가 와그작 몇 번했는지..)

    [SerializeField]
    public bool gameStart = false;

    [SerializeField]
    public int disappearAnimalCount; // 총 사라진 동물 수(이게 targetAnimalCount 와 같아지면 게임 끝!)

    [SerializeField]
    public bool exitGame;




    // 동물 생성 프리팹 관련
    [SerializeField]
    public GameObject animalParent; // 너구리가 생성될 부모

    [SerializeField]
    public GameObject animalPrefab;

    [SerializeField]
    public Button gameStartButton;



    // UI 관련
    public Button backgroundButton; // 게임 끝나고 누르면 진짜 팜씬 시작.. 
    public Text gameText;
    public GameObject titlePanel; // 게임 끝나면 안 보이도록 해야함..



    [Header("BGM & SFX")]
    // BGM 관련
    public Action OnAnimalGameClosed; // 동물 게임 끝났을 때 호출되어야 하는 함수 연결하는 델리게이트 선언..
    public AudioManager audioManager; // 음악 관리하기 위함..



    [Header("Close Farm Interaction Button")]
    public GameObject buttonParentGameObject; // 너구리 게임 띄우면 버튼 안 보이도록 하기 위함..


    private void Awake()
    {
        backgroundButton.onClick.AddListener(CloseAnimalGame);
        backgroundButton.onClick.AddListener(PressedBackgroundButton);
        backgroundButton.onClick.AddListener(farmingManager.CheckFailedFarm); // 농사땅이 망했는지 확인하고, 망했으면 그에 맞는 행동을 하도록 해주는 함수 연결..
        //gameStartButton.onClick.AddListener(() => GetAnimalGameRun(10));
    }


    private void Update()
    {
        if (gameStart)
        {
            exitGame = false;
            curTime += Time.deltaTime;
            animalEmergeTime += Time.deltaTime;

            gameSpeed = playTime / (float)targetAnimalCount;

            if (curAnimalCount < targetAnimalCount && animalEmergeTime >= gameSpeed)
            {
                // 새로 생성되는 동물 클래스의 델리게이트에 함수 연결해주기
                Animal tmp = animalSpawner.GetAnimal(0).GetComponent<Animal>();

                // 델리게이트가 비어있을 때만 함수 연결(중복 연결 막기 위함..)..
                if (tmp.OnClicked == null)
                    tmp.OnClicked += GetAnimal;
                if (tmp.OnDamaged == null)
                    tmp.OnDamaged += GetDamage;
                if (tmp.OnFailed == null)
                    tmp.OnFailed += GetFail;
                if (tmp.OnDisappeared == null)
                    tmp.OnDisappeared += GetDisappearAnimal;
                if (tmp.OnExited == null)
                    tmp.OnExited += ExitGame;
 
 
                curAnimalCount++;
                animalEmergeTime = 0;
            }

            // 소환되어야 하는만큼 다 소환되면 그리고 현재 시간이 플레이 타임에 +2 한 것보다 크거나 같으면 게임 종료..
            if (exitGame || ((curTime >= playTime + 2) && (disappearAnimalCount==targetAnimalCount)))
            {
                animalSpawner.DisableAnimal(0); // 동물 다 없애버리기

                // 게임 종료 시 로직
                curTime = 0;
                curAnimalCount = 0;
                gameStart = false;

                titlePanel.SetActive(false); // 제목 판넬 꺼버리기..
                if (catchAnimalCount >= targetAnimalCount)
                {
                    gameText.text = "게임 클리어! 농장을 무사히 지켜냈다!";
                } 
                else
                {
                    gameText.text = "망한 농사땅: " + failFarmCount + ", 총 늘어난 일수: " + damageFarmCount;
                }

                // 백그라운드 버튼 활성화 해주기..
                backgroundButton.gameObject.SetActive(true);

                // 2초 뒤에 게임 끄기 버튼 활성화..
                Invoke("SetBackgroundButton", 2);
                farmingManager.SaveFarmingData(); // 변경 사항 저장해주기..

                // UI 다시 켜주깅..
                UICanvas.SetActive(true);


                Debug.Log("게임 끝!");
            }
        }
    }


    void PressedBackgroundButton()
    {
        OnAnimalGameClosed?.Invoke(); // 델리게이트에 연결된 함수 호출(bgm 시작하는 함수 연결되어 있음..)
    }


    public void SetAnimalCount(int count)
    {
        backgroundButton.gameObject.SetActive(true);

        // 씨앗이 심어져 있는 땅의 개수를 셀 것..
        int seedOnTileCount = 0;
        foreach (var item in farmingManager.farmingData)
        {
            if (farmingManager.farmingData[item.Key].seedOnTile)
                seedOnTileCount++;
        }

        // 만약 씨앗이 심어져 있는 땅의 개수가 0보다 작거나 같으면 그냥 게임 종료하도록..
        if (seedOnTileCount <= 0)
        {
            gameStartButton.gameObject.SetActive(false); // 게임 시작 버튼 끄도록..
            gameText.text = "휴~ 망칠 땅이 존재하지 않아요! 개구리는 다시 돌아갑니다!";
            // 백그라운드 버튼 활성화 해주기..
            SetBackgroundButton(); // 동물 게임 끄기 버튼 활성화
        }
        else
        {
            gameStartButton.gameObject.SetActive(true); // 게임 시작 버튼 켜도록..
            gameStartButton.onClick.RemoveAllListeners(); // 일단 시작 버튼에 연결된 모든 함수 삭제한 후..
            gameStartButton.onClick.AddListener(() => GetAnimalGameRun(count)); // 함수 다시 연결해주기..
        }
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

    public void GetDisappearAnimal()
    {
        disappearAnimalCount++;
    }

    public void GetAnimal()
    {
        // 음향
        audioManager.SetSFX(AudioManager.SFX.ATTACK); // 동물 잡았을 때 나는 효과음..

        catchAnimalCount++;
    }

    public void GetDamage()
    {
        // 음향
        audioManager.SetSFX(AudioManager.SFX.DAMAGED); // 동물이 씨앗이 자라는 일수를 늘릴 때 나는 효과음..

        damageFarmCount++;
    }

    public void GetFail()
    {
        // 음향
        audioManager.SetSFX(AudioManager.SFX.FAILFARM); // 동물이 땅을 망쳤을 때 나는 효과음..

        failFarmCount++;
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
        backgroundButton.enabled = false;
        backgroundButton.gameObject.SetActive(false);
    }

    // 미니게임 로직..
    public void GetAnimalGameRun(int targetCount)
    {
        ResetGameData();
        UICanvas.SetActive(false);

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
    }

    public void ExitGame()
    {
        exitGame = true; // true 로 값 바꿔주기..
    }
}
