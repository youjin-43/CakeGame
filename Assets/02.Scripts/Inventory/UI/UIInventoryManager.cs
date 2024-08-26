using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;
using UnityEditor.Experimental.RestService;



// 데이터 저장 클래스
[Serializable]
public class InventoryData
{
    public InventorySO seedInventoryData;
    public InventorySO fruitInventoryData;
}



public class UIInventoryManager : MonoBehaviour
{
    // 인벤토리 컨트롤러에서는 인벤토리 UI 와 실제 인벤토리 데이터의 정보를 이용해서 상호작용 하도록 함..
    [SerializeField]
    public UIInventoryPage inventoryUI; // 인벤토리 창

    [SerializeField]
    public InventorySO seedInventoryData; // 실제 인벤토리(씨앗 인벤토리)

    [SerializeField]
    public InventorySO fruitInventoryData; // 실제 인벤토리(과일 인벤토리)

    [SerializeField]
    public InventorySO cakeInventoryData; // 실제 인벤토리(케이크 인벤토리)

    [SerializeField]
    public InventorySO curInventoryData; // 현재 보기로 선택한 인벤토리

    [SerializeField]
    public Button seedButton; // curInventoryData 의 값을 seedInventoryData 의 값으로 설정하는 버튼..

    [SerializeField]
    public Button fruitButton; // curInventoryData 의 값을 fruitInventoryData 의 값으로 설정하는 버튼..

    [SerializeField]
    public Button cakeButton; // curInventoryData 의 값을 cakeInventoryData 의 값으로 설정하는 버튼..

    [SerializeField]
    public Button inventoryOpenButton; // 인벤토리를 켜고 닫는 버튼..

    [SerializeField]
    public static UIInventoryManager instance; // 싱글톤 이용하기 위한 변수..



    // 과일의 개수를 이용하기 위해서 과일의 개수를 저장할 배열 선언
    [Header("Fruit Count")]
    public int[] fruitCount;
    // 아이템 스크립터블 오브젝트를 저장해놓기..
    public FruitItemSO[] fruitItems; // [0]: 사과, [1]: 바나나, [2]: 체리, [3]: 오렌지, [4]: 딸기
    // 이건 아이템 잘 추가되나 보려고 임시롤 만든 배열...
    public CakeSO[] cakeItems; // 케이크임요!!



    // 농장의 씨앗, 과일 컨테이너 게임 오브젝트 참조
    [Header("Farm GameObject")]
    [SerializeField]
    public SeedContainer seedContainer; // 이 값이 null 이면 현재 씬이 팜이 아닌 것(팜인지 아닌지 여부를 판단해야 인벤토리의 상태를 팜의 컨테이너에 반영할지 말지 선택할 수 있음)..
    [SerializeField]
    public FruitContainer fruitContainer; // 이 값이 null 이면 현재 씬이 팜이 아닌 것..

    [Header("Initial Inventory Items List")]
    [SerializeField]
    public List<InventoryItem> initialItems = new List<InventoryItem>(); // 처음 시작 인벤토리(이거 그냥 임시로 해놓음..)

    [Header("Sell Item Info")]
    [SerializeField]
    public ItemSellPanel itemSellPanel; // 아이템 판매 판넬


    [Header("Close Farm Interaction Button")]
    public GameObject buttonParentGameObject; // 인벤토리 UI 띄우면 버튼 안 보이도록 하기 위함..





    /*
     Awake와 Start의 호출 시점

    Awake는 게임 오브젝트가 생성될 때 호출됩니다. 하지만 싱글톤 패턴처럼 씬 전환 후에도 계속 존재하는 오브젝트의 경우, 씬 전환 시 Awake가 호출되지 않습니다.
    Start는 게임 오브젝트가 활성화된 이후 첫 번째 프레임 전에 호출됩니다. 씬 전환 시 새로 생성된 오브젝트가 아니라면 호출되지 않습니다.
     */

