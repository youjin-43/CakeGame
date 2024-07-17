using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public Image seedImage;
    public Text seedName;
    public GameObject BuySlot;
    public Text totalPrice;
    public Text seedCountText;
    public Button leftButton;
    public Button rightButton;

    public int seedCount = 1;
    public int maxCount = 64;
    public int minCount = 1;

    public void minusSeedCount()
    {
        if (seedCount <= minCount) {
            // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최소 개수보다 작아지는 순간 최댓값으로 넘어가도록
            seedCount = maxCount;
            return; 
        } 

        seedCount--;
    }

    public void plusSeedCount()
    {
        // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최대 개수보다 커지는 순간 최솟값으로 넘어가도록
        if (seedCount >= maxCount) {
            seedCount = minCount;
            return;
        }

        seedCount++;
    }

    private void Update()
    {
        // UI 가 아닌 부분을 클릭하면 그냥 꺼지도록..
        if (IsPointerOverUIObject()) return; 
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuySlot.SetActive(false);
            }
        }
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    public void ResetData()
    {
        seedCount = 1;
    }
}
