using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customers : MonoBehaviour
{
    private List<Vector2> pathPoints;   // 이동 경로 포인트들
    private Vector2 pathLine;           // 줄 서는 최종 위치
    private float moveSpeed;            // 이동 속도
    private float lineSpacing;          // 줄 서기 간격
    private float sideSpacing;          // 옆 간격
    private int currentPointIndex = 0;  // 현재 경로 인덱스
    private Vector2 targetPosition;     // 현재 목표 위치
    private bool isMoving = true;       // 이동 중인지 여부
    private CustomersManager customersManager;  // 고객 매니저 참조
    private Transform frontCustomer, firstCustomer;  // 앞 고객, 첫 고객 참조
    private LineType lineType;          // 현재 줄 서기 상태

    enum LineType
    {
        None,    // 줄 서지 않음
        Start,   // 줄 서기 시작
        During,  // 줄 서는 중
        End      // 줄 서기 완료
    }

    public void Initialize(List<Vector2> pathPoints, Vector2 pathLine, float moveSpeed, float lineSpacing, float sideSpacing, CustomersManager manager)
    {
        this.pathPoints = pathPoints;
        this.pathLine = pathLine;
        this.moveSpeed = moveSpeed;
        this.lineSpacing = lineSpacing;
        this.sideSpacing = sideSpacing;
        this.customersManager = manager;

        // 초기 목표 위치 설정
        if (pathPoints.Count > 0)
        {
            targetPosition = pathPoints[currentPointIndex];
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();  // 경로를 따라 이동
        }
        else
        {
            // 현재 줄 서기 상태에 따라 동작
            switch (lineType)
            {
                case LineType.Start:
                    LineUpStart();
                    break;
                case LineType.During:
                    LineUpDuring();
                    break;
                case LineType.End:
                    LineUpEnd();
                    break;
            }
        }
    }

    void MoveAlongPath()
    {
        // 목표 위치에 도달하지 않은 경우 이동
        if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // 다음 경로 포인트로 전환
            currentPointIndex++;
            if (currentPointIndex < pathPoints.Count)
            {
                targetPosition = pathPoints[currentPointIndex];
            }
            else
            {
                // 마지막 경로에 도달한 경우 줄 서기 시작
                isMoving = false;
                lineType = LineType.Start;
                frontCustomer = customersManager.GetFrontCustomer(this);
                firstCustomer = customersManager.GetFirstCustomer(this);
            }
        }
    }

    void LineUpStart()
    {
        if (frontCustomer != null)
        {
            // 첫 고객의 옆으로 이동 위치 계산 (아이소매트릭 환경을 고려)
            Vector2 firstPosition = (Vector2)firstCustomer.position + new Vector2(2 * sideSpacing, -sideSpacing);
            if (Vector2.Distance(transform.position, firstPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, firstPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                lineType = LineType.During;  // 옆으로 이동 완료 후 줄 서는 중으로 전환
            }
        }
        else
        {
            // 첫 고객의 경우 최종 줄 서는 위치로 이동
            if (Vector2.Distance(transform.position, pathLine) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, pathLine, moveSpeed * Time.deltaTime);
            }
            else
            {
                lineType = LineType.During;  // 줄 서는 중 상태로 전환
            }
        }
    }

    void LineUpDuring()
    {
        if (frontCustomer != null)
        {
            // 앞 고객의 뒤로 이동 위치 계산
            Vector2 lastPosition = (Vector2)frontCustomer.position + new Vector2(2 * sideSpacing, -sideSpacing) + new Vector2(2 * lineSpacing, sideSpacing);
            if (Vector2.Distance(transform.position, lastPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, lastPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                lineType = LineType.End;  // 줄 서기 완료 상태로 전환
            }
        }
        else
        {
            lineType = LineType.End;  // 첫 고객은 바로 줄 서기 완료
        }
    }

    void LineUpEnd()
    {
        if (frontCustomer != null)
        {
            // 줄 서기 완료 후 대기 위치 유지
            Vector2 endPosition = (Vector2)frontCustomer.position + new Vector2(2 * sideSpacing, -sideSpacing) + new Vector2(2 * lineSpacing, sideSpacing);
            if (Vector2.Distance(transform.position, endPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                lineType = LineType.None;  // 줄 서기 동작 종료
            }
        }
        else
        {
            lineType = LineType.None;  // 첫 고객의 경우 바로 종료
        }
    }
    void RandomMove()
    {
        
    }
}
