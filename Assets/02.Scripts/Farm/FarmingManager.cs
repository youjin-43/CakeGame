using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


// 타일이 가지는 농사 데이터
class FarmingData
{
    public Seed seed; // 타일이 가지는 씨앗 정보
    //public bool seedOnTile; // 타일 위에 씨앗이 있는지 여부 확인용(씨앗이 있으면 밭을 갈 수 없음)
    public bool plowEnableState; // 밭을 갈 수 있는 상태인지 여부 확인용(밭이 안 갈린 상태)
    public bool plantEnableState; // 씨앗을 심을 수 있는 상태인지 여부 확인용
    public bool harvestEnableState; // 작물이 다 자란 상태인지 여부 확인용
    public Button stateButton; // 타일을 누르면 타일 위에 뜨도록 하는 버튼
    public Button[] buttons; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼

    public string currentState = "None"; // 현재 상태(초기에는 아무것도 안 한 상태니까 None 으로.. -> plow: 밭 갈린 상태, plant: 씨앗 심은 상태, harvest: 다 자란 상태)
}


public class FarmingManager : MonoBehaviour
{
    [Header("Game Data")]
    public Camera mainCamera; // 마우스 좌표를 게임 월드 좌표로 변환하기 위해 필요한 변수(카메라 오브젝트 할당해줄 것)
    public SeedContainer seedContainer; // 현재 가진 씨앗을 가져오기 위해 필요한 변수(씨앗 컨테이너 게임 오브젝트 할당해줄 것)
    public FruitContainer fruitContainer; // 수확한 과일을 저장하기 위해 필요한 변수(과일 컨테이너 게임 오브젝트 할당해줄 것)

    [Header("Tile")]
    public TileBase borderTile; // 제한 구역 상태
    public TileBase grassTile; // 밭 갈기 전 상태
    public TileBase farmTile; // 밭 간 후 상태
    public TileBase plantTile; // 씨앗 심은 후 상태
    public TileBase harvestTile; // 과일 다 자란 상태
    public Vector3Int prevSelectTile; // 이전 클릭된 타일

    [Header("Tilemap")]
    public Tilemap farmEnableZoneTilemap; // 농사 가능 부지를 나타내는 타일맵
    public Tilemap farmTilemap; // 진짜로 현재 타일의 상태에 따라 타일이 변경되는 타일맵

    [Header("Farm interaction Button")]
    // 버튼을 프리팹으로 만들어 놓은 다음 동적으로 생성해서 쓸 것.
    public GameObject[] buttonPrefabs; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼
    public GameObject buttonParent; // 버튼 생성할 때 부모 지정하기 위한 변수

    [Header("Farm interaction Panel")]
    public GameObject growTimePanel; // 다 자라기까지 남은 시간 보여주는 판넬
    public Text growTimeText; // 다 자라기까지 남은 시간

    [Header("Farming Data")]
    public Vector2 clickPosition; // 현재 마우스 위치를 게임 월드 위치로 바꿔서 저장
    public Vector3Int cellPosition; // 게임 월드 위치를 타일 맵의 타일 셀 위치로 변환
    Dictionary<Vector3Int, FarmingData> farmingData;
    public int FarmLevel = 0; // 농장 레벨. 농장 레벨 업그레이드 함수 호출하면 증가하도록..
    public int expansionSize = 1; // 농장 한 번 업그레이트 할 때 얼마나 확장될 건지.. 일단 임시로 1로 해놨다.. 나중에 변경할 것.

    [Header("PlantSeed Information")]
    public GameObject plantSeedPanel; // 씨앗 선택창
    public int selectedSeedIdx; // 현재 심을 씨앗 종류
    public bool clickedSelectedSeedButton = false; // 이 값이 true 가 되면 씨앗 심기 함수 호출하도록(씨앗 심기 함수에서는 이 값을 다시 false 로 돌림).. 


    private void Awake()
    {
        farmingData = new Dictionary<Vector3Int, FarmingData>();
        clickPosition = Vector2.zero;
    }

    private void Start()
    {
        // 농사 가능 구역만 farmingData 에 저장할 것임.
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            if (!farmEnableZoneTilemap.HasTile(pos)) continue;

            SetFarmingData(pos); // FarmingData 타입 인스턴스의 정보를 세팅해주는 함수.
        }
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


        GrowTimeUpdate(); // 과일이 다 자라기까지 남은 시간 업데이트 해주는 함수..
        CheckGrowedFruit(); // 과일이 다 자랐는지 확인하고, 다 자랐으면 그에 맞는 행동을 하도록 해주는 함수..


