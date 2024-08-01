using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CakeManager : MonoBehaviour
{
    public GameObject spritesToDisable;
    public Sprite[] cakeImages;           // 케이크 이미지 배열
    public int[] cakeCounts;              // 각 케이크의 보유 수를 저장하는 배열
    public bool[] cakeLocked;             // 각 케이크가 잠겨 있는지 여부를 저장하는 배열

    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "cakeData.json");
        LoadCakeData();
        InitializeCakeStatus();
    }

    void InitializeCakeStatus()
    {
        if (cakeCounts == null || cakeLocked == null)
        {
            int cakeCount = cakeImages.Length;
            cakeCounts = new int[cakeCount];
            cakeLocked = new bool[cakeCount];

            cakeLocked[0] = false;
            for (int i = 1; i < cakeCount; i++)
            {
                cakeLocked[i] = true;
            }
        }
    }

    public void SaveCakeData()
    {
        CakeData data = new CakeData
        {
            cakeCounts = this.cakeCounts,
            cakeLocked = this.cakeLocked
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadCakeData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            CakeData data = JsonUtility.FromJson<CakeData>(json);
            this.cakeCounts = data.cakeCounts;
            this.cakeLocked = data.cakeLocked;
        }
    }

    public void CloseMenu(GameObject menu)
    {
        DisableSprites(false);
        menu.SetActive(false);
        SaveCakeData();
    }

    public void DisableSprites(bool isActive)
    {
        foreach (Transform child in spritesToDisable.transform)
        {
            foreach (Transform grandchild in child)
            {
                Collider2D collider = grandchild.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = !isActive;
                }
            }
        }
    }

    public void UnlockNextCake()
    {
        for (int i = 0; i < cakeLocked.Length - 1; i++)
        {
            if (cakeLocked[i] == false && cakeLocked[i + 1] == true)
            {
                cakeLocked[i + 1] = false;
                SaveCakeData();
                break;
            }
        }
    }

    public bool CanMakeCake(int index)
    {
        return cakeCounts[index] > 0 && !cakeLocked[index];
    }

    public void AddCake(int index)
    {
        cakeCounts[index]++;
        SaveCakeData();
    }

    public void RemoveCake(int index)
    {
        if (cakeCounts[index] > 0)
        {
            cakeCounts[index]--;
            SaveCakeData();
        }
    }
}
