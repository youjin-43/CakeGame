using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float moveSpeed = 2f; // NPC의 이동 속도
    private Vector2 moveDirection; // NPC의 이동 방향
    private bool isMovingToTarget = false; // 타겟 위치로 이동 중인지 여부
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        // 초기 이동 방향 설정 (오른쪽 대각선 아래)
        moveDirection = new Vector2(1, -0.5f).normalized;
    }

    void Update()
    {
        if (!isMovingToTarget)
        {
            // NPC의 이동
            Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
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
                Debug.Log("collid RU - direction : " + direction);
                break;
            case "LU":
                direction = Random.Range(0, 3);
                Debug.Log("collid LU - direction : " + direction);
                break;
            case "RT":
                direction = 3;
                Debug.Log("collid RT - direction : " + direction);
                break;
            case "STOP":
                direction = 4;
                Debug.Log("collid STOP - direction : " + direction);
                break;
            case "RT2":
                direction = 2;
                Debug.Log("collid RT2 - direction : " + direction);
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
            yield return new WaitForSeconds(2f);
            SetMoveDirection(3); // 오른쪽 아래로 이동 방향 설정
            SetAnimation(0); // 기본 애니메이션 설정
        }

        isMovingToTarget = false;
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