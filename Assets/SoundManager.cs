using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioClip[] storeAudios;
    private AudioClip[] farmAudios;
    private AudioSource audioSource;
    private Slider volumeSlider;     // 슬라이더 연결
    private int currentClipNum = 0;
    private string CAKESTORESCENE = "CakeStore 1";
    private string FARMSCENE = "Farm";


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로딩될 때마다 함수를 호출하기위해 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SoundController.instance != null)
        {
            storeAudios = SoundController.instance.storeAudios;
            farmAudios = SoundController.instance.farmAudios;
            volumeSlider = SoundController.instance.volumeSlider;
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        // 슬라이더 초기값을 현재 오디오 소스의 볼륨으로 설정
        if (audioSource != null && volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;

            // 슬라이더 값이 변경될 때마다 볼륨 조절 함수 호출
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        StartCoroutine(PlayClip());
    }
    IEnumerator PlayClip()
    {
        RandomClip(SceneManager.GetActiveScene().name);
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }

    }
    void RandomClip(string sceneName)
    {
        int frontClipNum = currentClipNum;
        if (sceneName == CAKESTORESCENE)
        {
            if (storeAudios.Length == 0) return;
            if (storeAudios.Length == 1)
            {
                currentClipNum = 0;
            }
            else
            {
                while (frontClipNum == currentClipNum)
                {
                    currentClipNum = Random.Range(0, storeAudios.Length);
                }
            }
            audioSource.clip = storeAudios[currentClipNum];
        }
        else if (sceneName == FARMSCENE)
        {
            if (farmAudios.Length == 0) return;
            if (farmAudios.Length == 1)
            {
                currentClipNum = 0;
            }
            else
            {
                while (frontClipNum == currentClipNum)
                {
                    currentClipNum = Random.Range(0, farmAudios.Length);
                }
            }
            audioSource.clip = farmAudios[currentClipNum];
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