    // Awake 랑 Start 는 게임오브젝트가 생성될 때 호출됨..
    // 만약 씬을 전환해서 다른 씬으로 넘어갔을 때, 이 클래스를 가지는 게임 오브젝트는 삭제되지 않고 그대로 있음
    // 새로 생성된게 아니므로 Awake 랑 Start 함수는 호출되지 않음..
    // 그래서 씬이 전환되고 난 후, 참조 변수의 값들을 다시 넣어줘야 하는데 이와 관련된 로직을 Awake 와 Start 함수에 쓰면 안됨..
    // 즉, 해결 방법은 유니티가 제공하는 UnityEngine.SceneManagement 의 OnSceneLoaded 함수에 관련 로직을 적어주면 됨..
    // 그럼 씬이 로드될 때 필요한 모든 참조를 초기화 함..


    /*
    OnSceneLoaded 활용

    OnSceneLoaded는 씬이 로드될 때 호출되는 콜백 함수입니다. 씬 전환 시 기존 씬에서의 참조를 재설정할 때 유용합니다.
    SceneManager.sceneLoaded 이벤트를 구독하여 씬이 로드될 때 필요한 초기화 작업을 수행할 수 있습니다. 

     */



    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("인벤토리 매니저가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 인벤토리가 삭제되지 않도록(인벤토리는 모든 씬에서 이용 가능해야 하기 때문에..)..

            // 델리게이트에 씬 로드 시 참조를 재설정하는 함수 연결..
            // 싱글톤 인스턴스가 처음 생성될 때만 실행되도록..
            // 중복된 오브젝트가 파괴될 때 이벤트가 중복으로 연결되는 문제 해결 위함..
            SceneManager.sceneLoaded += OnInventorySceneLoaded;
        }
        else
        {
            // instance에 이미 다른 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 인벤토리 매니저가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("인벤토리 매니저를 죽입니다");
        }


