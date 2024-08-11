using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CakeMaker : MonoBehaviour
{
    public int cakeMakerIndex;
    public GameObject cakeMakerPanel;
    public GameObject storeManager;
    public GameObject timerUI;
    public Sprite[] timerSprites; // 0.2초마다 순환할 4개의 이미지
    public Sprite completedSprite; // 제작 완료 시 설정할 이미지

    private int currentCakeIndex;
    private bool isMakingCake;
    private bool isMakeComplete;
    private Image timerImage;
    private float totalMakeTime;
    private float elapsedTime;

    void Start()
    {
        timerImage = timerUI.transform.GetComponent<Image>();
        timerImage.sprite = timerSprites[0];
        timerUI.SetActive(false);
        isMakingCake = false;
        isMakeComplete = false;
    }

    void Update()
    {
        if (isMakingCake)
        {
            UpdateTimerPosition();
        }
    }

    void UpdateTimerPosition()
    {
        // CakeMaker 오브젝트의 월드 공간 위치를 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        // Timer UI의 RectTransform 위치를 화면 좌표로 변환한 위치로 설정
        timerUI.GetComponent<RectTransform>().position = screenPos + new Vector3(0, 50, 0); // 필요에 따라 오프셋 조정
    }

    void OnMouseDown()
    {
        if (!isMakingCake)
        {
            storeManager.GetComponent<CakeMakerController>().OpenPanel(cakeMakerIndex);
        }
    }

    public void StartMakingCake(int index, int cakeMakeTime)
    {
        currentCakeIndex = index;
        totalMakeTime = cakeMakeTime;
        elapsedTime = 0f;
        StartCoroutine(TimerRoutine(cakeMakeTime));
        StartCoroutine(SpriteChangeRoutine());
    }

    IEnumerator TimerRoutine(int cakeMakeTime)
    {
        isMakingCake = true;
        timerUI.SetActive(true);
        UpdateTimerPosition();
        while (elapsedTime < cakeMakeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMakeComplete = true;
        timerImage.sprite = completedSprite; // 제작 완료 이미지로 변경
    }

    IEnumerator SpriteChangeRoutine()
    {
        int spriteIndex = 0;
        while (elapsedTime < totalMakeTime)
        {
            timerImage.sprite = timerSprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % timerSprites.Length;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void CompleteCake()
    {
        storeManager.GetComponent<CakeMakerController>().CompleteCake(currentCakeIndex);
        timerUI.SetActive(false);
        isMakingCake = false;
        isMakeComplete = false;
        timerImage.sprite = timerSprites[0]; // 처음 이미지로 리셋
    }

    public bool IsMakeComplete()
    {
        return isMakeComplete;
    }

    public bool TimerUIActive()
    {
        return timerUI.activeSelf;
    }

    public int GetCakeMakeTime()
    {
        // 여기에 케이크 제작 시간을 반환하는 로직을 추가합니다.
        // 예시로 5초로 설정합니다.
        return 5;
    }
}
