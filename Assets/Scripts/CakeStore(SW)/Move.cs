using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 2f; // NPC의 이동 속도
    private Vector2 moveDirection; // NPC의 이동 방향
    private bool isMovingToTarget = false; // 타겟 위치로 이동 중인지 여부
    private Animator animator;

    public Direction initialDirection = Direction.RightDown; // 외부에서 설정 가능한 초기 방향

    public enum Direction { RightUp, LeftUp, LeftDown, RightDown, Stop }

    void Start()
    {
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

    private void MoveNPC()
    {
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Direction? direction = null;
        bool shouldDestroy = false;

        // 충돌한 오브젝트의 태그에 따라 방향 설정
        switch (collision.gameObject.tag)
        {
            case "MainCorner":
                direction = (Direction)Random.Range(0, 2);
                break;
            case "Enterance":
                direction = (Direction)Random.Range(0, 3);
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
        }

        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
        else if (direction.HasValue)
        {
            StartCoroutine(MoveToTarget(collision.transform.position, direction.Value));
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition, Direction direction)
    {
        SetAnimation(direction);
        isMovingToTarget = true;

        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetMoveDirection(direction);

        if (direction == Direction.Stop)
        {
            yield return new WaitForSeconds(2f);
            SetMoveDirection(Direction.RightDown); // 오른쪽 아래로 이동 방향 설정
            SetAnimation(Direction.RightDown); // 기본 애니메이션 설정
        }

        isMovingToTarget = false;
    }

    public void SetAnimation(Direction direction)
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
}
