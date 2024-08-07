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
    public GameObject storeManager;
    
    public int cakeShowcaseIndex;
    private CakeShowcaseManager cakeShowcaseManager;
    void Start(){
        cakeShowcaseManager = storeManager.GetComponent<CakeShowcaseManager>();
        isCakeSelected = new bool[cakeShowcaseManager.cakePlaceNum];
        cakeType = new int[cakeShowcaseManager.cakePlaceNum];
        cakeImages = new GameObject[cakeShowcaseManager.cakePlaceNum];
        for (int i = 0; i < cakeShowcaseManager.cakePlaceNum; i++){
            isCakeSelected[i] = false;
            cakeType[i] = 0;
            cakeImages[i] = this.transform.GetChild(i).gameObject;
        }
        cakeShowcaseManager.SetCake(cakeShowcaseIndex);
    }
    void OnMouseDown()
    {
        cakeShowcaseManager.OpenPanel(cakeShowcaseIndex);
    }
}
