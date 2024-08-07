using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class CakeManager : MonoBehaviour
{
    public GameObject spritesToDisable;
    public List<CakeSO> cakeDataList; // Unity Editor에서 설정
    public int[] cakeCounts; // 케이크 개수를 관리하는 배열
    private string filePath;
    private int unlockedIndex = 0;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "cakeData.json");
        Debug.Log("Save file path: " + filePath);
        LoadCakeData();
        if (cakeDataList.Count == 0)
        {
            InitializeCakeStatus();
        }
        else
        {
            InitializeUnlockIndex();
        }
    }

    public void CloseMenu(GameObject menu)
    {
        DisableSprites(false);
        menu.SetActive(false);
    }

    public void DisableSprites(bool isActive)
    {
        for (int i = 0; i < spritesToDisable.transform.childCount; i++)
        {
            for (int j = 0; j < spritesToDisable.transform.GetChild(i).childCount; j++)
            {
                Collider2D collider = spritesToDisable.transform.GetChild(i).GetChild(j).GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = !isActive;
                }
            }
        }
    }

    void InitializeCakeStatus()
    {
        cakeCounts = new int[cakeDataList.Count];

        foreach (var cakeData in cakeDataList)
        {
            if (cakeData.cakeIdx == 0)
            {
                cakeData.isLocked = false;
            }
            else
            {
                cakeData.isLocked = true;
            }
        }
    }

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

    public void IncreaseCakeCount(int index)
    {
        if (index >= 0 && index < cakeDataList.Count)
        {
            cakeCounts[index]++;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

    public void DecreaseCakeCount(int index)
    {
        if (index >= 0 && index < cakeDataList.Count && cakeCounts[index] > 0)
        {
            cakeCounts[index]--;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);
            SaveCakeData();
        }
    }

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

    private void SaveCakeData()
    {
        List<CakeDataSerializable> serializableList = new List<CakeDataSerializable>();
        for (int i = 0; i < cakeDataList.Count; i++)
        {
            CakeSO cakeData = cakeDataList[i];
            CakeDataSerializable serializable = new CakeDataSerializable
            {
                cakeName = cakeData.name,
                cakeCost = cakeData.cakeCost,
                bakeTime = cakeData.bakeTime,
                cakePrice = cakeData.cakePrice,
                isLocked = cakeData.isLocked,
                cakeIdx = cakeData.cakeIdx,
                cakeCount = cakeCounts[i],
                materialType = cakeData.materialType,
                materialCount = cakeData.materialCount
            };
            serializableList.Add(serializable);
        }

        CakeDataSave dataSave = new CakeDataSave
        {
            cakeDataList = serializableList,
            unlockedIndex = this.unlockedIndex
        };

        string json = JsonConvert.SerializeObject(dataSave, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    private void LoadCakeData()
    {
        // Initialize cake data from ScriptableObject
        cakeCounts = new int[cakeDataList.Count];
        for (int i = 0; i < cakeDataList.Count; i++)
        {
            CakeSO cakeData = cakeDataList[i];
            cakeCounts[i] = 0; // 초기 케이크 개수는 0으로 설정
            cakeData.isLocked = (cakeData.cakeIdx != 0); // 첫 번째 케이크만 잠금 해제
        }

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CakeDataSave dataSave = JsonConvert.DeserializeObject<CakeDataSave>(json);
            List<CakeDataSerializable> serializableList = dataSave.cakeDataList;

            foreach (var serializable in serializableList)
            {
                CakeSO cakeData = cakeDataList.Find(cake => cake.cakeIdx == serializable.cakeIdx);
                if (cakeData != null)
                {
                    serializable.cakeName = cakeData.name;
                    serializable.cakeCost = cakeData.cakeCost;
                    serializable.bakeTime = cakeData.bakeTime;
                    serializable.cakePrice = cakeData.cakePrice;
                    serializable.isLocked = cakeData.isLocked;
                    serializable.cakeIdx = cakeData.cakeIdx;
                    serializable.materialType = cakeData.materialType;
                    serializable.materialCount = cakeData.materialCount;
                    cakeCounts[cakeData.cakeIdx] = serializable.cakeCount;
                }
            }

            this.unlockedIndex = dataSave.unlockedIndex;
        }
        else
        {
            // JSON 파일이 없을 때 기본값으로 초기화하고 저장
            InitializeCakeStatus();
            SaveCakeData();
        }
    }

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

    public void ResetCakeData(){
        cakeDataList = new List<CakeSO>();
    }

    private void OnApplicationQuit()
    {
        SaveCakeData();
    }

    [System.Serializable]
    private class CakeDataSerializable
    {
        public string cakeName;
        public int cakeCost;
        public int bakeTime;
        public int cakePrice;
        public int[] materialType;
        public int[] materialCount;
        public bool isLocked;
        public int cakeIdx;
        public int cakeCount;
    }

    [System.Serializable]
    private class CakeDataSave
    {
        public List<CakeDataSerializable> cakeDataList;
        public int unlockedIndex;
    }
}
