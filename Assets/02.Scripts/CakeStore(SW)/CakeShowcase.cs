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
    private CakeShowcaseController cakeShowcaseController;
    void Start(){
        cakeShowcaseController = storeManager.GetComponent<CakeShowcaseController>();
        isCakeSelected = new bool[cakeShowcaseController.cakePlaceNum];
        cakeType = new int[cakeShowcaseController.cakePlaceNum];
        cakeImages = new GameObject[cakeShowcaseController.cakePlaceNum];
        for (int i = 0; i < cakeShowcaseController.cakePlaceNum; i++){
            isCakeSelected[i] = false;
            cakeType[i] = 0;
            cakeImages[i] = this.transform.GetChild(i).gameObject;
        }
        cakeShowcaseController.UpdateShowcaseUI(cakeShowcaseIndex);
    }
    void OnMouseDown()
    {
        cakeShowcaseController.OpenPanel(cakeShowcaseIndex);
    }
}
