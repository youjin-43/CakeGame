// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;
// public class CakeDataLoader : MonoBehaviour
// {
//     private string path;
//     private string csvFileName = "CakeItemData.csv";
//     public List<CakeData> cakeDataList;
//     void Start()
//     {
//         path = Path.Combine(Application.streamingAssetsPath, csvFileName);
//         cakeDataList = LoadCakeDataFromCSV();
//     }



//     // CSV 파일을 읽어서 CakeData 리스트로 변환하는 함수
//     public List<CakeData> LoadCakeDataFromCSV()
//     {
//        var cakeDataList = new List<CakeData>();


//         if (File.Exists(path))
//         {
//             string[] csvLines = File.ReadAllLines(path);


//             // 첫 번째 라인은 헤더이므로 스킵
//             for (int i = 1; i < csvLines.Length; i++)
//             {
//                 // csv 파일의 데이터를 ,로 구분
//                 string[] values = csvLines[i].Split(',');


//                 // cakeData에 데이터 넣기
//                 var cakeData = new CakeData
//                 {
//                     IsStackable = bool.Parse(values[0]),
//                     MaxStackSize = int.Parse(values[1]),
//                     name = values[2],
//                     Description = values[3],
//                     itemType = int.Parse(values[4]),
//                     cakeCost = int.Parse(values[5]),
//                     bakeTime = int.Parse(values[6]),
//                     cakePrice = int.Parse(values[7]),
//                     materialIdx = int.Parse(values[8]),
//                     materialCount = int.Parse(values[9]),
//                     exp = int.Parse(values[10]),
//                     isLock = bool.Parse(values[11]),
//                     cakeIdx = int.Parse(values[12]),
//                     cakeCount = int.Parse(values[13]),
//                 };
//                 cakeData.itemImage = Resources.Load<Sprite>(values[14]);


//                 // 배열에 추가
//                 cakeDataList.Add(cakeData);
//             }
//         }
//         else Debug.LogError("CSV file not found at " + path);

//         return cakeDataList;
//     }

    

//     public void SaveCakeData()
//     {
//         // 헤더 추가
//         var csvLines = new List<string>
//         {
//             "IsStackable,MaxStackSize,name,Description,itemType,cakeCost,bakeTime,cakePrice,materialIdx,materialCount,exp,isLock,cakeIdx,cakeCount"
//         };
        
        
//         // 데이터를 CSV 형식의 문자열로 변환
//         foreach (CakeData cakeData in cakeDataList)
//         {
//             string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
//                 cakeData.IsStackable,
//                 cakeData.MaxStackSize,
//                 cakeData.name,
//                 cakeData.Description,
//                 cakeData.itemType,
//                 cakeData.cakeCost,
//                 cakeData.bakeTime,
//                 cakeData.cakePrice,
//                 cakeData.materialIdx,
//                 cakeData.materialCount,
//                 cakeData.exp,
//                 cakeData.isLock,
//                 cakeData.cakeIdx,
//                 cakeData.cakeCount);

//             csvLines.Add(line);
//         }
//     }
// }