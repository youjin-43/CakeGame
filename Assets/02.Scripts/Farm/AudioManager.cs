using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("#BGM")]
    public AudioClip[] bgmClips; // [0]: 가게, [1]: 농장
    public float bgmVolume;
    public AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public AudioSource sfxPlayer;


    public enum BGM { CAKESTORE, FARM }
    public enum SFX { PLOW, PLANT, HARVEST, FAIL, BUTTON, UPGRADE, RETURN, COMPLETE }


    private void Awake()
    {
        Init();
    }


    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // AudioManger 밑에 자식으로 생성..
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClips[0]; // 일단 맨 처음은 가게 bgm 으로..


        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // AudioManager 밑에 자식으로 생성..
        sfxPlayer = sfxObject.AddComponent<AudioSource>();
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = sfxVolume;



        // 씬 이름 확인하기 위함..
        Scene scene = SceneManager.GetActiveScene(); //함수 안에 선언하여 사용한다.
        Debug.Log(scene.name);

        switch (scene.name)
        {
            case "CakeStore 1":
                SetBGM(BGM.CAKESTORE);
                bgmPlayer.Play(); // 가게씬에서는 바로 bgm 실행하도록..
                break;
            case "Farm":
                SetBGM(BGM.FARM); // 농장씬은 동물 게임이 일정 확률로 뜨니까 바로 실행하지는 않도록..
                break;
        }
    }


    public void SetBGM(BGM bgmState)
    { 
        bgmPlayer.clip = bgmClips[(int)bgmState]; // 현재 씬에 맞는 배경음으로 바꿔주기..
    }


    public void SetSFX(SFX sfxState)
    {
        sfxPlayer.clip = sfxClips[(int)sfxState]; // 현재 효과 상태에 맞는 효과음으로 바꿔주기..
        sfxPlayer.Play(); // 효과음 실행..
    }


    public void SetUISFX()
    {
        // 이 함수를 UI 버튼에 연결해줄 것..

        sfxPlayer.clip = sfxClips[(int)SFX.BUTTON]; // 효과음을 버튼 효과음으로 바꿔주기..
        sfxPlayer.Play(); // 효과음 실행..
    }
}