        // 씨앗 선택창에서 버튼 클릭하면 진입하도록..
        if (clickedSelectedSeedButton)
        {
            // 씨앗 심는 함수 호출
            PlantTile(cellPosition, selectedSeedIdx);
        }
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
                    if (farmingData[cellPosition].seed == null || (farmingData[cellPosition].seed.isGrown))
                    {
                        farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                    }
                    // 씨앗이 자라는 중이면 남은 시간 나타내는 판넬 뜨도록
                    else if (!farmingData[cellPosition].seed.isGrown)
                    {
                        // 판넬 위치를 현재 클릭한 타일 위치로..
                        growTimePanel.transform.position = mainCamera.WorldToScreenPoint(farmTilemap.CellToWorld(cellPosition)) + new Vector3(0, 50, 0);
                        growTimePanel.SetActive(true);
                        growTimeText.text = "남은시간\n" + (int)(farmingData[cellPosition].seed.growTime - farmingData[cellPosition].seed.currentTime);
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
                        if (farmingData[cellPosition].seed == null || (farmingData[cellPosition].seed.isGrown))
                        {
                            farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                        }
                        // 씨앗이 자라는 중이면 남은 시간 나타내는 판넬 뜨도록
                        else if (!farmingData[cellPosition].seed.isGrown)
                        {
                            // 판넬 위치를 현재 클릭한 타일 위치로..
                            growTimePanel.transform.position = mainCamera.WorldToScreenPoint(farmTilemap.CellToWorld(cellPosition)) + new Vector3(0, 50, 0);
                            growTimePanel.SetActive(true);
                            growTimeText.text = "남은시간\n" + (int)(farmingData[cellPosition].seed.growTime - farmingData[cellPosition].seed.currentTime);
                        }
                    }
                }
            }

            prevSelectTile = cellPosition; // 지금 누른 타일을 이전에 누른 타일 위치를 저장하는 변수에 저장..
        }
    }

    private void GrowTimeUpdate()
    {
        // 자라는데 남은 시간이 계속 업데이트 되어야 하므로..
        if (farmEnableZoneTilemap.HasTile(cellPosition) && farmingData[cellPosition].seed != null)
        {
            if (!farmingData[cellPosition].seed.isGrown)
                growTimeText.text = "남은시간\n" + (int)(farmingData[cellPosition].seed.growTime - farmingData[cellPosition].seed.currentTime);
            else
                growTimePanel.SetActive(false); // 다 자라면 남은시간 나타내는 판넬 꺼지도록..
        }
    }

    private void CheckGrowedFruit()
    {
        foreach (Vector3Int pos in farmingData.Keys)
        {
            if (farmingData[pos].seed != null)
            {
                if (farmingData[pos].seed.isGrown)
                {
                    farmTilemap.SetTile(pos, harvestTile); // 타일을 과일이 다 자란 상태로 변경
                    farmingData[pos].harvestEnableState = true; // 작물 수확할 수 있는 상태
                    farmingData[pos].stateButton.gameObject.SetActive(true); // 수확하기 버튼은 항상 떠있어야 함
                    farmingData[pos].currentState = "harvest";
                }
            }
        }
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
    }

    public void OpenPlantSeedPanel(Vector3Int pos)
    {
        // 심기 버튼이랑 연결해줘야함.
        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..

        plantSeedPanel.SetActive(true); // 심기 버튼 눌렀을 때 씨앗 선택창 뜨도록 하기 위함
    }

    public void PlantTile(Vector3Int pos, int seedIdx)
    {
        // 씨앗을 심는 함수
        // 이 함수는 씨앗 선택창에서 씨앗 버튼 눌렀을 때 호출되도록..

        farmingData[pos].seed = seedContainer.GetSeed(seedIdx).GetComponent<Seed>();

        farmTilemap.SetTile(pos, plantTile); // 타일 모습을 씨앗 심은 상태로 바꿔주기
        farmingData[pos].plantEnableState = true; // 씨앗을 심을 수 없는 상태를 나타내기 위해 false 로 변경
        farmingData[pos].currentState = "plant"; // 씨앗 심은 상태니까 plant 로 바꿔주기

        farmingData[pos].stateButton = farmingData[pos].buttons[2]; // harvest 버튼을 가지고 있도록..

        clickedSelectedSeedButton = false; // 한 번 심고 난 다음에 바로 변수값 false 로 바꿔주기
    }

    public void HarvestTile(Vector3Int pos)
    {
        // 과일을 수확하는 함수
        // 생각해보니까 씨앗 인덱스 여기로 안 보내줘도 pos 보내줬으니까, pos 가 가지는 씨앗 인스턴스의 씨앗 인덱스 이용하면 될 듯.

        farmingData[pos].plowEnableState = true;
        farmingData[pos].currentState = "None"; // 과일을 수확한 상태니까 None 으로 바꿔주기

        fruitContainer.GetFruit(farmingData[pos].seed.seedIdx); // 씨앗의 인덱스와 같은 과일 생성
        fruitContainer.fruitCount[farmingData[pos].seed.seedIdx]++; // 씨앗의 인덱스와 같은 과일의 수 증가시키기

        farmingData[pos].stateButton.gameObject.SetActive(false); // 버튼 한 번 눌렀으니까 꺼지도록..
        farmingData[pos].stateButton = farmingData[pos].buttons[0]; // plow 버튼을 가지고 있도록..

        farmingData[pos].seed = null; // 수확 완료 했으니까 타일의 seed 변수를 다시 null 로 설정해주기..

        farmTilemap.SetTile(pos, grassTile); // 타일 모습을 초기 상태의로 바꿔주기
    }


    public void BuySeed(int count, int idx)
    {
        // 돈이 부족하면 씨앗 못사!
        if (GameManager.instance.money < seedContainer.prefabs[idx].GetComponent<Seed>().seedPrice * count) {
            Debug.Log("돈 없어!!!");
            return; 
        }

        seedContainer.seedCount[idx] += count; // 씨앗의 개수를 저장하고 있는 배열의 인덱스 요소에 구매한 씨앗의 개수만큼 더해주기
        GameManager.instance.money -= seedContainer.prefabs[idx].GetComponent<Seed>().seedPrice * count; // 가진 돈에서 차감!
    }

    public void SellFruit(int count, int idx)
    {
        // 만약 판매하려고 하는 과일의 개수가 현재 과일의 개수보다 적으면 그냥 빠져나가도록..
        if (fruitContainer.fruitCount[idx] < count) {
            Debug.Log("과일이 부족해!!!");
            return;
        } 

        fruitContainer.fruitCount[idx] -= count; // 판매할 과일의 수만큼 과일 컨테이너에서 빼주기
        GameManager.instance.money += fruitContainer.prefabs[idx].GetComponent<Fruit>().fruitPrice * count; // 가진 돈에 더하기!

        for (int i=0; i<count; i++)
        {
            fruitContainer.pools[idx][i].SetActive(false); // 판매한 과일 수만큼 과일 활성화 끄기..
        }

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
        farmingData[pos].buttons = new Button[3]; // [0]: plow 버튼, [1]: plant 버튼, [2]: harvest 버튼

        // 각 타일마다 세 개의 버튼을 가지고 시작하도록..
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
        }

        // 맨 처음에는 plow 버튼을 저장하고 있도록
        farmingData[pos].stateButton = farmingData[pos].buttons[0];
    }


    public void UpgradeFarmSize()
    {
        // 일단 임시로 만원으로 해놨다..
        if (GameManager.instance.money < 10000)
        {
            Debug.Log("돈 없어!");
            return;
        }

        // 땅의 크기를 업그레이드 하는 함수

        BoundsInt bounds = farmEnableZoneTilemap.cellBounds; // 농사 가능 구역 타일맵의 현재 크기 가져오기

        // 새로 확장할 영역 좌표 계산 로직..
        Debug.Log(bounds.xMin);

        int minX = bounds.xMin - expansionSize;
        int maxX = bounds.xMax + expansionSize;
        int minY = bounds.yMin - expansionSize;
        int maxY = bounds.yMax + expansionSize;

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

        
        // 경계 타일맵 깔기 위한 로직
        bounds = farmEnableZoneTilemap.cellBounds; // 업데이트된 농사 가능 구역 타일맵의 현재 크기 가져오기
        minX = bounds.xMin - 1;
        maxX = bounds.xMax + 1;
        minY = bounds.yMin - 1;
        maxY = bounds.yMax + 1;

        Debug.Log("maxX: " + maxX + " maxY: " + maxY);
        Debug.Log("minX: " + minX + " minY: " + minY);

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                // 테투리 부분만 경계타일 까는 로직
                // max 값은 1 이 더 더해져있기 때문에 이를 고려해서 조건식 짜야함.
                // 그래서 maxX, maxY 일 때는 i, j 에 1 을 더해줌..
                if (i == minX || i+1 == maxX)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
                if (j == minY || j+1 == maxY)
                    farmTilemap.SetTile(new Vector3Int(i, j, 0), borderTile);
            }
        }


        // 농사 가능 구역 타일맵의 타일들을 모두 돌면서..
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            SetFarmingData(pos); // 새로운 농사 가능 구역의 타일 정보를 딕셔너리에 저장..
        }

        FarmLevel++; // 농장 레벨 증가
        Debug.Log("농장을 업그레이드 했다!");
    }
}
