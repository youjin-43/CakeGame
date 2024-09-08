using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Fruit Data", menuName = "Scriptable Object/Fruit Data", order = int.MaxValue)]
public class FruitItemSO : ItemSO
{
    [SerializeField]
    public int fruitPrice;

    [SerializeField]
    public int fruitIdx; // 과일 인덱스
}
