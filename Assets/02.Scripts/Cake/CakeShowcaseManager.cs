using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeShowcaseManager : MonoBehaviour
{
    public GameObject cakeShowcasePool;
    public GameObject cakeShowcasePlace;
    private int cakeShowcasePlaceIndex;
    public GameObject cakeShowcaseMenu;
    public GameObject[] cakeShowcases;
    private int cakeShowcaseIndex;
    public Sprite[] cakeImages;

    public GameObject scrollViewContent;
    public int cakeCountNum;
    public int lockedNum;
    public int placeButtonImageNum;
    public int cakePlaceNum = 4;

    private CakeManager cakeManager;

    void Awake()
    {
        cakeManager = gameObject.GetComponent<CakeManager>();
        cakeShowcaseMenu.SetActive(false);
        cakeShowcasePlace.SetActive(false);
        cakeShowcases = new GameObject[cakeShowcasePool.transform.childCount];

        int childCount = scrollViewContent.transform.childCount;

        for (int i = 0; i < cakeShowcasePool.transform.childCount; i++)
        {
            cakeShowcases[i] = cakeShowcasePool.transform.GetChild(i).gameObject;
            cakeShowcases[i].GetComponent<CakeShowcase>().storeManager = this.gameObject;
            cakeShowcases[i].GetComponent<CakeShowcase>().cakeShowcaseIndex = i;
        }
        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++)
        {
            int index = i; // 로컬 변수로 캡처
            Button placeButton = cakeShowcasePlace.transform.GetChild(i).GetComponent<Button>();
            placeButton.onClick.AddListener(() => IsClickedPlaceSelect(index));
        }
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int index = i; // 로컬 변수로 캡처
            Button menuButton = scrollViewContent.transform.GetChild(i).GetComponent<Button>();
            menuButton.onClick.AddListener(() => IsClickedMenuSelect(index));
        }
    }

    void IsClickedPlaceSelect(int index)
    {
        cakeShowcaseMenu.SetActive(true);
        cakeShowcasePlaceIndex = index;
        cakeShowcasePlace.SetActive(false);
    }

    void IsClickedMenuSelect(int index)
    {
        CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();
        if (cakeManager.cakeDataList[index].cakeCount == 0)
        {
            Debug.Log("케이크 수가 부족합니다.");
            return;
        }
        if (cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex])
        {
            Debug.Log("이미 케이크 존재");
            cakeManager.IncreaseCakeCount(cakeShowcase.cakeType[cakeShowcasePlaceIndex]);
        }

        cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex] = true;
        cakeShowcase.cakeType[cakeShowcasePlaceIndex] = index;
        cakeManager.DecreaseCakeCount(index);
        SetCake(cakeShowcaseIndex);

        gameObject.GetComponent<CakeMakeManager>().DisableSprites(false);
        cakeShowcaseMenu.SetActive(false);

        UpdateUI();
    }

    public void OpenPanel(int index)
    {
        gameObject.GetComponent<CakeMakeManager>().DisableSprites(true);
        cakeShowcasePlace.SetActive(true);
        cakeShowcaseIndex = index;
        UpdateUI();
    }

    public void SetCake(int index)
    {
        CakeShowcase cakeShowcase = cakeShowcases[index].GetComponent<CakeShowcase>();
        for (int i = 0; i < cakePlaceNum; i++)
        {
            cakeShowcase.cakeImages[i].SetActive(cakeShowcase.isCakeSelected[i]);
            cakeShowcase.cakeImages[i].GetComponent<SpriteRenderer>().sprite = cakeManager.cakeDataList[cakeShowcase.cakeType[i]].cakeImage;
        }
    }

    public void CakeSell(int soldCakeNum)
    {
        CakeShowcase cakeShowcase = cakeShowcases[0].GetComponent<CakeShowcase>();
        cakeShowcase.isCakeSelected[soldCakeNum] = false;
        SetCake(0);
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int cakeCount = cakeManager.cakeDataList[i].cakeCount;
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(cakeCountNum).GetComponent<Text>().text = "보유 수: " + cakeCount;
            panel.GetChild(lockedNum).gameObject.SetActive(cakeManager.cakeDataList[i].isLocked);
            panel.GetComponent<Button>().interactable = !cakeManager.cakeDataList[i].isLocked;
        }
        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++)
        {
            GameObject placeButton = cakeShowcasePlace.transform.GetChild(i).GetChild(placeButtonImageNum).gameObject;
            CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();
            placeButton.SetActive(cakeShowcase.isCakeSelected[i]);
            Image placeButtonImage = placeButton.GetComponent<Image>();
            placeButtonImage.sprite = cakeManager.cakeDataList[cakeShowcase.cakeType[i]].cakeImage;
        }
    }
}
