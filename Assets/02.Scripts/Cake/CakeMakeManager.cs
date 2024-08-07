using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeMakeManager : MonoBehaviour
{
    public GameObject cakeMakersPool;
    public GameObject cakeMakerPanel;
    public GameObject scrollViewContent;
    public GameObject[] cakeMakers;
    public int cakeImageNum;
    public int cakeNameNum;
    public int cakeCountNum;
    public int lockedNum;
    public int clickedNum;
    public int costNum;
    public int materialNum;
    public int bakeTimeNum;
    public int buttonNum;
    public int cakeMakerIndex;

    public GameObject timers;
    public Sprite[] timerSprites;
    public Sprite completedSprite;

    private CakeManager cakeManager;

    void Awake()
    {
        cakeMakerPanel.SetActive(false);
        cakeManager = gameObject.GetComponent<CakeManager>();
        InitializeCakeMakers();
        SetupButtons();
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

    void SetupButtons()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int index = i;
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
        cakeManager.DisableSprites(true);
        cakeMakerPanel.SetActive(true);
        cakeMakerIndex = index;
        UpdateUI();
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
        cakeManager.DisableSprites(false);
        int cakeMakeTime = cakeManager.cakeDataList[index].bakeTime;
        cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().StartMakingCake(index, cakeMakeTime);
    }

    public void CompleteCake(int index)
    {
        cakeManager.IncreaseCakeCount(index);
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

    public void Unlock(int index)
    {
        cakeManager.UnlockCake(index);
        UpdateUI();
    }
    void UpdateUI()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(cakeImageNum).GetComponent<Image>().sprite = cakeManager.cakeDataList[i].itemImage;
            panel.GetChild(cakeNameNum).GetComponent<Text>().text = cakeManager.cakeDataList[i].name;
            panel.GetChild(cakeCountNum).GetComponent<Text>().text = $"보유 수 : {cakeManager.cakeCounts[i]}";
            panel.GetChild(lockedNum).gameObject.SetActive(cakeManager.cakeDataList[i].isLocked);
            panel.GetComponent<Button>().interactable = !cakeManager.cakeDataList[i].isLocked;
            panel.GetChild(clickedNum).gameObject.SetActive(false);
            panel.GetChild(clickedNum).GetChild(costNum).GetComponent<Text>().text = $"{cakeManager.cakeDataList[i].cakeCost}";
            for(int j = 1; j < panel.GetChild(clickedNum).GetChild(materialNum).childCount; j++) panel.GetChild(clickedNum).GetChild(materialNum).GetChild(j).gameObject.SetActive(false);
            for(int j = 1; j < cakeManager.cakeDataList[i].materialCount.Length; j++){
                panel.GetChild(clickedNum).GetChild(materialNum).GetChild(j).gameObject.SetActive(true);
                panel.GetChild(clickedNum).GetChild(materialNum).GetChild(j).GetChild(materialImgaeNum).GetComponent<Image>.sprite = null/*매터리얼 타입의 인덱스를 가진 과일 이미지*/;
                material +=  $", {cakeManager.cakeDataList[i].materialType[j]/*이번호에 해당되는 과일 이름으로 대체 예정*/} {cakeManager.cakeDataList[i].materialCount[j]}/{0/*현재 보유중인 과일 수로 변경 예정*/}";
            }
            panel.GetChild(clickedNum).GetChild(materialNum).GetChild(i).GetComponent<Text>().text = material;
            panel.GetChild(clickedNum).GetChild(materialNum).GetComponent<Text>().text = material;
            panel.GetChild(clickedNum).GetChild(bakeTimeNum).GetComponent<Text>().text = $"{cakeManager.cakeDataList[i].bakeTime}초" ;
        }

        gameObject.GetComponent<CakeShowcaseManager>().UpdateUI();
    }
}
