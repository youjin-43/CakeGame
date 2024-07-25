using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeMaker : MonoBehaviour
{
    public GameObject scrollViewContent; // ScrollView의 Content를 참조
    private int[] cakeCounts; // 각 케이크의 보유 수를 저장하는 배열
    private bool[] cakeLocked; // 각 케이크가 잠겨 있는지 여부를 저장하는 배열
    public int cakeImageNum;
    public int cakeNameNum;
    public int cakeCountNum;
    public int cakeMaterialNum;
    public int cakeCostNum;
    public int cakeTimeNum;
    public int lockedNum;
    void Start()
    {
        int childCount = scrollViewContent.transform.childCount;
        cakeCounts = new int[childCount];
        cakeLocked = new bool[childCount];

        // 잠김 상태 초기화 (예를 들어 첫 번째 케이크만 잠금 해제된 상태)
        cakeLocked[0] = false;
        for (int i = 1; i < childCount; i++)
        {
            cakeLocked[i] = true;
        }

        for (int i = 0; i < childCount; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 지역 변수를 사용
            Button button = scrollViewContent.transform.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnCakeClicked(index));
            }
        }

        UpdateUI();
    }

    void OnCakeClicked(int index)
    {
        if (cakeLocked[index])
        {
            Debug.Log("이 케이크는 잠겨 있습니다!");
            return;
        }

        cakeCounts[index]++;
        Debug.Log("케이크 " + index + " 보유 수: " + cakeCounts[index]);

        // 다음 케이크 잠금 해제 로직
        if (index + 1 < cakeLocked.Length && cakeLocked[index + 1])
        {
            cakeLocked[index + 1] = false;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Transform panel = scrollViewContent.transform.GetChild(i);
            Text countText = panel.GetChild(cakeCountNum).GetComponent<Text>(); // 보유 수를 표시하는 텍스트 참조
            
            GameObject lockedPanel = panel.GetChild(lockedNum).gameObject; // 잠금 패널 참조
            

            countText.text = "보유 수: " + cakeCounts[i];
            lockedPanel.SetActive(cakeLocked[i]);

            Button button = panel.GetComponent<Button>();
            button.interactable = !cakeLocked[i]; // 버튼 클릭 가능 여부 설정
        }
    }
}