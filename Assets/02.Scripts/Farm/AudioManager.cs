using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    public AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public AudioSource sfxPlayer;


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
        bgmPlayer.clip = bgmClip;


        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // AudioManager 밑에 자식으로 생성..
        sfxPlayer = sfxObject.AddComponent<AudioSource>();
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = sfxVolume;
    }

    public void SetSFX(SFX sfxState)
    {
        sfxPlayer.clip = sfxClips[(int)sfxState]; // 현재 효과 상태에 맞는 효과음으로 바꿔주기..
        sfxPlayer.Play(); // 효과음 실행..
    }
}
