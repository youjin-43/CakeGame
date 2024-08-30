using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [Serializable]
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        public List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10; // 인벤토리 사이즈

        [field: SerializeField]
        public int inventoryType; // 인벤토리 타입 0: 씨앗, 1: 과일, 2: 보석, 3: 케이크...


        // 인벤토리가 업데이트 됐는지 여부 확인 후 함수 호출하기 위함(매번 Update 함수에서 확인 안해도됨)..
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated; // Dictionary<int, InventoryItem> 타입의 매개변수를 받는 함수를 연결할 수 있음..
        public event Action<int> OnInventoryUpdatedInt, OnInventorySizeUpdated; // int 타입의 매개변수를 받는 함수를 연결할 수 있음..


        // 맨 처음 게임 시작시 호출하는 함수
        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();

            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public void AddItem(ItemSO item, int quantity)
        {
            // 만약 아이템의 수량을 쌓을 수 있으면..
            if (item.IsStackable)
            {
                for (int i=0; i<inventoryItems.Count; i++)
                {
                    // 아이템 칸이 비어있으면 그냥 지나가도록..
                    if (inventoryItems[i].IsEmpty) continue;

                    if (inventoryItems[i].item.ID == item.ID)
                    {
                        // 남아 있는 여분 공간
                        int remainingSpace = item.MaxStackSize - inventoryItems[i].quantity;
                    
                        // 남아 있는 여분 공간이 현재 더하려는 양보다 크거나 같으면
                        if (remainingSpace >= quantity)
                        {
                            inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                            return;
                        } 
                        // 남아 있는 여분 공간이 현재 더하려는 양보다 작으면
                        else
                        {
                            // 여분 공간을 모두 채워 준 후..
                            inventoryItems[i] = inventoryItems[i].ChangeQuantity(item.MaxStackSize);
                            quantity -= remainingSpace; // 남은 양 구하기ㅏ..
                        }
                    }
                }
            }

            // 인벤토리를 돌면서
            for (int i=0; i<inventoryItems.Count; i++)
            {
                // 비어있는 인벤토리 칸을 찾기..
                if (inventoryItems[i].IsEmpty)
                {
                    // 아이템이 스택 가능하고, 아이템의 양이 아이템의 맥스치보다 크면 
                    if (item.IsStackable && quantity > item.MaxStackSize)
                    {
                        // 새로운 아이템 칸을 만들고
                        inventoryItems[i] = new InventoryItem
                        {
                            item = item,
                            quantity = item.MaxStackSize
                        };
                        quantity -= item.MaxStackSize; // 남은 양 구하기..
                    } 
                    // 아이템의 종류가 스택 불가능 하거나, 아이템의 양이 아이템의 맥스치보다 작으면
                    else
                    {
                        // 새로운 아이템 칸을 만들고
                        inventoryItems[i] = new InventoryItem
                        {
                            item = item,
                            quantity = quantity // 양 그대로 넣어주기..
                        };
                        return;
                    }
                }
            }


            // 아이템을 저장할 공간이 부족하면..
            if (quantity > 0)
            {
                while (quantity > 0) {
                    SizeUpInventory(); // 인벤토리를 한칸 늘려주고..

                    // 아이템이 스택 가능하고, 아이템의 양이 아이템의 맥스치보다 크면 
                    if (item.IsStackable && quantity > item.MaxStackSize)
                    {
                        // 새로운 아이템 칸을 만들고
                        inventoryItems[inventoryItems.Count - 1] = new InventoryItem
                        {
                            item = item,
                            quantity = item.MaxStackSize
                        };
                        quantity -= item.MaxStackSize; // 남은 양 구하기..
                    }
                    // 아이템의 종류가 스택 불가능 하거나, 아이템의 양이 아이템의 맥스치보다 작으면
                    else
                    {
                        // 새로운 아이템 칸을 만들고
                        inventoryItems[inventoryItems.Count - 1] = new InventoryItem
                        {
                            item = item,
                            quantity = quantity // 양 그대로 넣어주기..
                        };
                        return;
                    }
                }
            }
        }

        public void SizeUpInventory()
        {
            // 인벤토리 사이즈 늘리고, 새로 만든 칸을 리스트에 넣어주고, 변경사항 알리는 함수 호출!!
            Size += 1;
            inventoryItems.Add(InventoryItem.GetEmptyItem());

            // 델리게이트에 연결되어 있는 함수 호출..
            // 인벤토리의 사이즈가 업데이트 될 때 실행되어야 하는 함수 연결되어있음..
            OnInventorySizeUpdated?.Invoke(Size); // 매개변수로 현재 인벤토리의 사이즈 보내주기..
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity);
            InformAboutChange(); // 아이템을 추가하면 인벤토리의 상태가 변경되므로 UI 상태 업데이트 해주기..
        }


        public void MinusItem(ItemSO item, int quantity)
        {
            int totalItemCount = 0;
            int minIdx = -1;
            int min = int.MaxValue;
            List<int> itemIdxTmp = new List<int>();

            // 스택 가능한 아이템인 경우
            if (item.IsStackable)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    if (inventoryItems[i].IsEmpty) continue; // 만약 비어있는 칸이면 그냥 넘기도록..

                    // 빼려고 하는 아이템의 아이디와 같다면..
                    if (inventoryItems[i].item.ID == item.ID)
                    {
                        // 총 아이템 카운트를 올려주기..
                        totalItemCount += inventoryItems[i].quantity;
                        itemIdxTmp.Add(i); // 아이템의 인덱스 넣어주기..

                        if (min > inventoryItems[i].quantity)
                        {
                            min = inventoryItems[i].quantity;
                            minIdx = i;
                        }
                    }
                }

                // 아이템의 총수량이 현재 빼려고 하는 아이템의 수량보다 크거나 같다면..
                if (totalItemCount >= quantity)
                {
                    if (minIdx != -1) // 가장 작은 수를 못 찾은 경우가 아니면..
                    {
                        if (inventoryItems[minIdx].quantity > quantity) // 제거해야할 아이템의 수량이, 해당 아이템의 최소 수량 인벤토리 칸의 수량보다 작다면..
                        {
                            // 인벤토리 아이템 수량 변경..
                            inventoryItems[minIdx] = inventoryItems[minIdx].ChangeQuantity(inventoryItems[minIdx].quantity - quantity);
                            return; // 빠져나가기..
                        }
                        else if (inventoryItems[minIdx].quantity == quantity) // 제거해야할 아이템의 수량이, 해당 아이템의 최소 수량 인벤토리 칸의 수량이랑 같다면..
                        {
                            // 인벤토리 아이템칸을 비어있는 아이템으로 변경..
                            inventoryItems[minIdx] = InventoryItem.GetEmptyItem();
                            return; // 빠져나가기..
                        }
                        else if (inventoryItems[minIdx].quantity < quantity) // 제거해야할 아이템의 수량이, 해당 아이템의 최소 수량 인벤토리 칸의 수량보다 크다면..
                        {
                            quantity -= inventoryItems[minIdx].quantity;
                            inventoryItems[minIdx] = InventoryItem.GetEmptyItem();
                        }
                    }

                    // 위 조건문 둘다 만족 안하면..
                    // 빼야 하는 수량이 0 이랑 같아질때까지 반복문 돌면서(뒤에 있는 아이템부터 없앨 것..)..
                    int idx = 0;
                    while (quantity > 0 && idx < itemIdxTmp.Count)
                    {
                        Debug.Log(itemIdxTmp.Count);
                        int temp = itemIdxTmp[itemIdxTmp.Count - 1 - idx]; // 해당 아이템의 인덱스를 모아놓은 리스트에서 요소 가져옴..
                        idx++;

                        // 최소 인덱스의 인벤토리 아이템은 이미 없앴으므로 넘기기..
                        if (temp == minIdx) continue; 

                        // 제거해야할 수량이 인벤토리 아이템의 수량보다 많거나 같을 때..
                        if (quantity >= inventoryItems[temp].quantity)
                        {
                            quantity -= inventoryItems[temp].quantity; // 뺄 수량을 인벤토리 아에팀 수량만큼 감소시킴..
                            inventoryItems[temp] = InventoryItem.GetEmptyItem(); // 아이템칸 비우기..
                        }
                        // 제거해야할 수량이 인벤토리 아이템의 수량보다 적을 때..
                        else
                        { 
                            inventoryItems[temp] = inventoryItems[temp].ChangeQuantity(inventoryItems[temp].quantity - quantity);
                            quantity = 0;
                        }
                    }
                }
                // 아이템의 총수량이 현재 빼려고 하는 아이템의 수량보다 작다면..
                else
                {
                    Debug.Log($"{item.Name}이 부족해!!!!!");
                    return; // 그냥 빠져나오도록..
                }
            }
            // 아이템이 스택가능한 아이템이 아닌 경우에는..
            else
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    // 빼려고 하는 아이템의 아이디와 같다면..
                    if (inventoryItems[i].item.ID == item.ID)
                    {
                        inventoryItems[i] = InventoryItem.GetEmptyItem(); // 인벤토리 칸을 비워주고..
                        return; // 빠져나가기..
                    }
                }

                // 여기까지 도달했으면 아이템을 못 찾은거니까 빠져나가기..
                Debug.Log($"{item.Name}이 없어요!!!!!");
                return; 
            }
        }

        public void MinusItem(InventoryItem item)
        {
            MinusItem(item.item, item.quantity);
            InformAboutChange();
        }

        public void MinusItemAt(int itemIdx, int quantity)
        {
            if (inventoryItems[itemIdx].quantity == quantity) // 해당 인덱스의 아이템의 양과 똑같은 양을 뺀다면 아이템칸이 빈 인벤토리 아이템을 가지도록..
                inventoryItems[itemIdx] = InventoryItem.GetEmptyItem();
            else // 해당 인덱스의 아이템의 양보다 적은 양을 뺀다면, 그 수만큼 반영해주기(해당 아이템 양보다 더 많은 양을 빼려고 하는 경우가 생기지 않도록 다른 클래스에서 조정할 것..)
                inventoryItems[itemIdx] = inventoryItems[itemIdx].ChangeQuantity(inventoryItems[itemIdx].quantity - quantity);

            InformAboutChange();
        }


        // 현재 인벤토리의 상태를 반환하는 함수..
        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

            // 현재 인벤토리 사이즈만큼 반복문을 돌면서 인벤토리 아이템이 비어있지 않은 것만 딕셔너리에 넣어줌..
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }


        // 특정 인덱스의 아이템을 가져올 것..
        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }


        // 아이템을 서로 바꿈!!
        public void SwapItems(int itemIndex1, int itemIndex2)
        {
            // InventoryItem 이 구조체이므로 값을 복사해서 저장할 수 있음(참조형은 값 복사 x).
            InventoryItem item1 = inventoryItems[itemIndex1];
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = item1; // item1 은 복사된 값이니까 문제없이 원하는 대로 기능함..
            InformAboutChange();
        }

        public void InformAboutChange()
        {
            // 델리게이트에 연결되어 있는 함수 호출..
            // UpdateInventoryUI 함수가 연결되어 있음..
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
            OnInventoryUpdatedInt?.Invoke(inventoryType);
        }
    }



    /*
    !!구조체의 장점!!
    간단하고 가벼움: 두 개의 필드만 가지고 있어 구조체로서 이상적이다..
    값 타입: 인스턴스를 복사하여 전달하므로 원본 데이터를 보호할 수 있다..
    불변성 유지: 변경 가능한 메서드(ChangeQuantity)는 새로운 인스턴스를 반환하므로 불변성을 유지한다..
    성능: 스택에 할당되어 가비지 컬렉션의 부하를 줄일 수 있다..
    */
    // struct 는 a - value 가 아니라 null 값 가질 수 없음
    [System.Serializable] // C#에서 클래스, 구조체 또는 열거형을 직렬화할 수 있도록 지정하는 데 사용
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;

        // 이 속성은 item이 null인지 여부를 확인하여 true 또는 false 값을 반환함..
        public bool IsEmpty => item == null; // 읽기 전용 속성..


        // 아이템의 수량만 바꾼 InventoryItem 을 새로 만들어서 반환함..
        // 아이템의 아이디는 계속 같아야 하므로 item 에 this.item 넣어준 것..
        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
            };
        }


        // 맨 처음 게임 시작할 때, 인벤토리에 빈 칸을 만들어놓기 위함..
        public static InventoryItem GetEmptyItem()

            => new InventoryItem
            {
                item = null,
                quantity = 0,
            };
    }
}