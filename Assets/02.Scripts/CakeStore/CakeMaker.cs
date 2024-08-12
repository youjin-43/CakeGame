using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CakeMaker : MonoBehaviour
{
    public int cakeMakerIndex;
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
    }

    void UpdateTimerPosition()
    {
        Canvas canvas = timerUI.GetComponentInParent<Canvas>();
        // CakeMaker 오브젝트의 월드 공간 위치를 뷰포트 좌표로 변환
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // 뷰포트 좌표를 캔버스의 스크린 좌표로 변환
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            new Vector2(viewportPos.x * Screen.width, viewportPos.y * Screen.height),
            Camera.main,
            out screenPos);

        // Timer UI의 RectTransform 위치를 설정
        timerUI.GetComponent<RectTransform>().anchoredPosition = screenPos; // 필요에 따라 오프셋 조정

    }

    void OnMouseDown()
    {
        if (!isMakingCake)
        {
            CakeManager.instance.GetComponent<CakeMakerController>().OpenPanel(cakeMakerIndex);
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
        CakeManager.instance.GetComponent<CakeMakerController>().CompleteCake(currentCakeIndex);
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
}
