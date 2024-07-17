using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuySeedSlotManager : MonoBehaviour
{
    [Header("FarmingManager")]
    public FarmingManager farmingManager;

    [Header("Slot Button UI")]
    public Image seedImage;
    public Text seedName;
    public GameObject BuySlot;
    public Text totalPrice;
    public Text seedCountText;
    public Button leftButton;
    public Button rightButton;
    public Button buySeedButton;

    [Header("Slot Imformation")]
    public int prevSeedCount = 1;
    public int seedCount = 1;
    public int maxCount = 64;
    public int minCount = 1;
    public int seedIdx; // BuySeedUIManager 에서 값 설정해줄 것..


    private void Start()
    {
        farmingManager = FindObjectOfType<FarmingManager>(); // 음.. 처음에는 BuySeedUIManager 에서 슬롯 정보 초기화 해줄 때 이 값도 초기화 해주려고 했는데 안됨. 왜?? 그래서 일단 find 함수 사용
        buySeedButton.onClick.AddListener(() => farmingManager.BuySeed(1, seedIdx)); // 일단 초기 함수 연결 해놓기
    }

    private void Update()
    {

        if (IsPointerOverUIObject()) return;
        else
        {
            // UI 가 아닌 부분을 클릭하면 그냥 꺼지도록..
            if (Input.GetMouseButtonDown(0))
            {
                BuySlot.SetActive(false);
            }
        }
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void minusSeedCount()
    {
        if (seedCount <= minCount)
        {
            // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최소 개수보다 작아지는 순간 최댓값으로 넘어가도록
            seedCount = maxCount + 1;
        }

        seedCount--;

        buySeedButton.onClick.RemoveAllListeners(); // 현재 선택 씨앗 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        buySeedButton.onClick.AddListener(() => farmingManager.BuySeed(seedCount, seedIdx));
    }

    public void plusSeedCount()
    {
        // 현재 구매하려고 하는 씨앗 개수가 씨앗 구매 최대 개수보다 커지는 순간 최솟값으로 넘어가도록
        if (seedCount >= maxCount)
        {
            seedCount = minCount - 1;
        }

        seedCount++;

        buySeedButton.onClick.RemoveAllListeners(); // 현재 선택 씨앗 개수가 변경되었으므로 모든 Listener 를 제거하고 시작
        buySeedButton.onClick.AddListener(() => farmingManager.BuySeed(seedCount, seedIdx));
    }


    public void ResetData()
    {
        seedCount = 1;
    }
}