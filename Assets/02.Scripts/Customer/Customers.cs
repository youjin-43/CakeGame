using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customers : MonoBehaviour
{
    // 객체 내부의 컴포넌트
    private Animator animator;
    private NavMeshAgent agent;
    private CakeManager cakeManager;
    private CakeShowcaseController cakeShowcaseController;



    // 컨트롤러에서 받아오는 변수
    private float speed;
    private Transform spawnPos, cashierPos;
    private int currentImgIndex;
    private int wantedCakeIndex;
    private Sprite heartImg;
    private Vector2 targetLoc;
    private Transform[] showcasePoses;

    /// <summary>
    /// 1,2: RandomMove, 3: TakeIt, 4: BuyIt, 5: Out
    /// </summary>
    private int moveState;

    /// <summary>
    /// 0: Idle, 1: Up, 2: Down
    /// </summary>   
    private int animeState;

    /// <summary>
    /// 0: Idle, 1: Up, 2: Down
    /// </summary>
    private Sprite[][] customerImgs;



    /// <summary>
    /// Customer의 초기값을 상속하고 초기화한다.
    /// </summary>
    /// <param name="speed">고객 이동 속도</param>
    /// <param name="scale">고객 크기 비율</param>
    /// <param name="heartImg">하트 이미지</param>
    /// <param name="spawnPos">고객 스폰 위치</param>
    /// <param name="cashierPos">계산대 위치</param>
    /// <param name="idleImg">대기 상태 이미지</param>
    /// <param name="upImgs">위쪽 방향 이미지</param>
    /// <param name="downImgs">아래쪽 방향 이미지</param>
    public void Initialize(
        float speed,
        float scale,
        Sprite heartImg,
        Transform spawnPos,
        Transform cashierPos,
        Sprite idleImg,
        Sprite[] upImgs,
        Sprite[] downImgs
        )
    {
        // 값 상속
        this.speed = speed;
        this.heartImg = heartImg;
        this.spawnPos = spawnPos;
        this.cashierPos = cashierPos;
        customerImgs = new Sprite[4][];
        customerImgs[0] = new Sprite[1]; // Idle
        customerImgs[1] = new Sprite[upImgs.Length]; // Up 배열
        customerImgs[2] = new Sprite[downImgs.Length]; // Down 배열
        customerImgs[0][0] = idleImg;
        for (int i = 0; i < upImgs.Length; i++) customerImgs[1][i] = upImgs[i];
        for (int i = 0; i < downImgs.Length; i++) customerImgs[2][i] = downImgs[i];
        transform.GetChild(1).GetComponent<Transform>().localScale = new Vector3(scale, scale, 1);


        // 컴포넌트 할당
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cakeManager = CakeManager.instance;
        cakeShowcaseController = cakeManager.cakeShowcaseController;


        // 기본 값으로 초기화
        timer = 0;
        moveState = 1;
        animeState = 1;
        currentImgIndex = 0;
        agent.speed = speed;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        transform.GetChild(0).gameObject.SetActive(false);


        // 무작위 값 할당
        randTime = Random.Range(10, 15);
        wantedCakeIndex = Random.Range(0, cakeManager.TOTALCAKENUM);


        // 쇼케이스 앞 위치 가져오기
        targetLoc = showcasePoses[Random.Range(0, showcasePoses.Length)].position;
        StartCoroutine(UpdateAnimation());
    }



    void Update()
    {
        // 영업 종료 5초전 moveState를 Out상태로 변경
        if (moveState != 5 && GameManager.instance.MaxRunTime - GameManager.instance.runTime < 5)
        {
            if (moveState > 2) moveState = 5;
            else moveState = 5;
        }


        // moveState에 따라 함수를 호출
        switch (moveState)
        {
            case 1:
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
            default:
                Out();
                break;
        }


        MoveTo();
    }



    /// <summary>
    /// 오브젝트가 목표 지점으로 이동한다.
    /// </summary>
    void MoveTo()
    {
        // 속도가 0일 때 idle상태로 변경
        if (agent.speed == 0)
        {
            animeState = 0;
            return;
        }


        // 목표 지점으로 이동
        agent.SetDestination(targetLoc);


        // 방향에 따라 오브젝트 모습 변경
        Vector3 direction = agent.velocity.normalized; // 방향 추출
        if (direction.y > 0) animeState = 1; // 위쪽을 바라 볼 때
        else if (direction.y < 0) animeState = 2; // 아래쪽을 바라 볼 때


        // 좌상 (x < 0, y > 0) 또는 우하 (x > 0, y < 0)일 때 flipX = true로 설정
        if ((direction.x < 0 && direction.y > 0) || (direction.x > 0 && direction.y < 0))
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
        else
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
    }



    int randTime;
    float timer;
    /// <summary>
    /// <para>무작위 쇼케이스의 앞으로 이동하고,</para>
    /// 정해진 시간 동안 원하는 케이크가 없으면 나간다.
    /// </summary>
    void RandomMove()
    {
        timer += Time.deltaTime;


        // 목표 지점에 도달하면 정지 후 밑에 코드 진행
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f) StartCoroutine(StopForSeconds(1f));
        else return;


        targetLoc = showcasePoses[Random.Range(0, showcasePoses.Length)].position;


        switch (moveState)
        {
            // 무작위 쇼케이스 앞으로 이동
            case 1: 
                if (timer > randTime)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    CakeCheck();
                    timer = 0;
                }
                break;
            // 원하는 케이크가 있는지 감지, 있다면 moveState를 3(TakeIt)으로 없다면 moveState를 5(Out)로 변경
            case 2:
                CakeCheck();
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = cakeManager.cakeSODataList[wantedCakeIndex].itemImage;
                

                if (timer > randTime)
                {
                    moveState = 5;
                    timer = 0;
                }
                break;
        }


    }



    int wantedShowcaseIndex;
    /// <summary>
    /// <para>
    /// 원하는 케이크가 있다면 목표 지점을 설정 후, 
    /// 케이크를 배열에서 제거하고,
    /// moveState를 3(TakeIt)으로 변경한다.
    /// </para>
    /// 만약 없다면 moveState를 2(RandomMove)로 변경한다.
    /// </summary>
    void CakeCheck()
    {
        List<int> wantedCakeIndexes = cakeShowcaseController.CakeFindIndex(wantedCakeIndex);


        // 원하는 케이크가 있는지 판별
        if (wantedCakeIndexes != null && wantedCakeIndexes.Count > 0)
        {
            // 원하는 케이크가 있는 쇼케이스 위치 번호 추출
            wantedShowcaseIndex = wantedCakeIndexes[Random.Range(0, wantedCakeIndexes.Count)];
            List<int> cakePlaces = cakeShowcaseController.CakeFindPlace(wantedShowcaseIndex, wantedCakeIndex);


            // 쇼케이스에서 케이크 제거
            cakeShowcaseController.CakeSell(wantedShowcaseIndex, cakePlaces[Random.Range(0, cakePlaces.Count)]);


            // 하트 말풍선
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = heartImg;

            
            // 목표 지점 변경 (원하는 케이크가 있는 쇼케이스 앞)
            targetLoc = showcasePoses[wantedShowcaseIndex].position;
            

            moveState = 3;
        }
        else
        {
            moveState = 2;
        }
    }



    /// <summary>
    /// <para>
    /// *목표 지점 도달 시,
    /// 말풍선을 없애고, 
    /// 목표 지점을 계산대 앞으로 변경.
    /// </para>
    /// *목표 지점: 원하는 케이크가 있는 쇼케이스 앞
    /// </summary>
    void TakeIt()
    {
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            // 하트 말풍선 제거
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(StopForSeconds(1f));


            //목표 지점 변경 (계산대 앞)
            targetLoc = cashierPos.position;


            moveState = 4;
        }
    }



    /// <summary>
    /// <para>
    /// *목표 지점 도달 시, 
    /// 1초 동안 하트 이미지를 띄운 뒤, 
    /// 보유한 돈을 증가 시키고 동전 소리 재생, 
    /// 목표 지점을 포탈 위치로 변경
    /// </para>
    /// *목표 지점: 계산대 앞
    /// </summary>
    void BuyIt()
    {
        
        MoveTo();
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            // 하트 말풍선
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = heartImg;
            StartCoroutine(StopForSeconds(0.1f));
            transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(StopForSeconds(1.0f));
            transform.GetChild(0).gameObject.SetActive(false);


            // 보유한 금액 증가, 동전 소리 재생
            GameManager.instance.getMoney(cakeManager.cakeSODataList[wantedCakeIndex].cakePrice);
            UIManager.instance.RaiseUpCakeCntForEndBoard(wantedCakeIndex);
            cakeManager.soldCakeCount[wantedCakeIndex]++;
            cakeManager.CallSellAudio();


            // 목표 지점을 포탈 위치로 변경
            targetLoc = spawnPos.position;
        }
    }



    /// <summary>
    /// <para>
    /// *목표 지점 도달 시,
    /// 포탈음을 재생한 뒤,
    /// 오브젝트를 삭제한다.
    /// </para>
    /// *목표 지점: 포탈
    /// </summary>
    void Out()
    {
        if (Vector2.Distance(transform.position, targetLoc) <= 0.01f)
        {
            // 포탈음 재생
            cakeManager.CallPortalAudio();

            
            // 오브젝트 삭제
            Destroy(gameObject);
        }
    }



    /// <summary>
    /// 속도를 0으로 설정한 뒤, 일정 시간 동안 정지한다.
    /// </summary>
    /// <param name="seconds">정지해 있을 시간</param>
    /// <returns></returns>
    IEnumerator StopForSeconds(float seconds)
    {
        agent.speed = 0f;
        yield return new WaitForSeconds(seconds);
        agent.speed = speed;
    }



    float delay = 0.25f;
    int currentAnimeState;
    /// <summary>
    /// <para>animeState값에 따라 애니메이션을 진행</para>
    /// 0: Idle, 1: Up, 2: Down
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateAnimation()
    {
        while (true)
        {
            // animeState값을 animator에 전달
            animator.SetInteger("animeState", animeState);


            // 이미지 순환
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = customerImgs[animeState][currentImgIndex];
            if (animeState != 0) currentImgIndex = (currentImgIndex + 1) % 4;


            // delay 동안 정지, animeState가 바뀌면 빠져나옴 
            for (float time = 0; time < delay; time += Time.deltaTime)
            {
                if (animeState != currentAnimeState)
                {
                    currentAnimeState = animeState;
                    currentImgIndex = 0;
                    break;
                }
                yield return null;
            }


            yield return null;
        }
    }
}
