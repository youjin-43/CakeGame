using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject 를 상속받는 클래스를 스크립터블 오브젝트 애셋으로 생성해주는 역할
// fileName: 생성되는 애셋의 이름
// menuName: 스크립터블 오브젝트 애셋을 생성하는 메뉴의 이름(상단 메뉴 바의 [Assets > Create)] 아래의 메뉴에서 찾을 수 있음
// order: Create 메뉴들 중에서 몇 번째 위치에 표시할 것인지를 정하는 매개변수 
[CreateAssetMenu(fileName = "Cake Data", menuName = "Scriptable Object/Cake Data", order = int.MaxValue)] 
public class CakeData : ScriptableObject
{
    [SerializeField]
    private string cakeName; // 케이크 이름
    public string CakeName { get { return cakeName; } }


    [SerializeField]
    private string neededFruitName; // 케이크 만드는데 필요한 과일
    public string NeededFruitName { get { return neededFruitName; } }


    [SerializeField]
    private int neededFruitCount; // 케이크 만드는데 필요한 과일 개수
    public int NeededFruitCount { get { return neededFruitCount; } }


    [SerializeField]
    private int neededFruitIdx; // 케이크 만드는데 필요한 과일의 인덱스
    public int NeededFruitIdx { get { return neededFruitIdx; } }


    [SerializeField]
    private int makeTime; // 만드는데 걸리는 시간
    public int MakeTime { get { return makeTime; } }


    [SerializeField]
    private int makeMoney; // 만드는데 드는 돈
    public int MakeMoney { get { return makeMoney; } }


    [SerializeField]
    private int sellPrice; // 판매 가격
    public int SellPrice { get { return sellPrice; } }
}
