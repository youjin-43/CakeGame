using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    public UIInventoryItem inventoryItemPrefab;
    [SerializeField]
    public GameObject inventoryItemParent; // 새로 생성된 인벤토리 아이템 UI 가 위치할 부모(스크롤뷰의 content 가 부모가 될 것..)
    [SerializeField]
    public int initialInventorySize = 10; // 인벤토리 사이즈
    [SerializeField]
    public List<UIInventoryItem> inventoryUIItems; // 아이템 UI 들을 담아놓을 리스트


    [SerializeField]
    public UIMouseDragItem mouseDragItem;
    [SerializeField]
    public int currentMouseDragIndex = -1; // 현재 마우스로 드래그 하고 있는 아이템의 인덱스(아무것도 드래그 안 할때는 -1 로..)..


    [SerializeField]
    public UIInventoryDescription inventoryDescription;


    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;

    public event Action<int, int> OnSwapItems;


    private void Awake()
    {
        mouseDragItem.gameObject.SetActive(false); // 활성화 끈채로 시작..
        transform.gameObject.SetActive(false); // 창 끈채로 시작..
    }

    private void DeselectAllItems() { 
        // 모든 아이템의 경계 이미지를 끄는 함수..

        foreach (UIInventoryItem item in inventoryUIItems)
        {
            item.Deselect();
        }
    }

    public void UpdateDescription(int itemIndex, Sprite itemImage, string itemName, string itemDescription)
    {
        // 클릭한 아이템의 정보로 Description 값 업데이트 해주기...
        inventoryDescription.SetDescription(itemImage, itemName, itemDescription);

        DeselectAllItems();
        inventoryUIItems[itemIndex].Select(); // 현재 디스크립션 대상 아이템의 경계 이미지 켜주기..
    }

    public void ResetDescription()
    {
        inventoryDescription.ResetDescription();
    }


    public void ResetInventoryItems()
    {
        foreach (UIInventoryItem item in inventoryUIItems)
        {
            item.ResetData();
        }
    }


    public void InitializeInventoryUI(int inventorySize)
    {
        inventoryUIItems = new List<UIInventoryItem>(initialInventorySize);

        for (int i=0; i<inventorySize; i++)
        {
            UIInventoryItem item = Instantiate(inventoryItemPrefab, Vector2.zero, Quaternion.identity);
            item.Initialize(); // 아이템 초기화..
            item.transform.SetParent(inventoryItemParent.transform); // 부모 지정해주기
            inventoryUIItems.Add(item);

            // 델리게이트에 함수 연결해주기!!
            inventoryUIItems[i].OnItemClicked += HandleItemClicked; // 아이템을 클릭했을 때 로직
            inventoryUIItems[i].OnItemDroppedOn += HandleItemDroppedOn; // 아이템을 놓았을 때 로직
            inventoryUIItems[i].OnItemBeginDrag += HandleItemBeginDrag; // 아이템 드래그를 시작했을 때 로직
            inventoryUIItems[i].OnItemEndDrag += HandleItemEndDrag; // 아이템 드래그를 종료했을 때 로직
        }
    }

    public void SetInventoryUI(int inventorySize)
    {
        int curInventorySize = inventoryUIItems.Count; // 현재 인벤토리 페이지의 아이템칸 사이즈를 가져옴..


        if (curInventorySize <= inventorySize) // 새로 설정하려는 인벤토리 사이즈가 현재 인벤토리 사이즈보다 크거나 같다면..
        {
            for (int i = 0; i < curInventorySize; i++)
            {
                inventoryUIItems[i].gameObject.SetActive(true); // 새로 설정하려는 사이즈보다 크거나 같을 때, 일단 가지고 있는 인벤토리칸 다 켜기..
            }

            for (int i = 0; i < inventorySize - curInventorySize; i++) // 부족한 아이템 칸만큼 새로 생성해주기 위한 반복문..
            {
                UIInventoryItem item = Instantiate(inventoryItemPrefab, Vector2.zero, Quaternion.identity);
                item.Initialize(); // 아이템 초기화..
                item.transform.SetParent(inventoryItemParent.transform); // 부모 지정해주기
                inventoryUIItems.Add(item);

                // 델리게이트에 함수 연결해주기!!
                inventoryUIItems[i].OnItemClicked += HandleItemClicked; // 아이템을 클릭했을 때 로직
                inventoryUIItems[i].OnItemDroppedOn += HandleItemDroppedOn; // 아이템을 놓았을 때 로직
                inventoryUIItems[i].OnItemBeginDrag += HandleItemBeginDrag; // 아이템 드래그를 시작했을 때 로직
                inventoryUIItems[i].OnItemEndDrag += HandleItemEndDrag; // 아이템 드래그를 종료했을 때 로직
            }
        }
        else if (curInventorySize > inventorySize) //새로 설정하려는 인벤토리 사이즈가 현재 인벤토리 사이즈보다 작다면..
        {
            for (int i = 0; i < curInventorySize-inventorySize; i++) // 그냥 불필요한 아이템칸 수만큼 활성화 꺼주면 됨..
            {
                // 뒤에서부터 차례대로 활성화 꺼주기..
                inventoryUIItems[curInventorySize - i - 1].gameObject.SetActive(false);
            }
        }
    }


    private void HandleItemEndDrag(UIInventoryItem item)
    {
        mouseDragItem.gameObject.SetActive(false); // 마우스 드래그 아이템 활성화 꺼주기..
    }

    private void HandleItemBeginDrag(UIInventoryItem item)
    {
        CloseBorderImage(); // 다른 경계 다 꺼주고 시작..

        currentMouseDragIndex = inventoryUIItems.IndexOf(item); // 매개변수로 전달받은 UIInventoryItem 의 인스턴스가 리스트 속 몇 번 째 인덱스 요소인지 가져옴..

        // 마우스 드래그 아이템의 상태를 현재 드래그 하려고 하는 아이템의 상태로 업데이트..
        mouseDragItem.item.itemImage.sprite = inventoryUIItems[currentMouseDragIndex].itemImage.sprite;
        mouseDragItem.item.quantityText.text = item.quantityText.text;
        //mouseDragItem.item.quantityText.text = "1"; // 일단 임시로 1..
        mouseDragItem.gameObject.SetActive(true); // 마우스 드래그 아이템 활성화 켜주기..

        HandleItemClicked(item); // 아이템 클릭했을 때 효과를 드래그 시작할 때도 적용되도록..
    }


    // 이 매개변수로는 아이템을 드롭한 곳의 아이템칸이 들어옴
    private void HandleItemDroppedOn(UIInventoryItem item)
    {
        int index = inventoryUIItems.IndexOf(item);


        if (inventoryUIItems[currentMouseDragIndex].empty) return; // 만약 드래그 시작한 아이템이 비어있는 칸이면 그냥 빠져나가도록..

        OnSwapItems?.Invoke(currentMouseDragIndex, index); // 아이템 swap 함수 호출

        HandleItemClicked(item); // 아이템 클릭했을 때 효과가 드래그 끝난 후에도 적용되도록..
    }


    // 이 함수로 매개변수가 전달되는 때는, UIInventoryItem 인스턴스가 이 함수가 연결되어 있는 델리게이트를 호출할 때!
    private void HandleItemClicked(UIInventoryItem item)
    {
        CloseBorderImage(); // 다른 경계 다 꺼주고 시작..

        if (item.empty)
        {
            // 클릭한 아이템 칸이 비어있으면 경계도 끄고, 설명도 끄고..
            inventoryDescription.ResetDescription(); // 설명 초기화..
            return; // 밑에 로직 수행 안하도록 그냥 나가버리기..
        }

        int index = inventoryUIItems.IndexOf(item); // 매개변수로 전달받은 UIInventoryItem 의 인스턴스가 리스트 속 몇 번 째 인덱스 요소인지 가져옴..

        if (index == -1) return;
        
        inventoryUIItems[index].itemBorderImage.enabled = true; // 경계 활성화 키기..

        // 아이템이 클릭되었으면 이제 설명 띄워줘야 함.
        OnDescriptionRequested?.Invoke(index);
    }


    private void CloseBorderImage()
    {
        foreach (UIInventoryItem item in inventoryUIItems)
        {
            item.itemBorderImage.enabled = false; // 경계 활성화 끄기..
        } 
    }

    public void Show()
    {
        transform.gameObject.SetActive(true);
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
    }
}
