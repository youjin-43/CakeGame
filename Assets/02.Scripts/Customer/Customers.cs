using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Customers : MonoBehaviour
{
    NavMeshAgent agent;
    private float moveSpeed;
    private float lineSpacing;
    private float sideSpacing;
    public int wantedCakeIndex;
    private int randomIndex = 0;
    private int randomShowcaseIndex;
    private float randomTime = 0;
    private float timer;
    private bool isCakeCheck;
    private bool isDestroy;
    private bool isClose;
    private CustomersManager customersManager;
    private Transform frontCustomer;
    private CustomersMoveType.LineType lineType;
    private CustomersMoveType.EnterType enterType;
    private CustomersMoveType.ShopType shopType;
    public CustomersMoveType.MoveType moveType;
    private Transform leftEnd, rightEnd, linePostion, enterOutPosition, enterInPosition, cashierPosition;
    private Transform[] showcasePosition;
    public Vector2 targetPosition;

    public void Initialize(Transform leftEnd, Transform rightEnd, Transform linePostion, Transform enterOutPosition, Transform enterInPosition, Transform cashierPosition, float moveSpeed, float lineSpacing, float sideSpacing, int wantedCakeIndex, CustomersManager customersManager)
    {
        this.leftEnd = leftEnd;
        this.rightEnd = rightEnd;
        this.linePostion = linePostion;
        this.enterOutPosition = enterOutPosition;
        this.enterInPosition = enterInPosition;
        this.cashierPosition = cashierPosition;
        this.moveSpeed = moveSpeed;
        this.lineSpacing = lineSpacing;
        this.sideSpacing = sideSpacing;
        this.wantedCakeIndex = wantedCakeIndex;
        this.customersManager = customersManager;
        isCakeCheck = false;
        isDestroy = false;
        isClose = false;
        transform.GetChild(0).gameObject.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        moveType = CustomersMoveType.MoveType.Move;
    }

    void Update()
    {
        switch (moveType)
        {
            case CustomersMoveType.MoveType.Move:
                BeforeLineUp();
                break;
            case CustomersMoveType.MoveType.Line:
                GoToLineUp();
                UpdateCustomer();
                break;
            case CustomersMoveType.MoveType.Enter:
                GoToEnter();
                break;
            case CustomersMoveType.MoveType.Random:
                GoToRandom();
                break;
            case CustomersMoveType.MoveType.Shop:
                GoToShop();
                break;
        }
        if (Routine.instance.routineState == RoutineState.Close)
        {
            isClose = true;
            customersManager.customersList.Remove(this);
            Destroy(gameObject);
        }
    }

    void MoveTo()
    {
        agent.SetDestination(targetPosition);
    }

    void UpdateCustomer()
    {
        frontCustomer = customersManager.GetFrontCustomer(this);
    }

    void BeforeLineUp()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            MoveTo();
        }
        else
        {
            if (isDestroy)
            {
                Destroy(gameObject);
            }
            int r = Random.Range(0, 5);
            switch (r)
            {
                case 0:
                    moveType = CustomersMoveType.MoveType.Line;
                    lineType = CustomersMoveType.LineType.Start;
                    UpdateCustomer();
                    break;
                case 1:
                    targetPosition = leftEnd.position;
                    break;
                case 2:
                    targetPosition = rightEnd.position;
                    break;
                case 3:
                    targetPosition = leftEnd.position;
                    isDestroy = true;
                    break;
                case 4:
                    targetPosition = rightEnd.position;
                    isDestroy = true;
                    break;
                default:
                    break;
            }
        }
    }

    void GoToLineUp()
    {
        switch (lineType)
        {
            case CustomersMoveType.LineType.Start:
                targetPosition = (Vector2)linePostion.position + ((2 * Vector2.right + Vector2.down) * sideSpacing);
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    customersManager.customersList.Add(this);
                    lineType = CustomersMoveType.LineType.During;
                }
                break;
            case CustomersMoveType.LineType.During:
                if (frontCustomer != null)
                {
                    targetPosition = (Vector2)linePostion.position + customersManager.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing) + ((2 * Vector2.right + Vector2.down) * sideSpacing);
                }
                else
                {
                    targetPosition = linePostion.position;
                }
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) lineType = CustomersMoveType.LineType.End;
                break;
            case CustomersMoveType.LineType.End:
                if (frontCustomer != null)
                {
                    targetPosition = (Vector2)linePostion.position + customersManager.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing);
                    if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
                    {
                        MoveTo();
                    }
                    else if (frontCustomer.GetComponent<Customers>().moveType == CustomersMoveType.MoveType.Random || frontCustomer.GetComponent<Customers>().moveType == CustomersMoveType.MoveType.Shop)
                    {
                        agent.speed = moveSpeed / 2;
                        customersManager.customersList.Remove(this);
                        lineType = CustomersMoveType.LineType.None;
                        moveType = CustomersMoveType.MoveType.Enter;
                        enterType = CustomersMoveType.EnterType.None;
                    }

                }
                else
                {
                    if (Routine.instance.routineState == RoutineState.Open)
                    {
                        agent.speed = moveSpeed / 2;
                        customersManager.customersList.Remove(this);
                        lineType = CustomersMoveType.LineType.None;
                        moveType = CustomersMoveType.MoveType.Enter;
                        enterType = CustomersMoveType.EnterType.None;
                    }
                }
                break;
        }
    }

    void GoToEnter()
    {
        switch (enterType)
        {
            case CustomersMoveType.EnterType.None:
                enterType = CustomersMoveType.EnterType.Out;
                break;
            case CustomersMoveType.EnterType.Out:
                targetPosition = enterOutPosition.position;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) enterType = CustomersMoveType.EnterType.In;
                break;
            case CustomersMoveType.EnterType.In:
                targetPosition = enterInPosition.position;
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
        if (isCakeCheck)
        {
            if (timer > randomTime || CakeCheck())
            {
                transform.GetChild(0).gameObject.SetActive(false);
                moveType = CustomersMoveType.MoveType.Shop;
                shopType = CustomersMoveType.ShopType.Check;
                UpdateCustomer();
                isCakeCheck = false;
            }
        }
        if (timer > randomTime && !isCakeCheck)
        {
            if (CakeCheck())
            {
                moveType = CustomersMoveType.MoveType.Shop;
                shopType = CustomersMoveType.ShopType.Check;
                UpdateCustomer();
                isCakeCheck = false;
            }
            else
            {
                timer = 0;
                randomTime *= 2;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = CakeManager.instance.cakeSODataList[wantedCakeIndex].itemImage;
            }
        }
    }
    bool CakeCheck()
    {
        isCakeCheck = true;
        List<int> wantedCakeIndexes = CakeManager.instance.cakeShowcaseController.CakeFindIndex(wantedCakeIndex);
        if (wantedCakeIndexes != null && wantedCakeIndexes.Count > 0) return true;
        else return false;
    }
    void GoToShop()
    {
        switch (shopType)
        {
            case CustomersMoveType.ShopType.Check:
                List<int> wantedCakeIndexes = CakeManager.instance.cakeShowcaseController.CakeFindIndex(wantedCakeIndex);
                if (CakeCheck())
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
                targetPosition = cashierPosition.position;
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
                targetPosition = enterInPosition.position;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f) shopType = CustomersMoveType.ShopType.Out;
                break;
            case CustomersMoveType.ShopType.Out:
                targetPosition = enterOutPosition.position;
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
        agent.speed = 0f; // 이동을 멈추기 위해 속도를 0으로 설정
        yield return new WaitForSeconds(seconds);
        agent.speed = moveSpeed / 2; // 이동을 재개하기 위해 원래 속도로 설정 (원래 속도로 변경)
    }
}
