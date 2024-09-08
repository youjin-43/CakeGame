using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CakeMaker : MonoBehaviour
{
    public int cakeMakerIndex; // 케이커 메이커 번호
    private SpriteRenderer timerUI; // 타이머 오브젝트
    public Sprite[] timerSprites; // 0.2초마다 순환할 4개의 이미지
    public Sprite completedSprite; // 제작 완료 시 설정할 이미지
    private int currentCakeIndex;
    public bool isMakingCake;
    private bool isMakeComplete;
    private float cakeBakeTime;
    private float passedTime;

    void Start()
    {
        InitializeCakeMaker();
    }
    void InitializeCakeMaker() // 상태를 초기화함.
    {
        timerUI = transform.GetChild(0).GetComponent<SpriteRenderer>();
        isMakingCake = false;
        isMakeComplete = false;
        UpdateColliders();
    }
    void OnMouseDown()
    {
        if (/*!CakeManager.instance.IsPointerOverUIObjectMobile(Input.touches[0])*/!CakeManager.instance.IsPointerOverUIObjectPC())
        {
            if (!isMakingCake) // 케이크 제작 중 아닐 시
            {
                // 케이크 메이커 패널 활성화
                CakeManager.instance.cakeMakerController.OpenPanel(cakeMakerIndex);
                CakeManager.instance.CallOpenAudio();
            }
            else if (isMakeComplete) // 케이크 제작 완료 시
            {
                isMakingCake = false;
                isMakeComplete = false;
                UpdateColliders();
                CakeManager.instance.CallGetCakeAudio();
                // 케이크 제작 완료 호출
                CakeManager.instance.cakeMakerController.CompleteCake(currentCakeIndex);
            }
        }
    }

    public void StartMakingCake(int index, int BakeTime)
    {
        currentCakeIndex = index;
        cakeBakeTime = BakeTime;
        passedTime = 0f;
        isMakingCake = true;
        UpdateColliders();
        transform.GetChild(2).GetComponent<AudioSource>().loop = true;
        transform.GetChild(2).GetComponent<AudioSource>().clip = CakeManager.instance.BakeSound[0];
        transform.GetChild(2).GetComponent<AudioSource>().Play();
        StartCoroutine(TimerRoutine());
        StartCoroutine(TimerImageChangeRoutine());
    }

    IEnumerator TimerRoutine() // 케이크 제작 시간 후 케이크 제작 완료
    {
        while (passedTime < cakeBakeTime) //제작 완료 시간까지 루틴을 반복
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        transform.GetChild(2).GetComponent<AudioSource>().loop = false;
        transform.GetChild(2).GetComponent<AudioSource>().clip = CakeManager.instance.BakeSound[1];
        transform.GetChild(2).GetComponent<AudioSource>().Play();
        isMakeComplete = true;
        timerUI.sprite = completedSprite; // 제작 완료 이미지로 변경
    }

    IEnumerator TimerImageChangeRoutine() // 케이크 제작 동안 타이머 이미지 변경
    {
        int spriteIndex = 0; //이미지 초기화

        while (passedTime < cakeBakeTime)  //제작 완료 시간까지 루틴을 반복
        {
            timerUI.sprite = timerSprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % timerSprites.Length;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateColliders() // 타이머와 오븐의 콜리더를 설정 
    // !!!!!!!반드시 isMakingCake 값의 변화 이후에 와야함.!!!!!!!
    {
        if (timerUI == null) timerUI = transform.GetChild(0).GetComponent<SpriteRenderer>();
        timerUI.gameObject.SetActive(isMakingCake);
        GetComponent<PolygonCollider2D>().enabled = !isMakingCake;
        GetComponent<BoxCollider2D>().enabled = isMakingCake;
    }
}