using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    public AudioClip[] storeAudios;
    public AudioClip[] farmAudios;
    public AudioClip coinAudio;
    public Slider volumeSlider;     // 슬라이더 연결
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
