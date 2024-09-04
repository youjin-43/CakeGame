using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Inventory.Model;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// 케이크 전반적인 관리 및 다른 매니저 참조
public class CakeManager : MonoBehaviour
{
    public CakeShowcaseController cakeShowcaseController;
    public CakeMakerController cakeMakerController;
    public CakeUIController cakeUIController;
    public AudioManager audioManager;
    public AudioManager.SFX openSFX;
    public List<CakeSO> cakeSODataList;    // 케이크 데이터를 저장하는 리스트 (Unity Editor에서 설정)
    public int[] cakeCounts;             // 각 케이크의 개수를 관리하는 배열
    private string filePath;             // 케이크 데이터 저장 파일 경로
    public static CakeManager instance;
    public int TOTALCAKENUM = 6;
    public int CAKEPLACENUM = 4;
    public int[] soldCakeCount;
    public bool canClick;
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록 
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로딩될 때마다 함수를 호출하기위해
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Update()
    {
        if (IsPointerOverUIObjectPC() && canClick) { canClick = false; return; }
        if (Input.GetMouseButtonDown(0))
        {
            if (!canClick)
            {
                canClick = true;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeCakeManager();
        //CakeBack();
    }
    void InitializeCakeManager()
    {
        // 데이터 저장 파일 경로 설정
        filePath = Path.Combine(Application.persistentDataPath, "cakeData.json");
        Debug.Log("Save file path: " + filePath);

        // 케이크 데이터 로드
        LoadCakeData();
        cakeShowcaseController = FindObjectOfType<CakeShowcaseController>();
        cakeMakerController = FindObjectOfType<CakeMakerController>();
        audioManager = FindAnyObjectByType<AudioManager>();
        cakeUIController = CakeUIController.instance;
        soldCakeCount = new int[TOTALCAKENUM];
    }
    public void CakeBack()
    {
        for (int i = 0; i < TOTALCAKENUM; i++)
        {
            cakeShowcaseController.cakeShowcases[i].GetComponent<CakeShowcase>().GetBack();
        }
    }

    public void SetupButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    // 케이크 상태 초기화: 첫 번째 케이크만 잠금 해제, 나머지는 잠금
    void InitializeCakeStatus()
    {
        cakeCounts = new int[TOTALCAKENUM];

        for (int i = 0; i < cakeSODataList.Count; i++)
        {
            CakeSO cakeData = cakeSODataList[i];
            cakeCounts[i] = 0; // 초기 케이크 개수는 0으로 설정
            cakeData.isLocked = cakeData.cakeIdx != 0; // 첫 번째 케이크만 잠금 해제
        }
    }

    // 케이크 개수 증가 및 데이터 저장
    public void PlusCakeCount(int index)
    {
        if (index >= 0 && index < TOTALCAKENUM)
        {
            cakeCounts[index]++;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

    // 케이크 개수 감소 및 데이터 저장
    public void MinusCakeCount(int index)
    {
        if (index >= 0 && index < TOTALCAKENUM && cakeCounts[index] > 0)
        {
            cakeCounts[index]--;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

    // 케이크 잠금 해제 및 데이터 저장
    public void UnlockCake(int index)
    {
        if (index >= 0 && index < TOTALCAKENUM && cakeSODataList[index].isLocked)
        {
            cakeSODataList[index].isLocked = false;
            SaveCakeData();
            Debug.Log("케이크 잠금 해제됨: " + cakeSODataList[index].Name);
        }
        else
        {
            Debug.LogWarning("잠금 해제 실패: 유효하지 않은 인덱스 또는 이미 잠금 해제된 케이크.");
        }
    }
    public void ResetUnlockCake()
    {
        cakeSODataList[0].isLocked = false;
        for (int i = 1; i < TOTALCAKENUM; i++)
        {
            cakeSODataList[i].isLocked = true;
        }
        SaveCakeData();
    }

    //인테리어 클릭 효과음
    public void CallOpenAudio()
    {
        audioManager.SetSFX(openSFX);
    }


    // 케이크 데이터를 JSON 파일로 저장
    private void SaveCakeData()
    {
        CakeDataSave dataSave = new CakeDataSave
        {
            cakeDataList = SerializeCakeDataList(), // 케이크 데이터 직렬화
        };

        // JSON 파일로 저장
        string json = JsonConvert.SerializeObject(dataSave, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    // 케이크 데이터를 JSON 파일에서 로드
    private void LoadCakeData()
    {
        // 기본값으로 초기화
        cakeCounts = new int[cakeSODataList.Count];
        InitializeCakeStatus();

        if (File.Exists(filePath))
        {
            // JSON 파일이 존재할 경우 파일에서 데이터 로드
            string json = File.ReadAllText(filePath);
            CakeDataSave dataSave = JsonConvert.DeserializeObject<CakeDataSave>(json);
            DeserializeCakeDataList(dataSave.cakeDataList);
        }
        else
        {
            // JSON 파일이 없을 때 기본값으로 초기화하고 저장
            SaveCakeData();
        }
    }

    // 케이크 데이터를 직렬화하여 저장할 수 있는 형태로 변환
    private List<CakeDataSerializable> SerializeCakeDataList()
    {
        List<CakeDataSerializable> serializableList = new List<CakeDataSerializable>();

        for (int i = 0; i < cakeSODataList.Count; i++)
        {
            CakeDataSerializable serializable = new CakeDataSerializable
            {
                cakeName = cakeSODataList[i].name,
                cakeCost = cakeSODataList[i].cakeCost,
                bakeTime = cakeSODataList[i].bakeTime,
                cakePrice = cakeSODataList[i].cakePrice,
                isLocked = cakeSODataList[i].isLocked,
                cakeIdx = cakeSODataList[i].cakeIdx,
                cakeCount = cakeCounts[i],
                materialIdxs = cakeSODataList[i].materialIdxs,
                materialCounts = cakeSODataList[i].materialCounts,
                exp = cakeSODataList[i].exp
            };

            serializableList.Add(serializable);
        }

        return serializableList;
    }

    // 직렬화된 데이터를 다시 로드하여 케이크 데이터에 반영
    private void DeserializeCakeDataList(List<CakeDataSerializable> serializableList)
    {
        foreach (var serializable in serializableList)
        {
            CakeSO cakeData = cakeSODataList.Find(cake => cake.cakeIdx == serializable.cakeIdx);
            if (cakeData != null)
            {
                cakeData.isLocked = serializable.isLocked;
                cakeCounts[cakeData.cakeIdx] = serializable.cakeCount;
            }
        }
    }

    // 새로운 케이크 데이터를 리스트에 추가
    public void AddCakeData(CakeSO cakeSO)
    {
        if (cakeSODataList == null)
        {
            cakeSODataList = new List<CakeSO>();
        }

        if (!cakeSODataList.Contains(cakeSO))
        {
            cakeSODataList.Add(cakeSO);
            Debug.Log($"Added {cakeSO.Name} to CakeDataList");
        }
    }

    // 케이크 데이터를 초기화
    public void ResetCakeData()
    {
        cakeSODataList = new List<CakeSO>();
    }

    // 애플리케이션 종료 시 데이터 저장
    private void OnApplicationQuit()
    {
        SaveCakeData();
    }


    public void ResetContainer()
    {
        // 이거 케이크 개수 배열 초기화 하기 위한 함수

        // 모든 요소의 값을 0으로 리셋해주기..
        for (int i = 0; i < cakeCounts.Length; i++)
        {
            cakeCounts[i] = 0;
        }
    }


    public void SetContainer(Dictionary<int, InventoryItem> curInventory)
    {
        // curInventory 에서 키값은, 인벤토리 속에서 해당 아이템의 인덱스 번호임
        // 현재 인벤토리의 내용을 가져올 때 비어있는 아이템 칸은 제외하고 가져옴.
        // 즉, 인벤토리 속 비어있는 아이템 칸이 있다면 가져온 아이템 딕셔너리의 내용은 [0]: 사과, [2]: 바나나, [5]: 오렌지 이럴 가능성이 있음
        // 그래서 key 가 0, 1, 2, 3, 4, 5... 이런식으로 순차적으로 온다는 보장이 없으므로 그냥 키값들을 가져와서 반복문 도는 것..
        foreach (int idx in curInventory.Keys)
        {
            cakeCounts[((CakeSO)curInventory[idx].item).cakeIdx] += curInventory[idx].quantity; // 해당 아이템의 아이템 인덱스에 맞는 요소의 값을 증가시켜줌..
        }
    }


    // 직렬화할 케이크 데이터를 위한 클래스
    [System.Serializable]
    private class CakeDataSerializable
    {
        public string cakeName;
        public int cakeCost;
        public int bakeTime;
        public int cakePrice;
        public int[] materialIdxs;
        public int[] materialCounts;
        public int exp;
        public bool isLocked;
        public int cakeIdx;
        public int cakeCount;
    }

    // 케이크 데이터 저장을 위한 클래스
    [System.Serializable]
    private class CakeDataSave
    {
        public List<CakeDataSerializable> cakeDataList; // 직렬화된 케이크 데이터 리스트
        public int unlockedIndex;                       // 마지막으로 잠금 해제된 케이크의 인덱스
    }
}
