using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnTextPanel : MonoBehaviour
{
    [SerializeField]
    public static ReturnTextPanel instance;

    // 안내 판넬
    [Header("Info Panel UI")]
    public Text returnText;
    public GameObject returnPanel;


    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 인벤토리가 삭제되지 않도록(인벤토리는 모든 씬에서 이용 가능해야 하기 때문에..)..
        }
        else
        {
            // instance에 이미 다른 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Destroy(gameObject);
        }
    }


    // 함수 호출하면 안내 판넬 뜨도록..
    public void SetInfoPanel(string text)
    {
        returnText.text = text;
        returnPanel.GetComponent<Animator>().SetTrigger("Start");
    }
}