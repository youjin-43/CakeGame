using UnityEngine;
using UnityEditor;
using System.IO;
public class CsvToScriptableObject
{

    private static string SeedCSVPath = Application.dataPath + "/StreamingAssets/FruitItemSoData.csv";
    private static string CakeCSVPath = Application.dataPath + "/StreamingAssets/CakeItemSoData.csv";



    [MenuItem("Utilities/Generate Seed")]
    public static void GenerateSeed()
    {
        string[] allLines = File.ReadAllLines(SeedCSVPath);

        //0번은 헤더 
        for (int i = 1; i < allLines.Length - 1; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            SeedItemSO seed = ScriptableObject.CreateInstance<SeedItemSO>();

            //IsStackable,MaxStackSize,Name,Description,itemImage,itemType,fruitPrice,fruitIdx
            seed.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            seed.MaxStackSize = int.Parse(splitData[1].ToString());
            seed.Name = splitData[2].ToString();
            seed.Description = splitData[3].ToString();
            seed.itemImage = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/04.Images/FruitImange/{seed.Name}.png", typeof(Sprite));
            seed.itemType = int.Parse(splitData[5].ToString());
            seed.seedPrice = int.Parse(splitData[6].ToString());
            seed.seedIdx = int.Parse(splitData[7].ToString()); //fruit idx

            AssetDatabase.CreateAsset(seed, $"Assets/Resources/SO/SeedSO/{seed.Name}.asset");
        }

        AssetDatabase.SaveAssets();
    }



    [MenuItem("Utilities/Generate Cake")]
    public static void GenerateCake()
    {
        string[] allLines = File.ReadAllLines(CakeCSVPath);

        //0번은 헤더, 마지막줄은 빈 줄 
        for (int i = 1; i < allLines.Length - 1; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            CakeSO cake = ScriptableObject.CreateInstance<CakeSO>();

            //IsStackable,MaxStackSize,Name,Description,itemImage,itemType,cakeCost,bakeTime,cakePrice,materialIdx,materialCount,isLock,cakeIdx
            cake.IsStackable = (splitData[0].ToString() == "TRUE") ? true : false;
            cake.MaxStackSize = int.Parse(splitData[1].ToString());
            cake.Name = splitData[2].ToString();
            cake.Description = splitData[3].ToString();
            //이미지 할당은 뒤에 있음
            cake.itemType = int.Parse(splitData[5].ToString());

            cake.cakeCost = int.Parse(splitData[6].ToString());
            cake.bakeTime = int.Parse(splitData[7].ToString()); 
            cake.cakePrice = int.Parse(splitData[8].ToString());
            //materialIdx가 없음 
            //cake.materialCount = int.Parse(splitData[10].ToString()); //배열이라 다른 방법을 찾아봐야 할 것 같음 
            cake.isLocked = (splitData[0].ToString() == "TRUE") ? true : false;
            cake.cakeIdx = int.Parse(splitData[12].ToString());

            //이미지 이름을 cakeIdx로 설정해놔서 순서를 뒤로 뺌 
            cake.itemImage = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/04.Images/CakeImage/{cake.cakeIdx}.asset", typeof(Sprite));

            AssetDatabase.CreateAsset(cake, $"Assets/Resources/SO/CakeSO/{cake.Name}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}