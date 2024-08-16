using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedContainer : MonoBehaviour
{
    public GameObject[] prefabs; // 씨앗 프리팹을 저장해놓기 위한 배열
    public int[] seedCount; // 게임 상에서 어떤 씨앗이 몇 개 있는지 저장할 배열

    private void Awake()
    {
        // [0]:사과, [1]:바나나, [2]:체리, [3]:오렌지, [4]:딸기
        seedCount = new int[prefabs.Length]; // 프리팹의 개수만큼 배열 크기 지정
    }


    public void ResetContainer()
    {
        // 모든 요소의 값으르 0으로 리셋해주기..
        for (int i = 0; i < seedCount.Length; i++)
        {
            seedCount[i] = 0;
        }
    }

    public void SetContainer(Dictionary<int, InventoryItem> curInventory)
    {
        // curInventory 에서 키값은, 인벤토리 속에서 해당 아이템의 인덱스 번호임
        // 현재 인벤토리의 내용을 가져올 때 비어있는 아이템 칸은 제외하고 가져옴.
        // 즉, 인벤토리 속 비어있는 아이템 칸이 있다면 가져온 아이템 딕셔너리의 내용은 [0]: 사과, [2]: 바나나, [5]: 오렌지 이럴 가능성이 있음
        // 그래서 key 가 1, 2, 3, 4, 5... 이런식으로 순차적으로 온다는 보장이 없으므로 그냥 키값들을 가져와서 반복문 도는 것..
        foreach (int idx in curInventory.Keys)
        {
            seedCount[((SeedItemSO)(curInventory[idx].item)).seedIdx] += curInventory[idx].quantity; // 해당 아이템의 아이템 인덱스에 맞는 요소의 값을 증가시켜줌..
        }
    }
}
