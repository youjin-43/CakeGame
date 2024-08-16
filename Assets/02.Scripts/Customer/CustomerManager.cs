using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    public GameObject customersPrefab;    // Customers 프리팹
    public Transform spawnPoint;          // Customers가 생성될 위치
    public List<Vector2> pathPoints;      // Customers가 이동할 경로
    public Vector2 linePosition;
    public Vector2 enterOutPosition;
    public Vector2 enterInPosition;
    public Vector2 cashierPosition;

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
            int wantedCakeIndex = Random.Range(0, CakeManager.instance.totalCakeNum);
            customers.Initialize(pathPoints, linePosition, enterOutPosition, enterInPosition, cashierPosition, moveSpeed, lineSpacing, sideSpacing, wantedCakeIndex, this);
            customers.moveType = CustomersMoveType.MoveType.Move;
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
    public int GetCustomerNum(Customers currentCustomer)
    {
        if (customersList.Count == 0)
        {
            return -1;
        }
        int index = customersList.IndexOf(currentCustomer);
        if (index >= 0)
        {
            return index-1;
        }
        return -1;
    }
}

public class CustomersMoveType
{
    public enum LineType
    {
        None,
        Start,
        During,
        End
    }
    public enum EnterType
    {
        None,
        In,
        Out
    }
    public enum ShopType
    {
        None,
        Check,
        Shop,
        Pay,
        In,
        Out
    }
    public enum MoveType
    {
        None,
        Move,
        Line,
        Enter,
        Random,
        Shop
    }
}
