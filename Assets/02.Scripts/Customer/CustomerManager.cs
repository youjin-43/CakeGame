using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public GameObject customersPrefab;    // Customers 프리팹
    public Transform[] spawnPoint;          // Customers가 생성될 위치
    public Transform leftEnd, rightEnd;
    public Transform linePosition;
    public Transform enterOutPosition;
    public Transform enterInPosition;
    public Transform cashierPosition;
    public Sprite[] customerImages;
    public Sprite heart;

    public float customersSpawnInterval = 2.0f; // Customers 생성 간격
    public float lineSpacing = 0.5f;      // 줄서기 간격
    public float sideSpacing = 0.5f;      // 줄옆 간격
    public float moveSpeed = 2.0f;        // 이동 속도

    public List<Customers> customersList = new List<Customers>();
    private bool isStartSpawning = false;

    void Update()
    {
        if (Routine.instance.routineState == RoutineState.Prepare && !isStartSpawning)
        {
            StartCoroutine(SpawnCustomers());
            isStartSpawning = true;
        }
        for (int i = 0; i < customersList.Count; i++)
        {
            if (customersList[i] == null)
            {
                customersList.Remove(customersList[i]);
            }
        }
    }

    IEnumerator SpawnCustomers()
    {
        while (true)
        {
            int r = Random.Range(0, spawnPoint.Length);
            GameObject customersObject = Instantiate(customersPrefab, spawnPoint[r].position, Quaternion.identity);
            customersObject.transform.parent = this.transform;
            Customers customers = customersObject.GetComponent<Customers>();
            if (r % 2 == 0) customers.targetPosition = leftEnd.position;
            else customers.targetPosition = rightEnd.position;
            int wantedCakeIndex = Random.Range(0, CakeManager.instance.TOTALCAKENUM);
            customers.GetComponent<NavMeshAgent>().speed = Random.Range(moveSpeed * 0.7f, moveSpeed * 1.3f);
            customers.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = customerImages[Random.Range(0, customerImages.Length)];
            float randomSize = Random.Range(0.7f, 1.3f);
            customers.transform.GetChild(1).GetComponent<Transform>().localScale = new Vector3(randomSize, randomSize, 1);
            customers.Initialize(leftEnd, rightEnd, linePosition, enterOutPosition, enterInPosition, cashierPosition, moveSpeed, lineSpacing, sideSpacing, wantedCakeIndex, this);
            yield return new WaitForSeconds(customersSpawnInterval);
            if (Routine.instance.routineState == RoutineState.Close)
            {
                StopCoroutine(SpawnCustomers());
                isStartSpawning = false;
            }
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
            if (customersList[index - 1] != null)
            {
                return customersList[index - 1].transform;
            }
            else return null;
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
            return index;
        }
        return -1;
    }
}

public class CustomersMoveType
{
    public enum LineType
    { None, Start, During, End }
    public enum EnterType
    { None, In, Out }
    public enum ShopType
    { None, Check, Shop, Pay, In, Out }
    public enum MoveType
    { None, Move, Line, Enter, Random, Shop }
}
