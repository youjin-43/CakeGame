using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellFruitSlotManager : SlotManager
{
    private void Start()
    {
        farmingManager = FindObjectOfType<FarmingManager>();
        interactionButton.onClick.AddListener(() => farmingManager.SellSeed(1, idx)); // 일단 초기 함수 연결 해놓기
        maxCount = farmingManager.fruitContainer.fruitCount[idx]; // 해당 슬롯 인덱스에 맞는 현재 과일의 수를 maxCount 에 저장함.
    }

    private void Update()
    {
        maxCount = farmingManager.fruitContainer.fruitCount[idx]; // 현재 과일 개수로 계속 업데이트 해주기..

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
            // 현재 구매하려고 하는 과일 개수가 과일 구매 최소 개수보다 작아지는 순간 최댓값으로 넘어가도록
            curCount = maxCount + 1;
        }

        curCount--;
        if (curCount == 0) curCount = 1;

        interactionButton.onClick.RemoveAllListeners(); // 현재 선택 과일 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        interactionButton.onClick.AddListener(() => farmingManager.SellSeed(curCount, idx));
    }

    public override void plusCount()
    {
        // 현재 구매하려고 하는 과일 개수가 과일 구매 최대 개수보다 커지는 순간 최솟값으로 넘어가도록
        // 구매 최대 개수보다 커지는 순간 최솟값으로 넘어가도록
        if (curCount >= maxCount)
        {
            curCount = minCount - 1;
        }

        curCount++;

        interactionButton.onClick.RemoveAllListeners(); // 현재 선택 과일 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        interactionButton.onClick.AddListener(() => farmingManager.SellSeed(curCount, idx));
    }
}
