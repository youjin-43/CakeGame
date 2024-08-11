using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customers : MonoBehaviour
{
    private List<Vector2> pathPoints;   // 이동 경로
    private Vector2 pathLine;
    private float moveSpeed;            // 이동 속도
    private float lineSpacing;          // 줄서기 간격
    private float sideSpacing;          // 옆 간격
    private int currentPointIndex = 0;
    private Vector2 targetPosition;
    private bool isMoving = true;
    private CustomersManager customersManager;
    private Transform frontCustomer, firstCustomer;
    private LineType lineType;

    enum LineType
    {
        None,
        Start,
        During,
        End
    }

    public void Initialize(List<Vector2> pathPoints, Vector2 pathLine, float moveSpeed, float lineSpacing, float sideSpacing, CustomersManager manager)
    {
        this.pathPoints = pathPoints;
        this.pathLine = pathLine;
        this.moveSpeed = moveSpeed;
        this.lineSpacing = lineSpacing;
        this.sideSpacing = sideSpacing;
        this.customersManager = manager;

        if (pathPoints.Count > 0)
        {
            targetPosition = pathPoints[currentPointIndex];
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
        else
        {
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
        if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            // 현재 위치에서 타겟 위치로 부드럽게 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // 다음 지점으로 타겟 변경
            currentPointIndex++;
            if (currentPointIndex < pathPoints.Count)
            {
                targetPosition = pathPoints[currentPointIndex];
            }
            else
            {

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
            // 옆으로 이동하는 위치를 계산
            Vector2 firstPosition = (Vector2)firstCustomer.position + ((2*Vector2.right+Vector2.down) * sideSpacing);
            if (Vector2.Distance(transform.position, firstPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, firstPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // 옆으로 이동한 후, 줄 서는 중으로 상태 변경
                lineType = LineType.During;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, pathLine) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, pathLine, moveSpeed * Time.deltaTime);
            }
            // 줄의 첫 번째 고객인 경우 바로 During으로 넘어갑니다.
            else lineType = LineType.During;
        }
    }

    void LineUpDuring()
    {
        if (frontCustomer != null)
        {
            // 줄 서는 위치를 계산
            Vector2 EndPosition = (Vector2)frontCustomer.position + ((2*Vector2.right+Vector2.up) * lineSpacing); // 앞 Customers 위치에서 간격을 둠
            Vector2 lastPosition = EndPosition + ((2*Vector2.right+Vector2.down) * sideSpacing);
            if (Vector2.Distance(transform.position, lastPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, lastPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // 줄 서기 완료 후 End 상태로 변경
                lineType = LineType.End;
            }
        }
        else
        {
            lineType = LineType.End; // 첫 번째 고객은 바로 줄 서기를 완료
        }
    }

    void LineUpEnd()
    {
        if (frontCustomer != null)
        {
            Vector2 EndPosition = (Vector2)frontCustomer.position + ((2*Vector2.right+Vector2.up) * lineSpacing); // 앞 Customers 위치에서 간격을 둠
            if (Vector2.Distance(transform.position, EndPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, EndPosition, moveSpeed * Time.deltaTime);
            }// 줄 서기 완료 후 추가적인 동작을 하지 않음
            else
            {
                lineType = LineType.None;
            }

        }
        else
        {
            // 줄의 첫 번째 고객인 경우 바로 During으로 넘어갑니다.
            lineType = LineType.None;
        }
    }
}