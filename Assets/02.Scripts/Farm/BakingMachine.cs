using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BakingMachine : MonoBehaviour
{
    [Header("Cake Info")]
    [SerializeField]
    public CakeContainer cakeContainer;

    [Header("Button Prefabs")]
    public Button[] buttons; // [0]: 케이크 제작대 켜는 버튼, [2]: 케이크 수확 버튼

    [Header("Machine State")]
    public string curState = "없음"; // 상태: 1. 없음, 2. 제작, 3. 제작완료
    public Button curButton = null; // 버튼 종류: 1. 케이크 제작대 켜는 버튼, 2. 케이크 수확 버튼 (상태 1일 때 버튼 1, 상태 2일 때 버튼 x, 상태 3일 때 버튼 2)
    public CakeData curCakeData = null; // 이거 케이크 제작 버튼 눌렀을 때 그 슬롯과 일치하는 케이크 데이터 넣어주기.
    public int curTime = 0; // 빵 만들기 시작한 후 지난 시간



    public void MakeCake(string fruitIdx, int fruitCount, int bakingTime)
    {

    }
}
