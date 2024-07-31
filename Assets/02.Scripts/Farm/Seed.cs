using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    // 씨앗은 다 프리팹으로 만들어 놓을 것
    // 만들어놓은 프리팹은 SeedContainer 에 저장할 것..

    public float currentTime; // 심은 후부터 현재까지 시간
    public bool isGrown = false; // 다 자랐는지 여부 확인용 변수

    public SeedItemSO seedData; // 씨앗 데이터(씨앗 이름, 씨앗 가격, 성장 시간, 씨앗 인덱스)


    private void OnEnable()
    {
        isGrown = false;
        currentTime = 0;
        Debug.Log("씨앗을 얻었다!");
    }

    private void Update()
    {
        if (currentTime >= seedData.growTime)
        {
            isGrown = true;
            Debug.Log("다 자랐다!");
            transform.gameObject.SetActive(!isGrown);
        }
        currentTime += Time.deltaTime;
    }
}
