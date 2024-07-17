using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    // 씨앗은 다 프리팹으로 만들어 놓을 것
    // 만들어놓은 프리팹은 SeedContainer 에 저장할 것..

    public string seedName;
    public float seedPrice;
    public float growTime; // 성장하는데 걸리는 시간
    public float currentTime; // 심은 후부터 현재까지 시간

    public bool isPlanted = false; // 이 값이 트루가 되어야 currentTime 에 시간을 더하기 시작할 수 있음(씨앗 구매하자마자 시간 오르면 안되니까..)
    public bool isGrown = false; // 다 자랐는지 여부 확인용 변수

    public int seedIdx; // 씨앗 인덱스


    private void OnEnable()
    {
        isGrown = false;
        currentTime = 0;
        Debug.Log("씨앗을 얻었다!");
    }

    private void Update()
    {
        if (currentTime >= growTime)
        {
            isGrown = true;
            Debug.Log("다 자랐다!");
            transform.gameObject.SetActive(!isGrown);
        }
        currentTime += Time.deltaTime;
    }
}
