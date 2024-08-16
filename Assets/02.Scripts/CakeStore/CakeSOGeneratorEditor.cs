using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CakeSOGeneratorEditor : EditorWindow
{
    private const string CsvFilePathKey = "CakeSOGenerator_CsvFilePath";
    private const string OutputFolderKey = "CakeSOGenerator_OutputFolder";
    private string csvFilePath;
    private string outputFolder;
    private List<Sprite> itemImages = new List<Sprite>(); // Sprite 리스트
    private ReorderableList reorderableList; // ReorderableList

    [MenuItem("Utilities/Generate Cake Beta")]
    public static void ShowWindow()
    {
        GetWindow<CakeSOGeneratorEditor>("Generate Cake");
    }

    private void OnEnable()
    {
        csvFilePath = EditorPrefs.GetString(CsvFilePathKey, "Assets/Data/cakes.csv");
        outputFolder = EditorPrefs.GetString(OutputFolderKey, "Assets/Resources/CakeData");

        reorderableList = new ReorderableList(itemImages, typeof(Sprite), true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Item Images");
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                itemImages[index] = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), itemImages[index], typeof(Sprite), false);
            },
            onAddCallback = (ReorderableList list) =>
            {
                itemImages.Add(null);
            },
            onRemoveCallback = (ReorderableList list) =>
            {
                itemImages.RemoveAt(list.index);
            }
        };
    }

    private void OnDisable()
    {
        EditorPrefs.SetString(CsvFilePathKey, csvFilePath);
        EditorPrefs.SetString(OutputFolderKey, outputFolder);
    }

    private void OnGUI()
    {
        csvFilePath = EditorGUILayout.TextField("CSV File Path", csvFilePath);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        reorderableList.DoLayoutList();

        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Add multiple Sprites here");

        if (evt.type == EventType.DragUpdated)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
        }

        if (evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.AcceptDrag();
                foreach (Object draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is Sprite sprite)
                    {
                        itemImages.Add(sprite);
                    }
                }
                evt.Use();
            }
        }

        if (GUILayout.Button("Generate CakeSO"))
        {
            GenerateCakeSO();
        }
    }

    private void GenerateCakeSO()
    {
        Debug.Log("Starting to generate CakeSO from CSV");
        var data = CSVReader.Read(csvFilePath);

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Debug.Log("Output folder not found. Creating new folder.");
            AssetDatabase.CreateFolder("Assets/Resources", "CakeData");
        }

        if (data.Count != itemImages.Count)
        {
            Debug.LogError("CSV 데이터의 항목 수와 제공된 이미지 수가 일치하지 않습니다.");
            return;
        }
        
        CakeManager cakeManager = FindObjectOfType<CakeManager>();
        if (cakeManager == null)
        {
            Debug.LogError("CakeManager를 찾을 수 없습니다. 씬에 CakeManager가 있는지 확인하세요.");
            return;
        }
        cakeManager.ResetCakeData();

        for (int i = 0; i < data.Count; i++)
        {
            var entry = data[i];
            try
            {
                Debug.Log($"Processing row {i}: {string.Join(", ", entry.Values)}");
                
                CakeSO cake = ScriptableObject.CreateInstance<CakeSO>();

                cake.IsStackable = bool.Parse(entry["IsStackable"]);
                cake.MaxStackSize = int.Parse(entry["MaxStackSize"]);
                cake.Name = entry["Name"];
                cake.Description = entry["Description"];
                cake.itemImage = itemImages[i];
                cake.itemType = int.Parse(entry["itemType"]);
                cake.cakeCost = int.Parse(entry["cakeCost"]);
                cake.bakeTime = int.Parse(entry["bakeTime"]);
                cake.cakePrice = int.Parse(entry["cakePrice"]);
                cake.isLocked = bool.Parse(entry["isLock"]);
                cake.cakeIdx = int.Parse(entry["cakeIdx"]);

                var materialIdxList = entry["materialIdx"].Split('/');
                var materialCountList = entry["materialCount"].Split('/');

                cake.materialIdxs = new int[materialIdxList.Length];
                cake.materialCounts = new int[materialCountList.Length];

                for (int j = 0; j < materialIdxList.Length; j++)
                {
                    cake.materialIdxs[j] = int.Parse(materialIdxList[j]);
                    cake.materialCounts[j] = int.Parse(materialCountList[j]);
                }

                string assetPath = $"{outputFolder}/{cake.Name}.asset";
                AssetDatabase.CreateAsset(cake, assetPath);
                Debug.Log($"Created CakeSO: {cake.Name} at {assetPath}");
                cakeManager.AddCakeData(cake);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating CakeSO for {entry["Name"]}: {ex.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Finished generating CakeSO from CSV");
    }
}
