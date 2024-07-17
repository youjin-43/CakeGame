using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerFalse : MonoBehaviour
{
    public enum States
    {
        moving, // 거리활보중 
        waiting,// 케이크 받길 기다리는 중 
        goback // 받고 돌아가는 중 
    }

    [SerializeField]private States currentState;

    private float moveSpeed = 4f; // NPC의 이동 속도

    private Vector2 moveDirection; // NPC의 이동 방향
    int direction; // NPC의 이동 방향 

    //private bool isMovingToTarget = false; // 타겟 위치로 이동 중인지 여부 -> state를 enum으로 나눔 

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentState = States.moving;
        //moveDirection = new Vector2(1, -0.5f).normalized; // 초기 이동 방향 설정 (오른쪽 대각선 아래) -> 이후에는 손님 양방향에서 오도록 수정
        direction = 3;
        SetMoveDirection(direction);
        //StartCoroutine(MoveToTarget(GameObject.Find("Corner").GetComponent<Transform>().position, direction));
    }

    void Update()
    {
        if (currentState == States.moving)
        {
            Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //int direction = -1;

        // 충돌한 오브젝트의 태그에 따라 방향 설정
        switch (collision.gameObject.tag)
        {
            case "RU":
                direction = Random.Range(0, 2);
                Debug.Log("collid RU - direction : " + direction);
                break;

            case "LU":
                //가게 인지도나 인테리어에 따라 확률 높아지도록 하면 좋을것 같음 
                direction = Random.Range(0, 3);
                Debug.Log("collid LU - direction : " + direction);
                break;

            case "RT":
                direction = 3;
                Debug.Log("collid RT - direction : " + direction);
                break;
            case "STOP":
                direction = 3;

                currentState = States.waiting;
                SetAnimation(4); // 기본 애니메이션 설정

                Debug.Log("collid STOP - direction : " + direction);
                break;
            case "RT2":
                direction = 2;
                Debug.Log("collid RT2 - direction : " + direction);
                break;
        }
        SetMoveDirection(direction);

        // 타겟 위치로 자연스럽게 이동
        //if (direction != -1)
        //{
        //    StartCoroutine(MoveToTarget(collision.transform.position, direction));
        //}
    }

    IEnumerator MoveToTarget(Vector3 targetPosition, int direction)
    {
        SetAnimation(direction);

        //isMovingToTarget = true;
        currentState = States.moving;

        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            // 타겟 위치로 부드럽게 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetMoveDirection(direction); //중간에 충돌하면 디렉션이 바

        if (direction == 4)
        {
            yield return new WaitForSeconds(2f);
            SetMoveDirection(3); // 오른쪽 아래로 이동 방향 설정
            SetAnimation(0); // 기본 애니메이션 설정
        }

        //isMovingToTarget = false;
    }

    void SetAnimation(int direction)
    {
        switch (direction)
        {
            case 0:
            case 3:
                animator.SetInteger("State", 1);
                break;
            case 1:
            case 2:
                animator.SetInteger("State", 2);
                break;
            case 4:
                animator.SetInteger("State", 0);
                break;
        }
    }

    void SetMoveDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                moveDirection = new Vector2(1, 0.5f).normalized; // 오른쪽 위
                break;
            case 1:
                moveDirection = new Vector2(-1, 0.5f).normalized; // 왼쪽 위
                break;
            case 2:
                moveDirection = new Vector2(-1, -0.5f).normalized; // 왼쪽 아래
                break;
            case 3:
                moveDirection = new Vector2(1, -0.5f).normalized; // 오른쪽 아래
                break;
            case 4:
                moveDirection = Vector2.zero; // 멈춤
                break;
        }
    }

    
}