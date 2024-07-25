using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("About UI")]
    public GameObject runningOverBoard; //가게 운영이 끝났을때 활성화 할 오브젝트 
    public Text runningTimeText;//현재 시간을 표시할 텍스트 컴포넌트
    public Text seasonText;
    public Text dateText;
    public Text moneyText;//현재 돈을 표시할 텍스트 컴포넌트
    public Text popularityText;//현재 돈을 표시할 텍스트 컴포넌트

    public GameObject RunStartButton; //가게 운영이 끝났을때 활성화 할 오브젝트 


    //private void Update()
    //{
    //    if (GameManager.instance.isRunning)
    //    {
    //        runningTimeText.text = "Time :" + (int)GameManager.instance.runTime;
    //    }
    //}
    // 아나ㅣ 여따 하면 왤케 오류가 생기냐....

    public void SetDatainUI()
    {
        seasonText.text = GameManager.instance.season.ToString();
        dateText.text = GameManager.instance.date.ToString();
        moneyText.text = "Money : " + GameManager.instance.money;
        popularityText.text = "Popularity : " + GameManager.instance.popularity;
    }
}
