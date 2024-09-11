//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public GameObject customersPrefab;    // Customers 프리팹
    public Transform[] spawnPoint;          // Customers가 생성될 위치
    public Transform currentSpawnPoint, cashierPosition;
    public CustomerSprites[] customerImages;
    public Sprite heart;

    public float customersSpawnInterval = 2.0f; // Customers 생성 간격
    public float moveSpeed = 2.0f;        // 이동 속도

    public List<Customers> customersList = new List<Customers>();
    private bool isStartSpawning = false;

    void Update()
    {
        if (Routine.instance.routineState == RoutineState.Open && !isStartSpawning)
        {
            StartCoroutine(SpawnCustomers());
            isStartSpawning = true;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("s");
            Routine.instance.routineState = RoutineState.Open;
        }
    }
    IEnumerator SpawnCustomers()
    {
        while (true)
        {
            Debug.Log("spawning");
            CakeManager.instance.CallPortalAudio();
            yield return new WaitForSeconds(0.5f);
            GameObject customersObject = Instantiate(customersPrefab, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);
            Customers customers = customersObject.GetComponent<Customers>();
            Debug.Log(customersObject);
            customersObject.transform.parent = transform;
            float scale = Random.Range(0.7f, 1.3f);
            customers.sprites = new Sprite[9];
            customers.sprites = customerImages[Random.Range(0,customerImages.Length)].sprites;
            customers.Initialize(currentSpawnPoint, cashierPosition, moveSpeed, scale, this);
            yield return new WaitForSeconds(customersSpawnInterval);
            if(GameManager.instance.MaxRunTime - GameManager.instance.runTime < 0)
            {
                StopCoroutine(SpawnCustomers());
                isStartSpawning = false;
            }
        }
    }
}
[System.Serializable]
public class CustomerSprites
{
    public Sprite[] sprites;
}
