using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CakeShowcaseController : MonoBehaviour
{
    // 필드 선언
    public Transform cakeShowcasePool;        // 케이크 쇼케이스 풀을 담는 게임 오브젝트
    public GameObject[] cakeShowcases;         // 케이크 쇼케이스 배열
    public GameObject cakeShowcasePlace;       // 케이크 쇼케이스 위치를 담는 게임 오브젝트
    public GameObject cakeShowcaseMenu;        // 케이크 쇼케이스 메뉴 오브젝트
    public Transform CakeShowcaseScrollViewContent;       // 스크롤 뷰의 콘텐츠

    // 인덱스 및 기타 변수
    public int cakeImageNum = 0;               // 케이크 수량 UI 요소의 인덱스
    public int cakeNameNum = 1;
    public int cakeCountNum = 2;               // 케이크 수량 UI 요소의 인덱스
    public int cakePriceNum = 3;
    public int lockedNum = 5;                  // 잠금 상태 UI 요소의 인덱스
    public int placeButtonNum = 1;             // 위치 버튼의 인덱스
    public int placeButtonTextNum = 0;         // 위치 버튼 텍스트의 인덱스
    public int placeButtonImageNum = 0;        // 위치 버튼 이미지의 인덱스
    public int cakePlaceNum = 4;               // 케이크 쇼케이스의 위치 수

    private int cakeShowcasePlaceIndex;        // 현재 선택된 케이크 쇼케이스 위치 인덱스
    private int cakeShowcaseIndex;             // 현재 선택된 케이크 쇼케이스 인덱스

    private CakeManager cakeManager;           // 케이크 매니저 참조

    void Start()
    {
        InitializeCakeShowcaseController();
    }
    void InitializeCakeShowcaseController()
    {   // 케이크 매니저 초기화
        cakeManager = CakeManager.instance;
        // 케이크 쇼케이스 패널 비활성화
        cakeManager.CloseMenu(cakeShowcasePlace);
        cakeManager.CloseMenu(cakeShowcaseMenu);
        // 케이크 쇼케이스 및 버튼 초기화
        InitializeShowcases();
        SetUpButtons();
    }
    // 케이크 쇼케이스 초기화
    private void InitializeShowcases()
    {
        cakeShowcaseMenu.SetActive(false);     // 케이크 쇼케이스 메뉴 비활성화
        cakeShowcasePlace.SetActive(false);    // 케이크 쇼케이스 위치 비활성화
        cakeShowcases = new GameObject[cakeShowcasePool.childCount]; // 쇼케이스 배열 초기화

        // 각 쇼케이스에 대한 설정
        for (int i = 0; i < cakeShowcasePool.childCount; i++)
        {
            cakeShowcases[i] = cakeShowcasePool.GetChild(i).gameObject;
            cakeShowcases[i].GetComponent<CakeShowcase>().cakeShowcaseIndex = i;
        }
    }

    // 버튼 초기화 (위치 선택 및 메뉴 선택 버튼)
    private void SetUpButtons()
    {
        // 케이크 쇼케이스 위치 선택 버튼 설정
        for (int i = 0; i < cakePlaceNum; i++)
        {
            int index = i; // 로컬 변수로 캡처
            cakeManager.SetupButton(cakeShowcasePlace.transform.GetChild(i).GetComponent<Button>(), () => OnPlaceSelect(index));
        }

        // 메뉴 선택 버튼 설정
        for (int i = 0; i < CakeShowcaseScrollViewContent.childCount; i++)
        {
            int index = i; // 로컬 변수로 캡처
            cakeManager.SetupButton(CakeShowcaseScrollViewContent.GetChild(i).GetComponent<Button>(), () => OnMenuSelect(index));
        }
    }

    // 패널 열기 메서드
    public void OpenPanel(int index)
    {
        cakeManager.DisableSprites(true);      // 스프라이트 비활성화
        cakeShowcasePlace.SetActive(true);     // 케이크 쇼케이스 위치 활성화
        cakeShowcaseIndex = index;             // 선택된 쇼케이스 인덱스 저장
        UpdateUI();                            // UI 업데이트

    }

    // 쇼케이스에 배치할 위치 선택 시 호출되는 메서드
    private void OnPlaceSelect(int index)
    {
        cakeShowcasePlaceIndex = index;        // 선택된 위치 인덱스 저장
        cakeShowcaseMenu.SetActive(true);      // 케이크 쇼케이스 메뉴 활성화
        cakeShowcasePlace.SetActive(false);    // 케이크 쇼케이스 위치 비활성화
    }

    // 쇼케이스에 전시할 메뉴 선택 시 호출되는 메서드
    private void OnMenuSelect(int index)
    {
        CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();

        // 선택한 케이크가 부족한 경우
        if (cakeManager.cakeCounts[index] == 0)
        {
            Debug.Log("케이크 수가 부족합니다.");
            return;
        }

        // 선택한 위치에 이미 케이크가 존재하는 경우
        if (cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex])
        {
            cakeManager.PlusCakeCount(cakeShowcase.cakeType[cakeShowcasePlaceIndex]);
        }

        // 케이크 선택 상태 업데이트
        cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex] = true;
        cakeShowcase.cakeType[cakeShowcasePlaceIndex] = index;
        cakeManager.MinusCakeCount(index);
        cakeManager.CloseMenu(cakeShowcaseMenu);
        UpdateShowcaseUI(index);               // 쇼케이스 UI 업데이트
    }
    // 주어진 케이크 인덱스에 해당하는 케이크 위치를 찾는 메서드
    public string[] CakeFind(int cakeIndex)
    {
        List<string> cakeIndexes = new List<string>();

        // 모든 쇼케이스 및 위치를 순회하며 케이크 위치를 찾음
        for (int i = 0; i < cakeShowcases.Length; i++)
        {
            CakeShowcase cakeShowcase = cakeShowcases[i].GetComponent<CakeShowcase>();

            for (int j = 0; j < cakePlaceNum; j++)
            {
                if (cakeShowcase.cakeType[j] == cakeIndex)
                {
                    cakeIndexes.Add($"{i},{j}");
                }
            }
        }

        return cakeIndexes.ToArray();
    }

    // 케이크 판매 메서드
    public void CakeSell(int cakeIndex)
    {
        string[] foundCakeIndex = CakeFind(cakeIndex);

        // 케이크 위치가 없을 경우 처리
        if (foundCakeIndex.Length == 0)
        {
            Debug.LogWarning("해당 케이크를 찾을 수 없습니다.");
            return;
        }

        int r = UnityEngine.Random.Range(0, foundCakeIndex.Length);
        string[] locateIndex = foundCakeIndex[r].Split(',');

        int showcaseIndex = int.Parse(locateIndex[0]);
        int showcasePlaceIndex = int.Parse(locateIndex[1]);

        GameManager.instance.money += cakeManager.cakeSODataList[cakeIndex].cakePrice;

        CakeShowcase cakeShowcase = cakeShowcases[showcaseIndex].GetComponent<CakeShowcase>();
        cakeShowcase.isCakeSelected[showcasePlaceIndex] = false;
        UpdateShowcaseUI(showcaseIndex);               // 쇼케이스 UI 업데이트
    }
    
    // 특정 쇼케이스 UI를 업데이트하는 메서드
    public void UpdateShowcaseUI(int index)
    {
        CakeShowcase cakeShowcase = cakeShowcases[index].GetComponent<CakeShowcase>();

        // 쇼케이스 내 각 위치의 케이크 이미지 상태를 업데이트
        for (int i = 0; i < cakePlaceNum; i++)
        {
            bool isCakeSelected = cakeShowcase.isCakeSelected[i];
            cakeShowcase.cakeImages[i].SetActive(isCakeSelected);

            if (isCakeSelected)
            {
                cakeShowcase.cakeImages[i].GetComponent<SpriteRenderer>().sprite =
                    cakeManager.cakeSODataList[cakeShowcase.cakeType[i]].itemImage;
            }
        }
    }



    // 전체 UI를 업데이트하는 메서드
    public void UpdateUI()
    { 
        // 케이크 쇼케이스 위치 패널 업데이트
        for (int i = 0; i < cakePlaceNum; i++)
        {
            GameObject placeButton = cakeShowcasePlace.transform.GetChild(i).GetChild(placeButtonNum).gameObject;
            GameObject placeText = cakeShowcasePlace.transform.GetChild(i).GetChild(placeButtonTextNum).gameObject;
            CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();

            placeButton.SetActive(cakeShowcase.isCakeSelected[i]);
            placeText.SetActive(!cakeShowcase.isCakeSelected[i]);

            if (cakeShowcase.isCakeSelected[i])
            {
                Image placeButtonImage = placeButton.transform.GetChild(placeButtonImageNum).GetComponent<Image>();
                placeButtonImage.sprite = cakeManager.cakeSODataList[cakeShowcase.cakeType[i]].itemImage;
            }
        }
        // 케이크 쇼케이스 메뉴 패널 업데이트
        for (int i = 0; i < CakeShowcaseScrollViewContent.childCount; i++)
        {
            Transform cakeShowcasePanel = CakeShowcaseScrollViewContent.GetChild(i);
            var cakeSO = cakeManager.cakeSODataList[i];

            cakeShowcasePanel.GetChild(cakeImageNum).GetComponent<Image>().sprite = cakeSO.itemImage;
            cakeShowcasePanel.GetChild(cakeCountNum).GetComponent<Text>().text = "보유 수: " + cakeManager.cakeCounts[i];
            cakeShowcasePanel.GetChild(lockedNum).gameObject.SetActive(cakeSO.isLocked);
            cakeShowcasePanel.GetComponent<Button>().interactable = !cakeSO.isLocked;
        }
    }
}
