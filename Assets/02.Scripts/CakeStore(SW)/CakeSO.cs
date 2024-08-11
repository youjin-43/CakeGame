using UnityEngine;

[CreateAssetMenu(fileName = "NewCakeData", menuName = "Cake Data", order = 1)]
public class CakeSO : ItemSO
{
    public int cakeCost;
    public int bakeTime;
    public int cakePrice;
    public int[] materialIdxs;
    public int[] materialCounts;
    public int cakeCount;
    public bool isLocked;
    public int cakeIdx;
}


