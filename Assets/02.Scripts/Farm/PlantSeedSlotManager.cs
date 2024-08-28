using Inventory.Model;
using System;
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


    // 델리게이트
    [Header("Delegate")]
    public Action<int> OnClickedPlantSeedButton;


    private void Start()
    {
        farmingManager = FindObjectOfType<FarmingManager>();
        transform.GetComponent<Button>().onClick.AddListener(ClickedPlantSeedButton);
    }

    public void ClickedPlantSeedButton()
    {
        farmingManager.plantSeedPanel.gameObject.SetActive(false); // 버튼 눌리는 즉시에 판넬 꺼버리기


        OnClickedPlantSeedButton?.Invoke(seedIdx); // 현재 심을 씨앗 인덱스를 매개변수로 넘겨주면서 연결된 함수 호출함..
    }
}
