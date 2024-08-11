using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Inventory.Model;

// 케이크 전반적인 관리 및 다른 매니저 참조
public class CakeManager : MonoBehaviour
{
    // 게임 매니저 및 인벤토리 매니저와 같은 다른 매니저들을 참조
    public GameManager gameManager;
    public UIInventoryController inventoryManager;
    public FruitContainer fruitContainer;
    public GameObject spritesToDisable;  // 케이크 제작 중 비활성화할 스프라이트 그룹
    public List<CakeSO> cakeDataList;    // 케이크 데이터를 저장하는 리스트 (Unity Editor에서 설정)
    public int[] cakeCounts;             // 각 케이크의 개수를 관리하는 배열
    private string filePath;             // 케이크 데이터 저장 파일 경로
    private int unlockedIndex = 0;       // 잠금 해제된 케이크의 마지막 인덱스

    void Awake()
    {
        // 다른 매니저들 초기화
        gameManager = FindObjectOfType<GameManager>();
        inventoryManager = FindObjectOfType<UIInventoryController>();
        fruitContainer = FindObjectOfType<FruitContainer>();

        // 데이터 저장 파일 경로 설정
        filePath = Path.Combine(Application.persistentDataPath, "cakeData.json");
        Debug.Log("Save file path: " + filePath);

        // 케이크 데이터 로드
        LoadCakeData();
    }

    // 메뉴 닫기 및 스프라이트 재활성화
    public void CloseMenu(GameObject menu)
    {
        DisableSprites(false);
        menu.SetActive(false);
    }

    // 스프라이트의 콜라이더를 활성화/비활성화
    public void DisableSprites(bool isActive)
    {
        for (int i = 0; i < spritesToDisable.transform.childCount; i++)
        {
            Transform child = spritesToDisable.transform.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                Collider2D collider = child.GetChild(j).GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = !isActive;
                }
            }
        }
    }

    // 케이크 상태 초기화: 첫 번째 케이크만 잠금 해제, 나머지는 잠금
    void InitializeCakeStatus()
    {
        cakeCounts = new int[cakeDataList.Count];

        for (int i = 0; i < cakeDataList.Count; i++)
        {
            CakeSO cakeData = cakeDataList[i];
            cakeCounts[i] = 0; // 초기 케이크 개수는 0으로 설정
            cakeData.isLocked = (cakeData.cakeIdx != 0); // 첫 번째 케이크만 잠금 해제
        }
    }

    // 잠금 해제된 마지막 케이크 인덱스를 초기화
    void InitializeUnlockIndex()
    {
        for (int i = 0; i < cakeDataList.Count; i++)
        {
            if (!cakeDataList[i].isLocked)
            {
                unlockedIndex = i;
            }
        }
    }

    // 케이크 개수 증가 및 데이터 저장
    public void IncreaseCakeCount(int index)
    {
        if (index >= 0 && index < cakeDataList.Count)
        {
            cakeCounts[index]++;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

    // 케이크 개수 감소 및 데이터 저장
    public void DecreaseCakeCount(int index)
    {
        if (index >= 0 && index < cakeDataList.Count && cakeCounts[index] > 0)
        {
            cakeCounts[index]--;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

    // 케이크 잠금 해제 및 데이터 저장
    public void UnlockCake(int index)
    {
        if (index >= 0 && index < cakeDataList.Count && cakeDataList[index].isLocked)
        {
            cakeDataList[index].isLocked = false;
            SaveCakeData();
            Debug.Log("케이크 잠금 해제됨: " + cakeDataList[index].Name);
        }
        else
        {
            Debug.LogWarning("잠금 해제 실패: 유효하지 않은 인덱스 또는 이미 잠금 해제된 케이크.");
        }
    }

    // 케이크 데이터를 JSON 파일로 저장
    private void SaveCakeData()
    {
        CakeDataSave dataSave = new CakeDataSave
        {
            cakeDataList = SerializeCakeDataList(), // 케이크 데이터 직렬화
            unlockedIndex = this.unlockedIndex
        };

        // JSON 파일로 저장
        string json = JsonConvert.SerializeObject(dataSave, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    // 케이크 데이터를 JSON 파일에서 로드
    private void LoadCakeData()
    {
        // 기본값으로 초기화
        cakeCounts = new int[cakeDataList.Count];
        InitializeCakeStatus(); 

        if (File.Exists(filePath))
        {
            // JSON 파일이 존재할 경우 파일에서 데이터 로드
            string json = File.ReadAllText(filePath);
            CakeDataSave dataSave = JsonConvert.DeserializeObject<CakeDataSave>(json);
            DeserializeCakeDataList(dataSave.cakeDataList);
            this.unlockedIndex = dataSave.unlockedIndex;
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

        for (int i = 0; i < cakeDataList.Count; i++)
        {
            CakeDataSerializable serializable = new CakeDataSerializable
            {
                cakeName = cakeDataList[i].name,
                cakeCost = cakeDataList[i].cakeCost,
                bakeTime = cakeDataList[i].bakeTime,
                cakePrice = cakeDataList[i].cakePrice,
                isLocked = cakeDataList[i].isLocked,
                cakeIdx = cakeDataList[i].cakeIdx,
                cakeCount = cakeCounts[i],
                materialIdxs = cakeDataList[i].materialIdxs,
                materialCounts = cakeDataList[i].materialCounts
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
            CakeSO cakeData = cakeDataList.Find(cake => cake.cakeIdx == serializable.cakeIdx);
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
        if (cakeDataList == null)
        {
            cakeDataList = new List<CakeSO>();
        }

        if (!cakeDataList.Contains(cakeSO))
        {
            cakeDataList.Add(cakeSO);
            Debug.Log($"Added {cakeSO.Name} to CakeDataList");
        }
    }

    // 케이크 데이터를 초기화
    public void ResetCakeData()
    {
        cakeDataList = new List<CakeSO>();
    }

    // 애플리케이션 종료 시 데이터 저장
    private void OnApplicationQuit()
    {
        SaveCakeData();
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
