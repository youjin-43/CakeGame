using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CakeMaker : MonoBehaviour
{
    public int cakeMakerIndex;
    public GameObject timerUI;
    public Sprite[] timerSprites;
    public Sprite completedSprite;

    private GameObject storeManager;
    private GameObject cakeMakerPanel;
    private int currentCakeIndex;
    private bool isMakingCake;
    private bool isMakeComplete;
    private Image timerImage;
    private float totalMakeTime;
    private float elapsedTime;

    public void Initialize(int index, GameObject manager, GameObject panel, Sprite[] sprites, Sprite completeSprite)
    {
        cakeMakerIndex = index;
        storeManager = manager;
        cakeMakerPanel = panel;
        timerSprites = sprites;
        completedSprite = completeSprite;

        timerImage = timerUI.GetComponent<Image>();
        timerImage.sprite = timerSprites[0];
        timerUI.SetActive(false);
        isMakingCake = false;
        isMakeComplete = false;

        Button timerButton = timerUI.GetComponent<Button>() ?? timerUI.AddComponent<Button>();
        timerButton.onClick.AddListener(() => storeManager.GetComponent<CakeMakeManager>().OnTimerUIClicked(index));
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
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        timerUI.GetComponent<RectTransform>().position = screenPos + new Vector3(0, 50, 0);
    }

    void OnMouseDown()
    {
        if (!isMakingCake)
        {
            storeManager.GetComponent<CakeMakeManager>().OpenPanel(cakeMakerIndex);
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
        timerImage.sprite = completedSprite;
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
        storeManager.GetComponent<CakeMakeManager>().CompleteCake(currentCakeIndex);
        timerUI.SetActive(false);
        isMakingCake = false;
        isMakeComplete = false;
        timerImage.sprite = timerSprites[0];
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
        return 5; // 예시로 5초 설정
    }
}
