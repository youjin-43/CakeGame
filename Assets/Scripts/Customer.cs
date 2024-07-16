using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum States
    {
        moving, // 거리활보중 
        waiting,// 케이크 받길 기다리는 중 
        goback // 받고 돌아가는 중 
    }

    [SerializeField] private States currentState;

    [SerializeField] private float moveSpeed = 2f; // NPC의 이동 속도
    [SerializeField] private Vector2 moveDirection; // NPC의 이동 방향
    [SerializeField] private bool isMovingToTarget = false; // 타겟 위치로 이동 중인지 여부
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        // 초기 이동 방향 설정 (오른쪽 대각선 아래)
        moveDirection = new Vector2(1, -0.5f).normalized;

        currentState = States.moving;
    }

    void Update()
    {
        if (!isMovingToTarget)
        {
            // NPC의 이동
            Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }

        if(currentState == States.waiting)
        {
            transform.GetChild(0).gameObject.SetActive(true); //케이크 달라는말 활성화

            ////마우스 클릭시
            //if (Input.GetMouseButtonDown(0))
            //{
            //    //마우스 클릭한 좌표값 가져오기
            //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    //해당 좌표에 있는 오브젝트 찾기
            //    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            //    if (hit.collider.gameObject.name == "want")
            //    {
            //        transform.GetChild(0).gameObject.SetActive(false);

            //    }
            //}

            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.instance.getMoney();
                transform.GetChild(0).gameObject.SetActive(false); //케이크 달라는말 비활성화
                currentState = States.moving;

                SetMoveDirection(3);
                SetAnimation(0);
                isMovingToTarget = false;
            }

            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        int direction = -1;

        // 충돌한 오브젝트의 태그에 따라 방향 설정
        switch (collision.gameObject.tag)
        {
            case "RU":
                direction = Random.Range(0, 2);
                break;
            case "LU":
                direction = Random.Range(0, 3);
                break;
            case "RT":
                direction = 3;
                break;
            case "STOP":
                direction = 4;
                break;
            case "RT2":
                direction = 2;
                break;
        }

        // 타겟 위치로 자연스럽게 이동
        if (direction != -1)
        {
            StartCoroutine(MoveToTarget(collision.transform.position, direction));
        }
    }

    IEnumerator MoveToTarget(Vector3 targetPosition, int direction)
    {
        SetAnimation(direction);

        isMovingToTarget = true;
        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            // 타겟 위치로 부드럽게 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetMoveDirection(direction);

        if (direction == 4)
        {
            //yield return new WaitForSeconds(2f);
            yield return null;
            currentState = States.waiting;
            SetAnimation(4);

            //SetMoveDirection(3); // 오른쪽 아래로 이동 방향 설정
            //SetAnimation(0); // 기본 애니메이션 설정
        }
        else
        {
            isMovingToTarget = false;
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