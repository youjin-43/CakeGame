using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuySeedUIManager : MonoBehaviour
{
    [Header("Seed Info")]
    public FarmingManager farmingManager; // 구매하기 버튼이랑 farmingManager 에서 SeedContainer 의 GetSeed 함수랑 연동하기 위해서..
    public SeedContainer seedInfo;
    public Sprite[] seedImages; // 스프라이트는 미리 배열에 넣어놔야 사용할 수 있음..

    [Header("Buy Seed UI")]
    public GameObject BuySeedPanel;
    public GameObject slotContainer;
    public List<Button> BuySeedSlots;

    [Header("Current Button")]
    public Button selectSlot;

    private void Awake()
    {
        // 현재 씨앗 구매 판넬에 존재하는 슬롯들을 가져와서 저장함.
        // 자식만 가져와야 하기 때문에 (자손은 가져오면 안 됨) GetComponentsInChildren 못 씀.
        for (int i=0; i< slotContainer.transform.childCount; i++)
        {
            Transform child = slotContainer.transform.GetChild(i);
            BuySeedSlots.Add(child.GetComponent<Button>());
        }


        //BuySeedSlots = BuySeedPanel.GetComponentsInChildren<Button>().Skip(1).ToArray();
        
        // 현재 게임 상 존재하는 씨앗 구매 버튼 정보 설정
        for (int i=0; i<BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            // 각 버튼의 초기값 설정
            slot.seedImage.sprite = seedImages[i];
            slot.seedName.text = slotSeedInfo.seedName;
            slot.totalPrice.text = "가격: " + slotSeedInfo.seedPrice;
            slot.seedCountText.text = "1";
        }
    }

    private void Update()
    {
        // 임시로 W 키 누르면 구매창 켜지도록..
        if (Input.GetKeyDown(KeyCode.W))
            BuySeedPanel.SetActive(true);


        for (int i=0; i<BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            // BuySlot 이 활성화 되어 있는 슬롯의 정보만 계속해서 변경해줄 것
            if (slot.BuySlot.activeSelf)
            {
                // 선택된 과일 개수랑 총 가격만 계속해서 업데이트 해주면 됨.
                slot.seedCountText.text = slot.seedCount + "";
                slot.totalPrice.text = "가격: " + (int)(slot.seedCount * slotSeedInfo.seedPrice);
            }
        }
    }

    public void CloseBuySlot()
    {
        for (int i = 0; i < BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            slot.ResetData(); // 슬롯 데이터 한번 리셋해주기(껐다 켜졌는데 상태 그대로면 이상하니까)
            slot.BuySlot.SetActive(false);
        }
    }

    public void SlotClick()
    {
        CloseBuySlot(); // 슬롯 버튼 눌렀을 때, 다른 슬롯의 구매 슬롯이 켜져있으면 다 끄고 시작..
    }

    
    public void ExitButton()
    {
        BuySeedPanel.SetActive(false); // 구매 창 없어지도록..

        CloseBuySlot(); // 나가기 버튼 누르면 켜져있던 구매 슬롯 없어지도록..
    }
}
