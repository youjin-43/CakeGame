using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customers : MonoBehaviour
{
    // 기존 코드 유지
    private List<Vector2> pathPoints;
    private float originMoveSpeed;
    private float moveSpeed;
    private float lineSpacing;
    private float sideSpacing;
    public int wantedCakeIndex;
    private int currentPointIndex = 0;
    private int randomIndex = 0;
    private int randomShowcaseIndex;
    private float randomTime = 0;
    private float timer;
    private CustomersManager customersManager;
    private Transform frontCustomer;
    private CustomersMoveType.LineType lineType;
    private CustomersMoveType.EnterType enterType;
    private CustomersMoveType.ShopType shopType;
    public CustomersMoveType.MoveType moveType;
    private Vector2 linePostion, enterOutPosition, enterInPosition, cashierPosition;
    private Transform[] showcasePosition;
    private Vector2 targetPosition;

    public void Initialize(List<Vector2> pathPoints, Vector2 linePostion, Vector2 enterOutPosition, Vector2 enterInPosition, Vector2 cashierPosition, float moveSpeed, float lineSpacing, float sideSpacing, int wantedCakeIndex, CustomersManager customersManager)
    {
        this.pathPoints = pathPoints;
        this.linePostion = linePostion;
        this.enterOutPosition = enterOutPosition;
        this.enterInPosition = enterInPosition;
        this.cashierPosition = cashierPosition;
        originMoveSpeed = moveSpeed;
        this.moveSpeed = moveSpeed;
        this.lineSpacing = lineSpacing;
        this.sideSpacing = sideSpacing;
        this.wantedCakeIndex = wantedCakeIndex;
        this.customersManager = customersManager;

        if (pathPoints.Count > 0)
        {
            targetPosition = pathPoints[currentPointIndex];
        }
    }

    void Update()
    {
        switch (moveType)
        {
            case CustomersMoveType.MoveType.Move:
                MoveAlongPath();
                break;
            case CustomersMoveType.MoveType.Line:
                GoToLineUp();
                break;
            case CustomersMoveType.MoveType.Enter:
                GoToEnter();
                break;
            case CustomersMoveType.MoveType.Random:
                GoToRandom();
                break;
            case CustomersMoveType.MoveType.Shop:
                customersManager.customersList.Remove(this);
                GoToShop();
                break;
        }
    }

    void MoveTo()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void UpdateCustomer()
    {
        frontCustomer = customersManager.GetFrontCustomer(this);
    }

    void MoveAlongPath()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            MoveTo();
        }
        else
        {
            currentPointIndex++;
            if (currentPointIndex < pathPoints.Count)
            {
                targetPosition = pathPoints[currentPointIndex];
            }
            else
            {
                moveType = CustomersMoveType.MoveType.Line;
                lineType = CustomersMoveType.LineType.Start;
                UpdateCustomer();
            }
        }
    }

    void GoToLineUp()
    {
        switch (lineType)
        {
            case CustomersMoveType.LineType.Start:
                if (frontCustomer != null)
                {
                    targetPosition = linePostion + ((2 * Vector2.right + Vector2.down) * sideSpacing);
                }
                else
                {
                    targetPosition = linePostion;
                }
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) lineType = CustomersMoveType.LineType.During;
                break;
            case CustomersMoveType.LineType.During:
                if (frontCustomer != null)
                {
                    targetPosition = linePostion + customersManager.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing) + ((2 * Vector2.right + Vector2.down) * sideSpacing);
                    MoveTo();
                    if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) lineType = CustomersMoveType.LineType.End;
                }
                else
                {
                    lineType = CustomersMoveType.LineType.End;
                }
                break;
            case CustomersMoveType.LineType.End:
                if (frontCustomer != null)
                {
                    targetPosition = linePostion + customersManager.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing);

                    if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
                    {
                        MoveTo();
                    }
                    else if (frontCustomer.GetComponent<Customers>().moveType == CustomersMoveType.MoveType.Shop)
                    {
                        lineType = CustomersMoveType.LineType.None;
                        moveType = CustomersMoveType.MoveType.Enter;
                        enterType = CustomersMoveType.EnterType.Out;
                        UpdateCustomer();
                    }

                }
                else
                {
                    moveSpeed = originMoveSpeed / 2;
                    lineType = CustomersMoveType.LineType.None;
                    moveType = CustomersMoveType.MoveType.Enter;
                    enterType = CustomersMoveType.EnterType.Out;
                    UpdateCustomer();
                }
                break;
        }
    }

    void GoToEnter()
    {
        switch (enterType)
        {
            case CustomersMoveType.EnterType.Out:
                targetPosition = enterOutPosition;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) enterType = CustomersMoveType.EnterType.In;
                break;
            case CustomersMoveType.EnterType.In:
                targetPosition = enterInPosition;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    CakeShowcaseController cakeShowcaseController = CakeManager.instance.cakeShowcaseController;
                    showcasePosition = new Transform[cakeShowcaseController.cakeShowcasePool.childCount];
                    timer = 0;
                    randomTime = Random.Range(4, 7);
                    for (int i = 0; i < cakeShowcaseController.cakeShowcasePool.childCount; i++)
                    {
                        showcasePosition[i] = cakeShowcaseController.cakeShowcasePool.GetChild(i).GetChild(7).transform;
                    }
                    enterType = CustomersMoveType.EnterType.None;
                    moveType = CustomersMoveType.MoveType.Random;
                    UpdateCustomer();
                }
                break;
        }
    }

    void GoToRandom()
    {
        timer += Time.deltaTime;
        if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        {
            StartCoroutine(StopForSeconds(1f)); // 3초 멈춤
            randomIndex = Random.Range(0, showcasePosition.Length);
            targetPosition = showcasePosition[randomIndex].position;
        }
        else
        {
            MoveTo();
        }
        if (timer > randomTime)
        {
            moveType = CustomersMoveType.MoveType.Shop;
            shopType = CustomersMoveType.ShopType.Check;
            UpdateCustomer();
        }
    }

    void GoToShop()
    {
        switch (shopType)
        {
            case CustomersMoveType.ShopType.Check:
                List<int> wantedCakeIndexes = CakeManager.instance.cakeShowcaseController.CakeFindIndex(wantedCakeIndex);
                if (wantedCakeIndexes != null && wantedCakeIndexes.Count > 0)
                {
                    int r = Random.Range(0, wantedCakeIndexes.Count);
                    randomShowcaseIndex = wantedCakeIndexes[r];
                    targetPosition = showcasePosition[randomShowcaseIndex].position;
                    shopType = CustomersMoveType.ShopType.Shop;
                }
                else
                {
                    shopType = CustomersMoveType.ShopType.In;
                }
                break;
            case CustomersMoveType.ShopType.Shop:
                Debug.Log("케이크 앞으로 가는중!");
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    Debug.Log("케이크 앞에 왔다!");
                    StartCoroutine(StopForSeconds(1f)); // 3초 멈춤
                    int r = Random.Range(0, CakeManager.instance.cakePlaceNum);
                    CakeManager.instance.cakeShowcaseController.CakeSell(randomShowcaseIndex, r);
                    shopType = CustomersMoveType.ShopType.Pay;
                }
                break;
            case CustomersMoveType.ShopType.Pay:
                targetPosition = cashierPosition;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    StartCoroutine(StopForSeconds(1f)); // 3초 멈춤
                    shopType = CustomersMoveType.ShopType.In;
                    GameManager.instance.getMoney(100);
                    Debug.Log("케이크가 판매 되었습니다.");
                }
                break;
            case CustomersMoveType.ShopType.In:
                targetPosition = enterInPosition;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) shopType = CustomersMoveType.ShopType.Out;
                break;
            case CustomersMoveType.ShopType.Out:
                targetPosition = enterOutPosition;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    IEnumerator StopForSeconds(float seconds)
    {
        moveSpeed = 0f; // 이동을 멈추기 위해 속도를 0으로 설정
        yield return new WaitForSeconds(seconds);
        moveSpeed = originMoveSpeed; // 이동을 재개하기 위해 원래 속도로 설정 (원래 속도로 변경)
    }
}
