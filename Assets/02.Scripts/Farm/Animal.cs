using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Animal : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public int speed;

    [SerializeField]
    public FarmingManager farmingManager;

    [SerializeField]
    public AnimalInteractionManager animalInteractionManager;

    [SerializeField]
    public Vector3 startPos; // 동물이 스폰될 위치

    [SerializeField]
    public Vector3 targetPos; // 동물이 도착할 위치

    [SerializeField]
    public float stopTime; // 목적지에 도달한 후 지난 시간

    [SerializeField]
    public float failTime; // 이 시간에 도달하면 농작물이 망함..

    [SerializeField]
    float tmpTime = 0;

    [SerializeField]
    public Action OnClicked, OnDamaged, OnFailed, OnDisappeared, OnExited;



    // 동물 생성&도착 위치 관련
    [SerializeField]
    public List<Vector3Int> seedFarmingDataPos; // 씨앗이 있는 땅의 위치 저장용

    [SerializeField]
    public List<Vector3Int> farmingDataPos; // 모든 땅 위치 저장용

    [SerializeField]
    public Tilemap farmTilemap;



    // 애니메이션 관련
    [SerializeField]
    public Animator animator;



    private void Awake()
    {
        farmTilemap = GameObject.Find("farmTilemap").GetComponent<Tilemap>();
        farmingManager = FindAnyObjectByType<FarmingManager>();
        animalInteractionManager = FindAnyObjectByType<AnimalInteractionManager>();

        seedFarmingDataPos = new List<Vector3Int>();
        farmingDataPos = new List<Vector3Int>();

        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 리스트 요소 다 지워버리고..
        seedFarmingDataPos.Clear();
        farmingDataPos.Clear();

        foreach (var item in farmingManager.farmingData)
        {
            // 일단 농사 가능 구역 다 넣고..
            farmingDataPos.Add(item.Key);

            // 씨앗이 있고 망하지 않은 상태의 타일 위치만 리스트에 넣을 것..
            if (farmingManager.farmingData[item.Key].seedOnTile && !farmingManager.farmingData[item.Key].failedState)
                seedFarmingDataPos.Add(item.Key);
        }
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position == targetPos)
        {
            stopTime += Time.deltaTime;
            tmpTime += Time.deltaTime;

            // 데미지 입을 정도의 시간이 지났는데도 동물 못잡으면 그냥 땅 망하도록..
            if (stopTime >= failTime)
            {
                // 만약 이미 망한 상태면
                if (farmingManager.farmingData[farmTilemap.WorldToCell(targetPos)].failedState)
                {
                    OnDisappeared?.Invoke();
                    gameObject.SetActive(false);
                    return; // 그냥 빠져나가도록..
                }
                else
                {
                    Debug.Log("농작물이 망했어여!!");
                    // 현재 동물이 도착해있는 타일의 농사 상태 망치기
                    OnFailed?.Invoke(); // 델리게이트에 연결된 함수 호출..
                    farmingManager.FailFarmAt(farmTilemap.WorldToCell(targetPos));
                    OnDisappeared?.Invoke();
                    gameObject.SetActive(false);
                }       
            }
            // 데미지 입을 정도의 시간이 지나지 않으면 그냥 1초마다 일수 +1 해주기..
            else
            {
                // 음.. 과일이 다 안 자랐을 때만 일수 늘어나도록..
                // 과일이 다 자랐을 때도, 일수 늘어나도록 해야할지 말지 고민중... -_-
                // 근데 이미 다 자란 과일이 다시 자라기 상태로 돌아가는 것도 좀 이상하지 않나 -_-
                // 이미 다 자란 과일은 그냥 망하도록 하는 것밖엔 경우가 없을 것같기도 하고.. 음....
                if (!farmingManager.farmingData[farmTilemap.WorldToCell(targetPos)].isGrown)
                {
                    // 1.5초 지날 때마다 일수 +1..
                    if (tmpTime >= 1f)
                    {
                        animator.SetTrigger("Attack");

                        // 만약 이미 망한 상태면
                        if (farmingManager.farmingData[farmTilemap.WorldToCell(targetPos)].failedState)
                            return; // 그냥 빠져나가도록..

                        Debug.Log("으악!! 자라기까지 일수가 늘어났다");
                        // 현재 일수 값을 1 만큼 감소시켜주면 됨!
                        OnDamaged?.Invoke(); // 델리게이트에 연결된 함수 호출..
                        farmingManager.farmingData[farmTilemap.WorldToCell(targetPos)].curDay--;
                        tmpTime = 0;
                    }
                }
            }
        }
    }

    public void SetPos()
    {
        // 시작 위치 정해주기
        int randomIdx = Random.Range(0, farmingDataPos.Count);
        startPos = farmTilemap.CellToWorld(farmingDataPos[randomIdx]);

        // 도착 위치 정해주기
        randomIdx = Random.Range(0, seedFarmingDataPos.Count);
        if (randomIdx < 0 || randomIdx >= seedFarmingDataPos.Count)
        {
            animalInteractionManager.exitGame = true; // 남아있는 씨앗 존재 땅이 없으면 강제로 게임 꺼버리기
            OnExited?.Invoke();
        }
        else
            targetPos = farmTilemap.CellToWorld(seedFarmingDataPos[randomIdx]);
    }

    public void ResetAnimalData()
    {
        stopTime = 0;
        SetPos();
        transform.position = startPos;
    }

    public void DieAnimal()
    {
        gameObject.SetActive(false);
    }


    // 모바일에서는 OnmouseDown 같은 함수를 이용할 수 없다고 함.
    // 그래서 IPointerClickHandler 인터페이스를 가져온 후 함수 만들어주기..
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("개구리 클릭했어용!");
        OnClicked?.Invoke();
        OnDisappeared?.Invoke();
        DieAnimal();
        ResetAnimalData();
    }
}
