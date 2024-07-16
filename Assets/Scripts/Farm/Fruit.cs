using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    // 과일은 다 프리팹으로 만들어 놓을 것
    // 만들어놓은 프리팹은 FruitContainer 에 저장할 것..

    public string fruitName;
    public float fruitPrice;
    public bool isEnabled = false;

    public int fruitIdx = 0; // 과일 인덱스

    private void OnEnable()
    {
        Debug.Log("과일을 얻었다!");
    }
}
