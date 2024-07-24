using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    public CakeData cakeData;

    public void PrintCakeData()
    {
        Debug.Log("케이크 이름: " + cakeData.CakeName);
        Debug.Log("필요한 과일: " + cakeData.NeededFruitName);
        Debug.Log("필요한 과일 개수: " + cakeData.NeededFruitCount);
        Debug.Log("제작 시간: " + cakeData.MakeTime);
        Debug.Log("제작 가격: " + cakeData.MakeMoney);
        Debug.Log("판매 가격: " + cakeData.SellPrice);
    }
}
