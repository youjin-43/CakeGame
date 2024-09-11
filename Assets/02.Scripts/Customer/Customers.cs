using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customers : MonoBehaviour
{
    private int SHOWCASELOCATE = 11;
    NavMeshAgent agent;
    private float moveSpeed;
    public int wantedCakeIndex;
    private int randomShowcaseIndex;
    private CustomerController customerController;
    private Transform cashierPos, spawnPos;
    private Transform[] showcasePoses;
    public Vector2 targetLoc;
    public Sprite[] sprites;
    public int currentSpriteIndex;
    public int moveState = 0;
    public int animeState = 0;
    public int MAXLiNENUM = 10;
    private Animator animator;

    public void Initialize(Transform spawnPos, Transform cashierPos, float moveSpeed, float scale, CustomerController customerController)
    {
        this.spawnPos = spawnPos;
        this.cashierPos = cashierPos;
        this.moveSpeed = moveSpeed;
        this.customerController = customerController;

        timer = 0;
        animeState = 1;
        moveState = 1;
        randTime = Random.Range(10, 15);
        CakeManager cakeManager = CakeManager.instance;
        CakeShowcaseController cakeShowcaseController = cakeManager.cakeShowcaseController;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
        wantedCakeIndex = Random.Range(0, CakeManager.instance.TOTALCAKENUM);

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<Transform>().localScale = new Vector3(scale, scale, 1);

        showcasePoses = new Transform[cakeShowcaseController.cakeShowcasePool.childCount];
        for (int i = 0; i < cakeShowcaseController.cakeShowcasePool.childCount; i++) showcasePoses[i] = cakeShowcaseController.cakeShowcasePool.GetChild(i).GetChild(SHOWCASELOCATE).transform;

        targetLoc = showcasePoses[Random.Range(0, showcasePoses.Length)].position;
        StartCoroutine(UpdateAnimation());
    }
    void Update()
    {
        MoveTo();
        if (moveState != 5 && GameManager.instance.MaxRunTime -
        GameManager.instance.runTime < 5)
        {
            if (moveState > 2) moveState = 5;
            else moveState = 5;
        }

        switch (moveState)
        {
            case 1:
                RandomMove();
                break;
            case 2:
                RandomMove();
                break;
            case 3:
                TakeIt();
                break;
            case 4:
                BuyIt();
                break;
            case 5:
                Out();
                break;
            default:
                Out();
                break;
        }
    }

    float delay = 0.5f;
    int currentAnimeState;
    IEnumerator UpdateAnimation()
    {
        while (true)
        {
            animator.SetInteger("animeState", animeState);
            currentSpriteIndex = (currentSpriteIndex + 1) % 4; // 애니메이션 프레임 순환
            switch (animeState)
            {
                case 0: // Idle 상태
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[0];
                    break;
                case 1: // 움직이는 상태 (위로 이동)
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[1 + currentSpriteIndex];
                    break;
                case 2: // 움직이는 상태 (아래로 이동)
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[5 + currentSpriteIndex];
                    break;
            }
            for (float time = 0; time < delay; time += Time.deltaTime)
            {
                if (animeState != currentAnimeState)
                {
                    currentSpriteIndex = 0;  // currentSpriteIndex 초기화
                    currentAnimeState = animeState;  // 현재 상태를 업데이트
                    break;
                }  // animState가 바뀌면 즉시 빠져나옴
                yield return null;  // 딜레이 중에도 매 프레임마다 상태를 체크
            }
            yield return null;
        }
    }


    void MoveTo()
    {
        if (agent.speed == 0)
        {
            animeState = 0;
            return;
        }
        
        agent.SetDestination(targetLoc);

        Vector3 direction = agent.velocity.normalized;
        
        if (direction.y > 0) animeState = 1;
        else if (direction.y < 0) animeState = 2;

        // 좌상 (x < 0, y > 0) 또는 우하 (x > 0, y < 0)일 때 flipX 설정
        if ((direction.x < 0 && direction.y > 0) || (direction.x > 0 && direction.y < 0))
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
        else
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
    }
    int randTime;
    float timer;
    void RandomMove()
    {
        timer += Time.deltaTime;
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f) StartCoroutine(StopForSeconds(1f));
        else return;
        targetLoc = showcasePoses[Random.Range(0, showcasePoses.Length)].position;
        timer = 0;
        if (moveState == 1)
        {
            if (timer > randTime)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                CakeCheck();
            }
        }
        if (moveState == 2)
        {
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = CakeManager.instance.cakeSODataList[wantedCakeIndex].itemImage;
            CakeCheck();
            if (timer > randTime)
            {
                moveState = 5;
            }
        }
    }
    void CakeCheck()
    {
        List<int> wantedCakeIndexes = CakeManager.instance.cakeShowcaseController.CakeFindIndex(wantedCakeIndex);
        if (wantedCakeIndexes != null && wantedCakeIndexes.Count > 0)
        {
            randomShowcaseIndex = wantedCakeIndexes[Random.Range(0, wantedCakeIndexes.Count)];
            List<int> cakePlaces = CakeManager.instance.cakeShowcaseController.CakeFindPlace(randomShowcaseIndex, wantedCakeIndex);
            CakeManager.instance.cakeShowcaseController.CakeSell(randomShowcaseIndex, cakePlaces[Random.Range(0, cakePlaces.Count)]);
            targetLoc = showcasePoses[randomShowcaseIndex].position;
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = customerController.heart;
            moveState = 3;
        }
        else
        {
            moveState = 2;
        }
    }
    void GoToShop()
    {
        // switch (shopType)
        // {
        //     case CustomersMoveType.ShopType.Check:
        //         List<int> wantedCakeIndexes = CakeManager.instance.cakeShowcaseController.CakeFindIndex(wantedCakeIndex);
        //         if (CakeCheck())
        //         {
        //             int r = Random.Range(0, wantedCakeIndexes.Count);
        //             randomShowcaseIndex = wantedCakeIndexes[r];
        //             targetPosition = showcasePoses[randomShowcaseIndex].position;
        //             shopType = CustomersMoveType.ShopType.Shop;
        //         }
        //         else
        //         {
        //             shopType = CustomersMoveType.ShopType.Out;
        //         }
        //         break;
        //     case CustomersMoveType.ShopType.Shop:
        //         Debug.Log("케이크 앞으로 가는중!");
        //         MoveTo();
        //         if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        //         {
        //             Debug.Log("케이크 앞에 왔다!");
        //             StartCoroutine(StopForSeconds(1f)); // 3초 멈춤
        //             List<int> cakePlaces = CakeManager.instance.cakeShowcaseController.CakeFindPlace(randomShowcaseIndex, wantedCakeIndex);
        //             int r = Random.Range(0, cakePlaces.Count);
        //             CakeManager.instance.cakeShowcaseController.CakeSell(randomShowcaseIndex, cakePlaces[r]);
        //             shopType = CustomersMoveType.ShopType.Pay;
        //         }
        //         break;
        //     case CustomersMoveType.ShopType.Pay:
        //         targetPosition = cashierPosition.position;
        //         MoveTo();
        //         if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        //         {
        //             StartCoroutine(StopForSeconds(0.1f));
        //             transform.GetChild(0).gameObject.SetActive(true);
        //             transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = customerController.heart;
        //             StartCoroutine(StopForSeconds(1f));
        //             transform.GetChild(0).gameObject.SetActive(false);
        //             shopType = CustomersMoveType.ShopType.Out;
        //             GameManager.instance.getMoney(CakeManager.instance.cakeSODataList[wantedCakeIndex].cakePrice);
        //             customerController.SellAudio();
        //             CakeManager.instance.soldCakeCount[wantedCakeIndex]++;
        //             CakeManager.instance.CallSellAudio();
        //             UIManager.instance.RaiseUpCakeCntForEndBoard(wantedCakeIndex);
        //             Debug.Log("케이크가 판매 되었습니다.");
        //         }
        //         break;
        //     case CustomersMoveType.ShopType.Out:
        //         targetPosition = spawnPoint.position;
        //         MoveTo();
        //         if (Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        //         {
        //             CakeManager.instance.CallPortalAudio();
        //             StopForSeconds(.7f);
        //             Destroy(gameObject);
        //         }
        //         break;
        // }
    }

    void TakeIt()
    {
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(StopForSeconds(1f));
            moveState = 4;
        }
    }
    void BuyIt()
    {
        targetLoc = cashierPos.position;
        MoveTo();
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            StartCoroutine(StopForSeconds(0.1f));
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = customerController.heart;
            StartCoroutine(StopForSeconds(1f));
            transform.GetChild(0).gameObject.SetActive(false);
            GameManager.instance.getMoney(CakeManager.instance.cakeSODataList[wantedCakeIndex].cakePrice);
            CakeManager.instance.CallSellAudio();
            CakeManager.instance.soldCakeCount[wantedCakeIndex]++;
            UIManager.instance.RaiseUpCakeCntForEndBoard(wantedCakeIndex);
        }
    }
    void Out()
    {
        targetLoc = spawnPos.position;
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            CakeManager.instance.CallPortalAudio();
            StopForSeconds(.7f);
            Destroy(gameObject);
        }
    }

    IEnumerator StopForSeconds(float seconds)
    {
        agent.speed = 0f; // 이동을 멈추기 위해 속도를 0으로 설정
        yield return new WaitForSeconds(seconds);
        agent.speed = moveSpeed; // 이동을 재개하기 위해 원래 속도로 설정 (원래 속도로 변경)
    }
}
