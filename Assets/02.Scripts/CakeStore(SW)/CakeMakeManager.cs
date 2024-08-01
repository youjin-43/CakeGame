using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeMakeManager : MonoBehaviour
{
    public CakeManager cakeManager;
    public GameObject cakeMakersPool;
    public GameObject cakeMakerPanel;
    public GameObject scrollViewContent; // ScrollView의 Content를 참조
    public Sprite[] timerSprites; // 4개의 타이머 스프라이트
    public Sprite completedSprite; // 제작 완료 스프라이트

    private GameObject[] cakeMakers;
    private int cakeMakerIndex;

    void Awake()
    {
        InitializeCakeMakers();
        SetupButtons();
    }


    void InitializeCakeMakers()
    {
        int childCount = cakeMakersPool.transform.childCount;
        cakeMakers = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            cakeMakers[i] = cakeMakersPool.transform.GetChild(i).gameObject;
            var cakeMaker = cakeMakers[i].GetComponent<CakeMaker>();
            cakeMaker.Initialize(i, this.gameObject, cakeMakerPanel, timerSprites, completedSprite);
        }
    }

    void SetupButtons()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 지역 변수를 사용
            var panel = scrollViewContent.transform.GetChild(i);
            SetupButton(panel.GetComponent<Button>(), () => OnCakeClicked(index));
            SetupButton(panel.GetChild(0).GetComponent<Button>(), () => OnMakeClicked(index));
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
        cakeManager.DisableSprites(true);
        cakeMakerPanel.SetActive(true);
        cakeMakerIndex = index;
        UpdateUI();
    }

    void OnCakeClicked(int index)
    {
        foreach (Transform panel in scrollViewContent.transform)
        {
            panel.GetChild(0).gameObject.SetActive(panel.GetSiblingIndex() == index);
        }
    }

    void OnMakeClicked(int index)
    {
        cakeMakerPanel.SetActive(false);
        cakeManager.DisableSprites(false);
        int cakeMakeTime = cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().GetCakeMakeTime();
        cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().StartMakingCake(index, cakeMakeTime);
    }

    public void CompleteCake(int index)
    {
        FindObjectOfType<CakeManager>().AddCake(index);
        UpdateUI();
    }

    public void OnTimerUIClicked(int index)
    {
        var cakeMaker = cakeMakers[index].GetComponent<CakeMaker>();
        if (cakeMaker.IsMakeComplete() && cakeMaker.TimerUIActive())
        {
            cakeMaker.CompleteCake();
        }
    }

    void UpdateUI()
    {
        var cakeManager = FindObjectOfType<CakeManager>();

        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(0).GetComponent<Text>().text = "보유 수: " + cakeManager.cakeCounts[i];
            panel.GetChild(1).gameObject.SetActive(cakeManager.cakeLocked[i]);
            panel.GetComponent<Button>().interactable = !cakeManager.cakeLocked[i];
            panel.GetChild(0).gameObject.SetActive(false);
        }

        FindObjectOfType<CakeShowcaseManager>().UpdateUI();
    }
}
