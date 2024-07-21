using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SeedFruitUIManager : MonoBehaviour
{
    [Header("Seed Info")]
    public FarmingManager farmingManager; // 구매하기 버튼이랑 farmingManager 에서 SeedContainer 의 GetSeed 함수랑 연동하기 위해서..
    public SeedContainer seedInfo;
    public Sprite[] seedImages; // 스프라이트는 미리 배열에 넣어놔야 사용할 수 있음..

    [Header("Buy Seed UI")]
    public GameObject buySeedPanel;
    public GameObject buySlotContainer; // 구매 버튼 슬롯 가지고 있는 게임 오브젝트
    public List<Button> buySeedSlots; // 씨앗 구매창의 슬롯들 저장

    [Header("Plant Seed UI")]
    public GameObject plantSlotContainer; // 씨앗 선택 버튼 슬롯 가지고 있는 게임 오브젝트
    public List<Button> plantSeedSlots; // 씨앗 선택창의 슬롯들 저장(씨앗 심기 버튼 눌렀을 때 씨앗 선택창 뜸)

    [Header("Sell Fruit UI")]
    public GameObject sellFruitPanel;
    public GameObject sellSlotContainer; // 과일 판매 버튼 슬롯 가지고 있는 게임 오브젝트
    public FruitContainer fruitInfo;
    public List<Button> sellFruitSlots; // 과일 판매창의 슬롯들 저장

    [Header("Current Button")]
    public Button selectSlot;

    private void Awake()
    {
        // 현재 씨앗 구매 판넬에 존재하는 슬롯들을 가져와서 저장함.
        // + 현재 씨앗 판매 판넬에 존재하는 슬롯들을 가져와서 저장함.
        // 자식만 가져와야 하기 때문에 (자손은 가져오면 안 됨) GetComponentsInChildren 못 씀.
        for (int i = 0; i < buySlotContainer.transform.childCount; i++)
        {
            // 씨앗 구매 판넬에 존재하는 슬롯 저장
            Transform child = buySlotContainer.transform.GetChild(i);
            buySeedSlots.Add(child.GetComponent<Button>());

            // 씨앗 판매 판넬에 존재하는 슬롯 저장
            child = sellSlotContainer.transform.GetChild(i);
            sellFruitSlots.Add(child.GetComponent<Button>());
        }

        // 현재 게임 상 존재하는 씨앗 구매 버튼 정보 설정
        for (int i = 0; i < buySeedSlots.Count; i++)
        {
            BuySeedSlotManager slot = buySeedSlots[i].GetComponent<BuySeedSlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            slot.slotImage.sprite = seedImages[i];
            slot.slotName.text = slotSeedInfo.seedName;
            slot.totalPrice.text = "가격: " + slotSeedInfo.seedPrice;
            slot.idx = slotSeedInfo.seedIdx;
            slot.countText.text = "1";
        }

        // 현재 게임 상 존재하는 과일 판매 버튼 정보 설정
        for (int i=0; i<sellFruitSlots.Count; i++)
        {
            SellFruitSlotManager slot = sellFruitSlots[i].GetComponent<SellFruitSlotManager>();
            Fruit slotFruitInfo = fruitInfo.prefabs[i].GetComponent<Fruit>();

            slot.slotImage.sprite = seedImages[i]; // 일단 과일 이미지도 씨앗 이미지랑 똑같이 해놓음..
            slot.slotName.text = slotFruitInfo.fruitName;
            slot.totalPrice.text = "가격: " + slotFruitInfo.fruitPrice;
            slot.idx = slotFruitInfo.fruitIdx;
            slot.countText.text = "1";
        }


        // 얘는 그냥 GetComponentsInChildren 써도 되긴 하는데 그냥 통일감 주려고..
        // 씨앗 선택 판넬에 존재하는 슬롯 저장
        for (int i = 0; i < plantSlotContainer.transform.childCount; i++)
        {
            Transform child = plantSlotContainer.transform.GetChild(i);
            plantSeedSlots.Add(child.GetComponent<Button>());
        }

        // 현재 게임 상 존재하는 씨앗 선택 버튼 정보 설정
        for (int i=0; i < plantSeedSlots.Count; i++)
        {
            PlantSeedSlotManager slot = plantSeedSlots[i].GetComponent<PlantSeedSlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            slot.seedImage.sprite = seedImages[i];
            slot.seedNameText.text = slotSeedInfo.seedName;
            slot.seedCountText.text = seedInfo.seedCount[i] + "";
            slot.seedIdx = slotSeedInfo.seedIdx;
        }
    }

    private void Update()
    {
        //// 임시로 W 키 누르면 구매창 켜지도록..
        //if (Input.GetKeyDown(KeyCode.W))
        //    buySeedPanel.SetActive(true);


        // 씨앗 구매 관련
        for (int i = 0; i < buySeedSlots.Count; i++)
        {
            BuySeedSlotManager slot = buySeedSlots[i].GetComponent<BuySeedSlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            // BuySlot 이 활성화 되어 있는 슬롯의 정보만 계속해서 변경해줄 것
            if (slot.openSlot.activeSelf)
            {
                // 선택된 씨앗 개수랑 총 가격만 계속해서 업데이트 해주면 됨.
                slot.countText.text = slot.curCount + "";
                slot.totalPrice.text = "가격: " + (int)(slot.curCount * slotSeedInfo.seedPrice);
            }
        }


        // 과일 판매 관련
        for (int i=0; i<sellFruitSlots.Count; i++)
        {
            SellFruitSlotManager slot = sellFruitSlots[i].GetComponent<SellFruitSlotManager>();
            Fruit slotFruitInfo = fruitInfo.prefabs[i].GetComponent<Fruit>();

            // SellSlot 이 활성화 되어 있는 슬롯의 정보만 계속해서 변경해줄 것
            if (slot.openSlot.activeSelf)
            {
                // 선택된 과일 개수랑 총 가격만 계속해서 업데이트 해주면 됨.
                slot.countText.text = slot.curCount + "";
                slot.totalPrice.text = "가격: " + (int)(slot.curCount * slotFruitInfo.fruitPrice);
            }
        }


        // 씨앗 선택 관련
        for (int i=0; i<plantSeedSlots.Count; i++)
        {
            // 씨앗의 개수만 계속해서 업데이트 해주면 됨..
            PlantSeedSlotManager slot = plantSeedSlots[i].GetComponent<PlantSeedSlotManager>();
            slot.seedCountText.text = farmingManager.seedContainer.seedCount[i] + "";
        }
    }

    public void CloseSlot()
    {
        for (int i=0; i<buySeedSlots.Count; i++)
        {
            SlotManager buySlot = buySeedSlots[i].GetComponent<BuySeedSlotManager>();
            buySlot.ResetData();
            buySlot.openSlot.SetActive(false);

            SlotManager sellSlot = sellFruitSlots[i].GetComponent<SellFruitSlotManager>();
            sellSlot.ResetData();
            sellSlot.openSlot.SetActive(false);
        }
    }


    public void SlotClick()
    {
        CloseSlot(); // 슬롯 버튼 눌렀을 때, 다른 슬롯의 구매 슬롯이 켜져있으면 다 끄고 시작..
    }


    public void ExitButton()
    {
        buySeedPanel.SetActive(false); // 구매 창 없어지도록..
        sellFruitPanel.SetActive(false); // 판매 창 없어지도록..

        CloseSlot(); // 나가기 버튼 누르면 켜져있던 구매 슬롯 없어지도록..
    }
}
