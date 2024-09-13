using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Inventory.Model;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// 케이크 전반적인 관리 및 다른 컨트롤러 참조를 위한 클래스
/// </summary>
public class CakeManager : MonoBehaviour
{
    //싱글톤
    public static CakeManager instance;


    [Header("Component")]
    public MakerController makerController;
    public ShowcaseController showcaseController;
    public CakeUIController cakeUIController;



    [Header("Save Data")]
    public List<CakeData> cakeDataList;
    public int[] soldCakeCounts;
    private string path;



    [Header("Audio")]
    public AudioClip[] BakeSound;



    [Header("Integer")]
    public int TOTALCAKENUM = 6;
    public int CAKEPLACENUM = 4;
    private string CSVFILENAME = "CakeItemData.csv";



    private string isFirstLaunch;



    void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;


            // 씬이 변경되어도 삭제 X
            DontDestroyOnLoad(gameObject);


            // 씬 로드 시 호출
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);


        if (IsFirstLaunch())
        {
            ResetCakeData();
            SaveCakeData();


            PlayerPrefs.SetInt(isFirstLaunch, 0);
            PlayerPrefs.Save();                
            

            Debug.Log("앱이 최초로 실행되었습니다.");   
        }

    }



    // 씬 로드 시 호출
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 데이터 저장 파일 경로 설정
        path = Path.Combine(Application.streamingAssetsPath, CSVFILENAME);



        // 케이크 데이터 로드
        cakeUIController = FindObjectOfType<CakeUIController>();
        showcaseController = FindObjectOfType<ShowcaseController>();
        makerController = FindObjectOfType<MakerController>();
        audioManager = FindAnyObjectByType<AudioManager>();
        soldCakeCounts = new int[TOTALCAKENUM];
        cakeDataList = LoadCakeData();
    }



    /// <summary>
    /// 케이크 데이터 초기화
    /// </summary>
    public void ResetCakeData()
    {
        for (int i = 0; i < cakeDataList.Count; i++)
        {
            cakeDataList[i].cakeCount = 0;
            ResetUnlockCake();
        }
    }



    // 첫 번째 케이크만 잠금 해제
    public void ResetUnlockCake()
    {
        for (int i = 1; i < cakeDataList.Count; i++) cakeDataList[i].isLock = cakeDataList[i].cakeIdx != 0;
        SaveCakeData();
    }



    // 최초 실행인지 감지
    private bool IsFirstLaunch()
    {
        return PlayerPrefs.GetInt(isFirstLaunch, 1) == 1; // 저장된 값이 없으면 기본값 1 반환 (최초 실행)
    }



    // 케이크 데이터를 JSON 파일로 저장
    public void SaveCakeData()
    {
        // 헤더 추가
        var csvLines = new List<string>
        {
            "IsStackable,MaxStackSize,name,Description,itemType,cakeCost,bakeTime,cakePrice,materialIdx,materialCount,exp,isLock,cakeIdx,cakeCount,imagePath"
        };
        
        
        // 데이터를 CSV 형식의 문자열로 변환
        foreach (CakeData cakeData in cakeDataList)
        {
            string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                cakeData.IsStackable,
                cakeData.MaxStackSize,
                cakeData.name,
                cakeData.Description,
                cakeData.itemType,
                cakeData.cakeCost,
                cakeData.bakeTime,
                cakeData.cakePrice,
                cakeData.materialIdx,
                cakeData.materialCount,
                cakeData.exp,
                cakeData.isLock,
                cakeData.cakeIdx,
                cakeData.cakeCount,
                cakeData.imagePath);

            csvLines.Add(line);
        }
        File.WriteAllLines(path, csvLines.ToArray());
        Debug.Log("케이크 데이터 저장 완료");
    }



    // CSV 파일을 읽어서 CakeData 리스트로 변환하는 함수
    public List<CakeData> LoadCakeData()
    {
        if (File.Exists(path))
        {
            string[] csvLines = File.ReadAllLines(path);


            // 첫 번째 라인은 헤더이므로 스킵
            for (int i = 0; i < cakeDataList.Count; i++)
            {
                // csv 파일의 데이터를 ,로 구분
                string[] values = csvLines[i+1].Split(',');

                CakeData cakeData = cakeDataList.Find(cake => cake.cakeIdx == i);
                Debug.Log(cakeData.name);
                Debug.Log(cakeData.isLock);
                Debug.Log(values[11]);
                Debug.Log(bool.Parse(values[11]));
                cakeData.isLock = bool.Parse(values[11]);
                cakeData.cakeCount = int.Parse(values[13]);
                // cakeData에 데이터 넣기
                // var cakeData = new CakeData
                // {
                //     IsStackable = bool.Parse(values[0]),
                //     MaxStackSize = int.Parse(values[1]),
                //     name = values[2],
                //     Description = values[3],
                //     itemType = int.Parse(values[4]),
                //     cakeCost = int.Parse(values[5]),
                //     bakeTime = int.Parse(values[6]),
                //     cakePrice = int.Parse(values[7]),
                //     materialIdx = int.Parse(values[8]),
                //     materialCount = int.Parse(values[9]),
                //     exp = int.Parse(values[10]),
                //     isLock = bool.Parse(values[11]),
                //     cakeIdx = int.Parse(values[12]),
                //     cakeCount = int.Parse(values[13]),
                //     imagePath = values[14],
                // // };
                // cakeData.itemImage = Resources.Load<Sprite>(values[14]);


                // // 배열에 추가
                // cakeDataList.Add(cakeData);
            }
            Debug.Log("케이크 데이터 로드 완료");
        }
        else Debug.LogError("CSV file not found at " + path);

        return cakeDataList;
    }



    /// <summary>
    /// 메뉴 뒤의 오브젝트 클릭 방지 (모바일용)
    /// </summary>
    /// <param name="touch"></param>
    /// <returns></returns>
    public bool IsPointerOverUI_Mobile(Touch touch)
    {
        // 현재 터치 위치를 기반으로 PointerEventData 생성
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };


        // Raycast 결과를 담을 리스트
        var raycastResults = new List<RaycastResult>(10);
        EventSystem.current.RaycastAll(eventData, raycastResults);



        // UI 요소가 터치된 경우 결과가 하나 이상이므로 true 반환
        return raycastResults.Count > 0;
    }



    /// <summary>
    /// 케이크 개수 증가 및 데이터 저장
    /// </summary>
    /// <param name="idx">추가할 케이크 인덱스</param>
    public void PlusCakeCount(int idx)
    {
        if (idx >= 0 && idx < TOTALCAKENUM)
        {
            cakeDataList[idx].cakeCount++;
            SaveCakeData();
        }
    }



    /// <summary>
    /// 케이크 개수 감소 및 데이터 저장
    /// </summary>
    /// <param name="idx">감소할 케이크 인덱스</param>
    public void MinusCakeCount(int idx)
    {
        if (idx >= 0 && idx < TOTALCAKENUM && cakeDataList[idx].cakeCount > 0)
        {
            cakeDataList[idx].cakeCount--;
            SaveCakeData();
        }
    }



    /// <summary>
    /// 케이크 잠금 해제 및 데이터 저장
    /// </summary>
    /// <param name="idx">잠금 해제할 케이크 인덱스</param>
    public void UnlockCake(int idx)
    {
        if (idx >= 0 && idx < TOTALCAKENUM && cakeDataList[idx].isLock)
        {
            cakeDataList[idx].isLock = false;
            SaveCakeData();
        }
        else
        {
            Debug.LogWarning("잠금 해제 실패: 유효하지 않은 인덱스 또는 이미 잠금 해제된 케이크.");
        }
    }



    // 애플리케이션 종료 시 데이터 저장
    private void OnApplicationQuit()
    {
        SaveCakeData();
    }



    //TODO 수정 필요 내가 만든거 아닌듯
    public void ResetContainer()
    {
        // 이거 케이크 개수 배열 초기화 하기 위한 함수

        // 모든 요소의 값을 0으로 리셋해주기..
        for (int i = 0; i < TOTALCAKENUM; i++)
        {
        //    cakeDataList[i].cakeCount = 0;
        }
    }
    public void SetContainer(Dictionary<int, InventoryItem> curInventory)
    {
        // curInventory 에서 키값은, 인벤토리 속에서 해당 아이템의 인덱스 번호임
        // 현재 인벤토리의 내용을 가져올 때 비어있는 아이템 칸은 제외하고 가져옴.
        // 즉, 인벤토리 속 비어있는 아이템 칸이 있다면 가져온 아이템 딕셔너리의 내용은 [0]: 사과, [2]: 바나나, [5]: 오렌지 이럴 가능성이 있음
        // 그래서 key 가 0, 1, 2, 3, 4, 5... 이런식으로 순차적으로 온다는 보장이 없으므로 그냥 키값들을 가져와서 반복문 도는 것..
        foreach (int idx in curInventory.Keys)
        {
            cakeDataList[((CakeData)curInventory[idx].item).cakeIdx].cakeCount += curInventory[idx].quantity; // 해당 아이템의 아이템 인덱스에 맞는 요소의 값을 증가시켜줌..
        }
    }



    //케이크 가게 내부 클릭 효과음
    public AudioManager audioManager;
    public AudioManager.SFX openSFX = AudioManager.SFX.BUTTON;
    public AudioManager.SFX sellSFX = AudioManager.SFX.SELLCAKE;
    public AudioManager.SFX portalSFX = AudioManager.SFX.PORTAL;
    public AudioManager.SFX getCakeSFX = AudioManager.SFX.HARVEST;
    public void CallOpenAudio()
    {
        audioManager.SetSFX(openSFX);
    }
    public void CallSellAudio()
    {
        audioManager.SetSFX(sellSFX);
    }
    public void CallPortalAudio()
    {
        audioManager.SetSFX(portalSFX);
    }
    public void CallGetCakeAudio()
    {
        audioManager.SetSFX(getCakeSFX);
    }
}
