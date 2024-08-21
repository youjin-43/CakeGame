using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject[] prefabs;

    [SerializeField]
    public List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int idx = 0; idx < pools.Length; idx++)
        {
            pools[idx] = new List<GameObject>();
        }
    }

    private void Update()
    {
        // 임시 확인용
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetAnimal(0);
        }
    }

    public GameObject GetAnimal(int idx)
    {
        GameObject select = null;

        foreach (GameObject gameObj in pools[idx])
        {
            if (gameObj.activeSelf == false)
            {
                select = gameObj;
                select.GetComponent<Animal>().ResetAnimalData();
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[idx], transform);
            select.GetComponent<Animal>().ResetAnimalData();
            pools[idx].Add(select);
        }

        return select;
    }

    public void DisableAnimal(int idx)
    {
        pools[idx].Clear(); // 싹 다 없애버리기..
    }
}