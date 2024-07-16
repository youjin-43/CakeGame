using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawn : MonoBehaviour
{
    // 복사할 Prefab을 저장할 변수
    public GameObject customer;

    // 위치를 참조할 대상 오브젝트
    public Transform spawnPostion;

    // 인지도 변수 (0에서 10000까지의 값)
    [Range(0, 10000)]
    public int awareness = 5;

    // 최소 소환 간격 (초)
    public float minSpawnInterval = 1.0f;

    // 최대 소환 간격 (초)
    public float maxSpawnInterval = 10.0f;

    // Prefab을 생성할 회전값을 저장할 변수 (필요시)
    public Quaternion spawnRotation = Quaternion.identity;

    void Start()
    {
        // Prefab과 대상 오브젝트가 할당되었는지 확인
        if (customer != null && spawnPostion != null)
        {
            // 코루틴 시작
            StartCoroutine(SpawnPrefabCoroutine());
        }
        else
        {
            if (customer == null)
                Debug.LogError("Prefab이 할당되지 않았습니다.");
            if (spawnPostion == null)
                Debug.LogError("대상 오브젝트가 할당되지 않았습니다.");
        }
    }

    IEnumerator SpawnPrefabCoroutine()
    {
        while (true)
        {
            // 인지도 변수에 반비례한 시간 간격 계산 (0에서 1000s0 사이의 값을 0에서 1 사이의 값으로 변환)
            float normalizedAwareness = awareness / 10000.0f;
            float spawnInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, normalizedAwareness);
            
            // 지정된 시간 대기
            yield return new WaitForSeconds(spawnInterval);
            
            // 대상 오브젝트의 위치에 Prefab을 생성
            Instantiate(customer, spawnPostion.position, spawnRotation);
        }
    }
}