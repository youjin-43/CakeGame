using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedContainer : MonoBehaviour
{
    public GameObject[] prefabs;
    public List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int idx = 0; idx < pools.Length; idx++)
        {
            pools[idx] = new List<GameObject>();
        }
    }

    public GameObject GetSeed(int idx)
    {
        GameObject select = null;

        foreach (GameObject gameObj in pools[idx])
        {
            if (gameObj.activeSelf == false)
            {
                select = gameObj;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[idx], transform);
            pools[idx].Add(select);
        }

        return select;
    }
}
