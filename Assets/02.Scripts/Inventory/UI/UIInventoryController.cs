using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIInventoryController : MonoBehaviour
{
    // 인벤토리 컨트롤러에서는 인벤토리 UI 와 실제 인벤토리 데이터의 정보를 이용해서 상호작용 하도록 함..
    [SerializeField]
    public UIInventoryPage inventoryUI; // 인벤토리 창

    [SerializeField]
    public InventorySO seedInventoryData; // 실제 인벤토리(씨앗 인벤토리)

    [SerializeField]
    public InventorySO fruitInventoryData; // 실제 인벤토리(과일 인벤토리)

    [SerializeField]
    public InventorySO curInventoryData; // 현재 보기로 선택한 인벤토리

    [SerializeField]
    public Button seedButton; // curInventoryData 의 값을 seedInventoryData 의 값으로 설정하는 버튼..

    [SerializeField]
    public Button fruitButton; // curInventoryData 의 값을 fruitInventoryData 의 값으로 설정하는 버튼..

    [SerializeField]
    public static UIInventoryController instance; // 싱글톤 이용하기 위한 변수..

    // 농장의 씨앗, 과일 컨테이너 게임 오브젝트 참조
    [Header("Farm GameObject")]
    [SerializeField]
    public SeedContainer seedContainer; // 이 값이 null 이면 현재 씬이 팜이 아닌 것(팜인지 아닌지 여부를 판단해야 인벤토리의 상태를 팜의 컨테이너에 반영할지 말지 선택할 수 있음)..
    [SerializeField]
    public FruitContainer fruitContainer; // 이 값이 null 이면 현재 씬이 팜이 아닌 것..

    [Header("Initial Inventory Items List")]
    [SerializeField]
    public List<InventoryItem> initialItems = new List<InventoryItem>(); // 처음 시작 인벤토리(이거 그냥 임시로 해놓음..)


    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("인벤토리 매니저가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 인벤토리가 삭제되지 않도록(인벤토리는 모든 씬에서 이용 가능해야 하기 때문에..)..
        }
        else
        {
            // instance에 이미 다른 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 인벤토리 매니저가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("인벤토리 매니저를 죽입니다");
        }

        PrepareUI();
        PrepareInventoryData();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                SetCurInventoryDataSeed(); // 인벤토리 창 켜질때는 씨앗을 기준으로 켜지도록..
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }

    public void AddItem(InventoryItem item)
    {
        switch (item.item.itemType)
        {
            case 0:
                // 씨앗
                seedInventoryData.AddItem(item);
                break;
            case 1:
                // 과일
                fruitInventoryData.AddItem(item);
                break;
            case 2:
                // 보석
                break;
            case 3:
                // 케이크
                break;
        }
    }

    public void MinusItem(InventoryItem item)
    {
        switch (item.item.itemType)
        {
            case 0:
                // 씨앗
                seedInventoryData.MinusItem(item);
                break;
            case 1:
                // 과일
                fruitInventoryData.MinusItem(item);
                break;
            case 2:
                // 보석
                break;
            case 3:
                // 케이크
                break;
        }
    }


    private void UpdateInventoryUI(Dictionary<int, InventoryItem> curInventory)
    {
        inventoryUI.ResetInventoryItems(); // 한 번 UI 리셋하고 시작..

        // 현재 인벤토리 상태를 매개변수로 받은 후, 그 상태에 맞게 UI 새로 업데이트 해주기
        foreach (var item in curInventory)
        {
            int index = item.Key;
            InventoryItem temp = item.Value;

            inventoryUI.inventoryUIItems[index].SetData(temp.item.itemImage, temp.quantity);
        }
    }

    private void SetInventoryToContainer(int itemType)
    {
        // 이 함수에서 농장씬의 씨앗, 과일 컨테이너에 인벤토리의 정보를 반영해줄 것임..
        // 만약 값이 null 이라면 현재 씬이 농장이 아닌 거니까 인벤토리의 정보를 반영해주는 함수를 호출하면 안됨(에러남)..
        // 그러니까 그냥 빠져나오도록..
        if (seedContainer == null || fruitContainer == null) return;


        // 현재 씬이 농장이면 여기로 도달함..
        // 여기서 정보 반영 함수 호출!!
        Dictionary<int, InventoryItem> curInventory;

        switch (itemType)
        {
            case 0:
                // 씨앗
                curInventory = seedInventoryData.GetCurrentInventoryState();

                // 컨테이너 한 번 리셋해주기..
                seedContainer.ResetContainer();
                seedContainer.SetContainer(curInventory);
                break;
            case 1:
                // 과일
                curInventory = fruitInventoryData.GetCurrentInventoryState();

                // 컨테이너 한 번 리셋해주기..
                fruitContainer.ResetContainer();
                fruitContainer.SetContainer(curInventory);
                break;
            case 2:
                // 보석
                break;
            case 3:
                // 케이크
                break;
        }
    }


    private void PrepareInventoryData()
    {
        // 인벤토리 각각 초기화해주기..
        seedInventoryData.Initialize();
        fruitInventoryData.Initialize();


        // 델리게이트에 UpdateInventoryUI 함수를 연결하기..
        // 인벤토리 데이터에 변경사항이 생기면 UpdateInventoryUI 함수를 호출할 수 있도록..
        seedInventoryData.OnInventoryUpdated += UpdateInventoryUI;
        fruitInventoryData.OnInventoryUpdated += UpdateInventoryUI;

        // 델리게이트에 SetInvenetoryToContainer 함수를 연결하기..
        // 인벤토리 데이터에 변경사항이 생기면 SetInvenetoryToContainer 함수를 호출할 수 있도록..
        seedInventoryData.OnInventoryUpdatedInt += SetInventoryToContainer;
        fruitInventoryData.OnInventoryUpdatedInt += SetInventoryToContainer;



        // 이건 게임 시작할 때 인벤토리에 아이템 몇 개 넣어놓을 때 사용하려고 일단 임시로 쓴 코드..
        // 아예 아무것도 안 준채로 시작할지 아니면 뭐 좀 주고 시작할지 고민..
        foreach (InventoryItem item in initialItems)
        {
            if (item.IsEmpty) continue;

            seedInventoryData.AddItem(item);
        }
    }

    private void PrepareUI()
    {
        curInventoryData = seedInventoryData; // 일단 처음 시작은 씨앗 인벤토리로..

        // 버튼에 함수 연결
        seedButton.onClick.AddListener(SetCurInventoryDataSeed); // 씨앗 버튼에 인벤토리 데이터를 씨앗 인벤토리 데이터로 바꿔주는 함수 연결
        fruitButton.onClick.AddListener(SetCurInventoryDataFruit); // 과일 버튼에 인벤토리 데이터를 과일 인벤토리 데이터로 바꿔주는 함수 연결


        inventoryUI.InitializeInventoryUI(curInventoryData.Size); // 씨앗 인벤토리 사이즈만큼 UI 초기화해주기
        inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnSwapItems += HandleSwapItems;
        inventoryUI.OnStartDragging += HandleDragging;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }


    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem;

        // 현재 인벤토리 데이터 변수가 가리키는 값이 씨앗 인벤토리 데이터 값이라면..
        if (curInventoryData == seedInventoryData)
        {
            inventoryItem = seedInventoryData.GetItemAt(itemIndex); // 전달받은 아이템의 인덱스로 인벤토리 아이템을 가져옴..

            if (inventoryItem.IsEmpty) // 만약 인벤토리 아이템이 비어있으면 디스크립션 초기화하고 빠져나가도록..
            {
                inventoryUI.ResetDescription();
                return;
            }

            ItemSO item = inventoryItem.item;
            inventoryUI.UpdateDescription(itemIndex, item.itemImage, item.Name, item.Description);
        }
        // 현재 인벤토리 데이터 변수가 가리키는 값이 과일 인벤토리 데이터 값이라면.. 
        else if (curInventoryData == fruitInventoryData)
        {
            inventoryItem = fruitInventoryData.GetItemAt(itemIndex); // 전달받은 아이템의 인덱스로 인벤토리 아이템을 가져옴..

            if (inventoryItem.IsEmpty) // 만약 인벤토리 아이템이 비어있으면 디스크립션 초기화하고 빠져나가도록..
            {
                inventoryUI.ResetDescription();
                return;
            }

            ItemSO item = inventoryItem.item;
            inventoryUI.UpdateDescription(itemIndex, item.itemImage, item.Name, item.Description);
        }
    }

    private void HandleSwapItems(int index1, int index2)
    {
        // 현재 인벤토리 데이터 변수가 가리키는 값이 씨앗 인벤토리 데이터 값이라면.. 
        if (curInventoryData == seedInventoryData)
        {
            seedInventoryData.SwapItems(index1, index2); // 씨앗 인벤토리 데이터의 SwapItems 함수를 호출함!
        }
        // 현재 인벤토리 데이터 변수가 가리키는 값이 과일 인벤토리 데이터 값이라면.. 
        else if (curInventoryData == fruitInventoryData)
        {
            fruitInventoryData.SwapItems(index1, index2); // 과일 인벤토리 데이터의 SwapItems 함수를 호출함!
        }
    }

    private void HandleDragging(int index)
    {

    }

    private void HandleItemActionRequest(int index)
    {

    }


    // 버튼 관련
    private void SetCurInventoryDataSeed()
    {
        // 현재 인벤토리 데이터 값을 Seed 인벤토리 데이터 값으로 설정하는 함수
        curInventoryData = seedInventoryData;
        inventoryUI.SetInventoryUI(curInventoryData.Size); // 인벤토리 UI 를 현재 보려고 선택한 인벤토리 데이터에 맞게 설정..
        curInventoryData.InformAboutChange();
    }

    private void SetCurInventoryDataFruit()
    {
        // 현재 인벤토리 데이터 값을 Fruit 인벤토리 데이터 값으로 설정하는 함수
        curInventoryData = fruitInventoryData;
        inventoryUI.SetInventoryUI(curInventoryData.Size); // 인벤토리 UI 를 현재 보려고 선택한 인벤토리 데이터에 맞게 설정..
        curInventoryData.InformAboutChange();
    }
}


