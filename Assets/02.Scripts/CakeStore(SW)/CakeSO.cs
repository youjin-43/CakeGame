using UnityEngine;

[CreateAssetMenu(fileName = "NewCakeData", menuName = "Cake Data", order = 1)]
public class CakeSO : ItemSO
{
    public int cakeCost;
    public int bakeTime;
    public int cakePrice;
    public int[] materialType;
    public int[] materialCount;
    public int cakeCount;
    public bool isLocked;
    public int cakeIdx;
}


