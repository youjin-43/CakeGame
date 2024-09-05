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
    public int wantedCakeIndex;
    private int randomIndex = 0;
    private int randomShowcaseIndex;
    private float randomTime = 0;
    private float timer;
    private bool isCakeCheck;
    private CustomerController customerController;
    private CustomersMoveType.ShopType shopType;
    public CustomersMoveType.MoveType moveType;
    private Transform cashierPosition, spawnPoint;
    private Transform[] showcasePosition;
    public Vector2 targetPosition;
    public Sprite[] sprites;
    public int currentSpriteIndex;
    public int moveState = 0;
    public int MAXLiNENUM = 10;

    public void Initialize(Transform spawnPoint, Transform cashierPosition, float moveSpeed, int wantedCakeIndex, CustomerController customerController)
    {
        this.spawnPoint = spawnPoint;
        this.cashierPosition = cashierPosition;
        this.moveSpeed = moveSpeed;
        this.wantedCakeIndex = wantedCakeIndex;
        this.customerController = customerController;
        isCakeCheck = false;
        transform.GetChild(0).gameObject.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        moveType = CustomersMoveType.MoveType.Random;
        CakeShowcaseController cakeShowcaseController = CakeManager.instance.cakeShowcaseController;
        showcasePosition = new Transform[cakeShowcaseController.cakeShowcasePool.childCount];
        timer = 0;
        randomTime = Random.Range(4, 7);
        for (int i = 0; i < cakeShowcaseController.cakeShowcasePool.childCount; i++) showcasePosition[i] = cakeShowcaseController.cakeShowcasePool.GetChild(i).GetChild(SHOWCASELOCATE).transform;

        randomIndex = Random.Range(0, showcasePosition.Length);
        targetPosition = showcasePosition[randomIndex].position;
        moveState = 2;
    }
    void Update()
    {
        switch (moveType)
        {
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
        if (GameManager.instance.MaxRunTime - GameManager.instance.runTime < 5)
        {
            if (moveType == CustomersMoveType.MoveType.Shop)
            {
                agent.speed = moveSpeed * 2;
            }
            else
            {
                moveType = CustomersMoveType.MoveType.Shop;
                shopType = CustomersMoveType.ShopType.Out;
            }
        }
    }
    private float nextFrameTime = 0.1f;
    private bool isIdle;
    float posy;
    float delay = 0.5f;
    void UpdateAnimation()
    {
        if (Time.time >= nextFrameTime)
        {
            nextFrameTime = Time.time + delay; // 다음 프레임 시간 설정

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

        if (agent.speed == 0)
        {
            moveState = 1;
        }
        else
        {
            agent.SetDestination(targetPosition);

            Vector3 direction = agent.velocity.normalized;
            if (direction.y > 0.1f)
            {
                moveState = 1;
            }
            else if (direction.y < -0.1f)
            {
                moveState = 2;
            }

            if ((targetPosition.x < transform.position.x && moveState == 1) || (targetPosition.x > transform.position.x && moveState == 2))
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
    void GoToRandom()
    {

        timer += Time.deltaTime;
        if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        {
            StartCoroutine(StopForSeconds(.5f)); // 3초 멈춤
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
                isCakeCheck = false;
            }
        }
        if (timer > randomTime && !isCakeCheck)
        {
            if (CakeCheck())
            {
                moveType = CustomersMoveType.MoveType.Shop;
                shopType = CustomersMoveType.ShopType.Check;
                isCakeCheck = false;
            }
            else
            {
                isCakeCheck = true;
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
                    shopType = CustomersMoveType.ShopType.Out;
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
                    shopType = CustomersMoveType.ShopType.Out;
                    GameManager.instance.getMoney(CakeManager.instance.cakeSODataList[wantedCakeIndex].cakePrice);
                    customerController.SellAudio();
                    CakeManager.instance.soldCakeCount[wantedCakeIndex]++;
                    CakeManager.instance.CallSellAudio();
                    UIManager.instance.RaiseUpCakeCntForEndBoard(wantedCakeIndex);
                    Debug.Log("케이크가 판매 되었습니다.");
                }
                break;
            case CustomersMoveType.ShopType.Out:
                targetPosition = spawnPoint.position;
                MoveTo();
                if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
                {
                    CakeManager.instance.CallPortalAudio();
                    StopForSeconds(.7f);
                    Destroy(gameObject);
                }
                break;
        }
    }

    IEnumerator StopForSeconds(float seconds)
    {
        agent.speed = 0f; // 이동을 멈추기 위해 속도를 0으로 설정
        yield return new WaitForSeconds(seconds);
        agent.speed = moveSpeed; // 이동을 재개하기 위해 원래 속도로 설정 (원래 속도로 변경)
    }
}
