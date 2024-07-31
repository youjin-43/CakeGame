using System;
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
    void Awake(){
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
        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++){
            Button placeButton = cakeShowcasePlace.transform.GetChild(i).GetComponent<Button>();
        }
    }
    void IsClickedPlaceSelect(int index){
        cakeShowcaseMenu.SetActive(true);
        cakeShowcasePlaceIndex = index;
        cakeShowcasePlace.SetActive(false);
    }
    void IsClickedMenuSelect(int index){
        CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();
        CakeMakeManager cakeMakeManager =  gameObject.GetComponent<CakeMakeManager>();
        if(cakeMakeManager.cakeCounts[index] == 0){
            Debug.Log("케이크 수가 부족합니다.");
            return;
        }
        if(cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex])
        {
            Debug.Log("이미 케이크 존재");
            cakeMakeManager.cakeCounts[cakeShowcase.cakeType[cakeShowcasePlaceIndex]]++;
        }
        
        cakeShowcase.isCakeSelected[cakeShowcasePlaceIndex] = true;
        cakeShowcase.cakeType[cakeShowcasePlaceIndex] = index;
        cakeMakeManager.cakeCounts[index]--;
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

    public void SetCake(int index){
        CakeShowcase cakeShowcase = cakeShowcases[index].GetComponent<CakeShowcase>();
        for(int i = 0; i < cakePlaceNum; i++){
            cakeShowcase.cakeImages[i].SetActive(cakeShowcase.isCakeSelected[i]);
            cakeShowcase.cakeImages[i].GetComponent<SpriteRenderer>().sprite = cakeImages[cakeShowcase.cakeType[i]];
        }
    }
    public void CakeSell(/*int index, */int soldCakeNum){
        CakeShowcase cakeShowcase = cakeShowcases[0/*index*/].GetComponent<CakeShowcase>();
        cakeShowcase.isCakeSelected[soldCakeNum] = false;
        SetCake(0/*index*/);
        UpdateUI();
    }

    public void UpdateUI()
    {
        CakeMakeManager cakeMakeManager = gameObject.GetComponent<CakeMakeManager>();
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            int cakeCount = cakeMakeManager.cakeCounts[i];
            Transform panel = scrollViewContent.transform.GetChild(i);
            panel.GetChild(cakeCountNum).GetComponent<Text>().text = "보유 수: " + cakeCount;
            panel.GetChild(lockedNum).gameObject.SetActive(cakeMakeManager.cakeLocked[i]);
            panel.GetComponent<Button>().interactable = !cakeMakeManager.cakeLocked[i];
        }
        for (int i = 0; i < cakeShowcasePlace.transform.childCount; i++)
        {
            GameObject placeButton = cakeShowcasePlace.transform.GetChild(i).GetChild(placeButtonImageNum).gameObject;
            CakeShowcase cakeShowcase = cakeShowcases[cakeShowcaseIndex].GetComponent<CakeShowcase>();
            placeButton.SetActive(cakeShowcase.isCakeSelected[i]);
            Image placeButtonImage = placeButton.GetComponent<Image>();
            placeButtonImage.sprite = cakeImages[cakeShowcase.cakeType[i]];
        }
    }
}
