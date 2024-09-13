using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
public class MakerController : MonoBehaviour
{
    // 컴포넌트
    private CakeManager cakeManager;
    private CakeUIController cakeUIController;



    // 메이커 데이터
    [SerializeField]
    private Transform makersPool;
    private GameObject[] makers;



    // 타이머 이미지
    [SerializeField]
    private Sprite[] timerSprites;
    [SerializeField]
    private Sprite completedSprite;



    private Maker currentMaker;



    void Start()
    {
        ResetMakerController();
    }



    // 메이커 컨트롤러 초기화
    void ResetMakerController()
    {
        // 컴포넌트 할당
        cakeManager = CakeManager.instance;
        cakeUIController = GetComponent<CakeUIController>();

        
        // 버튼 세팅 및 메이커 초기화
        cakeUIController.SetUpMakerButtons();
        ResetMakers();
    }


    
    // 케이크 메이커들을 초기화
    void ResetMakers()
    {
        makers = new GameObject[makersPool.childCount];


        for (int i = 0; i < makers.Length; i++)
        {
            makers[i] = makersPool.GetChild(i).gameObject;
            var cakeMaker = makers[i].GetComponent<Maker>();

            // 데이터 초기화
            cakeMaker.cakeMakerIndex = i;
            cakeMaker.timerSprites = timerSprites;
            cakeMaker.completedSprite = completedSprite;
        }
    }



    /// <summary>
    ///  케이크 제작 버튼을 눌렀을 시 재료 및 돈 감소
    /// </summary>
    /// <param name="idx">제작하는 케이크 인덱스</param>
    public void OnBakeClicked(int idx) 
    {
        var cakeData = cakeManager.cakeDataList[idx];
        var fruitCounts = UIInventoryManager.instance.fruitCount;


        Debug.Log("재료 수 : "+fruitCounts[cakeData.materialIdx]);
        //재료와 돈이 부족한지 판단
        if (GameManager.instance.money < cakeData.cakeCost)
        {
            ReturnTextPanel.instance.SetInfoPanel("제작에 필요한 돈이 부족합니다.");
            Debug.Log("돈이 부족합니다.");
            return;
        }
        else if (fruitCounts[cakeData.materialIdx] < cakeData.materialCount)
        {
            ReturnTextPanel.instance.SetInfoPanel("재료가 부족합니다.");
            Debug.Log($"재료가 {cakeData.materialCount - fruitCounts[cakeData.materialIdx]}개 부족합니다.");
            return;
        }


        // 돈 감소  
        GameManager.instance.getMoney(-CakeManager.instance.cakeDataList[idx].cakeCost);

        
        // 재료 감소
        InventoryItem tempItem = new InventoryItem()
        {
            item = UIInventoryManager.instance.fruitItems[cakeData.materialIdx],
            quantity = cakeData.materialCount,
        };
        UIInventoryManager.instance.MinusItem(tempItem);


        // 창 닫고 제작 시작
        cakeUIController.CloseMenu(cakeUIController.makerPanel);
        currentMaker.StartMakingCake(idx, cakeManager.cakeDataList[idx].bakeTime);
    }



    /// <summary>
    /// 케이크 제작을 완료한다.
    /// </summary>
    /// <param name="idx">제작한 케이크 인덱스 값</param>
    public void CompleteCake(int idx)
    {
        // 케이크 추가
        InventoryItem tmpItem = new InventoryItem()
        {
            item = cakeManager.cakeDataList[idx],
            quantity = 1,
        };
        cakeManager.PlusCakeCount(idx);
        UIInventoryManager.instance.AddItem(tmpItem);
        cakeManager.soldCakeCounts[idx]++;


        // 경험치 추가
        ExpManager.instance.getExp(cakeManager.cakeDataList[idx].exp);
    }



    /// <summary>
    /// 현재 메이커를 변경한다.
    /// </summary>
    /// <param name="idx">현재 메이커 인덱스 값</param>
    public void ChangeCurrentMaker(int idx)
    {
        currentMaker = makers[idx].GetComponent<Maker>();
    }
}
