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
    private CakeShowcaseController cakeShowcaseController;
    void Start(){
        
        cakeShowcaseController = CakeManager.instance.GetComponent<CakeShowcaseController>();
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
        CakeManager.instance.GetComponent<CakeShowcaseController>().OpenPanel(cakeShowcaseIndex);
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
