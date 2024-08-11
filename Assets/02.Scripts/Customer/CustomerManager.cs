using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    public GameObject customersPrefab;    // Customers 프리팹
    public Transform spawnPoint;          // Customers가 생성될 위치
    public List<Vector2> pathPoints;      // Customers가 이동할 경로
    public Vector2 pathLine;
    public float customersSpawnInterval = 2.0f; // Customers 생성 간격
    public float lineSpacing = 0.5f;      // 줄서기 간격
    public float sideSpacing = 0.5f;      // 줄옆 간격
    public float moveSpeed = 2.0f;        // 이동 속도

    public List<Customers> customersList = new List<Customers>();

    void Start()
    {
        StartCoroutine(SpawnCustomers());
    }

    IEnumerator SpawnCustomers()
    {
        while (true)
        {
            GameObject customersObject = Instantiate(customersPrefab, spawnPoint.position, Quaternion.identity);
            Customers customers = customersObject.GetComponent<Customers>();
            customers.Initialize(pathPoints,pathLine, moveSpeed, lineSpacing, sideSpacing, this);
            customersList.Add(customers);
            yield return new WaitForSeconds(customersSpawnInterval);
        }
    }

    public Transform GetFrontCustomer(Customers currentCustomer)
    {
        if (customersList.Count == 0)
        {
            return null;
        }

        int index = customersList.IndexOf(currentCustomer);
        if (index > 0)
        {
            return customersList[index - 1].transform;
        }

        return null;
    }
    public Transform GetFirstCustomer(Customers currentCustomer)
    {
        if (customersList.Count == 0)
        {
            return null;
        }
        int index = customersList.IndexOf(currentCustomer);
        if (index > 0)
        {
            return customersList[0].transform;
        }
        return null;
    }
}
