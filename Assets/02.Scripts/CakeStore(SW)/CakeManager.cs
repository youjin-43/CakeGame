using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CakeManager : MonoBehaviour
{
    public List<CakeSO> cakeDataList; // Unity Editor에서 설정
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

    void InitializeCakeStatus()
    {
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
            cakeData.cakeCount = 0;
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
            cakeDataList[index].cakeCount++;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeDataList[index].cakeCount);
            SaveCakeData();
        }
    }

    public void DecreaseCakeCount(int index)
    {
        if (index >= 0 && index < cakeDataList.Count && cakeDataList[index].cakeCount > 0)
        {
            cakeDataList[index].cakeCount--;
            Debug.Log("케이크 " + index + " 보유 수: " + cakeDataList[index].cakeCount);
            SaveCakeData();
        }
    }

    public void UnlockCake()
    {
        if (unlockedIndex + 1 < cakeDataList.Count && cakeDataList[unlockedIndex + 1].isLocked)
        {
            cakeDataList[unlockedIndex + 1].isLocked = false;
            unlockedIndex++;
            SaveCakeData();
            Debug.Log("케이크 잠금 해제됨: " + cakeDataList[unlockedIndex].cakeName);
        }
        else
        {
            Debug.LogWarning("더 이상 잠금 해제할 케이크가 없습니다.");
        }
    }

    private void SaveCakeData()
    {
        List<CakeDataSerializable> serializableList = new List<CakeDataSerializable>();
        foreach (var cakeData in cakeDataList)
        {
            CakeDataSerializable serializable = new CakeDataSerializable
            {
                cakeName = cakeData.cakeName,
                cakeCost = cakeData.cakeCost,
                bakeTime = cakeData.bakeTime,
                cakePrice = cakeData.cakePrice,
                cakeIdx = cakeData.cakeIdx,
                cakeCount = cakeData.cakeCount,
                isLocked = cakeData.isLocked
            };
            serializableList.Add(serializable);
        }

        CakeDataSave dataSave = new CakeDataSave
        {
            cakeDataList = serializableList,
            unlockedIndex = this.unlockedIndex
        };

        string json = JsonUtility.ToJson(dataSave, true);
        File.WriteAllText(filePath, json);
    }

    private void LoadCakeData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CakeDataSave dataSave = JsonUtility.FromJson<CakeDataSave>(json);
            List<CakeDataSerializable> serializableList = dataSave.cakeDataList;

            foreach (var serializable in serializableList)
            {
                CakeSO cakeData = cakeDataList.Find(cake => cake.cakeIdx == serializable.cakeIdx);
                if (cakeData != null)
                {
                    cakeData.cakeCount = serializable.cakeCount;
                    cakeData.isLocked = serializable.isLocked;
                }
            }

            this.unlockedIndex = dataSave.unlockedIndex;
        }
    }

    [System.Serializable]
    private class CakeDataSerializable
    {
        public string cakeName;
        public int cakeCost;
        public int bakeTime;
        public int cakePrice;
        public int cakeIdx;
        public int cakeCount;
        public bool isLocked;
    }

    [System.Serializable]
    private class CakeDataSave
    {
        public List<CakeDataSerializable> cakeDataList;
        public int unlockedIndex;
    }
}
