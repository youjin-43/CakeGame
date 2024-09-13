using UnityEngine;
using UnityEngine.UI;

public class CakeUIController : MonoBehaviour
{
    CakeManager cakeManager;
    MakerController makerController;
    ShowcaseController showcaseController;



    [Header("Showcase UI")]
    public GameObject showcasePlace;
    public GameObject showcaseMenu;
    public Transform showcaseScrollViewContent;



    [Header("Maker UI")]
    public GameObject makerPanel;
    public Transform makerScrollViewContent;



    // 자식 오브젝트 위치
    private enum PlacePanelElements { Text = 0, Plate = 1, Image = 2 }
    private enum MenuPanelElements { Image = 1, Name = 2, Count = 3, Price = 4, Locked = 6 }
    private enum CakePanelElements { Image = 1, Name = 2, Count = 3, Locked = 4, Clicked = 5 }
    private enum CakeClickedPanelElements { Cost = 3, MaterialPanel = 4, BakeButton = 5 }
    private enum CakeMaterialPanelElements { MaterialImage = 0, MaterialCount = 1 }



    void Start()
    {
        cakeManager = CakeManager.instance;


        makerPanel.SetActive(true);
        makerController = GetComponent<MakerController>();
        showcaseController = GetComponent<ShowcaseController>();
        makerScrollViewContent = makerPanel.GetComponentInChildren<HorizontalLayoutGroup>().transform;
        showcaseScrollViewContent = showcaseMenu.GetComponentInChildren<HorizontalLayoutGroup>().transform;


        // UI 끄기 
        CloseMenu(showcasePlace);
        CloseMenu(showcaseMenu);
        CloseMenu(makerPanel);
        UpdateUI();
    }



