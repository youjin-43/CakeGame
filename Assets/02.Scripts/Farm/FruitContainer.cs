using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitContainer : MonoBehaviour
{
    public GameObject[] prefabs; // 과일 프리팹을 저장하기 위한 배열
    public List<GameObject>[] pools; // 과일이 생성되면 저장할 공간

    public int totalFruitCount;

    private void Awake()
    {
        totalFruitCount = 0;
        pools = new List<GameObject>[prefabs.Length]; // 프리팹의 개수만큼 리스트 배열 생성

        for (int idx = 0; idx < prefabs.Length; idx++)
        {
            pools[idx] = new List<GameObject>(); // 리스트 배열 요소에 리스트 생성
        }
    }

    public GameObject GetFruit(int idx)
    {
        GameObject select = null;

        // 선택된 인덱스에 들어있는 게임 오브젝트들을 for 문을 통해 모두 확인함.
        foreach (GameObject obj in pools[idx])
        {
            // 만약 놀고 있는 게임 오브젝트가 있다면(활성화 되지 않은 게임 오브젝트가 있다면) 가져오기
            if (!obj.activeSelf)
            {
                select = obj;
                obj.SetActive(true);
                break;
            }
        }

        // 놀고 있는 게임 오브젝트가 없다면(다 활성화 되어서 게임상에서 사용되고 있다면) 새로 만들기
        if (!select)
        {
            select = Instantiate(prefabs[idx], transform);
            pools[idx].Add(select);
        }

        totalFruitCount++;

        return select;
    }
}
