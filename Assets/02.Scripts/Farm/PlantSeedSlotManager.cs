using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantSeedSlotManager : MonoBehaviour
{
    [Header("FarmingManager")]
    public FarmingManager farmingManager;

    [Header("Slot Information")]
    public Text seedNameText; // 씨앗 이름 텍스트
    public Text seedCountText; // 씨앗 개수 텍스트
    public Image seedImage; // 씨앗 이미지
    public int seedIdx; // 씨앗 인덱스

    private void Start()
    {
        farmingManager = FindObjectOfType<FarmingManager>();
        transform.GetComponent<Button>().onClick.AddListener(ClickedPlantSeedButton);
    }

    public void ClickedPlantSeedButton()
    {
        farmingManager.plantSeedPanel.gameObject.SetActive(false); // 버튼 눌리는 즉시에 판넬 꺼버리기

        // 씨앗의 개수가 0보다 작거나 같으면 그냥 빠져나가도록..
        if (farmingManager.seedContainer.seedCount[seedIdx] <= 0) {
            Debug.Log("씨앗 없어!!!!");
            return; 
        } 

        farmingManager.selectedSeedIdx = seedIdx; // 현재 심을 씨앗 인덱스를 설정
        farmingManager.clickedSelectedSeedButton = true; // 버튼이 클릭됐다는 걸 알려줌..
        farmingManager.seedContainer.seedCount[seedIdx]--; // 버튼 클릭하면 씨앗 심는거니까 씨앗 개수 줄어들도록
    }
}
