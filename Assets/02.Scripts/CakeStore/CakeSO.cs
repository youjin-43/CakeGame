using UnityEngine;

[CreateAssetMenu(fileName = "NewCakeData", menuName = "Cake Data", order = 1)]
public class CakeSO : ItemSO
{
    public int cakeCost;
    public int bakeTime;
    public int cakePrice;
    public int[] materialIdxs = new int[5];
    public int[] materialCounts = new int[5]; // 최대로 설정할 수 있는 재료의 갯수를 우선 5개로 해놓음 ;
    public int cakeCount;
    public bool isLocked;
    public int cakeIdx;
}


