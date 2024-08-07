using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed_", menuName = "Assets/New Seed")]

public class Seedtmp : ScriptableObject
{
    public string seedName;
    public int seedPrice;
    public int growTime;
    public int seedIdx;
    public Sprite sprite;
}