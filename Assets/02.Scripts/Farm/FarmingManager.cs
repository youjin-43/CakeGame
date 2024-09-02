using Inventory.Model;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static AudioManager;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;




[Serializable]
// 딕셔너리를 클래스 형식으로 key, value 를 만들어서 구성하가ㅣ..
// 다른 Dictionary 도 고려하여 제네릭 타입으로 만들기..
public class DataDictionary<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}

[Serializable]
public class JsonDataArray<TKey, TValue>
{
    // 임의로 생성한 딕셔너리 값 저장용 리스트
    public List<DataDictionary<TKey, TValue>> data;
}

public static class DictionaryJsonUtility
{

    /// <summary>
    /// Dictionary를 Json으로 파싱하기
    /// </summary>
    /// <typeparam name="TKey">Dictionary Key값 형식</typeparam>
    /// <typeparam name="TValue">Dictionary Value값 형식</typeparam>
    /// <param name="jsonDicData"></param>
    /// <returns></returns>
    public static string ToJson<TKey, TValue>(Dictionary<TKey, TValue> jsonDicData, bool pretty = false)
    {
        List<DataDictionary<TKey, TValue>> dataList = new List<DataDictionary<TKey, TValue>>();
        DataDictionary<TKey, TValue> dictionaryData;
        foreach (TKey key in jsonDicData.Keys)
        {
            dictionaryData = new DataDictionary<TKey, TValue>();
            dictionaryData.Key = key;
            dictionaryData.Value = jsonDicData[key];
            dataList.Add(dictionaryData);
        }
        JsonDataArray<TKey, TValue> arrayJson = new JsonDataArray<TKey, TValue>();
        arrayJson.data = dataList;

        return JsonUtility.ToJson(arrayJson, pretty);
    }

    /// <summary>
    /// Json Data를 다시 Dictionary로 파싱하기
    /// </summary>
    /// <typeparam name="TKey">Dictionary Key값 형식</typeparam>
    /// <typeparam name="TValue">Dictionary Value값 형식</typeparam>
    /// <param name="jsonData">파싱되었던 데이터</param>
    /// <returns></returns>

    public static Dictionary<TKey, TValue> FromJson<TKey, TValue>(string jsonData)
    {
        JsonDataArray<TKey, TValue> arrayJson = JsonUtility.FromJson<JsonDataArray<TKey, TValue>>(jsonData);
        List<DataDictionary<TKey, TValue>> dataList = arrayJson.data;

        Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();


        for (int i = 0; i < dataList.Count; i++)
        {
            DataDictionary<TKey, TValue> dictionaryData = dataList[i];

            returnDictionary.Add(dictionaryData.Key, dictionaryData.Value);
        }

        return returnDictionary;
    }
}



[Serializable]
// 아예 따로 클래스 만들어서 값을 저장할 때 Vector3Int 대신 PosInt 써야할 것 같다..
public class PosInt
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;
    [SerializeField]
    public int z;
}


// 데이터 저장 클래스
[Serializable]
public class SaveFarmingData
{
    [SerializeField]
    public bool seedOnTile; // 타일 위에 씨앗이 있는지 여부 확인용
    [SerializeField]
    public bool plowEnableState; // 밭을 갈 수 있는 상태인지 여부 확인용
    [SerializeField]
    public bool plantEnableState; // 씨앗을 심을 수 있은 상태인지 여부 확인용
    [SerializeField]
    public bool harvestEnableState; // 작물이 다 자란 상태인지 여부 확인용
    [SerializeField]
    public bool failedState; // 농작물이 망한 상태인지 여부 확인용
    [SerializeField]
    public string currentState; // 농사 땅 상태..

    // 씨앗 데이터
    [SerializeField]
    public int seedIdx; // 씨앗 인덱스 저장(종류 저장하기 위함)..
    [SerializeField]
    public bool isGrown; // 씨앗이 다 자랐는지 안자랐는지 여부 저장..


    [SerializeField]
    // 씨앗 마다 자라기까지 걸리는 일수가 있어서 다르게 자라도록?_?
    public int curDay; // 씨앗 심고 지난 시간(씬 전환 될 때마다 1씩 증가하는 식으로 하면 될 것 같다)
}


// 타일이 가지는 농사 데이터
[Serializable]
public class FarmingData
{
    [SerializeField]
    public bool plowEnableState; // 밭을 갈 수 있는 상태인지 여부 확인용(밭이 안 갈린 상태)
    [SerializeField]
    public bool plantEnableState; // 씨앗을 심을 수 있는 상태인지 여부 확인용
    [SerializeField]
    public bool harvestEnableState; // 작물이 다 자란 상태인지 여부 확인용
    [SerializeField]
    public bool failedState; // 농작물이 망한 상태인지 여부 확인용


    /*
     Button과 같은 Unity 엔진 내장 컴포넌트들은 게임 오브젝트나 컴포넌트로 참조되어 있기 때문에 JSON 직렬화에서 제대로 다룰 수 없다고함..
     Unity의 직렬화 시스템도 이러한 Unity 엔진 내장 객체들을 처리하는 방식과 JSON 직렬화는 다르기 때문에, 이런 컴포넌트들을 JSON으로 저장할 수는 없다고 함..
     => 일반적인 경우는 UI 요소들은 직렬화에서 제외한다고 함..
     */
    [NonSerialized] public Button stateButton; // 타일을 누르면 타일 위에 뜨도록 하는 버튼
    [NonSerialized] public Button[] buttons; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼, [3]: failed 버튼

    [SerializeField]
    public string currentState = "None"; // 현재 상태(초기에는 아무것도 안 한 상태니까 None 으로.. -> plow: 밭 갈린 상태, plant: 씨앗 심은 상태, harvest: 다 자란 상태, failed: 망한 상태)



    [SerializeField]
    public int seedIdx = -1; // 그러면 이제 씨앗 인덱스도 저장해야 할듯(아무것도 안 심어져 있는 상태는 -1을 값으로 가지도록)..
    [SerializeField]
    public bool seedOnTile; // 그럼 이제 그냥 Seed 클래스 사용하지 말고 bool 값으로 씨앗이 있는지 여부 판단하는 게 좋을 듯..
    [SerializeField]
    public bool isGrown; // 다 자랐는지 여부도 이제 얘가 저장하도록 하는게..
    // 이제 시간이 아니라 일수로 관리하도록 수정할 것..
    // 근데 이러면 딱히 시간 안 더해줘도 되니까 씨앗도 그냥 ScriptableObject 클래스로만 관리해야 좋을 것 같은데
    // 원래는 시간 더해줘야 하니까 Monobehaviour 클래스를 상속받아야 했던 거고..
    [SerializeField]
    // 씨앗 마다 자라기까지 걸리는 일수가 있어서 다르게 자라도록?_?
    public int curDay; // 씨앗 심고 지난 일수(씬 전환 될 때마다 1씩 증가하는 식으로 하면 될 것 같다)


    public void SetData()
    {
        plowEnableState = true;
    }
}


