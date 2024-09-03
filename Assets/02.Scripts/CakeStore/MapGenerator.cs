using UnityEngine;
using UnityEditor;
using System;  // Array.Resize를 사용하기 위해 System 네임스페이스를 추가
using System.Collections.Generic;

public class MapGenerator : EditorWindow
{
    private GameObject Cashier;
    private GameObject Player;
    private GameObject Showcase;
    private GameObject Maker;
    private Vector2 top = new Vector2(-1.5f, 3);       // 상단 꼭짓점 좌표
    private Vector2 right = new Vector2(2.5f, .5f);     // 우측 꼭짓점 좌표
    private Vector2 bottom = new Vector2(-1.5f, -2);    // 하단 꼭짓점 좌표
    private Vector2 left = new Vector2(-6.5f, .5f);      // 좌측 꼭짓점 좌표
    private Vector2 tileSize = new Vector2(.5f, 0.25f); // 타일의 크기 (아이소매트릭)
    private interiorList[] interiorLists = new interiorList[0]; // Interior 리스트

    [MenuItem("Utilities/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGenerator>("Map Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Generator", EditorStyles.boldLabel);
        Cashier = (GameObject)EditorGUILayout.ObjectField("Cashier", Cashier, typeof(GameObject), false);
        Player = (GameObject)EditorGUILayout.ObjectField("Player", Player, typeof(GameObject), false);
        Showcase = (GameObject)EditorGUILayout.ObjectField("Showcase", Showcase, typeof(GameObject), false);
        Maker = (GameObject)EditorGUILayout.ObjectField("Maker", Maker, typeof(GameObject), false);
        top = EditorGUILayout.Vector2Field("Top Vertex", top);
        right = EditorGUILayout.Vector2Field("Right Vertex", right);
        bottom = EditorGUILayout.Vector2Field("Bottom Vertex", bottom);
        left = EditorGUILayout.Vector2Field("Left Vertex", left);
        tileSize = EditorGUILayout.Vector2Field("Tile Size", tileSize);

        // CSV 데이터에서 interiorList 가져오기
        if (GUILayout.Button("Load CSV"))
        {
            string csvData = "4,4,4,0,0,0,0,0,0,3\n0,0,0,0,0,0,0,0,0,3\n0,0,0,2,0,0,0,0,0,3\n3,3,3,1,3,3,3,0,0,3\n0,0,0,0,0,0,0,0,0,3\n0,0,0,0,0,0,0,0,0,3\n3,3,3,3,3,3,3,0,0,3\n0,0,0,0,0,0,0,0,0,0\n0,0,0,0,0,0,0,0,0,0\n0,0,0,0,0,0,0,0,0,0";
            LoadInteriorListFromCSV(csvData);
        }

        // interiorLists 배열의 각 항목을 수동으로 표시
        for (int i = 0; i < interiorLists.Length; i++)
        {
            if (interiorLists[i] == null)
            {
                interiorLists[i] = new interiorList(); // 배열 요소가 null이면 초기화
            }

            // EditorGUILayout.BeginHorizontal();
            // interiorLists[i].index = EditorGUILayout.IntField("Index", interiorLists[i].index);
            // interiorLists[i].type = EditorGUILayout.IntField("Type", interiorLists[i].type);
            // EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Create IsoMap"))
        {
            CreateIsoMap();
        }
    }

    private void LoadInteriorListFromCSV(string csvData)
    {
        List<interiorList> list = new List<interiorList>();
        string[] rows = csvData.Split('\n');

        for (int y = 0; y < rows.Length; y++)
        {
            string[] cols = rows[y].Split(',');

            for (int x = 0; x < cols.Length; x++)
            {
                int type = int.Parse(cols[x]);
                if (true) // type이 0이 아니면 interiorList에 추가
                {
                    interiorList item = new interiorList();
                    item.index = y * cols.Length + x;
                    item.type = type;
                    list.Add(item);
                }
            }
        }

        interiorLists = list.ToArray();
    }

    private void CreateIsoMap()
    {
        CustomerController customerController =FindAnyObjectByType<CustomerController>();
        GameObject newPool = new GameObject("MapPool");
        CakeUIController cakeUIController = FindAnyObjectByType<CakeUIController>();
        cakeUIController.spritesToDisable = newPool;
        newPool.transform.position = Vector3.zero;
        GameObject showcasePool = new GameObject("ShowcasePool");
        CakeShowcaseController cakeShowcaseController = FindAnyObjectByType<CakeShowcaseController>();
        cakeShowcaseController.cakeShowcasePool = showcasePool.transform;
        showcasePool.transform.position = Vector3.zero;
        showcasePool.transform.parent = newPool.transform;
        GameObject makerPool = new GameObject("MakerPool");
        CakeMakerController cakeMakerController = FindAnyObjectByType<CakeMakerController>();
        cakeMakerController.cakeMakersPool = makerPool.transform;
        makerPool.transform.position = Vector3.zero;
        makerPool.transform.parent = newPool.transform;


        float x = 0;
        float y = 0;

        int cols = Mathf.CeilToInt((top.x - left.x) / tileSize.x);
        int rows = Mathf.CeilToInt((left.y - bottom.y) / tileSize.y);

        // 루프 범위 설정
        for (int i = 0; i < rows; i++)
        {
            x = top.x + i * tileSize.x;
            y = top.y - i * tileSize.y;

            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;
                GameObject prefabToInstantiate = null;
                Transform objectParent = newPool.transform;

                foreach (var interior in interiorLists)
                {
                    if (interior.index == index)
                    {
                        switch (interior.type)
                        {
                            case 0: prefabToInstantiate = Player; objectParent = newPool.transform; break;
                            case 1: prefabToInstantiate = Cashier; objectParent = newPool.transform; customerController.cashierPosition = Cashier.transform; break;
                            case 2: prefabToInstantiate = Player; objectParent = newPool.transform; break;
                            case 3: prefabToInstantiate = Showcase; objectParent = showcasePool.transform; break;
                            case 4: prefabToInstantiate = Maker; objectParent = makerPool.transform; break;
                            default: break;
                        }
                        break;
                    }
                }

                if (prefabToInstantiate != null)
                {
                    GameObject newObject = Instantiate(prefabToInstantiate, new Vector3(x, y, 0), Quaternion.identity);
                    newObject.transform.parent = objectParent;
                    newObject.name = $"Object_{index}";
                }

                x -= tileSize.x;
                y -= tileSize.y;
            }
        }

        Debug.Log("아이소매트릭 맵 생성 완료.");

    }
}

[System.Serializable]
public class interiorList
{
    public int index;
    public int type; // 0: Floor, 1: Cashier, 2: Player, 3: ShowCase, 4: Maker
}
