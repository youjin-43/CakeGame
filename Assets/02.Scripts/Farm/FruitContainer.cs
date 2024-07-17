using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitContainer : MonoBehaviour
{
    public GameObject[] prefabs;
    public List<GameObject>[] pools;

    public int fruitCount;

    private void Awake()
    {
        fruitCount = 0;
        pools = new List<GameObject>[prefabs.Length];

        for (int idx = 0; idx < prefabs.Length; idx++)
        {
            pools[idx] = new List<GameObject>();
        }
    }

    public GameObject GetFruit(int idx)
    {
        GameObject select = null;

        foreach (GameObject obj in pools[idx])
        {
            if (!obj.activeSelf)
            {
                select = obj;
                obj.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[idx], transform);
            pools[idx].Add(select);
        }

        fruitCount++;

        return select;
    }
}
