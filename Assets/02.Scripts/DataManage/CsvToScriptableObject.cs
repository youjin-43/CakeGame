using UnityEngine;
using UnityEditor;
using System.IO;
public class CsvToScriptableObject
{

    //private static string  SeedCSVPath = Application.dataPath + "/StreamingAssets/Seed.csv";
    private static string SeedCSVPath = Application.dataPath + "/StreamingAssets/FruitItemSoData.csv";

    [MenuItem("Utilities/Generate Seed")]
    public static void GenerateSeed()
    {
        string[] allLines = File.ReadAllLines(SeedCSVPath);

        //0번은 헤더 
        for(int i=1;i<allLines.Length-1; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            SeedItemSO seed = ScriptableObject.CreateInstance<SeedItemSO>();

            //IsStackable,MaxStackSize,Name,Description,itemImage,itemType,fruitPrice,fruitIdx

            seed.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            seed.MaxStackSize = int.Parse(splitData[1].ToString());
            seed.Name = splitData[2].ToString();
            seed.Description = splitData[3].ToString();
            seed.itemImage = Resources.Load<Sprite>($"FruitImange/{seed.Name}");
            seed.itemType = int.Parse(splitData[5].ToString());
            seed.seedPrice = int.Parse(splitData[6].ToString());
            seed.seedIdx = int.Parse(splitData[7].ToString()); //fruit idx

            //seed.sprite = Resources.Load<Sprite>($"FruitImange/{seed.name}"); //나중에 스프라이트 정보가 필요하닥면 추가 

            AssetDatabase.CreateAsset(seed, $"Assets/Resources/SeedSO/{seed.Name}.asset");
        }

        AssetDatabase.SaveAssets();
    }


}