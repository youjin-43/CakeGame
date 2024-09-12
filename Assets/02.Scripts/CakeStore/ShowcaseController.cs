using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;

public class ShowcaseController : MonoBehaviour
{


    // 컴포넌트
    private CakeManager cakeManager;
    private CakeUIController cakeUIController;



    // 현재 쇼케이스 UI 데이터
    private int showcasePlaceIndex;
    private int CAKEPLACENUM = 4;



    // 현재 쇼케이스 데이터
    [SerializeField]
    private Transform showcasePool;
    private GameObject[] showcases;
    private Showcase currentShowcase;
    public Showcase CurrentShowcase
    {
        get { return currentShowcase; }
    }



    void Start()
    {
        ResetController();
    }



    void Update()
    {
        if (Routine.instance.routineState == RoutineState.Close) CakeBack();
    }



    // 쇼케이스 컨트롤러 초기화
    void ResetController()
    {
        // 컴포넌트 할당
        cakeManager = CakeManager.instance;
        cakeUIController = GetComponent<CakeUIController>();


        // 케이크 쇼케이스, 버튼 초기화
        cakeUIController.SetUpShowcaseButtons();
        ResetShowcases();
    }



    // 케이크 쇼케이스 초기화
    void ResetShowcases()
    {
        // 쇼케이스 배열 초기화
        showcases = new GameObject[showcasePool.childCount];


        // 각 쇼케이스에 대한 설정
        for (int i = 0; i < showcasePool.childCount; i++)
        {
            showcases[i] = showcasePool.GetChild(i).gameObject;
            currentShowcase = showcases[i].GetComponent<Showcase>();


            // 데이터 초기화
            currentShowcase.showcaseIndex = i;
            currentShowcase.cakeType = new int[CAKEPLACENUM];
            currentShowcase.isCakeSelected = new bool[CAKEPLACENUM];
            currentShowcase.cakeImages = new GameObject[CAKEPLACENUM];
            for (int j = 0; j < CAKEPLACENUM; j++)
            {
                currentShowcase.cakeType[j] = -1;
                currentShowcase.isCakeSelected[j] = false;
                currentShowcase.cakeImages[j] = currentShowcase.transform.GetChild(j).gameObject;
            }


            // 쇼케이스 이미지 초기화
            currentShowcase.UpdateShowcaseImg();
        }
    }



    // 쇼케이스 내부에 배치할 위치 선택
    public void OnPlaceSelect(int idx)
    {
        cakeUIController.OpenShowcaseUI(1, idx);
        cakeUIController.CloseMenu(cakeUIController.showcasePlace);
    }



    // 쇼케이스에 전시할 메뉴 선택
    public void OnMenuSelect(int idx)
    {
        // 선택한 케이크가 부족하거나 선택한 위치에 이미 케이크가 존재하는 경우
        if (cakeManager.cakeDataList[idx].cakeCount == 0) return;
        if (currentShowcase.isCakeSelected[showcasePlaceIndex]) return;


        // 쇼케이스 데이터 업데이트
        currentShowcase.isCakeSelected[showcasePlaceIndex] = true;
        currentShowcase.cakeType[showcasePlaceIndex] = idx;
        currentShowcase.UpdateShowcaseImg();


        // 케이크 수 감소
        InventoryItem tempItem = new InventoryItem()
        {
            item = UIInventoryManager.instance.cakeItems[idx],
            quantity = 1,
        };
        UIInventoryManager.instance.MinusItem(tempItem);
        cakeManager.MinusCakeCount(idx);


        cakeUIController.CloseMenu(cakeUIController.showcaseMenu);
    }



    /// <summary>
    /// 쇼케이스의 포지션을 가져온다.
    /// </summary>
    /// <returns>쇼케이스의 포지션 배열</returns>
    public List<Transform> GetShowcasePos()
    {
        var showcasePoses = new List<Transform>();
        for (int i = 0; i < showcasePool.childCount; i++) showcasePoses.Add(showcasePool.GetChild(i).GetChild(11).transform);
        return showcasePoses;
    }



    /// <summary>
    /// 인덱스에 해당하는 케이크 위치를 찾는다.
    /// </summary>
    /// <param name="cakeIdx">찾고자 하는 케이크 인덱스</param>
    /// <returns>케이크가 위치한 쇼케이스 인덱스 목록<</returns>
    public List<int> CakeFindIndex(int cakeIdx)
    {
        var showcaseIndexes = new List<int>();


        // 모든 쇼케이스 및 위치를 순회하며 케이크 위치를 찾음
        for (int i = 0; i < showcases.Length; i++)
        {
            currentShowcase = showcases[i].GetComponent<Showcase>();


            // 찾고자 하는 케이크가 포함된 쇼케이스의 인덱스를 배열에 추가
            for (int j = 0; j < CAKEPLACENUM; j++)
                if (currentShowcase.cakeType[j] == cakeIdx)
                {
                    showcaseIndexes.Add(currentShowcase.showcaseIndex);
                    break;
                }
        }


        return showcaseIndexes;
    }



    /// <summary>
    /// 인덱스에 해당하는 케이크의 쇼케이스 내부 위치를 찾는다.
    /// </summary>
    /// <param name="showcaseIdx">찾고자 하는 케이크가 있는 쇼케이스의 인덱스</param>
    /// <param name="cakeIdx">찾고자 하는 케이크의 인덱스</param>
    /// <returns>케이크가 위치한 내부 배열의 인덱스 목록</returns>
    public List<int> CakeFindPlace(int showcaseIdx, int cakeIdx)
    {
        var showcasePlaceIndexes = new List<int>();
        currentShowcase = showcases[showcaseIdx].GetComponent<Showcase>();


        // 쇼케이스 내부에서 케이크의 위치를 찾음
        for (int i = 0; i < CAKEPLACENUM; i++)
            if (currentShowcase.cakeType[i] == cakeIdx)
                showcasePlaceIndexes.Add(i);


        return showcasePlaceIndexes;
    }



    /// <summary>
    /// 현재 쇼케이스를 변경한다.
    /// </summary>
    /// <param name="idx">현재 쇼케이스 인덱스 값</param>
    public void ChangeCurrentShowcase(int idx)
    {
        currentShowcase = showcases[idx].GetComponent<Showcase>();
    }



    /// <summary>
    /// 케이크를 가져간다.
    /// </summary>
    /// <param name="ShowcaseIdx">쇼케이스 자체의 인덱스</param>
    /// <param name="ShowcasePlaceIdx">쇼케이스 내부의 위치</param>
    public void TakeCake(int ShowcaseIdx, int ShowcasePlaceIdx)
    {
        currentShowcase = showcases[ShowcaseIdx].GetComponent<Showcase>();


        //케이크 데이터를 쇼케이스에서 제거
        currentShowcase.isCakeSelected[ShowcasePlaceIdx] = false;
        currentShowcase.cakeType[ShowcasePlaceIdx] = -1;
        currentShowcase.UpdateShowcaseImg();
    }



    /// <summary>
    /// 쇼케이스 내의 케이크를 모두 인벤토리로 되돌린다.
    /// </summary>
    public void CakeBack()
    {
        for (int i = 0; i < showcases.Length; i++) showcases[i].GetComponent<Showcase>().GetBack();
    }
}
