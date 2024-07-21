using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// SellFruitSlotManager 랑 BuySeedSlotManager 랑 겹치는 부분이 많아서 그냥 다형성 이용하는 게 좋을 것 같다.
// 그래서 따로 SlotManager 로 이름지음..
public class SlotManager : MonoBehaviour
{
    [Header("FarmingManager")]
    public FarmingManager farmingManager;

    [Header("Slot Button UI")]
    public Image slotImage;
    public Text slotName;
    public GameObject openSlot; // 슬롯 누르면 슬롯의 뒷모습 보이도록..
    public Text totalPrice;
    public Text countText;
    public Button leftButton;
    public Button rightButton;
    public Button interactionButton; // 구매하기, 판매하기 버튼으로 사용할 것..

    [Header("Slot Imformation")]
    public int prevCount = 1; // 이전 카운트
    public int curCount = 1; // 현재 카운트
    public int maxCount; // 이건 하위 클래스에 따라 달라짐. SellFruitSlotManager 에서는 현재 과일 보유량에따라 달라지고, BuySeedSlotManager 에서는 64 로 고정..
    public int minCount = 1;
    public int idx; // 해당 슬롯의 (과일||씨앗) 인덱스


    protected bool IsPointerOverUIObject()
    {
        Touch touch = Input.GetTouch(0);

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            // Input.mousePosition 도 모바일에서 작동하니까 이런 간단한 거에서 사용해도 괜찮을 것 같습니다..
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    public virtual void minusCount() { }
    public virtual void plusCount() { }
    public void ResetData()
    {
        curCount = 1;
    }
}
