using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    // 씨앗 클래스도 이제 숙청대상..
    // 지금은 기존 만들어놓은 씨앗 구매, 선택창에서 쓰고 잇어서 아직은 없애면 안됑
    // 다음에 차차 없애야할듯..


    // 생성자.. 그 팜 씬으로 돌아왔을 때, 이전 농장의 상태를 반영해주기 위함..
    public Seed(float currentTime, bool isGrown)
    {
        this.currentTime = currentTime;
        this.isGrown = isGrown;
    }

    // 씨앗은 다 프리팹으로 만들어 놓을 것
    // 만들어놓은 프리팹은 SeedContainer 에 저장할 것..

    public float currentTime; // 심은 후부터 현재까지 시간
    public bool isGrown = false; // 다 자랐는지 여부 확인용 변수

    public SeedItemSO seedData; // 씨앗 데이터(씨앗 이름, 씨앗 가격, 성장 시간, 씨앗 인덱스)
}
