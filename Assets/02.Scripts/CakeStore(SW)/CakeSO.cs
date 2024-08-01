using UnityEngine;

[CreateAssetMenu(fileName = "NewCakeData", menuName = "Cake Data", order = 1)]
public class CakeSO : ScriptableObject
{
    public string cakeName;
    public int cakeCost;
    public int bakeTime;
    public int cakePrice;
    public int cakeIdx;
    public Sprite cakeImage;
    public int cakeCount;
    public bool isLocked;
}