public class FarmingManager : MonoBehaviour
{
    [Header("Game Data")]
    public Camera mainCamera; // 마우스 좌표를 게임 월드 좌표로 변환하기 위해 필요한 변수(카메라 오브젝트 할당해줄 것)
    public SeedContainer seedContainer; // 현재 가진 씨앗을 가져오기 위해 필요한 변수(씨앗 컨테이너 게임 오브젝트 할당해줄 것)
    public FruitContainer fruitContainer; // 수확한 과일을 저장하기 위해 필요한 변수(과일 컨테이너 게임 오브젝트 할당해줄 것)
    public UIInventoryManager inventoryController; // 인벤토리 관리하기 위해 필요한 변수(인벤토리 매니저 게임 오브젝트 할당해줄 것) 
    public AnimalInteractionManager animalInteractionManager; // 너구리 미니 게임을 관리하기 위해 필요한 변수(애니멀 인터랙션 매니저 게임 오브젝트 할당해줄 것)
    public GameObject farmButtonsParent;
    public Canvas canvas;

    // 아이템 스크립터블 오브젝트를 저장해놓기..
    public FruitItemSO[] fruitItems; // [0]: 사과, [1]: 바나나, [2]: 체리, [3]: 오렌지, [4]: 딸기
    public SeedItemSO[] seedItems; // [0]: 사과, [1]: 바나나, [2]: 체리, [3]: 오렌지, [4]: 딸기


    [Header("Tile")]
    public TileBase borderTile; // 제한 구역 상태
    public TileBase grassTile; // 밭 갈기 전 상태
    public TileBase farmTile; // 밭 간 후 상태
    public TileBase plantTile; // 씨앗 심은 후 상태
    public TileBase harvestTile; // 과일 다 자란 상태
    public TileBase failedTile; // 땅 망한 상태
    public Vector3Int prevSelectTile; // 이전 클릭된 타일

    [Header("Tilemap")]
    public Tilemap farmEnableZoneTilemap; // 농사 가능 부지를 나타내는 타일맵
    public Tilemap farmTilemap; // 진짜로 현재 타일의 상태에 따라 타일이 변경되는 타일맵

    [Header("Farm interaction Button")]
    // 버튼을 프리팹으로 만들어 놓은 다음 동적으로 생성해서 쓸 것.
    public GameObject[] buttonPrefabs; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼, [3]: failed 버튼
    public GameObject buttonParent; // 버튼 생성할 때 부모 지정하기 위한 변수
    public GameObject buttonsGameObject; // 버튼의 부모를 껐다 켜기 위해서 필요한 변수.. 

    [Header("Farm interaction Panel")]
    public GameObject growTimePanel; // 다 자라기까지 남은 시간 보여주는 판넬
    public Text growTimeText; // 다 자라기까지 남은 시간

    [Header("Farming Data")]
    public Vector2 clickPosition; // 현재 마우스 위치를 게임 월드 위치로 바꿔서 저장
    public Vector3Int cellPosition; // 게임 월드 위치를 타일 맵의 타일 셀 위치로 변환
    public Dictionary<Vector3Int, FarmingData> farmingData;
    public int farmLevel = 1; // 농장 레벨. 농장 레벨 업그레이드 함수 호출하면 증가하도록..
    public int expansionSize = 1; // 농장 한 번 업그레이트 할 때 얼마나 확장될 건지.. 일단 임시로 1로 해놨다.. 나중에 변경할 것.
    public int scarecrowLevel = 1; // 허수아비 레벨..
    public int farmSizeLevelUpCost = 10000;
    public int scarecrowLevelUpCost = 10000;


    [Header("PlantSeed Information")]
    public GameObject plantSeedPanel; // 씨앗 선택창



    [Header("Farm Upgrade UI")]
    public Text farmSizeUpgradeCostText; // 농장 사이즈 업그레이드 비용 텍스트
    public Text curFarmSizeLevelText; // 현재 농장 레벨 텍스트
    public Text nextFarmSizeLevelText; // 다음 농장 레벨 텍스트

    public Text scareCrowUpgradeCostText; // 허수아비 업그레이트 비용 텍스트
    public Text curScareCrowLevelText; // 현재 허수아비 레벨 텍스트
    public Text nextScareCrowLevelText; // 다음 허수아비 레벨 텍스트


    // 데이터 저장
    [Header("Save Data")]
    private string farmingDataFilePath; // 농사 데이터 저장 경로..


    // 농장씬 사운드
    [Header("Game Sound")]
    public AudioManager audioManager; // 오디오 매니저.. 인스펙터 창에서 오디오 매니저 게임 오브젝트 할당해줄것..



    private void Awake()
    {
        // 데이터 저장 경로 설정..
        farmingDataFilePath = Path.Combine(Application.persistentDataPath, "FarmingData.json"); // 데이터 경로 설정..


        farmingData = new Dictionary<Vector3Int, FarmingData>(); // 딕셔너리 생성
        clickPosition = Vector2.zero;
    }

    private void OnEnable()
    {
        // 델리게이트에 씬 로드 시 참조를 재설정하는 함수 연결..
        // FarmingManager 는 DontDestroyOnLoad 가 아니므로 Enable 에서 함수 연결해주고 Disable 에서 함수 연결 끊어줄 것임..
        SceneManager.sceneLoaded += OnFarmSceneLoaded;
    }


    private void Start()
    {
        // 농사 가능 구역만 farmingData 에 저장할 것임.
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            if (!farmEnableZoneTilemap.HasTile(pos)) continue;

            SetFarmingData(pos); // FarmingData 타입 인스턴스의 정보를 세팅해주는 함수.
        }

        Debug.Log(farmingData.Count + "딕셔너리에 저장된 땅 개수임1!!!!!");


        // 농사 땅 레벨 데이터 불러오기..
        farmLevel = PlayerPrefs.GetInt("FarmLevel");
        // 농사 땅 레벨 데이터를 불러온 다음에 레벨 데이터에 맞게끔 땅 업그레이드 해주기.. 
        int tmpFarmLevel = farmLevel;
        while (tmpFarmLevel > 1)
        {
            SetFarmSize();
            tmpFarmLevel--;
        }

        LoadFarmingData(); // 데이터 가져오기..

        Debug.Log(farmingData.Count + "딕셔너리에 저장된 땅 개수임2!!!!!");


        // LoadFarmingData 로 데이터를 가져왔으므로 이제 가능
        // 농사 땅을 전부 돌면서 씨앗이 심어져 있으면 일수 증가시킴..
        foreach (var item in farmingData)
        {
            // 만약 씨앗이 심어져 있으면
            if (farmingData[item.Key].seedOnTile)
            {
                // 씨앗이 심긴 후 지난 일수가 씨앗이 다 자라는데 걸리는 일수랑 같으면
                if (farmingData[item.Key].curDay == seedItems[farmingData[item.Key].seedIdx].growDay - 1)
                {
                    // 다 자랐음을 표시하기 위해 isGrown 의 값을 true 로 변경해주기..
                    farmingData[item.Key].isGrown = true;
                }
                // 씨앗이 심긴 후 지난 일수가 씨앗이 다 자라는데 걸리는 일수보다 작으면
                else if (farmingData[item.Key].curDay < seedItems[farmingData[item.Key].seedIdx].growDay - 1)
                {
                    // 일수를 증가시켜주기..
                    farmingData[item.Key].curDay++;
                }
            }
            else
            {
                Debug.Log("씨앗이 안 심어져있어용~");
            }
        }

