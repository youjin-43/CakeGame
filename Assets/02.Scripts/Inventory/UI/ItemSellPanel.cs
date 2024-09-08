using Inventory.Model;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSellPanel : MonoBehaviour
{
    [SerializeField]
    private Button minusButton;

    [SerializeField]
    private Button plusButton;

    [SerializeField]
    private Button sellButton;

    [SerializeField]
    private Text curItemCountText;

    [SerializeField]
    private Text totalPriceText;

    [SerializeField]
    private int curItemCount = 1;

    [SerializeField]
    private int minItemCount = 1;

    [SerializeField]
    private int maxItemCount;

    [SerializeField]
    private int totalPrice = 0;

    [SerializeField]
    int itemPrice = 0; // 현재 아이템의 아이템 가격 저장용 변수..

    [SerializeField]
    private InventoryItem curItem;


    [SerializeField]
    // UIInventoryController 의 SellItem 함수 연결..
    public event Action<int, int, int> sellButtonClicked; // 아이템 수량이랑 가격, 아이템 타입을 매개변수로 받는 함수를 연결할 것..


    private void Awake()
    {
        // 각 버튼에 함수 연결해주기..
        minusButton.onClick.AddListener(MinusItemCount);
        plusButton.onClick.AddListener(PlusItemCount);
        sellButton.onClick.AddListener(SellItem);

        // 판매 수량은 1에서 적어질 수 없으므로 일단 현재 개수가 1인 상태를 가격에 업데이트 하고 시작..
        UpdateTotalPrice(curItemCount);
    }

    public void SetItemInfo(InventoryItem item)
    {
        curItem = item;

        // 플러스 버튼을 눌러서 최대한으로 올라갈 수 있는 한계치를 현재 선택한 아이템의 개수로 설정
        maxItemCount = item.quantity;

        curItemCount = 1;

        // 현재 아이템 가격 결정용..
        switch (curItem.item.itemType)
        {
            case 1:
                // 과일
                itemPrice = ((FruitItemSO)curItem.item).fruitPrice;
                break;
            case 2:
                // 보석
                break;
            case 3:
                // 케이크
                break;
        }

        UpdateTotalPrice(curItemCount);
    }

    private void SellItem()
    {
        // 델리게이트에 연결된 함수 호출..
        sellButtonClicked?.Invoke(curItemCount, totalPrice, curItem.item.itemType);
    }

    private void UpdateTotalPrice(int itemCount)
    {
        // 총 가격을 현재 아이템 개수만큼 가격이랑 곱해서 구하기..
        totalPrice = itemCount * itemPrice;

        curItemCountText.text = curItemCount + ""; // 여기서 갯수 텍스토도 변경..
        totalPriceText.text = totalPrice + ""; // 총 가격만큼 텍스트도 변경..
    }

    private void MinusItemCount()
    {
        // 마이너스 버튼 눌렀을 때 현재 아이템 개수가 최소치보다 작거나 같다면 최대값으로 바꿔주기..
        if (curItemCount <= minItemCount)
        {
            curItemCount = maxItemCount;
            UpdateTotalPrice(curItemCount);
            return; // 빠져나가기..
        }
        curItemCount--;
        UpdateTotalPrice(curItemCount);
    }

    private void PlusItemCount()
    {
        // 플러스 버튼 눌렀을 때 현재 아이템 개수가 최대치보다 크거나 같다면 최소값으로 바꿔주기..
        if (curItemCount >= maxItemCount)
        {
            curItemCount = minItemCount;
            UpdateTotalPrice(curItemCount);
            return; // 빠져나가기..
        }
        curItemCount++;
        UpdateTotalPrice(curItemCount);
    }
}
