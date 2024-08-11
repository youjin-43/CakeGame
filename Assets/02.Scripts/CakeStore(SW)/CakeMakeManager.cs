using Inventory.Model;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CakeMakerController : MonoBehaviour
{
    public GameObject cakeMakersPool;        // 케이크 메이커들이 담긴 풀 오브젝트
    public GameObject cakeMakerPanel;        // 케이크 메이커 패널 오브젝트
    public GameObject scrollViewContent;     // 스크롤 뷰 콘텐츠
    public GameObject[] cakeMakers;          // 케이크 메이커 배열

    public int cakeImageNum = 0;             // 케이크 이미지 UI 요소의 인덱스
    public int cakeNameNum = 1;              // 케이크 이름 UI 요소의 인덱스
    public int cakeCountNum = 2;             // 케이크 수량 UI 요소의 인덱스
    public int lockedNum = 3;                // 잠금 상태 UI 요소의 인덱스
    public int clickedNum = 4;               // 클릭된 상태 UI 요소의 인덱스
    public int costNum = 0;                  // 비용 텍스트 인덱스
    public int materialNum = 1;              // 재료 패널 인덱스
    public int materialImgaeNum = 0;         // 재료 이미지 인덱스
    public int materialCountNum = 1;         // 재료 수량 인덱스
    public int bakeTimeNum = 2;              // 베이킹 시간 텍스트 인덱스
    public int buttonNum = 3;                // 버튼 UI 인덱스
    public int cakeMakerIndex;               // 현재 선택된 케이크 메이커의 인덱스

    public GameObject timers;                // 타이머 오브젝트
    public Sprite[] timerSprites;            // 타이머 스프라이트 배열
    public Sprite completedSprite;           // 완료된 타이머 스프라이트

    private CakeManager cakeManager;         // 케이크 매니저 참조

    void Awake()
    {
        cakeMakerPanel.SetActive(false);       // 케이크 메이커 패널 비활성화
        cakeManager = GetComponent<CakeManager>(); // 케이크 매니저 초기화
        InitializeCakeMakers();               // 케이크 메이커 초기화
        SetupButtons();                       // 버튼 설정
    }

    void InitializeCakeMakers()
    {
        int makerCount = cakeMakersPool.transform.childCount;
        cakeMakers = new GameObject[makerCount];

        for (int i = 0; i < makerCount; i++)
        {
            cakeMakers[i] = cakeMakersPool.transform.GetChild(i).gameObject;
            var cakeMaker = cakeMakers[i].GetComponent<CakeMaker>();

            cakeMaker.timerUI = timers.transform.GetChild(i).gameObject;
            cakeMaker.timerSprites = timerSprites;
            cakeMaker.completedSprite = completedSprite;
            cakeMaker.cakeMakerIndex = i;
            cakeMaker.cakeMakerPanel = cakeMakerPanel;
            cakeMaker.storeManager = this.gameObject;

            InitializeTimerButton(cakeMaker.timerUI, i);
        }
    }

    void InitializeTimerButton(GameObject timerUI, int index)
    {
        Button timerButton = timerUI.GetComponent<Button>() ?? timerUI.AddComponent<Button>();
        timerButton.onClick.AddListener(() => OnTimerUIClicked(index));
    }

    void SetupButtons()
    {
        int childCount = scrollViewContent.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            int index = i;
            var panel = scrollViewContent.transform.GetChild(index);

            SetupButton(panel.GetComponent<Button>(), () => OnCakeClicked(index));
            SetupButton(panel.GetChild(clickedNum).GetChild(buttonNum).GetComponent<Button>(), () => OnMakeClicked(index));
        }
    }

    void SetupButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    public void OpenPanel(int index)
    {
        cakeManager.DisableSprites(true); // 스프라이트 비활성화
        cakeMakerPanel.SetActive(true);   // 케이크 메이커 패널 활성화
        cakeMakerIndex = index;           // 현재 케이크 메이커 인덱스 설정
        UpdateUI();                       // UI 업데이트
    }

    void OnCakeClicked(int index)
    {
        int childCount = scrollViewContent.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(clickedNum).gameObject.SetActive(i == index); // 클릭된 패널만 활성화
        }
    }

    void OnMakeClicked(int index)
    {
        if (!AreMaterialsEnough(index)) // 재료가 충분한지 확인
        {
            Debug.Log("재료가 부족합니다.");
            return;
        }

        UseMaterials(index); // 재료 사용
        cakeMakerPanel.SetActive(false); // 케이크 메이커 패널 비활성화
        cakeManager.DisableSprites(false); // 스프라이트 활성화
        StartCakeMaking(index); // 케이크 제작 시작
    }

    bool AreMaterialsEnough(int index)
    {
        var cakeData = cakeManager.cakeDataList[index];
        var fruitCounts = cakeManager.fruitContainer.fruitCount;
        
        if(cakeManager.gameManager.money < cakeData.cakeCost)
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }

        for (int i = 0; i < cakeData.materialIdxs.Length; i++)
        {
            if (fruitCounts[cakeData.materialIdxs[i]] < cakeData.materialCounts[i])
            {
                Debug.Log($"재료({cakeData.materialIdxs[i]})가 부족합니다.");
                return false;
            }
        }
        return true;
    }

    void UseMaterials(int index)
{
    var cakeData = cakeManager.cakeDataList[index];
    var inventory = cakeManager.inventoryManager.fruitInventoryData;


    cakeManager.gameManager.money -= cakeData.cakeCost;
    for (int i = 0; i < cakeData.materialIdxs.Length; i++)
    {
        // InventoryItem을 가져오고, 그 안에서 ItemSO 타입의 아이템을 추출합니다.
        InventoryItem inventoryItem = inventory.GetItemAt(cakeData.materialIdxs[i]);
        ItemSO itemSO = inventoryItem.item;  // ItemSO 추출

        // MinusItem 메서드에 ItemSO와 감소할 수량을 전달합니다.
        inventory.MinusItem(itemSO, cakeData.materialCounts[i]);
    }
}


    void StartCakeMaking(int index)
    {
        int cakeMakeTime = cakeManager.cakeDataList[index].bakeTime;
        var cakeMaker = cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>();
        cakeMaker.StartMakingCake(index, cakeMakeTime);
    }

    public void CompleteCake(int index)
    {
        cakeManager.IncreaseCakeCount(index); // 케이크 수량 증가
        UpdateUI();                           // UI 업데이트
    }

    public void OnTimerUIClicked(int index)
    {
        var cakeMaker = cakeMakers[index].GetComponent<CakeMaker>();

        if (cakeMaker.IsMakeComplete() && cakeMaker.TimerUIActive())
        {
            cakeMaker.CompleteCake(); // 케이크 제작 완료 처리
        }
    }

    void UpdateUI()
    {
        int childCount = scrollViewContent.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            var cakeSO = cakeManager.cakeDataList[i];

            UpdateCakePanel(panel, cakeSO, i);
        }

        gameObject.GetComponent<CakeShowcaseController>().UpdateUI(); // CakeShowcaseController의 UI 업데이트 호출
    }

    void UpdateCakePanel(Transform panel, CakeSO cakeSO, int index)
    {
        panel.GetChild(cakeImageNum).GetComponent<Image>().sprite = cakeSO.itemImage;
        panel.GetChild(cakeNameNum).GetComponent<Text>().text = cakeSO.name;
        panel.GetChild(cakeCountNum).GetComponent<Text>().text = $"보유 수 : {cakeManager.cakeCounts[index]}";

        bool isLocked = cakeSO.isLocked;
        panel.GetChild(lockedNum).gameObject.SetActive(isLocked);
        panel.GetComponent<Button>().interactable = !isLocked;

        Transform clickedPanel = panel.GetChild(clickedNum);
        clickedPanel.gameObject.SetActive(false);

        clickedPanel.GetChild(costNum).GetComponent<Text>().text = $"{cakeSO.cakeCost}";
        UpdateMaterialPanel(clickedPanel.GetChild(materialNum), cakeSO);
        clickedPanel.GetChild(bakeTimeNum).GetComponent<Text>().text = $"{cakeSO.bakeTime}초";
    }

    void UpdateMaterialPanel(Transform materialPanel, CakeSO cakeSO)
    {
        for (int j = 0; j < materialPanel.childCount; j++)
        {
            materialPanel.GetChild(j).gameObject.SetActive(false);
        }

        for (int j = 0; j < cakeSO.materialCounts.Length; j++)
        {
            var material = materialPanel.GetChild(j);
            material.gameObject.SetActive(true);
            material.GetChild(materialImgaeNum).GetComponent<Image>().sprite = null; // 재료 이미지 참조 (현재 참조 불가능)
            //material.GetChild(materialCountNum).GetComponent<Text>().text = $"{cakeSO.materialCount[j]}/{cakeManager.fruitContainer.fruitCount[cakeSO.materialType[j]]}";
        }
    }
}
