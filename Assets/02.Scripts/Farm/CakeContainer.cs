using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Apple: 0, Banana: 1, ...
public enum CakeType { Apple, Banana, Cherry, Orange, Strawberry};

public class CakeContainer : MonoBehaviour
{
    [SerializeField]
    private List<CakeData> cakeDatas;

    [SerializeField]
    private GameObject cakePrefab;

    [SerializeField]
    public GameObject[] cakeCount; // 케이크 개수 저장.. [0]:사과, [1]:바나나, [2]:체리, [3]: 오렌지, [4]: 딸기

    private void Start()
    {
        for (int i=0; i<cakeDatas.Count; i++)
        {
            Cake cake = MakeCake((CakeType)i);
            cake.PrintCakeData();
        }
    }


    public Cake MakeCake(CakeType type)
    {
        Cake newCake = Instantiate(cakePrefab).GetComponent<Cake>();
        newCake.cakeData = cakeDatas[(int)type];
        newCake.name = newCake.cakeData.CakeName; // 게임 오브젝트의 이름을 설정..
        return newCake;
    }
}
