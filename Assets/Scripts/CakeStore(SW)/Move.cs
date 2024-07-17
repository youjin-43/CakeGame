using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Customer customer;
    private Animator animator;

    [SerializeField] private float defaultMoveSpeed = 2f; // 기본 이동 속도
    private float moveTime = 0.5f;
    [SerializeField] private float moveSpeed; // 실제 이동 속도
    [SerializeField] private Vector2 moveDirection; // NPC의 이동 방향
    [SerializeField] private bool isMovingToTarget = false; // 타겟 위치로 이동 중인지 여부
    private float minDistance = 0.1f; // 최소 이동 거리

    public Direction initialDirection = Direction.RightDown; // 외부에서 설정 가능한 초기 방향

    public enum Direction { RightUp, LeftUp, LeftDown, RightDown, Stop }

    void Start()
    {
        customer = GetComponent<Customer>();
        animator = GetComponent<Animator>();

        // 초기 이동 방향 설정 (외부에서 설정된 값 사용)
        SetMoveDirection(initialDirection);
        SetAnimation(initialDirection);
    }

    void Update()
    {
        if (!isMovingToTarget)
        {
            MoveNPC();
        }
    }
    public void EndWaitingMove()
    {
        Debug.Log("EndWaitingMove 실행");
        SetMoveDirection(Direction.RightDown);
        SetAnimation(Direction.RightDown);
        isMovingToTarget = false;
    }

    private void MoveNPC()
    {
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * defaultMoveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMovingToTarget) return; // 이미 이동 중인 경우 새로운 충돌 처리하지 않음

        Direction? direction = null;
        bool shouldDestroy = false;

        switch (collision.gameObject.tag)
        {
            case "MainCorner":
                direction = GetRandomDirection(new Direction[] { Direction.RightUp, Direction.LeftUp });
                break;
            case "Enterance":
                direction = GetRandomDirection(new Direction[] { Direction.RightUp, Direction.LeftUp, Direction.LeftDown });
                break;
            case "LeftExit":
                direction = Direction.RightDown;
                shouldDestroy = Random.Range(0f, 1f) < 0.5f; // 50% 확률로 사라짐
                break;
            case "RightExit":
                direction = Direction.LeftDown;
                shouldDestroy = Random.Range(0f, 1f) < 0.5f; // 50% 확률로 사라짐
                break;
            case "Counter":
                direction = Direction.Stop;
                break;
            case "Corner1":
                direction = GetRandomDirection(new Direction[] { Direction.RightUp, Direction.LeftUp, Direction.RightDown });
                break;
            case "Corner2":
                direction = GetRandomDirection(new Direction[] { Direction.RightUp, Direction.LeftUp, Direction.LeftDown });
                break;
            case "Corner3":
                direction = GetRandomDirection(new Direction[] { Direction.LeftDown, Direction.RightDown });
                break;
        }

        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
        else if (direction.HasValue)
        {
            Vector3 targetPosition = collision.transform.position; // 타겟 위치 저장
            StartCoroutine(MoveToTarget(targetPosition, direction.Value));
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition, Direction direction)
    {
        SetAnimation(direction);
        isMovingToTarget = true;

        Vector3 startPosition = transform.position;
        Vector3 directionToTarget = (targetPosition - startPosition).normalized;
        float targetDistance = Vector3.Distance(startPosition, targetPosition);

        // 이동 속도 계산: 1초 이내에 타겟에 도달하도록 설정
        moveSpeed = targetDistance / moveTime; // 1초 동안 이동할 수 있는 속도 설정

        float elapsedTime = 0f;
        while (elapsedTime < 1f && targetDistance > minDistance)
        {
            Vector3 movement = directionToTarget * moveSpeed * Time.deltaTime;
            transform.Translate(movement);

            elapsedTime += Time.deltaTime;
            targetDistance = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }

        // 이동 완료 후 설정
        SetMoveDirection(direction);
        if (direction == Direction.Stop)
        {
            yield return null;
            customer.ChangeStatetoWating();
            SetAnimation(Direction.Stop);

            //SetMoveDirection(Direction.RightDown); // 오른쪽 아래로 이동 방향 설정
            //SetAnimation(Direction.RightDown); // 기본 애니메이션 설정
        }
        else
        {
            isMovingToTarget = false;
        }
        moveSpeed = defaultMoveSpeed; // 이동 속도 초기화
    }

    private void SetAnimation(Direction direction)
    {
        int state = direction switch
        {
            Direction.RightUp or Direction.RightDown => 1,
            Direction.LeftUp or Direction.LeftDown => 2,
            Direction.Stop => 0,
            _ => 0
        };
        animator.SetInteger("State", state);
    }

    public void SetMoveDirection(Direction direction)
    {
        moveDirection = direction switch
        {
            Direction.RightUp => new Vector2(1, 0.5f).normalized,
            Direction.LeftUp => new Vector2(-1, 0.5f).normalized,
            Direction.LeftDown => new Vector2(-1, -0.5f).normalized,
            Direction.RightDown => new Vector2(1, -0.5f).normalized,
            Direction.Stop => Vector2.zero,
            _ => moveDirection
        };
    }

    private Direction GetRandomDirection(Direction[] allowedDirections)
    {
        return allowedDirections[Random.Range(0, allowedDirections.Length)];
    }
}
