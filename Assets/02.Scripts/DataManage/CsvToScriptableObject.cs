using UnityEngine;
using UnityEditor;
using System.IO;

public class CsvToScriptableObject
{

    private static string  SeedCSVPath = Application.dataPath + "/StreamingAssets/Seed.csv";

    [MenuItem("Utilities/Generate Seed")]
    public static void GenerateSeed()
    {
        string[] allLines = File.ReadAllLines(SeedCSVPath);

        //0번은 헤더 
        for(int i=1;i<allLines.Length-1; i++)
        {
            string Oneline = allLines[i];
            string[] splitData = Oneline.Split(',');

            //if (splitData.Length != 4)
            //{
            //    Debug.Log(Oneline + " Does not have 4 values");
            //}

            Seedtmp seed = ScriptableObject.CreateInstance<Seedtmp>();
            seed.name = splitData[0].ToString();
            seed.seedPrice = int.Parse(splitData[1].ToString());
            seed.growTime = int.Parse(splitData[2].ToString());
            seed.seedIdx = int.Parse(splitData[3].ToString());
            seed.sprite = Resources.Load<Sprite>($"FruitImange/{seed.name}");

            AssetDatabase.CreateAsset(seed, $"Assets/SeedsTmp/{seed.name}.asset");
        }

        AssetDatabase.SaveAssets();
    }


}