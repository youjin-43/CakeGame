using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuySeedSlotManager : SlotManager
{
    private void Start()
    {
        farmingManager = FindObjectOfType<FarmingManager>();
        interactionButton.onClick.AddListener(() => farmingManager.BuySeed(1, idx)); // 일단 초기 함수 연결 해놓기
    }

    private void Update()
    {
        // 부모의 IsPointerOverUIObject 함수 쓸 것..
        if (IsPointerOverUIObject()) return;
        else
        {
            // UI 가 아닌 부분을 클릭하면 그냥 꺼지도록..
            if (Input.GetMouseButtonDown(0))
            {
                openSlot.SetActive(false); // 슬롯의 뒷면 꺼지도록..
            }
        }
    }

    public override void minusCount()
    {
        if (curCount <= minCount)
        {
            // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최소 개수보다 작아지는 순간 최댓값으로 넘어가도록
            curCount = maxCount + 1;
        }

        curCount--;

        interactionButton.onClick.RemoveAllListeners(); // 현재 선택 씨앗 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        interactionButton.onClick.AddListener(() => farmingManager.BuySeed(curCount, idx));
    }

    public override void plusCount()
    {
        // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최대 개수보다 커지는 순간 최솟값으로 넘어가도록
        if (curCount >= maxCount)
        {
            curCount = minCount - 1;
        }

        curCount++;

        interactionButton.onClick.RemoveAllListeners(); // 현재 선택 씨앗 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        interactionButton.onClick.AddListener(() => farmingManager.BuySeed(curCount, idx));
    }
}