        fruitCount = new int[fruitItems.Length];
        // 시작할 때 한번 과일 컨테이너 개수 반영한다음 시작..
        SetFruitInventoryToFruitContainer(); // 현재 인벤토리의 과일 개수를 과일 개수 저장하는 배열에 반영..
    }


    private void Start()
    {
        // 씨앗 개수 반영한다음 시작..
        SetInventoryToContainer(0);
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                SetCurInventoryDataSeed(); // 인벤토리 창 켜질때는 씨앗을 기준으로 켜지도록..
                buttonParentGameObject.SetActive(false); // 농사 버튼 다 안 보이도록..
                inventoryUI.Show();
            }
            else
            {
                buttonParentGameObject.SetActive(true); // 농사 버튼 다시 다 보이도록..
                inventoryUI.Hide();
            }


            // 현재 인벤토리 UI 가 켜져 있는지 여부에 따라 씨앗, 과일, 케이크 버튼의 활성화 결정해줄 것..
            seedButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);
            fruitButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);
            cakeButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);

            inventoryUI.ResetDescription(); // 설명창 리셋해주기.. 
            inventoryUI.sellButtonPanel.gameObject.SetActive(false); // 판매 버튼 판넬도 꺼주기..
        }


        // 임시 확인 코드
        if (Input.GetKeyDown(KeyCode.C))
        {
            InventoryItem tmpItem = new InventoryItem()
            {
                item = cakeItems[0],
                quantity = 1,
            };

            cakeInventoryData.AddItem(tmpItem);
        }
    }


    void OnInventorySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 완전히 로드될 때까지 기다린 후 코루틴 시작..
        StartCoroutine(InitializeAfterSceneLoad());
    }

    private IEnumerator InitializeAfterSceneLoad()
    {
        // 다음 프레임에 실행 되도록 하는 구문..
        yield return null;

        Debug.Log("씬 로드됨!!!");

        // 씬이 로드될 때 참조 변수 설정
        InitializeReferences();
        PrepareUI();
        PrepareInventoryData();


        // 이건 UIInventoryManager 클래스 속 배열 관리
        SetFruitInventoryToFruitContainer(); // 현재 인벤토리의 과일 개수를 과일 배열에 반영..


        // 이건 씨앗, 과일 컨테이너 클래스 속 배열 관리
        SetInventoryToContainer(0); // 현재 인벤토리의 씨앗 개수를 씨앗 컨테이너 게임오브젝트 속 배열에 반영..
        SetInventoryToContainer(1); // 현재 인벤토리의 과일 개수를 과일 컨테이너 게임오브젝트 속 배열에 반영..
    }

    void InitializeReferences()
    {
        // 씬을 전환하면 이 클래스가 참조하고 있던 게임오브젝트들이 날아감..
        // 그래서 현재 씬에서 타입에 맞는 게임오브젝트를 찾아서 연결해줄것..
        // 씬에서 필요한 게임 오브젝트 찾기
        // 부모가 활성화 되어 있으면 활성화 되어 있지 않은 자식도 찾을수 있다고 해서 활성화 끄고 시작한 게임 오브젝트의
        // 부모를 먼저 찾은 다음 자식을 찾아서 넣어줌!!
        // 이제 끄고 시작해도 괜찮!!
        inventoryUI = GameObject.Find("InGameMenu").transform.Find("Inventory").GetComponent<UIInventoryPage>();
        itemSellPanel = GameObject.Find("InventoryCanvas").transform.Find("SellItemPanel").GetComponent<ItemSellPanel>();

        seedButton = GameObject.Find("InventoryUI").transform.Find("SeedButton").GetComponent<Button>();
        fruitButton = GameObject.Find("InventoryUI").transform.Find("FruitButton").GetComponent<Button>();
        cakeButton = GameObject.Find("InventoryUI").transform.Find("CakeButton").GetComponent<Button>();
        // GameObject 는 GetComponent 로 찾는게 아니라 그냥 gameObject 쓰면 됨..
        buttonParentGameObject = GameObject.Find("FarmButtonUICanvas")?.transform.Find("PlowPlantHarvestButtonsParent")?.gameObject;


        //// ?. 를 이용해서 null 인지 아닌지 판단함..
        //// seedContainer 랑 fruitcontainer 는 농장 씬에만 있도록 할거라서 다른 씬에서는 값이 null 로 설정되어 있을 것..
        seedContainer = GameObject.Find("SeedContainer")?.GetComponent<SeedContainer>();
        fruitContainer = GameObject.Find("FruitContainer")?.GetComponent<FruitContainer>();

        inventoryOpenButton = GameObject.Find("InventoryOpenButton")?.GetComponent<Button>();


        // 일단 다 끈 상태로 시작..
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.exitButton.gameObject.SetActive(false);
        seedButton.gameObject.SetActive(false);
        fruitButton.gameObject.SetActive(false);
        cakeButton.gameObject.SetActive(false);
        itemSellPanel.gameObject.SetActive(false);


        // 인벤토리 끄기 버튼에 함수 연결..
        inventoryUI.exitButton.onClick.AddListener(OpenInventoryUI);
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
                cakeInventoryData.AddItem(item);
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
                SetFruitInventoryToFruitContainer(); // 현재 인벤토리의 과일 개수를 과일 개수 저장하는 배열에 반영..
                break;
            case 2:
                // 보석
                break;
            case 3:
                // 케이크
                cakeInventoryData.MinusItem(item);
                // 얘도 현재 케이크 개수를 케이크 개수 저장 배열에 반영하는 함수 만들어야 하나? 월요일 회의 때 승우님께 질문하기!!!!!!
                break;
        }
    }

    public void SellItem(int count, int price, int itemType)
    {
        switch (itemType)
        {
            case 1:
                // 과일

                // 현재 마우스로 클릭한 아이템의 인덱스 요소를 판매하려는 아이템의 수만큼 감소시키기.. 
                fruitInventoryData.MinusItemAt(inventoryUI.currentMouseClickIndex, count);
                GameManager.instance.money += price; // 판매 가격만큼 돈을 더해줌..
                SetFruitInventoryToFruitContainer(); // 현재 인벤토리의 과일 개수를 과일 개수 저장하는 배열에 반영..

                break;
            case 2:
                // 보석
                break;
        }

        itemSellPanel.gameObject.SetActive(false); // 팔고 난 다음에 창 끄기..
        inventoryUI.currentMouseClickIndex = -1; // -1 로 다시 바꿔주기..
        inventoryUI.ResetDescription(); // 아이템 설명창도 리셋해주기..
        inventoryUI.sellButtonPanel.gameObject.SetActive(false); // 판매 버튼 판넬도 꺼주기..
    }

    private void SetItemSellPanel(int itemIndex)
    {
        InventoryItem item = curInventoryData.GetItemAt(itemIndex);
        itemSellPanel.gameObject.SetActive(true);

        itemSellPanel.SetItemInfo(item);
    }



    // 인벤토리 변경 사항 처리 관련 함수
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

    private void SetInventoryUI(int inventorySize)
    {
        inventoryUI.SetInventoryUI(inventorySize);
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

    public void ResetFruitContainer()
    {
        // 모든 요소의 값으르 0으로 리셋해주기..
        for (int i = 0; i < fruitCount.Length; i++)
        {
            fruitCount[i] = 0;
        }
    }

    public void SetFruitContainer(Dictionary<int, InventoryItem> curInventory)
    {
        ResetFruitContainer(); // 한 번 배열 상태 리셋..

        // curInventory 에서 키값은, 인벤토리 속에서 해당 아이템의 인덱스 번호임
        // 현재 인벤토리의 내용을 가져올 때 비어있는 아이템 칸은 제외하고 가져옴.
        // 즉, 인벤토리 속 비어있는 아이템 칸이 있다면 가져온 아이템 딕셔너리의 내용은 [0]: 사과, [2]: 바나나, [5]: 오렌지 이럴 가능성이 있음
        // 그래서 key 가 1, 2, 3, 4, 5... 이런식으로 순차적으로 온다는 보장이 없으므로 그냥 키값들을 가져와서 반복문 도는 것..
        foreach (int idx in curInventory.Keys)
        {
            fruitCount[((FruitItemSO)(curInventory[idx].item)).fruitIdx] += curInventory[idx].quantity; // 해당 아이템의 아이템 인덱스에 맞는 요소의 값을 증가시켜줌..
        }
    }

    private void SetFruitInventoryToFruitContainer()
    {
        // 인벤토리 매니저의 과일 카운트 배열을 관리하기 위한 함수..

        Dictionary<int, InventoryItem> fruitInventory;
        fruitInventory = fruitInventoryData.GetCurrentInventoryState();
        SetFruitContainer(fruitInventory);
    }


    private void SetOpenSellButton()
    {
        if (curInventoryData.inventoryType == 1 || curInventoryData.inventoryType == 2)
        {
            // 현재 인벤토리 데이터가 가리키는 게 과일이랑 보석이면 판매 버튼이 뜰 수 있도록..
            inventoryUI.isPossible = true;
        }
        else
        {
            inventoryUI.isPossible = false;
        }
    }



    // 인벤토리 준비 함수
    private void PrepareInventoryData()
    {
        // 인벤토리 각각 초기화해주기..
        //seedInventoryData.Initialize();
        //fruitInventoryData.Initialize();


        // 델리게이트에 UpdateInventoryUI 함수를 연결하기..
        // 인벤토리 데이터에 변경사항이 생기면 UpdateInventoryUI 함수를 호출할 수 있도록..
        seedInventoryData.OnInventoryUpdated += UpdateInventoryUI;
        fruitInventoryData.OnInventoryUpdated += UpdateInventoryUI;
        cakeInventoryData.OnInventoryUpdated += UpdateInventoryUI;

        // 델리게이트에 SetInvenetoryToContainer 함수를 연결하기..
        // 인벤토리 데이터에 변경사항이 생기면 SetInvenetoryToContainer 함수를 호출할 수 있도록..
        seedInventoryData.OnInventoryUpdatedInt += SetInventoryToContainer;
        fruitInventoryData.OnInventoryUpdatedInt += SetInventoryToContainer;
        cakeInventoryData.OnInventoryUpdatedInt += SetInventoryToContainer;

        // 델리게이트에 SetInventoryUI 함수 연결하기..
        // 인벤토리 사이즈에 변경사항이 생기면 호출할 수 있도록..
        seedInventoryData.OnInventorySizeUpdated += SetInventoryUI;
        fruitInventoryData.OnInventorySizeUpdated += SetInventoryUI;
        cakeInventoryData.OnInventorySizeUpdated += SetInventoryUI;

        // 델리게이트에 SetItemSellPanel 함수 연결해놓기..
        // 판매 버튼 눌렀을 때, 판매 창 정보를 현재 선택한 아이템의 정보로 설정하기 위함..
        inventoryUI.OnItemActionRequested += SetItemSellPanel;

        // 델리게이트에 SetOpenSellButton 함수 연결해놓기..
        // 아이템 눌렀을 때, 아이템이 과일, 보석이면 인벤토리에서 판매버튼 뜰 수 있도록 하기 위함.. 
        inventoryUI.OpenSellButtonPossible += SetOpenSellButton;



        //// 이건 게임 시작할 때 인벤토리에 아이템 몇 개 넣어놓을 때 사용하려고 일단 임시로 쓴 코드..
        //// 아예 아무것도 안 준채로 시작할지 아니면 뭐 좀 주고 시작할지 고민..
        //foreach (InventoryItem item in initialItems)
        //{
        //    if (item.IsEmpty) continue;

        //    seedInventoryData.AddItem(item);
        //}
    }

    private void PrepareUI()
    {
        curInventoryData = seedInventoryData; // 일단 처음 시작은 씨앗 인벤토리로..

        // 버튼에 함수 연결
        seedButton.onClick.AddListener(SetCurInventoryDataSeed); // 씨앗 버튼에 인벤토리 데이터를 씨앗 인벤토리 데이터로 바꿔주는 함수 연결
        fruitButton.onClick.AddListener(SetCurInventoryDataFruit); // 과일 버튼에 인벤토리 데이터를 과일 인벤토리 데이터로 바꿔주는 함수 연결
        cakeButton.onClick.AddListener(SetCurInventoryDataCake); // 케이크 버튼에 인벤토리 데이터를 케이크 인벤토리 데이터로 바꿔주는 함수 연결
        inventoryOpenButton.onClick.AddListener(OpenInventoryUI); // 버튼에 인벤토리창 여는 로직 함수 연결

        // 아이템 판매 판넬 클래스의 델리게이트에 SellItem 함수 연결..
        itemSellPanel.sellButtonClicked += SellItem;


        inventoryUI.InitializeInventoryUI(curInventoryData.Size); // 씨앗 인벤토리 사이즈만큼 UI 초기화해주기
        inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnSwapItems += HandleSwapItems;
    }

    private void OpenInventoryUI()
    {
        if (inventoryUI.isActiveAndEnabled == false)
        {
            SetCurInventoryDataSeed(); // 인벤토리 창 켜질때는 씨앗을 기준으로 켜지도록..

            // 농장씬에서만..
            if (buttonParentGameObject != null)
            {
                buttonParentGameObject.SetActive(false); // 농사 버튼 다 안 보이도록.. 
            }
            inventoryUI.Show();
        }
        else
        {
            // 농장씬에서만..
            if (buttonParentGameObject != null)
            {
                buttonParentGameObject.SetActive(true); // 농사 버튼 다시 다 보이도록..
            }
            inventoryUI.Hide();
        }

        // 인벤토리 창이 켜졌는지 여부에 따라 씨앗, 과일 인벤토리창 선택 버튼도 켜질지 꺼질지 결정..
        seedButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);
        fruitButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);
        cakeButton.gameObject.SetActive(inventoryUI.isActiveAndEnabled);

        inventoryUI.ResetDescription(); // 설명창 리셋해주기.. 
        inventoryUI.sellButtonPanel.gameObject.SetActive(false); // 판매 버튼 판넬도 꺼주기..
    }



    // 델리게이트 연결 함수
    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = new InventoryItem(); // InventoryItem 은 구조체라 null 값을 가질 수 없음(r-value 임..)

        // 현재 인벤토리 데이터 변수가 가리키는 값이 씨앗 인벤토리 데이터 값이라면..
        if (curInventoryData == seedInventoryData)
        {
            inventoryItem = seedInventoryData.GetItemAt(itemIndex); // 전달받은 아이템의 인덱스로 인벤토리 아이템을 가져옴.. 
        }
        // 현재 인벤토리 데이터 변수가 가리키는 값이 과일 인벤토리 데이터 값이라면.. 
        else if (curInventoryData == fruitInventoryData)
        {
            inventoryItem = fruitInventoryData.GetItemAt(itemIndex); // 전달받은 아이템의 인덱스로 인벤토리 아이템을 가져옴..
        }
        // 현재 인벤토리 데이터 변수가 가리키는 값이 케이크 인벤토리 데이터 값이라면..
        else if (curInventoryData == cakeInventoryData)
        {
            inventoryItem = cakeInventoryData.GetItemAt(itemIndex); // 전달 받은 아이템의 인덱스로 인벤토리 아이템을 가져옴..
        }

        // 중복되는 구문 처리 함수..
        HandleDescription(inventoryItem, itemIndex);
    }

    private void HandleDescription(InventoryItem inventoryItem, int itemIndex)
    {
        // 중복 되는 구문 따로 빼서 함수로 만든 것...
        // HandleDescriptionRequest 함수에서 호출할 것..

        if (inventoryItem.IsEmpty) // 만약 인벤토리 아이템이 비어있으면 디스크립션 초기화하고 빠져나가도록..
        {
            inventoryUI.ResetDescription();
            return;
        }

        ItemSO item = inventoryItem.item;
        inventoryUI.UpdateDescription(itemIndex, item.itemImage, item.Name, item.Description);
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
        // 현재 인벤토리 데이터 변수가 가리키는 값이 과일 인벤토리 데이터 값이라면..
        else if (curInventoryData == cakeInventoryData)
        {
            cakeInventoryData.SwapItems(index1, index2); // 케이크 인벤토리 데이터의 SwapItems 함수를 호출함!
        }
    }



    // 버튼 관련
    private void SetCurInventoryData(InventorySO inventoryData, bool canSell)
    {
        // 현재 인벤토리 데이터 값을 주어진 인벤토리 데이터 값으로 설정하는 함수
        inventoryUI.sellButtonPanel.gameObject.SetActive(canSell); // 판매 가능 여부에 따라 버튼 활성화 설정

        curInventoryData = inventoryData;
        inventoryUI.SetInventoryUI(curInventoryData.Size); // 인벤토리 UI 를 현재 보려고 선택한 인벤토리 데이터에 맞게 설정
        inventoryUI.ResetDescription(); // 인벤토리 창 변경하면 설명도 꺼지도록
        curInventoryData.InformAboutChange();
    }

    private void SetCurInventoryDataSeed()
    {
        // 현재 인벤토리 데이터 값을 Seed 인벤토리 데이터 값으로 설정하는 함수
        SetCurInventoryData(seedInventoryData, false); // 씨앗은 판매 불가능하니까 씨앗 인벤토리 창으로 전환하면 판매 버튼도 그냥 꺼질 수 있도록 false 넘겨주기..
    }

    private void SetCurInventoryDataFruit()
    {
        // 현재 인벤토리 데이터 값을 Fruit 인벤토리 데이터 값으로 설정하는 함수
        SetCurInventoryData(fruitInventoryData, true); // 과일은 판매 가능하니까 과일 인벤토리 창으로 전환하면 판매 버튼 켜질 수 있도록 true 넘겨주기..
    }

    private void SetCurInventoryDataCake()
    {
        // 현재 인벤토리 데이터 값을 Cake 인벤토리 데이터 값으로 설정하는 함수
        SetCurInventoryData(cakeInventoryData, false); // 씨앗은 판매 불가능하니까 씨앗 인벤토리 창으로 전환하면 판매 버튼도 그냥 꺼질 수 있도록 false 넘겨주기..
    }
}