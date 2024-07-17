using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuySeedUIManager : MonoBehaviour
{
    [Header("Seed Info")]
    public FarmingManager farmingManager; // �����ϱ� ��ư�̶� farmingManager ���� SeedContainer �� GetSeed �Լ��� �����ϱ� ���ؼ�..
    public SeedContainer seedInfo;
    public Sprite[] seedImages; // ��������Ʈ�� �̸� �迭�� �־���� ����� �� ����..

    [Header("Buy Seed UI")]
    public GameObject BuySeedPanel;
    public GameObject slotContainer;
    public List<Button> BuySeedSlots;

    [Header("Current Button")]
    public Button selectSlot;

    private void Awake()
    {
        // ���� ���� ���� �ǳڿ� �����ϴ� ���Ե��� �����ͼ� ������.
        // �ڽĸ� �����;� �ϱ� ������ (�ڼ��� �������� �� ��) GetComponentsInChildren �� ��.
        for (int i=0; i< slotContainer.transform.childCount; i++)
        {
            Transform child = slotContainer.transform.GetChild(i);
            BuySeedSlots.Add(child.GetComponent<Button>());
        }


        //BuySeedSlots = BuySeedPanel.GetComponentsInChildren<Button>().Skip(1).ToArray();
        
        // ���� ���� �� �����ϴ� ���� ���� ��ư ���� ����
        for (int i=0; i<BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            // �� ��ư�� �ʱⰪ ����
            slot.seedImage.sprite = seedImages[i];
            slot.seedName.text = slotSeedInfo.seedName;
            slot.totalPrice.text = "����: " + slotSeedInfo.seedPrice;
            slot.seedCountText.text = "1";
        }
    }

    private void Update()
    {
        // �ӽ÷� W Ű ������ ����â ��������..
        if (Input.GetKeyDown(KeyCode.W))
            BuySeedPanel.SetActive(true);


        for (int i=0; i<BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            Seed slotSeedInfo = seedInfo.prefabs[i].GetComponent<Seed>();

            // BuySlot �� Ȱ��ȭ �Ǿ� �ִ� ������ ������ ����ؼ� �������� ��
            if (slot.BuySlot.activeSelf)
            {
                // ���õ� ���� ������ �� ���ݸ� ����ؼ� ������Ʈ ���ָ� ��.
                slot.seedCountText.text = slot.seedCount + "";
                slot.totalPrice.text = "����: " + (int)(slot.seedCount * slotSeedInfo.seedPrice);
            }
        }
    }

    public void CloseBuySlot()
    {
        for (int i = 0; i < BuySeedSlots.Count; i++)
        {
            SlotManager slot = BuySeedSlots[i].GetComponent<SlotManager>();
            slot.ResetData(); // ���� ������ �ѹ� �������ֱ�(���� �����µ� ���� �״�θ� �̻��ϴϱ�)
            slot.BuySlot.SetActive(false);
        }
    }

    public void SlotClick()
    {
        CloseBuySlot(); // ���� ��ư ������ ��, �ٸ� ������ ���� ������ ���������� �� ���� ����..
    }

    
    public void ExitButton()
    {
        BuySeedPanel.SetActive(false); // ���� â ����������..

        CloseBuySlot(); // ������ ��ư ������ �����ִ� ���� ���� ����������..
    }
}
