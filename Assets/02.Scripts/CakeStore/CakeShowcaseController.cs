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
    private Transform cakeShowcaseScrollViewContent;       // 스크롤 뷰의 콘텐츠
    private enum PlacePanelElements
    {
        Text = 0,
        Image = 1
    }
    private enum MenuPanelElements
    {
        Image = 0,
        Name = 1,
        Count = 2,
        Price = 3,
        Locked = 5
    }
    private int cakeShowcasePlaceIndex;        // 현재 선택된 케이크 쇼케이스 위치 인덱스
    private int cakeShowcaseIndex;             // 현재 선택된 케이크 쇼케이스 인덱스
    private int cakePlaceNum;
    private CakeManager cakeManager;           // 케이크 매니저 참조
    private CakeUIController cakeUIController;

    void Start()
    {
        InitializeCakeShowcaseController();
    }
    void InitializeCakeShowcaseController()
    {
        // 케이크 매니저 초기화
        cakeManager = CakeManager.instance;
        cakeUIController = CakeUIController.instance;
        
        cakeShowcaseMenu.SetActive(true);
        cakeShowcaseScrollViewContent = cakeShowcaseMenu.GetComponentInChildren<HorizontalLayoutGroup>().transform;

        // 케이크 쇼케이스 패널 비활성화
        Debug.Log(cakeShowcasePlace);
        Debug.Log(cakeShowcaseMenu);
        cakeUIController.CloseMenu(cakeShowcasePlace);
        cakeUIController.CloseMenu(cakeShowcaseMenu);
        cakePlaceNum = cakeManager.cakePlaceNum;
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
            cakeShowcases[i].GetComponent<CakeShowcase>().InitializeCakeShowcase();
            UpdateShowcaseUI(i);               // 쇼케이스 UI 업데이트
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
        for (int i = 0; i < cakeShowcaseScrollViewContent.childCount; i++)
        {
            int index = i; // 로컬 변수로 캡처
            cakeManager.SetupButton(cakeShowcaseScrollViewContent.GetChild(i).GetComponent<Button>(), () => OnMenuSelect(index));
        }
    }

    // 패널 열기 메서드
    public void OpenPanel(int index)
    {
        cakeUIController.DisableSprites(true);      // 스프라이트 비활성화
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
        cakeUIController.CloseMenu(cakeShowcaseMenu);
        UpdateShowcaseUI(cakeShowcaseIndex);               // 쇼케이스 UI 업데이트
    }
    // 주어진 케이크 인덱스에 해당하는 케이크 위치를 찾는 메서드
    public List<int> CakeFindIndex(int cakeIndex)
    {
        List<int> cakeShowcaseIndexes = new List<int>();
        // 모든 쇼케이스 및 위치를 순회하며 케이크 위치를 찾음
        for (int i = 0; i < cakeShowcases.Length; i++)
        {
            CakeShowcase cakeShowcase = cakeShowcases[i].GetComponent<CakeShowcase>();

            for (int j = 0; j < cakePlaceNum; j++)
            {
                if (cakeShowcase.cakeType[j] == cakeIndex)
                {
                    cakeShowcaseIndexes.Add(cakeShowcase.cakeShowcaseIndex);
                }
            }
        }
        if (cakeShowcaseIndexes.Count > 0)
        {
            return cakeShowcaseIndexes;
        }
        else
        {
            return null;
        }
    }
    public List<int> CakeFindPlace(int showcaseIndex, int cakeIndex)
    {
        List<int> cakeShowcasePlaceIndexes = new List<int>();
        CakeShowcase cakeShowcase = cakeShowcases[showcaseIndex].GetComponent<CakeShowcase>();
        for (int i = 0; i < cakePlaceNum; i++)
        {
            if (cakeShowcase.cakeType[i] == cakeIndex)
            {
                cakeShowcasePlaceIndexes.Add(i);
            }
        }
        if (cakeShowcasePlaceIndexes.Count > 0)
        {
            return cakeShowcasePlaceIndexes;
        }
        else
        {
            return null;
        }
    }
    // 케이크 판매 메서드
    public void CakeSell(int ShowcaseIndex, int ShowcasePlaceIndex)
    {
        CakeShowcase cakeShowcase = cakeShowcases[ShowcaseIndex].GetComponent<CakeShowcase>();
        cakeShowcase.isCakeSelected[ShowcasePlaceIndex] = false;
        UpdateShowcaseUI(ShowcaseIndex);
        Debug.Log("케이크를 선택하였습니다.");
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
            GameObject placeText = cakeShowcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Text).gameObject;
            GameObject placeImage = cakeShowcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Image).gameObject;
            CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();

            placeText.SetActive(!cakeShowcase.isCakeSelected[i]);
            placeImage.SetActive(cakeShowcase.isCakeSelected[i]);

            if (cakeShowcase.isCakeSelected[i])
            {
                Image placeButtonImage = placeImage.GetComponent<Image>();
                placeButtonImage.sprite = cakeManager.cakeSODataList[cakeShowcase.cakeType[i]].itemImage;
            }
        }
        // 케이크 쇼케이스 메뉴 패널 업데이트
        for (int i = 0; i < cakeShowcaseScrollViewContent.childCount; i++)
        {
            Transform cakeShowcasePanel = cakeShowcaseScrollViewContent.GetChild(i);
            var cakeSO = cakeManager.cakeSODataList[i];

            cakeShowcasePanel.GetChild((int)MenuPanelElements.Image).GetComponent<Image>().sprite = cakeSO.itemImage;
            cakeShowcasePanel.GetChild((int)MenuPanelElements.Count).GetComponent<Text>().text = "보유 수: " + cakeManager.cakeCounts[i];
            cakeShowcasePanel.GetChild((int)MenuPanelElements.Price).GetComponent<Text>().text = cakeSO.cakePrice.ToString();
            cakeShowcasePanel.GetChild((int)MenuPanelElements.Locked).gameObject.SetActive(cakeSO.isLocked);
            cakeShowcasePanel.GetComponent<Button>().interactable = !cakeSO.isLocked;
        }
    }
}
