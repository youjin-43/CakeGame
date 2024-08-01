using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeShowcase : MonoBehaviour
{
    public GameObject[] cakeImages;       // 케이크 이미지를 담는 배열
    public bool[] isCakeSelected;         // 선택된 케이크를 추적하는 불린 배열
    public int[] cakeType;                // 각 케이크의 타입을 담는 배열
    public GameObject storeManager;       // 매장 관리자 객체에 대한 참조
    public int cakeShowcaseIndex;         // 현재 진열장의 인덱스

    private CakeShowcaseManager cakeShowcaseManager;

    void Start()
    {
        InitializeCakeShowcase();
    }

    private void InitializeCakeShowcase()
    {
        cakeShowcaseManager = storeManager.GetComponent<CakeShowcaseManager>();
        int cakePlaceNum = cakeShowcaseManager.cakePlaceNum;

        isCakeSelected = new bool[cakePlaceNum];
        cakeType = new int[cakePlaceNum];
        cakeImages = new GameObject[cakePlaceNum];

        for (int i = 0; i < cakePlaceNum; i++)
        {
            isCakeSelected[i] = false;
            cakeType[i] = 0;
            cakeImages[i] = this.transform.GetChild(i).gameObject;
        }

        cakeShowcaseManager.UpdateCakeShowcase(cakeShowcaseIndex);
    }

    void OnMouseDown()
    {
        cakeShowcaseManager.OpenPanel(cakeShowcaseIndex);
    }
}
