using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawn : MonoBehaviour
{
    // 복사할 Prefab을 저장할 변수
    public GameObject customer;

    // 위치를 참조할 대상 오브젝트 배열
    public Transform[] spawnPosition;

    // 인지도 변수 (0에서 10000까지의 값)
    [Range(0, 10000)]
    public int awareness = 5000;

    // 소환 시도 간격 (초)
    public float spawnAttemptInterval = 1.0f;

    // Prefab을 생성할 회전값을 저장할 변수 (필요시)
    public Quaternion spawnRotation = Quaternion.identity;

    void Start()
    {
        // Prefab과 대상 오브젝트가 할당되었는지 확인
        if (customer != null && spawnPosition != null && spawnPosition.Length > 0)
        {
            // 코루틴 시작
            StartCoroutine(SpawnPrefabCoroutine());
        }
        else
        {
            if (customer == null)
                Debug.LogError("Prefab이 할당되지 않았습니다.");
            if (spawnPosition == null || spawnPosition.Length == 0)
                Debug.LogError("대상 오브젝트 배열이 할당되지 않았거나 비어 있습니다.");
        }
    }

    void SpawnPrefab()
    {
        // 랜덤한 위치 선택
        int i = Random.Range(0, spawnPosition.Length);
        // 선택된 위치에 Prefab을 생성
        GameObject Customer = Instantiate(customer, spawnPosition[i].position,spawnRotation);
        if(i == 0){
            Customer.GetComponent<Move>().initialDirection = Move.Direction.RightDown;
        }
        else{
            Customer.GetComponent<Move>().initialDirection = Move.Direction.LeftDown;
        }

    }

    IEnumerator SpawnPrefabCoroutine()
    {
        while (true)
        {
            // 지정된 시간 대기
            yield return new WaitForSeconds(spawnAttemptInterval);

            // 인지도에 기반한 소환 확률 계산 (0에서 10000 사이의 값을 0에서 1 사이의 값으로 변환)
            float spawnProbability = awareness / 10000.0f;

            // 랜덤 값을 생성하여 소환 확률과 비교
            if (Random.value < spawnProbability)
            {
                // 소환 성공
                SpawnPrefab();
            }
        }
    }
}