        // 저장하기
        SaveFarmingData(); // 변경 사항 생겼으니까 저장해주기..


        CheckGrowedFruit(); // 과일이 다 자랐는지 확인하고, 다 자랐으면 그에 맞는 행동을 하도록 해주는 함수..


        // 아래 코드들은 그냥 임시로 확인하기 위한 거... 나중에 없앨 것...
        PlayerPrefs.SetInt("FarmLevel", farmLevel); // 농장 레벨 저장..
        scarecrowLevel = 1;
        PlayerPrefs.SetInt("ScareCrowLevel", scarecrowLevel); // 허수아비 레벨 저장..
    }


    void Update()
    {
        //// 모바일용
        //if (Input.touchCount > 0)
        //FarmingSystemMobile();


        // 버튼 눌렀을 때 뒤에 있는 타일 못 누르도록 하기 위한 구문..
        // 이거 데스크탑용
        if (IsPointerOverUIObjectPC()) return;

        // 데스크탑용
        // 땅을 왼쪽 마우스키로 누르면..
        if (Input.GetMouseButtonDown(0))
        {
            // 땅을 왼쪽 마우스키로 눌렀을 때, 땅의 현재 상태를 파악한 후 버튼 등 전반적인 UI 조정하는 것과 관련된 로직 함수..
            FarmingSystemPC();
        }
    }

    private void OnDisable()
    {
        // 델리게이트에 씬 로드 시 참조를 재설정하는 함수 없애기..
        // FarmingManager 는 DontDestroyOnLoad 가 아니므로 씬 매니저의 델리게이트에서 빼줘야함..
        // InventortManager 는 DontDestroyOnLoad 라 델리게이트에서 안 빼줘도 되는 거고..
        SceneManager.sceneLoaded -= OnFarmSceneLoaded;
    }



    private void FarmingSystemPC()
    {
        growTimePanel.SetActive(false); // 누르면 이전에 켜진 판넬 꺼지도록..

        // 땅에 아무것도 안 한 상태는 plow 버튼을 갖고, 갈린 상태는 버튼으로 plant 버튼을 갖는다.
        // 다른 땅을 클릭하면 전에 클릭한 땅의 버튼은 안 보여야 하므로 SetActive 로 안보이게 조정한다..
        // 수확하기 버튼은 과일이 다 자라면 계속 보여야함..
        if (farmEnableZoneTilemap.HasTile(prevSelectTile))
        {
            if (farmingData[prevSelectTile].currentState == "None" || farmingData[prevSelectTile].currentState == "plow")
            {
                farmingData[prevSelectTile].stateButton.gameObject.SetActive(false);
            }
        }

        // 현재 마우스 위치를 게임 월드 위치로 바꿔서 저장
        clickPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // 게임 월드 위치를 타일 맵의 타일 셀 위치로 변환
        cellPosition = farmTilemap.WorldToCell(clickPosition);


        foreach (Vector3Int pos in farmingData.Keys)
        {
            // 저장해놓은 타일 중에 현재 마우스로 클릭한 위치랑 같은 타일이 있으면
            if (pos == cellPosition)
            {
                // 밭이 안 갈린 상태면 눌렀을 때 버튼 뜰 수 있도록
                if (farmingData[cellPosition].plowEnableState)
                {
                    farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                }
                else
                {
                    // 씨앗이 안 심어져 있을 때 버튼 뜰 수 있도록
                    if (farmingData[cellPosition].seedOnTile == false)
                    {
                        farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                    }
                    // 씨앗이 자라는 중이면 남은 시간 나타내는 판넬 뜨도록
                    else if (!farmingData[cellPosition].isGrown)
                    {
                        // 판넬 위치를 현재 클릭한 타일 위치로..
                        growTimePanel.transform.position = mainCamera.WorldToScreenPoint(farmTilemap.CellToWorld(cellPosition)) + new Vector3(0, 100, 0);
                        growTimePanel.SetActive(true);

                        growTimeText.text = "남은일수\n" + (seedItems[farmingData[cellPosition].seedIdx].growDay - farmingData[cellPosition].curDay);
                    }
                }
            }
        }

        prevSelectTile = cellPosition; // 지금 누른 타일을 이전에 누른 타일 위치를 저장하는 변수에 저장..
    }


    private void FarmingSystemMobile()
    {
        // 맨 처음 터치만 이용할 것
        Touch touch = Input.GetTouch(0);
        if (IsPointerOverUIObjectMobile(touch)) return; // 터치한 곳에 UI 있으면 그냥 빠져나오도록..


        // 맨 처음 터치 시작시
        if (touch.phase == TouchPhase.Began)
        {
            growTimePanel.SetActive(false); // 누르면 이전에 켜진 판넬 꺼지도록..

            // 땅에 아무것도 안 한 상태는 plow 버튼을 갖고, 갈린 상태는 버튼으로 plant 버튼을 갖는다.
            // 다른 땅을 클릭하면 전에 클릭한 땅의 버튼은 안 보여야 하므로 SetActive 로 안보이게 조정한다..
            // 수확하기 버튼은 과일이 다 자라면 계속 보여야함..
            if (farmEnableZoneTilemap.HasTile(prevSelectTile))
            {
                if (farmingData[prevSelectTile].currentState == "None" || farmingData[prevSelectTile].currentState == "plow")
                {
                    farmingData[prevSelectTile].stateButton.gameObject.SetActive(false);
                }
            }


            // 현재 터치 위치를 게임 월드 위치로 바꿔서 저장
            clickPosition = mainCamera.ScreenToWorldPoint(touch.position);
            // 게임 월드 위치를 타일 맵의 타일 셀 위치로 변환
            cellPosition = farmTilemap.WorldToCell(clickPosition);
            Debug.Log(cellPosition);


            foreach (Vector3Int pos in farmingData.Keys)
            {
                // 저장해놓은 타일 중에 현재 마우스로 클릭한 위치랑 같은 타일이 있으면
                if (pos == cellPosition)
                {
                    // 밭이 안 갈린 상태면 눌렀을 때 버튼 뜰 수 있도록
                    if (farmingData[cellPosition].plowEnableState)
                    {
                        farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        // 씨앗이 안 심어져 있을 때 또는 씨앗이 다 자랐을 때 버튼 뜰 수 있도록
                        if (farmingData[cellPosition].seedOnTile == false || farmingData[cellPosition].isGrown)
                        {
                            farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                        }
                        // 씨앗이 자라는 중이면 남은 시간 나타내는 판넬 뜨도록
                        else if (!farmingData[cellPosition].isGrown)
                        {
                            // 판넬 위치를 현재 클릭한 타일 위치로..
                            growTimePanel.transform.position = mainCamera.WorldToScreenPoint(farmTilemap.CellToWorld(cellPosition)) + new Vector3(0, 50, 0);
                            growTimePanel.SetActive(true);

                            growTimeText.text = "남은일수\n" + (seedItems[farmingData[cellPosition].seedIdx].growDay - farmingData[cellPosition].curDay);
                        }
                    }
                }
            }

            prevSelectTile = cellPosition; // 지금 누른 타일을 이전에 누른 타일 위치를 저장하는 변수에 저장..
        }
    }

    private void GrowTimeUpdate()
    {
        // 이 함수는 씬이 전환될 때마다 호출되도록 하면 될 것 같다..

        // 농사 땅 위에 씨앗이 있으면
        if (farmEnableZoneTilemap.HasTile(cellPosition) && farmingData[cellPosition].seedOnTile)
        {
            if (!farmingData[cellPosition].isGrown)
                growTimeText.text = growTimeText.text = "남은일수\n" + (seedItems[farmingData[cellPosition].seedIdx].growDay - farmingData[cellPosition].curDay);
            else
                growTimePanel.SetActive(false); // 다 자라면 남은시간 나타내는 판넬 꺼지도록..
        }
    }

    private void CheckGrowedFruit()
    {
        foreach (Vector3Int pos in farmingData.Keys)
        {
            // 씨앗이 농사 땅 위에 있으면
            if (farmingData[pos].seedOnTile)
            {
                // 만약 다 자랐으면
                if (farmingData[pos].isGrown)
                {
                    farmTilemap.SetTile(pos, harvestTile); // 타일을 과일이 다 자란 상태로 변경
                    farmingData[pos].harvestEnableState = true; // 작물 수확할 수 있는 상태
                    farmingData[pos].stateButton.gameObject.SetActive(true); // 수확하기 버튼은 항상 떠있어야 함
                    farmingData[pos].currentState = "harvest";
                }
            }
        }
    }


    public void CheckFailedFarm()
    {
        // 농사가 망했는지 안 망했는지 판단하는 함수..
        // AnimalInteractionManager 클래스의 backgroundButton 에 연결해줄 것..
        // background 버튼은 동물 게임이 종료되면 나타나는 버튼임..

        foreach (Vector3Int pos in farmingData.Keys)
        {
            // 농사땅 망했으면
            if (farmingData[pos].failedState)
            {
                //farmTilemap.SetTile(pos, failedTile); // 일단 임시로
                farmingData[pos].stateButton.gameObject.SetActive(true); // 망한거 없애기 버튼은 항상 떠있어야 함
            }
        }
    }


    public void FailFarmAt(Vector3Int pos)
    {
        // 특정 위치의 농사땅을 망하게 하는 함수

        // 만약 망치려는 땅이 다 자라있는 상태라면
        if (farmingData[pos].currentState == "harvest")
        {
            farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 활성화 끄기
        }

        farmTilemap.SetTile(pos, failedTile); // 망한 땅 모습으로..
        farmingData[pos].stateButton = farmingData[pos].buttons[3]; // 망한 버튼으로 바꾸기
        farmingData[pos].seedOnTile = false;
        farmingData[pos].currentState = "failed";
        farmingData[pos].plowEnableState = false;
        farmingData[pos].plantEnableState = false;
        farmingData[pos].harvestEnableState = false;
        farmingData[pos].failedState = true;

        SaveFarmingData(); // 데이터 저장!
    }


    private bool IsPointerOverUIObjectPC()
    {
        // 컴퓨터용

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            // 핸드폰 터치도 mousePosition 으로 이용할 수 있으므로 간단한 건 그냥 이것처럼 mousePosition 쓸 예정..
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool IsPointerOverUIObjectMobile(Touch touch)
    {
        // 핸드폰용

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(touch.position.x, touch.position.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public GameObject CreateButton(int buttonNumber, Vector3Int pos)
    {
        // 타일 맵 초기 설정할 때 쓰는 함수
        // 타일마다 버튼을 미리 만들어놓고 사용할 것임

        GameObject button = Instantiate(buttonPrefabs[buttonNumber], buttonParent.transform);

        // 셀 좌표를 월드 좌표로 바꿔서 저장
        Vector3 worldPos = farmTilemap.CellToWorld(pos);
        // 월드 좌표를 스크린 좌표로 바꿔서 저장
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);


        // 버튼의 좌표 설정
        button.transform.position = screenPos;
        button.transform.position += new Vector3(0, 50, 0);

        return button;
    }

    public void PlowTile(Vector3Int pos)
    {
        // 밭을 가는 함수

        farmTilemap.SetTile(pos, farmTile); // 타일 모습은 밭 간 상태로 바꿔주기
        farmingData[pos].plowEnableState = false; // 갈았으니까 이제 갈 수 없는 상태를 나타내기 위해 false 로 값 변경
        farmingData[pos].plantEnableState = true; // 씨앗을 심을 수 있는 상태를 나타내기 위해 true 로 값 변경
        farmingData[pos].currentState = "plow"; // 갈린 상태니까 plow 로 바꿔주기

        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..

        farmingData[pos].stateButton = farmingData[pos].buttons[1]; // plant 버튼으로 변경..


        // 음향
        audioManager.SetSFX(SFX.PLOW);


        // 저장하기
        SaveFarmingData();
    }

    public void OpenPlantSeedPanel(Vector3Int pos)
    {
        // 심기 버튼이랑 연결해줘야함.
        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..

        // 농사 버튼 꺼지도록..
        DisableFarmStateButton();

        plantSeedPanel.SetActive(true); // 심기 버튼 눌렀을 때 씨앗 선택창 뜨도록 하기 위함
    }

    public void PlantTile(int seedIdx)
    {
        // 씨앗을 심는 함수
        // 이 함수는 씨앗 선택창에서 씨앗 버튼 눌렀을 때 호출되도록..
        EnableFarmStateButton(); // 농사 버튼 다시 켜지도록..


        // 씨앗의 개수가 0보다 작거나 같으면 그냥 빠져나가도록..
        if (seedContainer.seedCount[seedIdx] <= 0)
        {
            // 음향
            // 음향
            audioManager.SetSFX(SFX.RETURN);

            Debug.Log("씨앗 없어!!!!");
            return;
        }



        // 구매하려는 씨앗의 개수만큼 InventoryItem 구조체의 인스턴스를 만들기..
        InventoryItem tempItem = new InventoryItem()
        {
            item = seedItems[seedIdx],
            quantity = 1,
        };

        inventoryController.seedInventoryData.MinusItem(tempItem); // 씨앗 심었으니까 인벤토리에서 개수 줄어들도록..



        farmingData[cellPosition].seedIdx = seedIdx; // 씨앗 인덱스 설정해주기
        farmingData[cellPosition].seedOnTile = true; // 씨앗을 심었으니까 true 로 값 변경해주기..
        farmTilemap.SetTile(cellPosition, plantTile); // 타일 모습을 씨앗 심은 상태로 바꿔주기
        farmingData[cellPosition].plantEnableState = false; // 씨앗을 심을 수 없는 상태를 나타내기 위해 false 로 변경
        farmingData[cellPosition].currentState = "plant"; // 씨앗 심은 상태니까 plant 로 바꿔주기

        farmingData[cellPosition].stateButton = farmingData[cellPosition].buttons[2]; // harvest 버튼을 가지고 있도록..


        // 음향
        audioManager.SetSFX(SFX.PLANT);


        // 저장하기
        SaveFarmingData();
    }

    public void HarvestTile(Vector3Int pos)
    {
        // 과일을 수확하는 함수
        // 생각해보니까 씨앗 인덱스 여기로 안 보내줘도 pos 보내줬으니까, pos 가 가지는 씨앗 인스턴스의 씨앗 인덱스 이용하면 될 듯.

        farmingData[pos].plowEnableState = true;
        farmingData[pos].currentState = "None"; // 과일을 수확한 상태니까 None 으로 바꿔주기


        // 이건 이제 이 함수에서 관리 안 할 것...
        //fruitContainer.fruitCount[farmingData[pos].seed.seedData.seedIdx]++; // 씨앗의 인덱스와 같은 과일의 수 증가시키기


        // 수확하려는 과일의 개수만큼 InventoryItem 구조체의 인스턴스를 만들기..
        InventoryItem tempItem = new InventoryItem()
        {
            item = fruitItems[farmingData[pos].seedIdx],
            quantity = 1,
        };
        inventoryController.AddItem(tempItem); // 새로 생성한 인벤토리 아이템을 인벤토리 데이터에 추가해주기..


        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..
        farmingData[pos].stateButton = farmingData[pos].buttons[0]; // plow 버튼을 가지고 있도록..
        farmingData[pos].seedOnTile = false; // 수확했으니까 seedOnTile 변수의 값을 다시 false 로 설정해주기..
        farmingData[pos].seedIdx = -1; // 씨앗 인덱스 다시 -1로 바꿔주기..
        farmingData[pos].curDay = 0; // 씨앗 심은 후 지난 일자 다시 0으로 바꿔주기..
        farmingData[pos].isGrown = false; // 이제 땅에 아무것도 없으므로 isGrown 값도 false 로 바꿔주기..


        farmTilemap.SetTile(pos, grassTile); // 타일 모습을 초기 상태로 바꿔주기


        // 음향
        audioManager.SetSFX(SFX.HARVEST);


        // 저장하기
        SaveFarmingData();
    }

    public void FailedTile(Vector3Int pos)
    {
        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..

        // 농사땅 망했으면 그냥 맨 초기 상태로 돌리는 로직..
        farmingData[pos].isGrown = false; // 이제 땅에 아무것도 없으므로 isGrown 값도 false 로 바꿔주기..
        farmingData[pos].seedOnTile = false; // 수확했으니까 seedOnTile 변수의 값을 다시 false 로 설정해주기..
        farmingData[pos].seedIdx = -1; // 씨앗 인덱스 다시 -1로 바꿔주기..
        farmingData[pos].curDay = 0; // 씨앗 심은 후 지난 일자 다시 0으로 바꿔주기..
        farmingData[pos].failedState = false;

        farmingData[pos].plowEnableState = true;
        farmingData[pos].currentState = "None"; // 과일을 수확한 상태니까 None 으로 바꿔주기
        
        farmingData[pos].stateButton = farmingData[pos].buttons[0]; // plow 버튼을 가지고 있도록..
        farmTilemap.SetTile(pos, grassTile); // 타일 모습을 초기 상태로 바꿔주기

        // 음향
        audioManager.SetSFX(SFX.FAIL);


        // 저장하기
        SaveFarmingData();
    }


    public void BuySeed(int count, int idx)
    {
        // 돈이 부족하면 씨앗 못사!
        if (GameManager.instance.money < seedContainer.prefabs[idx].GetComponent<Seed>().seedData.seedPrice * count)
        {
            // 음향
            audioManager.SetSFX(SFX.RETURN);

            Debug.Log("돈 없어!!!");
            return;
        }

        // 구매하려는 씨앗의 개수만큼 InventoryItem 구조체의 인스턴스를 만들기..
        InventoryItem tempItem = new InventoryItem()
        {
            item = seedItems[idx],
            quantity = count,
        };
        inventoryController.AddItem(tempItem); // 새로 생성한 인벤토리 아이템을 인벤토리 데이터에 추가해주기..


        // 음향
        audioManager.SetSFX(SFX.COMPLETE);
        GameManager.instance.money -= seedContainer.prefabs[idx].GetComponent<Seed>().seedData.seedPrice * count; // 가진 돈에서 차감!
    }

    public void SellFruit(int count, int idx)
    {
        // 만약 판매하려고 하는 과일의 개수가 현재 과일의 개수보다 적으면 그냥 빠져나가도록..
        if (fruitContainer.fruitCount[idx] < count)
        {
            // 음향
            audioManager.SetSFX(SFX.RETURN);

            Debug.Log("과일이 부족해!!!");
            return;
        }

        // 판매하려는 과일의 개수만큼 InventoryItem 구조체의 인스턴스를 만들기..
        InventoryItem tempItem = new InventoryItem()
        {
            item = fruitItems[idx],
            quantity = count,
        };


        inventoryController.MinusItem(tempItem); // 새로 생성한 인벤토리 아이템을 인벤토리 데이터에서 빼주기..



        // 음향
        audioManager.SetSFX(SFX.COMPLETE);

        GameManager.instance.money += fruitItems[idx].fruitPrice * count; // 가진 돈에 더하기!

        PlayerPrefs.SetInt("money", GameManager.instance.money); // 현재 돈 저장
    }

    public void SetFarmingData(Vector3Int pos)
    {
        // 이 함수는 FarmingManager 클래스의 Start 함수와 UpgradeFarmSize 함수에서 사용할 것..

        // 딕셔너리에 이미 현재 등록하려는 타일이 존재하면 걍 빠져나가도록..
        if (farmingData.ContainsKey(pos)) return;


        // 아니면 딕셔너리에 등록
        // 유니티에서는 new 를 쓰려면 class 가 MonoBehaviour 를 상속 받으면 안 됨.
        farmingData[pos] = new FarmingData();
        farmingData[pos].SetData();
        farmingData[pos].buttons = new Button[buttonPrefabs.Length]; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼, [3]: failed 버튼

        // 각 타일마다 네 개의 버튼을 가지고 시작하도록..
        for (int i = 0; i < buttonPrefabs.Length; i++)
        {
            // 클로저 문제를 피하기 위해서 값을 변수에 저장해놓고 이 변수를 사용함..
            int index = i;
            Vector3Int tilePos = pos;
            farmingData[pos].buttons[i] = CreateButton(index, tilePos).GetComponent<Button>();

            if (index == 0)
            {
                // 버튼에 함수를 저장해놓음(tilePos 도 같이 저장해놓기)
                farmingData[tilePos].buttons[index].onClick.AddListener(() => PlowTile(tilePos));
            }
            else if (index == 1)
            {
                //farmingData[tilePos].buttons[index].onClick.AddListener(() => PlantTile(tilePos));
                farmingData[tilePos].buttons[index].onClick.AddListener(() => OpenPlantSeedPanel(tilePos)); // 씨앗 선택창 화면에 띄우는 함수 연결시키기
            }
            else if (index == 2)
            {
                farmingData[tilePos].buttons[index].onClick.AddListener(() => HarvestTile(tilePos));
            }
            else if (index == 3)
            {
                farmingData[tilePos].buttons[index].onClick.AddListener(() => FailedTile(tilePos));
            }
        }

        // 맨 처음에는 plow 버튼을 저장하고 있도록
        farmingData[pos].stateButton = farmingData[pos].buttons[0];
    }


    public void SetFarmSize()
    {
        // Awake 함수에서 호출할 함수
        // 불러온 농장 레벨에 따라 호출 횟수가 달라짐..

        // 땅의 크기를 업그레이드 하는 함수


        BoundsInt bounds = farmEnableZoneTilemap.cellBounds; // 타일이 그려진 부분만 가져오는 건 줄 알았는데 아니었다.. 이거 가져온 후 직접 가공해줘야 한다..


        // 가공 로직.. 타일이 있는 위치 중 최소와 최대를 찾아준다..
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (farmEnableZoneTilemap.HasTile(pos))
            {
                // 최소 좌표 갱신
                min = Vector3Int.Min(min, pos);
                // 최대 좌표 갱신
                max = Vector3Int.Max(max, pos);
            }
        }

        // 타일이 있는 영역을 기준으로 새 BoundsInt 생성
        // min을 빼는 것은 최소 좌표와 최대 좌표 사이의 크기를 구하기 위함..
        // Vector3Int.one을 더해주는 것은 그 크기에 마지막 좌표를 포함하기 위함..
        BoundsInt adjustedBounds = new BoundsInt(min, max - min + Vector3Int.one);

        Debug.Log("조정된 Bounds: " + adjustedBounds);


        int minX = adjustedBounds.xMin - expansionSize;
        int maxX = adjustedBounds.xMax + expansionSize;
        int minY = adjustedBounds.yMin - expansionSize;
        int maxY = adjustedBounds.yMax + expansionSize;

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                // 테투리 부분만 경계타일 까는 로직
                // max 값은 1 이 더 더해져있기 때문에 이를 고려해서 조건식 짜야함.
                // 그래서 maxX, maxY 일 때는 i, j 에 1 을 더해줌..
                if (i == minX || i + 1 == maxX)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), grassTile);
                if (j == minY || j + 1 == maxY)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), grassTile);

                Vector3Int pos = new Vector3Int(i, j, 0);

                // 농사 가능 구역 타일맵에 타일이 없으면 진입
                if (!farmEnableZoneTilemap.HasTile(pos))
                {
                    farmEnableZoneTilemap.SetTile(pos, grassTile);
                }
            }
        }




        bounds = farmEnableZoneTilemap.cellBounds; // 타일이 그려진 부분만 가져오는 건 줄 알았는데 아니었다.. 이거 가져온 후 직접 가공해줘야 한다..

        // 가공 로직.. 타일이 있는 위치 중 최소와 최대를 찾아준다..
        min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (farmEnableZoneTilemap.HasTile(pos))
            {
                // 최소 좌표 갱신
                min = Vector3Int.Min(min, pos);
                // 최대 좌표 갱신
                max = Vector3Int.Max(max, pos);
            }
        }

        adjustedBounds = new BoundsInt(min, max - min + Vector3Int.one);
        Debug.Log("조정된 Bounds: " + adjustedBounds);

        minX = adjustedBounds.xMin - 1;
        maxX = adjustedBounds.xMax + 1;
        minY = adjustedBounds.yMin - 1;
        maxY = adjustedBounds.yMax + 1;

        Debug.Log("maxX: " + maxX + " maxY: " + maxY);
        Debug.Log("minX: " + minX + " minY: " + minY);

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                // 테투리 부분만 경계타일 까는 로직
                // max 값은 1 이 더 더해져있기 때문에 이를 고려해서 조건식 짜야함.
                // 그래서 maxX, maxY 일 때는 i, j 에 1 을 더해줌..
                if (i == minX || i + 1 == maxX)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
                if (j == minY || j + 1 == maxY)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
            }
        }


        // 농사 가능 구역 타일맵의 타일들을 모두 돌면서..
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            SetFarmingData(pos); // 새로운 농사 가능 구역의 타일 정보를 딕셔너리에 저장..
        }
    }

    public void UpgradeFarmSize()
    {
        // 농장 레벨은 5렙까지 존재하도록..
        // 5 에서 더 업그레이드 하려고 하면 그냥 빠져나가도록..
        if (farmLevel >= 5) {
            // 음향
            audioManager.SetSFX(SFX.RETURN);
            return;
        }

        switch (farmLevel)
        {
            case 1:
                farmSizeLevelUpCost = 10000;
                break;
            case 2:
                farmSizeLevelUpCost = 30000;
                break;
            case 3:
                farmSizeLevelUpCost = 50000;
                break;
            case 4:
                farmSizeLevelUpCost = 100000;
                break;
        }


        // 돈 부족하면 그냥 빠져나가도록..
        if (GameManager.instance.money < farmSizeLevelUpCost)
        {
            // 음향
            audioManager.SetSFX(SFX.RETURN);

            Debug.Log("돈 없어!");
            return;
        }


        // 땅의 크기를 업그레이드 하는 함수

        BoundsInt bounds = farmEnableZoneTilemap.cellBounds; // 타일이 그려진 부분만 가져오는 건 줄 알았는데 아니었다.. 이거 가져온 후 직접 가공해줘야 한다..


        // 가공 로직.. 타일이 있는 위치 중 최소와 최대를 찾아준다..
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (farmEnableZoneTilemap.HasTile(pos))
            {
                // 최소 좌표 갱신
                min = Vector3Int.Min(min, pos);
                // 최대 좌표 갱신
                max = Vector3Int.Max(max, pos);
            }
        }

        // 타일이 있는 영역을 기준으로 새 BoundsInt 생성
        // min을 빼는 것은 최소 좌표와 최대 좌표 사이의 크기를 구하기 위함..
        // Vector3Int.one을 더해주는 것은 그 크기에 마지막 좌표를 포함하기 위함..
        BoundsInt adjustedBounds = new BoundsInt(min, max - min + Vector3Int.one);

        Debug.Log("조정된 Bounds: " + adjustedBounds);


        int minX = adjustedBounds.xMin - expansionSize;
        int maxX = adjustedBounds.xMax + expansionSize;
        int minY = adjustedBounds.yMin - expansionSize;
        int maxY = adjustedBounds.yMax + expansionSize;

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                // 경계 테두리 일반 타일 모습으로 바꿔주기..
                // max 값은 1 이 더 더해져있기 때문에 이를 고려해서 조건식 짜야함.
                // 그래서 maxX, maxY 일 때는 i, j 에 1 을 더해줌..
                if (i == minX || i + 1 == maxX)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), grassTile);
                if (j == minY || j + 1 == maxY)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), grassTile);

                Vector3Int pos = new Vector3Int(i, j, 0);

                // 농사 가능 구역 타일맵에 타일이 없으면 진입
                if (!farmEnableZoneTilemap.HasTile(pos))
                {
                    farmEnableZoneTilemap.SetTile(pos, grassTile);
                }
            }
        }


        // 경계 타일맵 깔기 위한 로직
        bounds = farmEnableZoneTilemap.cellBounds; // 타일이 그려진 부분만 가져오는 건 줄 알았는데 아니었다.. 이거 가져온 후 직접 가공해줘야 한다..

        // 가공 로직.. 타일이 있는 위치 중 최소와 최대를 찾아준다..
        min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (farmEnableZoneTilemap.HasTile(pos))
            {
                // 최소 좌표 갱신
                min = Vector3Int.Min(min, pos);
                // 최대 좌표 갱신
                max = Vector3Int.Max(max, pos);
            }
        }

        adjustedBounds = new BoundsInt(min, max - min + Vector3Int.one);
        Debug.Log("조정된 Bounds: " + adjustedBounds);

        minX = adjustedBounds.xMin - 1;
        maxX = adjustedBounds.xMax + 1;
        minY = adjustedBounds.yMin - 1;
        maxY = adjustedBounds.yMax + 1;

        Debug.Log("maxX: " + maxX + " maxY: " + maxY);
        Debug.Log("minX: " + minX + " minY: " + minY);

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                // 테투리 부분만 경계타일 까는 로직
                // max 값은 1 이 더 더해져있기 때문에 이를 고려해서 조건식 짜야함.
                // 그래서 maxX, maxY 일 때는 i, j 에 1 을 더해줌..
                if (i == minX || i + 1 == maxX)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
                if (j == minY || j + 1 == maxY)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
            }
        }


        // 농사 가능 구역 타일맵의 타일들을 모두 돌면서..
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            // 타일이 있는 위치만 딕셔
            if (farmEnableZoneTilemap.HasTile(pos))
                SetFarmingData(pos); // 새로운 농사 가능 구역의 타일 정보를 딕셔너리에 저장..
        }


        farmLevel++; // 농장 레벨 증가

        // 음향
        audioManager.SetSFX(SFX.UPGRADE);

        PlayerPrefs.SetInt("FarmLevel", farmLevel); // 농장 레벨 저장..
        Debug.Log("농장을 업그레이드 했다!");
    }

    public void SetScareCrowLevel(int level)
    {
        // 게임 시작시 데이터 불러올 때 호출될 함수..

        switch (level)
        {
            case 1:
                scarecrowLevelUpCost = 10000;
                break;
            case 2:
                scarecrowLevelUpCost = 30000;
                break;
            case 3:
                scarecrowLevelUpCost = 50000;
                break;
            case 4:
                scarecrowLevelUpCost = 100000;
                break;
        }

        Debug.Log("허수아비 정보를 세팅했다!");
    } 

    public void UpgradeScareCrow()
    {
        // 허수아비 레벨은 5렙까지 존재하도록..
        // 5 에서 더 업그레이드 하려고 하면 그냥 빠져나가도록..
        if (scarecrowLevel >= 5) {
            // 음향
            audioManager.SetSFX(SFX.RETURN);
            return; 
        }

        switch (scarecrowLevel)
        {
            case 1:
                scarecrowLevelUpCost = 10000;
                break;
            case 2:
                scarecrowLevelUpCost = 30000;
                break;
            case 3:
                scarecrowLevelUpCost = 50000;
                break;
            case 4:
                scarecrowLevelUpCost = 100000;
                break;
        }


        if (GameManager.instance.money < scarecrowLevelUpCost)
        {
            // 음향
            audioManager.SetSFX(SFX.RETURN);

            Debug.Log("돈 없어!");
            return;
        }

        scarecrowLevel++;


        // 음향
        audioManager.SetSFX(SFX.UPGRADE);


        PlayerPrefs.SetInt("ScareCrowLevel", scarecrowLevel); // 허수아비 레벨 저장..
        Debug.Log("허수아비를 업그레이드 했다!");
    }

    public void SetFarmUpgradePanel()
    {
        // 농장 레벨 업그레이드 판넬 정보 설정 함수


        // 농장 사이즈 정보 설정
        switch (farmLevel)
        {
            case 1:
                farmSizeLevelUpCost = 10000;
                break;
            case 2:
                farmSizeLevelUpCost = 30000; 
                break;
            case 3:
                farmSizeLevelUpCost = 50000;
                break;
            case 4:
                farmSizeLevelUpCost = 100000;
                break;
        }

        if (farmLevel == 5)
        {
            farmSizeUpgradeCostText.text = "max";
            nextFarmSizeLevelText.text = "X";
        } 
        else
        {
            farmSizeUpgradeCostText.text = farmSizeLevelUpCost + "";
            nextFarmSizeLevelText.text = (farmLevel + 1) + "";
        }
        curFarmSizeLevelText.text = farmLevel + ""; // 현재 농장 사이즈 레벨 텍스트



        // 허수아비 정보 설정
        switch (scarecrowLevel)
        {
            case 1:
                scarecrowLevelUpCost = 10000;
                break;
            case 2:
                scarecrowLevelUpCost = 30000;
                break;
            case 3:
                scarecrowLevelUpCost = 50000;
                break;
            case 4:
                scarecrowLevelUpCost = 100000;
                break;
        }


        if (scarecrowLevel == 5)
        {
            scareCrowUpgradeCostText.text = "max";
            nextScareCrowLevelText.text = "X";
        }
        else
        {
            scareCrowUpgradeCostText.text = scarecrowLevelUpCost + "";
            nextScareCrowLevelText.text = (scarecrowLevel + 1) + "";
        }
        curScareCrowLevelText.text = scarecrowLevel + ""; // 현재 허수아비 레벨 텍스트
    }

    public void DisableFarmStateButton()
    {
        // 농장 씬에서만 사용해야 하는 함수라서.. 조건문 달아준 것..
        // UIInventoryManager 의 seedContainer 와 fruitContainer 의 값이 null 이면 현재 농장이 아닌 다른 씬에 있는 것..
        if (UIInventoryManager.instance.seedContainer != null && UIInventoryManager.instance.fruitContainer != null)
        {
            // 농사 버튼 끄는 함수
            buttonsGameObject.SetActive(false);
        }
    }

    public void EnableFarmStateButton()
    {
        // 농장 씬에서만 사용해야 하는 함수라서.. 조건문 달아준 것..
        // UIInventoryManager 의 seedContainer 와 fruitContainer 의 값이 null 이면 현재 농장이 아닌 다른 씬에 있는 것..
        if (UIInventoryManager.instance.seedContainer != null && UIInventoryManager.instance.fruitContainer != null)
        {
            // 농사 버튼 켜는 함수
            buttonsGameObject.SetActive(true);
        }
    }

    void OnFarmSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 완전히 로드될 때까지 기다린 후 코루틴 시작..
        StartCoroutine(InitializeAfterSceneLoad());
    }

    private IEnumerator InitializeAfterSceneLoad()
    {
        yield return null;

        // 허수아비 레벨 데이터 불러오기..
        scarecrowLevel = PlayerPrefs.GetInt("ScareCrowLevel");
        Debug.Log(scarecrowLevel + "허수아비 레벨임여!!!!!!!!!!!!!!!!!!!!!!!!!!");
        // 허수아비 레벨 데이터를 불러온 다음에 레벨 데이터에 맞게끔 정보 설정해주기..
        SetScareCrowLevel(scarecrowLevel);


        int probability = 1;
        switch (scarecrowLevel)
        {
            case 1:
                probability = 2;
                break;
            case 2:
                probability = 3;
                break;
            case 3:
                probability = 5;
                break;
            case 4:
                probability = 10;
                break;
            case 5:
                probability = 20;
                break;
        }

        Debug.Log("probability: " + probability);

        if (Random.Range(0, probability) == 0)
        {
            StartFarmAnimalBGM(); // 동물 게임 활성화 된거니까 그에 맞는 배경음 틀어주기..

            // 델리게이트에 아무것도 연결 안 되어 있을 때에만 함수 연결해줄 것..
            if (animalInteractionManager.OnAnimalGameClosed == null)
                animalInteractionManager.OnAnimalGameClosed += StartFarmBasicBGM; // bgm 시작하는 함수 연결..

            animalInteractionManager.UICanvas.gameObject.SetActive(true);
            animalInteractionManager.backgroundButton.gameObject.SetActive(true);

            // 여기 매개변수로 전해지는 값은 현재 허수아비 레벨에 따라 달라지도록 하는게 좋을 것 같다..
            // 허수아비 레벨 올라갈수록 수가 작아지도록..
            // 일단은 임의로 잡아야하는 너구리의 수 15 마리로 해놓음..
            // 음.. 너구리 수는 그냥 15 마리로 고정하는게 나은가..
            animalInteractionManager.SetAnimalCount(15);
        }
        else
        {
            animalInteractionManager.UICanvas.gameObject.SetActive(false);
            UIInventoryManager.instance.buttonParentGameObject.SetActive(true);
            StartFarmBasicBGM(); // bgm 시작..
        }


        CheckFailedFarm(); // 망한 땅 상태 반영..
    }


    public void StartFarmAnimalBGM()
    {
        // 이 함수는 동물 게임이 활성화 됐을 때 호출할 함수..

        audioManager.SetBGM(BGM.FARMANIMAL); // bgm 을 농장 동물 게임에 맞는 음악으로 변경..
        audioManager.bgmPlayer.Play();
    }

    public void StartFarmBasicBGM()
    {
        // 이 함수는 AnimalinteractionManager 의 OnAnimalGameClosed 델리게이트에 연결해줄것..

        audioManager.SetBGM(BGM.FARMBASIC); // bgm 을 농장 기본에 맞는 음악으로 변경..
        audioManager.bgmPlayer.Play();
    }


    public void SaveFarmingData()
    {
        Dictionary<PosInt, SaveFarmingData> tempDic = new Dictionary<PosInt, SaveFarmingData>();
        foreach (var item in farmingData)
        {
            // JSON 저장할 때 Vector3Int 가 직렬화가 안되므로 따로 만든 PosString 이용하가ㅣ..
            PosInt pos = new PosInt
            {
                x = item.Key.x,
                y = item.Key.y,
                z = item.Key.z
            };

            SaveFarmingData temp = new SaveFarmingData
            {
                plowEnableState = farmingData[item.Key].plowEnableState,
                plantEnableState = farmingData[item.Key].plantEnableState,
                harvestEnableState = farmingData[item.Key].harvestEnableState,
                failedState = farmingData[item.Key].failedState,
                currentState = farmingData[item.Key].currentState
            };



            // 이렇게 바꿀거임..

            // 농사 땅 위에 씨앗이 없을 때 진입..
            if (farmingData[item.Key].seedOnTile == false)
            {
                temp.seedOnTile = farmingData[item.Key].seedOnTile;
            }
            // 농사 땅 위에 씨앗 있을 때 진입..
            else
            {
                temp.seedOnTile = farmingData[item.Key].seedOnTile;
                temp.seedIdx = farmingData[item.Key].seedIdx;
                temp.curDay = farmingData[item.Key].curDay;
                temp.isGrown = farmingData[item.Key].isGrown;
            }
            tempDic.Add(pos, temp);
        }


        Debug.Log("저장할 땅 개수 " + tempDic.Count);
        string json = DictionaryJsonUtility.ToJson(tempDic, true);
        Debug.Log(json);
        Debug.Log("데이터 저장 완료!");


        // 외부 폴더에 접근해서 Json 파일 저장하기
        // Application.persistentDataPath: 특정 운영체제에서 앱이 사용할 수 있도록 허용한 경로
        File.WriteAllText(farmingDataFilePath, json);
    }


    // 씬 로드 된 후에 SetFarmingData 함수 먼저 호출한 후 호출할 함수..
    public void LoadFarmingData()
    {
        // Json 파일 경로 가져오기
        string path = Path.Combine(Application.persistentDataPath, farmingDataFilePath);

        // 지정된 경로에 파일이 있는지 확인한다
        if (File.Exists(path))
        {
            // 경로에 파일이 있으면 Json 을 다시 오브젝트로 변환한다.
            string json = File.ReadAllText(path);
            Debug.Log(json);
            Dictionary<PosInt, SaveFarmingData> tempDic = DictionaryJsonUtility.FromJson<PosInt, SaveFarmingData>(json);

            Debug.Log("가져온 땅의 개수입니다!! " + tempDic.Count);

            foreach (var item in tempDic)
            {
                Vector3Int pos = new Vector3Int(item.Key.x, item.Key.y, item.Key.z);

                switch (tempDic[item.Key].currentState)
                {
                    // 현재 농사 땅 상태에 맞는 버튼으로 설정해주기..

                    case "None":
                        farmingData[pos].stateButton = farmingData[pos].buttons[0];
                        farmTilemap.SetTile(pos, grassTile); // 타일을 아무것도 안 한 상태로 변경(키 값이 농사땅의 pos 임)
                        break;

                    case "plow":
                        farmingData[pos].stateButton = farmingData[pos].buttons[1];
                        farmTilemap.SetTile(pos, farmTile); // 타일을 밭 갈린 모습으로 변경..
                        break;

                    case "plant":
                        farmingData[pos].stateButton = farmingData[pos].buttons[2];
                        farmTilemap.SetTile(pos, plantTile); // 타일을 씨앗 심은 모습으로 변경..
                        break;

                    case "harvest":
                        farmingData[pos].stateButton = farmingData[pos].buttons[2];
                        farmTilemap.SetTile(pos, harvestTile); // 타일을 다 자란 모습으로 변경..
                        break;

                    case "failed":
                        farmingData[pos].stateButton = farmingData[pos].buttons[3];
                        farmTilemap.SetTile(pos, failedTile); // 타일을 망한 모습으로 변경..
                        break;
                }


                // 저장해놓은 데이터 가져와서 설정해주기..
                farmingData[pos].plowEnableState = tempDic[item.Key].plowEnableState;
                farmingData[pos].plantEnableState = tempDic[item.Key].plantEnableState;
                farmingData[pos].harvestEnableState = tempDic[item.Key].harvestEnableState;
                farmingData[pos].failedState = tempDic[item.Key].failedState;
                farmingData[pos].currentState = tempDic[item.Key].currentState;


                // 이렇게 수정할거임!!
                // 저장 당시 농사 땅 위에 씨앗 있었으면 씨앗 데이터 설정해주기..
                if (tempDic[item.Key].seedOnTile)
                {
                    farmingData[pos].seedOnTile = tempDic[item.Key].seedOnTile;
                    farmingData[pos].seedIdx = tempDic[item.Key].seedIdx;
                    farmingData[pos].curDay = tempDic[item.Key].curDay;
                    farmingData[pos].isGrown = tempDic[item.Key].isGrown;
                }
            }
        }
        // 지정된 경로에 파일이 없으면
        else
        {
            Debug.Log("파일이 없어요!!");
        }
    }
}