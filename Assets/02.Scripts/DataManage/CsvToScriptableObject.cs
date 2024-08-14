using UnityEngine;
using UnityEditor;
using System.IO;
public class CsvToScriptableObject
{
    private static string SeedCSVPath = Application.dataPath + "/08.Data/SeedItemSOData.csv";
    private static string FruitCSVPath = Application.dataPath + "/08.Data/FruitItemSOData.csv";
    private static string CakeCSVPath = Application.dataPath + "/08.Data/CakeItemSOData.csv";

    [MenuItem("Utilities/Generate Seed")]
    public static void GenerateSeed()
    {
        string[] allLines = File.ReadAllLines(SeedCSVPath);

        for (int i = 1; i < allLines.Length; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            SeedItemSO seed = ScriptableObject.CreateInstance<SeedItemSO>();

            //ItemSO 변수
            seed.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            seed.MaxStackSize = int.Parse(splitData[1].ToString());
            seed.Name = splitData[2].ToString();
            seed.Description = splitData[3].ToString();
            seed.itemImage = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/ImportedAsset/FarmAsset/SeedSprite/{seed.Name}.png", typeof(Sprite));
            seed.itemType = int.Parse(splitData[5].ToString());

            //SeedItemSO 변수 
            seed.seedPrice = int.Parse(splitData[6].ToString());
            seed.seedIdx = int.Parse(splitData[7].ToString());
            seed.growDay = int.Parse(splitData[8].ToString()); 
            

            AssetDatabase.CreateAsset(seed, $"Assets/SO/SeedSO/{seed.Name}.asset");
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Fruit")]
    public static void GenerateFruit()
    {
        string[] allLines = File.ReadAllLines(FruitCSVPath);

        for (int i = 1; i < allLines.Length; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            FruitItemSO fruit = ScriptableObject.CreateInstance<FruitItemSO>();

            //ItemSO 변수
            fruit.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            fruit.MaxStackSize = int.Parse(splitData[1].ToString());
            fruit.Name = splitData[2].ToString();
            fruit.Description = splitData[3].ToString();
            fruit.itemImage = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/04.Images/FruitImange/{fruit.Name}.png", typeof(Sprite));
            fruit.itemType = int.Parse(splitData[5].ToString());

            //SeedItemSO 변수 
            fruit.fruitPrice = int.Parse(splitData[6].ToString());
            fruit.fruitIdx = int.Parse(splitData[7].ToString()); //csv파일에는 fruit idx로 명시 돼있음 

            AssetDatabase.CreateAsset(fruit, $"Assets/SO/FruitSO/{fruit.Name}.asset");
        }
        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate Cake - youjin")]
    public static void GenerateCake()
    {
        string[] allLines = File.ReadAllLines(CakeCSVPath);

        for (int i = 1; i < allLines.Length; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            CakeSO cake = ScriptableObject.CreateInstance<CakeSO>();

            //ItemSO 변수
            cake.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            cake.MaxStackSize = int.Parse(splitData[1].ToString());
            cake.Name = splitData[2].ToString();
            cake.Description = splitData[3].ToString();
            //itemImage -> 이미지 이름을 cakeIdx로 설정해놔서 순서를 뒤로 빼서 할당 
            cake.itemType = int.Parse(splitData[5].ToString());

            //CakeSO 변수 
            cake.cakeCost = int.Parse(splitData[6].ToString());
            cake.bakeTime = int.Parse(splitData[7].ToString()); 
            cake.cakePrice = int.Parse(splitData[8].ToString());

            //materialIdxs 배열
            string[] materials = splitData[9].Split('/');
            for (int m=0;m<materials.Length;m++) cake.materialIdxs[m]= int.Parse(materials[m]);
            
            //materialCount 배열 
            string[] materialCnts = splitData[10].ToString().Split('/');
            for (int m = 0; m < materials.Length; m++) cake.materialCounts[m] = int.Parse(materialCnts[m]);

            cake.isLocked = (splitData[11].ToString() == "TRUE") ? true : false;
            cake.cakeIdx = int.Parse(splitData[12].ToString());

            cake.itemImage = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/04.Images/CakeImage/{cake.cakeIdx}.asset", typeof(Sprite));

            AssetDatabase.CreateAsset(cake, $"Assets/SO/CakeSO/{cake.Name}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}