    /// <summary>
    /// 창을 닫는다.
    /// </summary>
    /// <param name="menu">닫을 창</param>
    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }



    /// <summary>
    /// 쇼케이스 UI창을 연다.
    /// <para>0: showcasePlace, 1: showcaseMenu</param>
    /// </summary>
    /// <param name="menu">열 창</param>
    /// <param name="idx">idx</param>
    public void OpenShowcaseUI(int menu, int idx)
    {
        switch (menu)
        {
            case 0:
                showcasePlace.SetActive(true);
                break;
            case 1:
                showcaseMenu.SetActive(true);
                break;
        }
        showcaseController.ChangeCurrentShowcase(idx);
        Debug.Log(idx);
        UpdateUI();
    }



    /// <summary>
    /// 메이커 UI창을 연다.
    /// </summary>
    /// <param name="idx">idx</param>
    public void OpenMakerUI(int idx)
    {
        makerPanel.SetActive(true);
        makerController.ChangeCurrentMaker(idx);
        UpdateUI();
    }



    /// <summary>
    /// 버튼 초기화
    /// </summary>
    /// <param name="button">버튼</param>
    /// <param name="action">클릭 시 실행할 함수</param>
    public void SetupButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }



    /// <summary>
    /// 메이커의 버튼 함수 연결
    /// </summary>
    public void SetUpMakerButtons()
    {
        for (int i = 0; i < makerScrollViewContent.childCount; i++)
        {
            int idx = i;
            var cakeMakingPanel = makerScrollViewContent.GetChild(idx);
            SetupButton(cakeMakingPanel.GetComponent<Button>(), () => cakeManager.audioManager.SetUISFX());

            SetupButton(cakeMakingPanel.GetComponent<Button>(), () => OnMakerClicked(idx));
            SetupButton(cakeMakingPanel.GetChild((int)CakePanelElements.Clicked).GetChild((int)CakeClickedPanelElements.BakeButton).GetComponent<Button>(), () => makerController.OnBakeClicked(idx));
            SetupButton(cakeMakingPanel.GetChild((int)CakePanelElements.Clicked).GetChild((int)CakeClickedPanelElements.BakeButton).GetComponent<Button>(), () => cakeManager.audioManager.SetUISFX());
        }
    }



    // 케이크 메이커 패널 누르면 Clicked패널 활성화
    void OnMakerClicked(int index)
    {
        for (int i = 0; i < makerScrollViewContent.childCount; i++)
        {
            makerScrollViewContent.GetChild(i).GetChild((int)CakePanelElements.Clicked).gameObject.SetActive(i == index); // 클릭된 패널만 활성화
        }
    }    // 버튼 초기화 (위치 선택 및 메뉴 선택 버튼)



    public void SetUpShowcaseButtons()
    {
        // 쇼케이스 내부의 위치 선택 버튼 설정
        for (int i = 0; i < CakeManager.instance.CAKEPLACENUM; i++)
        {
            int idx = i; // 로컬 변수로 캡처
            SetupButton(showcasePlace.transform.GetChild(i).GetComponent<Button>(), () => showcaseController.OnPlaceSelect(idx));
            SetupButton(showcasePlace.transform.GetChild(i).GetComponent<Button>(), () => cakeManager.audioManager.SetUISFX());
        }


        // 쇼케이스 메뉴 선택 버튼 설정
        for (int i = 0; i < showcaseScrollViewContent.childCount; i++)
        {
            int idx = i; // 로컬 변수로 캡처
            SetupButton(showcaseScrollViewContent.GetChild(i).GetComponent<Button>(), () => showcaseController.OnMenuSelect(idx));
            SetupButton(showcaseScrollViewContent.GetChild(i).GetComponent<Button>(), () => cakeManager.audioManager.SetUISFX());
        }
    }



    /// <summary>
    /// 전체 UI를 업데이트하는 메서드
    /// </summary>
    public void UpdateUI()
    {
        UpdateShowcasePlaceUI();
        UpdateShowcaseMenuUI();
        UpdateaMakerUI();

    }



    // 케이크 쇼케이스 위치 패널 업데이트
    void UpdateShowcasePlaceUI()
    {
        Showcase currentShowcase = showcaseController.CurrentShowcase;


        for (int i = 0; i < cakeManager.CAKEPLACENUM; i++)
        {
            showcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Text).gameObject.SetActive(!currentShowcase.isCakeSelected[i]);
            showcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Plate).gameObject.SetActive(currentShowcase.isCakeSelected[i]);
            showcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Image).gameObject.SetActive(currentShowcase.isCakeSelected[i]);


            if (currentShowcase.isCakeSelected[i])
            {
                showcasePlace.transform.GetChild(i).GetChild((int)PlacePanelElements.Image).GetComponent<Image>().sprite
                = cakeManager.cakeDataList[currentShowcase.cakeType[i]].itemImage;
            }
        }
    }



    // 케이크 쇼케이스 메뉴 패널 업데이트
    void UpdateShowcaseMenuUI()
    {
        for (int i = 0; i < showcaseScrollViewContent.childCount; i++)
        {
            Transform showcasePanel = showcaseScrollViewContent.GetChild(i);
            CakeData cakeData = cakeManager.cakeDataList[i];

            showcasePanel.GetChild((int)MenuPanelElements.Image).GetComponent<Image>().sprite = cakeData.itemImage;
            showcasePanel.GetChild((int)MenuPanelElements.Name).GetComponent<Text>().text = cakeData.name;
            showcasePanel.GetChild((int)MenuPanelElements.Count).GetComponent<Text>().text = "보유 수: " + cakeManager.cakeDataList[i].cakeCount;
            showcasePanel.GetChild((int)MenuPanelElements.Price).GetComponent<Text>().text = cakeData.cakePrice.ToString();
            showcasePanel.GetChild((int)MenuPanelElements.Locked).gameObject.SetActive(cakeData.isLock);
            showcasePanel.GetComponent<Button>().interactable = !cakeData.isLock;
        }
    }



    void UpdateaMakerUI()
    {
        for (int i = 0; i < makerScrollViewContent.childCount; i++)
        {
            Transform cakeMakingPanel = makerScrollViewContent.GetChild(i);
            var cakeSO = cakeManager.cakeDataList[i];


            //케이크 패널 업데이트
            cakeMakingPanel.GetChild((int)CakePanelElements.Image).GetComponent<Image>().sprite = cakeSO.itemImage;
            cakeMakingPanel.GetChild((int)CakePanelElements.Name).GetComponent<Text>().text = cakeSO.name;
            cakeMakingPanel.GetChild((int)CakePanelElements.Count).GetComponent<Text>().text = $"보유 수 : {cakeManager.cakeDataList[i].cakeCount}";


            //잠금 패널 업데이트
            bool isLocked = cakeSO.isLock;
            cakeMakingPanel.GetChild((int)CakePanelElements.Locked).gameObject.SetActive(isLocked);
            cakeMakingPanel.GetComponent<Button>().interactable = !isLocked;


            //클릭 시 패널 업데이트
            Transform clickedPanel = cakeMakingPanel.GetChild((int)CakePanelElements.Clicked);
            clickedPanel.gameObject.SetActive(false);
            clickedPanel.GetChild((int)CakeClickedPanelElements.Cost).GetComponent<Text>().text = $"{cakeSO.cakeCost}";


            
            Transform materialPanel = clickedPanel.GetChild((int)CakeClickedPanelElements.MaterialPanel);
            materialPanel.gameObject.SetActive(false);
            clickedPanel.GetChild(0).GetComponent<Text>().text = " 재료 : ";
            if (cakeSO.materialCount > 0)
            {
                // 재료 표시 업데이트
                materialPanel.gameObject.SetActive(true);
                var material = materialPanel.GetChild(0);
                material.gameObject.SetActive(true);


                // 재료 이미지 참조
                material.GetChild((int)CakeMaterialPanelElements.MaterialImage).GetComponent<Image>().sprite =
                 UIInventoryManager.instance.fruitItems[cakeSO.materialIdx].itemImage;


                // 재료 갯수 참조
                material.GetChild((int)CakeMaterialPanelElements.MaterialCount).GetComponent<Text>().text =
                 $"{UIInventoryManager.instance.fruitCount[cakeSO.materialIdx]}/{cakeSO.materialCount}";
            }
            else 
            {
                clickedPanel.GetChild(0).GetComponent<Text>().text = " 재료 :  X ";
            }
        }
    }
}
