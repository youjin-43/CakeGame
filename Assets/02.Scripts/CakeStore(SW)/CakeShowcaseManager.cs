using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeShowcaseManager : MonoBehaviour
{
    public GameObject cakeShowcasePool;   // 케이크 진열장 객체 풀
    public GameObject cakeShowcasePlace;  // 케이크 배치 UI 요소
    public GameObject cakeShowcaseMenu;   // 케이크 선택 메뉴
    public GameObject scrollViewContent;  // 케이크 선택을 위한 스크롤뷰 컨텐츠
    public int cakePlaceNum = 4;          // 각 진열장 내 위치 수

    private GameObject[] cakeShowcases;    // 모든 케이크 진열장을 담는 배열
    private int currentShowcaseIndex;      // 현재 진열장 인덱스
    private int currentPlaceIndex;         // 진열장 내 현재 위치 인덱스
    private CakeManager cakeManager;

    void Awake()
    {
        cakeManager = FindObjectOfType<CakeManager>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        cakeShowcaseMenu.SetActive(false);
        cakeShowcasePlace.SetActive(false);

        InitializeCakeShowcases();
        InitializePlaceButtons();
        InitializeMenuButtons();
    }

    private void InitializeCakeShowcases()
    {
        int childCount = cakeShowcasePool.transform.childCount;
        cakeShowcases = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            cakeShowcases[i] = cakeShowcasePool.transform.GetChild(i).gameObject;
            var showcase = cakeShowcases[i].GetComponent<CakeShowcase>();
            showcase.cakeShowcaseIndex = i;
        }
    }

    private void InitializePlaceButtons()
    {
        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++)
        {
            int index = i;
            Button placeButton = cakeShowcasePlace.transform.GetChild(i).GetComponent<Button>();
            placeButton.onClick.AddListener(() => OnPlaceButtonClick(index));
        }
    }

    private void InitializeMenuButtons()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int index = i;
            Button menuButton = scrollViewContent.transform.GetChild(i).GetComponent<Button>();
            menuButton.onClick.AddListener(() => OnMenuButtonClick(index));
        }
    }

    private void OnPlaceButtonClick(int index)
    {
        currentPlaceIndex = index;
        cakeShowcaseMenu.SetActive(true);
        cakeShowcasePlace.SetActive(false);
    }

    private void OnMenuButtonClick(int index)
    {
        var cakeShowcase = cakeShowcases[currentShowcaseIndex].GetComponent<CakeShowcase>();

        if (!cakeManager.CanMakeCake(index))
        {
            Debug.Log("케이크 수가 부족하거나 잠겨 있습니다.");
            return;
        }

        if (cakeShowcase.isCakeSelected[currentPlaceIndex])
        {
            cakeManager.AddCake(cakeShowcase.cakeType[currentPlaceIndex]);
        }

        cakeShowcase.isCakeSelected[currentPlaceIndex] = true;
        cakeShowcase.cakeType[currentPlaceIndex] = index;
        cakeManager.RemoveCake(index);

        UpdateCakeShowcase(currentShowcaseIndex);
        cakeShowcaseMenu.SetActive(false);

        UpdateUI();
    }

    public void OpenPanel(int index)
    {
        currentShowcaseIndex = index;
        cakeShowcasePlace.SetActive(true);
        UpdateUI();
    }

    public void UpdateCakeShowcase(int index)
    {
        var cakeShowcase = cakeShowcases[index].GetComponent<CakeShowcase>();

        for (int i = 0; i < cakePlaceNum; i++)
        {
            cakeShowcase.cakeImages[i].SetActive(cakeShowcase.isCakeSelected[i]);
            cakeShowcase.cakeImages[i].GetComponent<SpriteRenderer>().sprite = cakeManager.cakeImages[cakeShowcase.cakeType[i]];
        }
    }

    public void SellCake(int soldCakeNum)
    {
        var cakeShowcase = cakeShowcases[0].GetComponent<CakeShowcase>();
        cakeShowcase.isCakeSelected[soldCakeNum] = false;
        UpdateCakeShowcase(0);
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int cakeCount = cakeManager.cakeCounts[i];
            var panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(0).GetComponent<Text>().text = "보유 수: " + cakeCount;
            panel.GetChild(1).gameObject.SetActive(cakeManager.cakeLocked[i]);
            panel.GetComponent<Button>().interactable = !cakeManager.cakeLocked[i];
        }

        var currentShowcase = cakeShowcases[currentShowcaseIndex].GetComponent<CakeShowcase>();

        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++)
        {
            var placeButton = cakeShowcasePlace.transform.GetChild(i).GetChild(0).gameObject;
            placeButton.SetActive(currentShowcase.isCakeSelected[i]);
            var placeButtonImage = placeButton.GetComponent<Image>();
            placeButtonImage.sprite = cakeManager.cakeImages[currentShowcase.cakeType[i]];
        }
    }
}
