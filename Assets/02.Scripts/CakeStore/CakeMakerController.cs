using Inventory.Model;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class CakeMakerController : MonoBehaviour
{
    public GameObject cakeMakerPanel;                 // 케이크 메이커 패널
    private Transform cakeMakerScrollViewContent;     // 스크롤 뷰 콘텐츠

    // 케이크 메이크 패널 자식 오브젝트 순서 저장
    private enum CakePanelElements
    {
        Image = 0,
        Name = 1,
        Count = 2,
        Locked = 3,
        Clicked = 4
    }
    private enum CakeClickedPanelElements
    {
        Cost = 0,
        MaterialPanel = 1,
        BakeTime = 2,
        BakeButton = 3
    }
    private enum CakeMaterialPanelElements
    {
        MaterialImage = 0,
        MaterialCount = 1
    }

    public Transform cakeMakersPool;        // 케이크 메이커 가져올 곳 
    private GameObject[] cakeMakers;          // 케이크 메이커 배열
    public Sprite[] timerSprites;            // 타이머 변화 이미지 배열
    public Sprite completedSprite;           // 타이머 완료 시 이미지

    private int cakeMakerIndex;              // 현재 선택된 케이크 메이커의 인덱스

    private CakeManager cakeManager;         // 케이크 매니저 참조
    private CakeUIController cakeUIController;

    void Start()
    {
        InitializeCakeMakerController();
    }
    void InitializeCakeMakerController()
    {   
        cakeManager = CakeManager.instance;   // 케이크 매니저 초기화
        cakeUIController = CakeUIController.instance;
        cakeMakerPanel.SetActive(true);
        cakeMakerScrollViewContent = cakeMakerPanel.GetComponentInChildren<HorizontalLayoutGroup>().transform;
        InitializeCakeMakers();               // 케이크 메이커 초기화
        SetUpButtons();                       // 버튼 설정
        cakeUIController.CloseMenu(cakeMakerPanel);     // 케이크 메이커 패널 비활성화
    }
    void InitializeCakeMakers() // 케이크 메이커들을 초기화
    {
        cakeMakers = new GameObject[cakeMakersPool.childCount];

        for (int i = 0; i < cakeMakers.Length; i++)
        {
            cakeMakers[i] = cakeMakersPool.GetChild(i).gameObject; //케이크 메이커를 가져옴
            var cakeMaker = cakeMakers[i].GetComponent<CakeMaker>();
            cakeMaker.cakeMakerIndex = i;                                    // 케이크 메이커 인덱스 할당

            cakeMaker.timerSprites = timerSprites;             // 타이머 이미지 설정
            cakeMaker.completedSprite = completedSprite;       // 제작 완료 이미지 설정
        }
    }

    void SetUpButtons()
    {
        for (int i = 0; i < cakeMakerScrollViewContent.childCount; i++)
        {
            int index = i;
            var cakeMakingPanel = cakeMakerScrollViewContent.GetChild(index);

            cakeManager.SetupButton(cakeMakingPanel.GetComponent<Button>(), () => OnClicked(index));
            cakeManager.SetupButton(cakeMakingPanel.GetChild((int)CakePanelElements.Clicked).GetChild((int)CakeClickedPanelElements.BakeButton).GetComponent<Button>(), () => OnBakeClicked(index));
        }
    }
    public void OpenPanel(int index) //케이크메이커를 눌렀을 때 패널 활성화
    {
        cakeUIController.DisableSprites(true); // 스프라이트 비활성화
        cakeMakerPanel.SetActive(true);   // 케이크 메이커 패널 활성화
        cakeMakerIndex = index;           // 현재 케이크 메이커 인덱스 설정
        UpdateUI();                       // UI 업데이트
    }

    void OnClicked(int index) // 케이크 메이커 패널 누르면 Clicked패널 활성화
    {
        for (int i = 0; i < cakeManager.totalCakeNum; i++)
        {
            cakeMakerScrollViewContent.GetChild(i).GetChild((int)CakePanelElements.Clicked).gameObject.SetActive(i == index); // 클릭된 패널만 활성화
        }
    }

    void OnBakeClicked(int index) // 케이크 제작 버튼 누를 시 작동
    {
        var cakeData = cakeManager.cakeSODataList[index];
        var fruitCounts = UIInventoryController.instance.fruitCount;
        var inventory = UIInventoryController.instance.fruitInventoryData;

        if (GameManager.instance.money < cakeData.cakeCost)    // 돈이 충분한지 확인
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        for (int i = 0; i < cakeData.materialIdxs.Length; i++) // 재료가 충분한지 확인
        {
            if (fruitCounts[cakeData.materialIdxs[i]] < cakeData.materialCounts[i])
            {
                Debug.Log($"재료가 {cakeData.materialCounts[i] - fruitCounts[cakeData.materialIdxs[i]]}개 부족합니다.");
                return;
            }
        }

        GameManager.instance.money -= cakeData.cakeCost;       // 돈 감소
        for (int i = 0; i < cakeData.materialIdxs.Length; i++) // 재료 감소
        {
            ItemSO itemSO = inventory.GetItemAt(cakeData.materialIdxs[i]).item;  // ItemSO 추출
            inventory.MinusItem(itemSO, cakeData.materialCounts[i]);             // ItemSO 감소 
        }
        cakeUIController.CloseMenu(cakeMakerPanel);                        // 케이크 메이커 패널 비활성화        
        cakeMakers[cakeMakerIndex].GetComponent<CakeMaker>().         // 케이크 제작 시작
        StartMakingCake(index, cakeManager.cakeSODataList[index].bakeTime);
    }


    public void CompleteCake(int index) // 케이크 제작 완료
    {
        cakeManager.PlusCakeCount(index); // 케이크 수량 증가
        ExpManager.instance.getExp(0);        //-----------------------------fix need-----------------------
        UpdateUI();                           // UI 업데이트
    }

    void UpdateUI()
    {
        for (int i = 0; i < cakeMakerScrollViewContent.childCount; i++)
        {
            Transform cakeMakingPanel = cakeMakerScrollViewContent.GetChild(i);
            var cakeSO = cakeManager.cakeSODataList[i];

            //케이크 패널 업데이트
            cakeMakingPanel.GetChild((int)CakePanelElements.Image).GetComponent<Image>().sprite = cakeSO.itemImage;
            cakeMakingPanel.GetChild((int)CakePanelElements.Name).GetComponent<Text>().text = cakeSO.name;
            cakeMakingPanel.GetChild((int)CakePanelElements.Count).GetComponent<Text>().text = $"보유 수 : {cakeManager.cakeCounts[i]}";

            //잠금 패널 업데이트
            bool isLocked = cakeSO.isLocked;
            cakeMakingPanel.GetChild((int)CakePanelElements.Locked).gameObject.SetActive(isLocked);
            cakeMakingPanel.GetComponent<Button>().interactable = !isLocked;

            //클릭 시 패널 업데이트
            Transform clickedPanel = cakeMakingPanel.GetChild((int)CakePanelElements.Clicked);
            clickedPanel.gameObject.SetActive(false);
            clickedPanel.GetChild((int)CakeClickedPanelElements.Cost).GetComponent<Text>().text = $"{cakeSO.cakeCost}";
            clickedPanel.GetChild((int)CakeClickedPanelElements.BakeTime).GetComponent<Text>().text = $"{cakeSO.bakeTime}초";

            // 재료 표시 업데이트
            Transform materialPanel = clickedPanel.GetChild((int)CakeClickedPanelElements.MaterialPanel);
            materialPanel.gameObject.SetActive(true);
            for (int j = 0; j < materialPanel.childCount; j++)
            {
                if (j < cakeSO.materialCounts.Length)
                {
                    var material = materialPanel.GetChild(j);
                    material.gameObject.SetActive(true);
                    // 재료 이미지 참조
                    material.GetChild((int)CakeMaterialPanelElements.MaterialImage).GetComponent<Image>().sprite =   
                     UIInventoryController.instance.fruitItems[cakeSO.materialIdxs[j]].itemImage;
                    // 재료 갯수 참조
                    material.GetChild((int)CakeMaterialPanelElements.MaterialCount).GetComponent<Text>().text =      
                     $"{cakeSO.materialCounts[j]}/{UIInventoryController.instance.fruitCount[cakeSO.materialIdxs[j]]}";
                }
                else materialPanel.GetChild(j).gameObject.SetActive(false);
            }
        }
        // 케이크 쇼케이스 UI 업데이트
        cakeManager.GetComponent<CakeShowcaseController>().UpdateUI();
    }
}
