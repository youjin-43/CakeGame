using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CakeShowcase : MonoBehaviour
{
    public GameObject[] cakeImages;
    public bool[] isCakeSelected;
    public int[] cakeType;
    public int cakeShowcaseIndex;
    void Start(){
        InitializeCakeMaker();
    }
    void InitializeCakeMaker()
    {
        isCakeSelected = new bool[CakeManager.instance.cakePlaceNum];
        cakeType = new int[CakeManager.instance.cakePlaceNum];
        cakeImages = new GameObject[CakeManager.instance.cakePlaceNum];
        for (int i = 0; i < CakeManager.instance.cakePlaceNum; i++){
            isCakeSelected[i] = false;
            cakeType[i] = -1;
            cakeImages[i] = transform.GetChild(i).gameObject;
        }
    }
    void OnMouseDown()
    {
        CakeManager.instance.cakeShowcaseController.OpenPanel(cakeShowcaseIndex);
    }
    public void GetBack()
    {
        for(int i = 0;i<cakeType.Length; i++)
        {
            if(isCakeSelected[i])
            {
                CakeManager.instance.cakeCounts[cakeType[i]]++;
                isCakeSelected[i] = false;

            }
        }
    }
}