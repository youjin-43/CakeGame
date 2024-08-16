using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    int deleteTime = 1;
    float curTime = 0;
    Button button;
    AnimalInteractionManager animalManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        animalManager = FindAnyObjectByType<AnimalInteractionManager>();

        // 버튼에 함수 연결..
        button.onClick.AddListener(OnClicked);
        button.onClick.AddListener(animalManager.GetAnimal);
    }


    private void Update()
    { 
        if (curTime >= deleteTime)
        {
            Destroy(gameObject);
        } 
        else
        {
            curTime += Time.deltaTime;
        }
    }


    public void OnClicked()
    {
        Destroy(gameObject);
    }
}
