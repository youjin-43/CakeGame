using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{ 
    // Customers 프리팹
    [Header("Prefab")]
    [SerializeField]
    private GameObject customersPrefab;


    // Bool 값
    [Header("Bool")]
    [SerializeField]
    private bool isSpawn = false; 



    // 손님 오브젝트에게 상속할 데이터
    [Header("Customer Data")]
    [SerializeField]
    private float speed = 2.0f; // 이동 속도
    [SerializeField]
    private float spawnDelay = 2.0f; // 생성 간격
    [SerializeField]
    private Sprite heartImg; //하트 이미지
    [SerializeField]
    private Transform spawnPos; // 생성 위치
    [SerializeField]
    private Transform cashierPos; // 계산대 위치
    [SerializeField]
    private CustomerSprites[] customerImgs; // 손님 이미지



    /// <summary>
    /// delay 간격으로 손님을 생성한다
    /// </summary>
    /// <param name="delay">손님 생성 간격</param> 
    /// <returns></returns>
    IEnumerator SpawnCustomers(float delay)
    {
        //쇼케이스 위치를 가져옴
        List<Transform> showcasePoses = CakeManager.instance.showcaseController.GetShowcasePos();


        while (true)
        {
            // 포탈음 재생
            CakeManager.instance.CallPortalAudio();


            // 손님 이미지 값 할당
            int r = Random.Range(0, customerImgs.Length);


            //손님 생성
            yield return new WaitForSeconds(0.5f); // 포탈음을 위한 0.5초 대기
            GameObject customer = Instantiate(customersPrefab, spawnPos.position, Quaternion.identity); // 빈 오브젝트 생성
            customer.transform.parent = transform; //부모 할당
            customer.GetComponent<Customers>().Initialize(
                speed,
                Random.Range(0.7f, 1.3f),
                heartImg,
                spawnPos,
                cashierPos,
                showcasePoses,
                customerImgs[r].idleImg,
                customerImgs[r].upImgs,
                customerImgs[r].downImgs
            );


            //생성 대기
            yield return new WaitForSeconds(delay);


            //영업 종료 5초전 손님 생성 멈춤
            if (GameManager.instance.MaxRunTime - GameManager.instance.runTime < 5)
            {
                StopCoroutine(SpawnCustomers(delay));
            }
        }
    }



    void Update()
    {
        // 루틴이 Open이고 생성 중이 아니면 생성 시작
        if (Routine.instance.routineState == RoutineState.Open && !isSpawn)
        {
            StartCoroutine(SpawnCustomers(spawnDelay));
            isSpawn = true;
        } 
        if (Routine.instance.routineState == RoutineState.Close && isSpawn)
        {
                isSpawn = false;
        }



        // 기능 테스트
        if (Input.GetKeyDown(KeyCode.S))
        {
            Routine.instance.routineState = RoutineState.Open;
        }
    }
}



/// <summary>
/// 손님의 이미지를 담는 클래스
/// </summary>
[System.Serializable]
public class CustomerSprites
{
    public Sprite idleImg;
    public Sprite[] upImgs;
    public Sprite[] downImgs;
}
