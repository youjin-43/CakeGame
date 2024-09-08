using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Seed Data", menuName = "Scriptable Object/Seed Data", order = int.MaxValue)]
public class SeedItemSO : ItemSO
{
    // ItemSO 의 공통적인 속성을 상속받기..
    // 이 클래스는 Seed 클래스에서 seedData 라는 이름의 변수로 이용될 것..

    [SerializeField]
    public int seedPrice; // 씨앗 구매 가격

    [SerializeField]
    public int seedIdx; // 씨앗 인덱스


    // 이제 growTime 안 쓰고 growDay 쓸 것..
    [SerializeField]
    public int growDay; // 다 자라는데 걸리는 일수
}
