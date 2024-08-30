using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Customers : MonoBehaviour
{
    private int SHOWCASELOCATE = 11;
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
    private CustomerController customerController;
    private Transform frontCustomer;
    public CustomersMoveType.LineType lineType;
    private CustomersMoveType.EnterType enterType;
    private CustomersMoveType.ShopType shopType;
    public CustomersMoveType.MoveType moveType;
    private Transform leftEnd, middleCorner, rightEnd, linePostion, enterOutPosition, enterInPosition, cashierPosition;
    private Transform[] showcasePosition;
    public Vector2 targetPosition;
    public Sprite[] sprites;
    public int currentSpriteIndex;
    public int moveState = 0;
    public int MAXLiNENUM = 10;

    public void Initialize(Transform leftEnd, Transform middleCorner, Transform rightEnd, Transform linePostion, Transform enterOutPosition, Transform enterInPosition, Transform cashierPosition, float moveSpeed, float lineSpacing, float sideSpacing, int wantedCakeIndex, CustomerController customerController)
    {
        this.leftEnd = leftEnd;
        this.middleCorner = middleCorner;
        this.rightEnd = rightEnd;
        this.linePostion = linePostion;
        this.enterOutPosition = enterOutPosition;
        this.enterInPosition = enterInPosition;
        this.cashierPosition = cashierPosition;
        this.moveSpeed = moveSpeed;
        this.lineSpacing = lineSpacing;
        this.sideSpacing = sideSpacing;
        this.wantedCakeIndex = wantedCakeIndex;
        this.customerController = customerController;
        isCakeCheck = false;
        isDestroy = false;
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
            customerController.customersList.Remove(this);
            Destroy(gameObject);
        }
        UpdateAnimation();
    }
    private float nextFrameTime = 0.1f;
    private bool isIdle;
    float posy;
    float delay = 0.5f;
    void UpdateAnimation()
    {
        if (Time.time >= nextFrameTime)
        {
            if (agent.speed != 0)
            {
                nextFrameTime = Time.time + (delay / agent.speed) / (transform.GetChild(1).localScale.x*2); // 다음 프레임 시간 설정
            }

            switch (moveState)
            {
                case 0: // Idle 상태
                    if (!isIdle)
                    {
                        posy = transform.GetChild(1).position.y;
                        isIdle = true;
                    }
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[0];
                    currentSpriteIndex = (currentSpriteIndex + 1) % 2;
                    transform.GetChild(1).position = new Vector3(transform.position.x, posy + 0.1f * currentSpriteIndex, 0);
                    break;
                case 1: // 움직이는 상태 (위로 이동)
                    transform.GetChild(1).position = new Vector3(transform.position.x, transform.position.y, 0);
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[1 + currentSpriteIndex];
                    currentSpriteIndex = (currentSpriteIndex + 1) % 4; // 애니메이션 프레임 순환
                    break;
                case 2: // 움직이는 상태 (아래로 이동)
                    transform.GetChild(1).position = new Vector3(transform.position.x, transform.position.y, 0);
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[5 + currentSpriteIndex];
                    currentSpriteIndex = (currentSpriteIndex + 1) % 4; // 애니메이션 프레임 순환
                    break;
            }
        }
    }
    void MoveTo()
    {
        agent.SetDestination(targetPosition);
        if (Vector2.Distance(transform.position, targetPosition) > 1f)
        {
            if (targetPosition.y > transform.position.y)
            {
                moveState = 1;
            }
            else
            {
                moveState = 2;
            }
            if (targetPosition.x < transform.position.x)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    void UpdateCustomer()
    {
        frontCustomer = customerController.GetFrontCustomer(this);
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
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    if (customerController.customersList.Count > MAXLiNENUM)
                    {
                        r = Random.Range(0, 3);
                        return;
                    }
                    if ((Vector3)targetPosition != middleCorner.position) targetPosition = middleCorner.position;
                    else
                    {
                        moveType = CustomersMoveType.MoveType.Line;
                        lineType = CustomersMoveType.LineType.Start;
                        UpdateCustomer();
                    }
                    break;
                case 1:
                    if ((Vector3)targetPosition != middleCorner.position) targetPosition = middleCorner.position;
                    else if ((Vector3)targetPosition == leftEnd.position) isDestroy = true;
                    else targetPosition = leftEnd.position;
                    break;
                case 2:
                    if ((Vector3)targetPosition != middleCorner.position) targetPosition = middleCorner.position;
                    else if ((Vector3)targetPosition == rightEnd.position) isDestroy = true;
                    else targetPosition = rightEnd.position;
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
                    customerController.customersList.Add(this);
                    lineType = CustomersMoveType.LineType.During;
                }
                break;
            case CustomersMoveType.LineType.During:
                if (frontCustomer != null)
                {
                    targetPosition = (Vector2)linePostion.position + customerController.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing) + ((2 * Vector2.right + Vector2.down) * sideSpacing);
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
                    targetPosition = (Vector2)linePostion.position + customerController.GetCustomerNum(this) * ((2 * Vector2.right + Vector2.up) * lineSpacing);
                    if (Vector2.Distance(transform.position, targetPosition) > 0.01f)
                    {
                        MoveTo();
                    }
                    else
                    {
                        moveState = 0;
                    }
                    if (frontCustomer.GetComponent<Customers>().moveType == CustomersMoveType.MoveType.Random || frontCustomer.GetComponent<Customers>().moveType == CustomersMoveType.MoveType.Shop)
                    {
                        StartCoroutine(StopForSeconds(1f));
                        agent.speed = moveSpeed / 2;
                        customerController.customersList.Remove(this);
                        lineType = CustomersMoveType.LineType.None;
                        moveType = CustomersMoveType.MoveType.Enter;
                        enterType = CustomersMoveType.EnterType.None;
                    }

                }
                else
                {
                    moveState = 0;
                    if (Routine.instance.routineState == RoutineState.Open)
                    {
                        agent.speed = moveSpeed / 2;
                        customerController.customersList.Remove(this);
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
                        showcasePosition[i] = cakeShowcaseController.cakeShowcasePool.GetChild(i).GetChild(SHOWCASELOCATE).transform;
                    }

                    randomIndex = Random.Range(0, showcasePosition.Length);
                    targetPosition = showcasePosition[randomIndex].position;
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
                    List<int> cakePlaces = CakeManager.instance.cakeShowcaseController.CakeFindPlace(randomShowcaseIndex, wantedCakeIndex);
                    int r = Random.Range(0, cakePlaces.Count);
                    CakeManager.instance.cakeShowcaseController.CakeSell(randomShowcaseIndex, cakePlaces[r]);
                    shopType = CustomersMoveType.ShopType.Pay;
                }
                break;
            case CustomersMoveType.ShopType.Pay:
                targetPosition = cashierPosition.position;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    StartCoroutine(StopForSeconds(0.1f));
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = customerController.heart;
                    StartCoroutine(StopForSeconds(1f));
                    transform.GetChild(0).gameObject.SetActive(false);
                    shopType = CustomersMoveType.ShopType.In;
                    GameManager.instance.getMoney(100);
                    //SoundManager.instance.GetMoneyClip();
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
