using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeMakeManager : MonoBehaviour
{
    public GameObject cakeMakersPool;
    public GameObject cakeMakerPanel;
    public GameObject scrollViewContent; // ScrollView의 Content를 참조
    private GameObject[] cakeMakers;
    private int[] cakeCounts; // 각 케이크의 보유 수를 저장하는 배열
    private bool[] cakeLocked; // 각 케이크가 잠겨 있는지 여부를 저장하는 배열
    public int cakeCountNum;
    public int clickedNum;
    public int lockedNum;
    public int buttonNum;
    public int cakeMakerIndex;
    int unlock = 0;

    public GameObject timers;
    public Sprite[] timerSprites; // 4개의 타이머 스프라이트
    public Sprite completedSprite; // 제작 완료 스프라이트

    void Awake()
    {
        InitializeCakeMakers();
        InitializeCakeStatus();
        SetupButtons();
        UpdateUI();
    }

    void InitializeCakeMakers()
    {
        cakeMakers = new GameObject[cakeMakersPool.transform.childCount];
        for (int i = 0; i < cakeMakers.Length; i++)
        {
            cakeMakers[i] = cakeMakersPool.transform.GetChild(i).gameObject;
            var cakeMaker = cakeMakers[i].GetComponent<CakeMaker>();
            cakeMaker.timerUI = timers.transform.GetChild(i).gameObject;
            cakeMaker.timerSprites = timerSprites;
            cakeMaker.completedSprite = completedSprite;
            cakeMaker.cakeMakerIndex = i;
            cakeMaker.cakeMakerPanel = cakeMakerPanel;
            cakeMaker.storeManager = this.gameObject;
            
            GameObject timerUI = cakeMaker.timerUI;
            Button timerButton = timerUI.GetComponent<Button>();
            if (timerButton == null)
            {
                timerButton = timerUI.AddComponent<Button>();
            }
            int index = i;
            timerButton.onClick.AddListener(() => OnTimerUIClicked(index));
        }
    }

    void InitializeCakeStatus()
    {
        int childCount = scrollViewContent.transform.childCount;
        cakeCounts = new int[childCount];
        cakeLocked = new bool[childCount];

        cakeLocked[0] = false;
        for (int i = 1; i < childCount; i++)
        {
            cakeLocked[i] = true;
        }
    }

    void SetupButtons()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 지역 변수를 사용
            var panel = scrollViewContent.transform.GetChild(i);
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
        cakeMakerPanel.SetActive(true);
        cakeMakerIndex = index;
    }

    void OnCakeClicked(int index)
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(clickedNum).gameObject.SetActive(i == index);
        }
    }

    void OnMakeClicked(int index)
    {
        cakeMakerPanel.SetActive(false);
        int cakeMakeTime = cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().GetCakeMakeTime();
        cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().StartMakingCake(index, cakeMakeTime);
    }

    public void CompleteCake(int index)
    {
        cakeCounts[index]++;
        Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
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

    public void Unlock()
    {
        if (unlock + 1 < cakeLocked.Length && cakeLocked[unlock + 1])
        {
            cakeLocked[unlock + 1] = false;
            unlock++;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(cakeCountNum).GetComponent<Text>().text = "보유 수: " + cakeCounts[i];
            panel.GetChild(lockedNum).gameObject.SetActive(cakeLocked[i]);
            panel.GetComponent<Button>().interactable = !cakeLocked[i];
            panel.GetChild(clickedNum).gameObject.SetActive(false);
        }
    }
}
