using UnityEngine;

[CreateAssetMenu(fileName = "NewCakeData", menuName = "CakeSO", order = 1)]
public class CakeSO : ItemSO
{
    public int cakeCost;
    public int bakeTime;
    public int cakePrice;
    public int[] materialType;
    public int[] materialCount;
    public bool isLocked;
    public int cakeIdx;
}
