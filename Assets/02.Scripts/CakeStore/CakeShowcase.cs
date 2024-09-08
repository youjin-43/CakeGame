using Inventory.Model;
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
    public void InitializeCakeShowcase()
    {
        isCakeSelected = new bool[CakeManager.instance.CAKEPLACENUM];
        cakeType = new int[CakeManager.instance.CAKEPLACENUM];
        cakeImages = new GameObject[CakeManager.instance.CAKEPLACENUM];
        for (int i = 0; i < CakeManager.instance.CAKEPLACENUM; i++)
        {
            isCakeSelected[i] = false;
            cakeType[i] = -1;
            cakeImages[i] = transform.GetChild(i).gameObject;
        }
    }
    void OnMouseDown()
    {
        if (!CakeManager.instance.IsPointerOverUIObjectMobile(Input.touches[0]))
        {
            CakeManager.instance.cakeShowcaseController.OpenPanel(cakeShowcaseIndex);
            CakeManager.instance.CallOpenAudio();
        }
    }
    public void GetBack()
    {
        for (int i = 0; i < cakeType.Length; i++)
        {
            if (isCakeSelected[i])
            {
                InventoryItem tempItem = new InventoryItem()
                {
                    item = UIInventoryManager.instance.cakeItems[cakeType[i]],
                    quantity = 1,
                };
                UIInventoryManager.instance.AddItem(tempItem);
                isCakeSelected[i] = false;
                cakeType[i] = -1;
            }
        }
    }
}