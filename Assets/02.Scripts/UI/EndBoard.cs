using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBoard : MonoBehaviour
{
    int[] cnt;
    private void Start()
    {
        cnt = new int[5] { 0, 0, 0, 0, 0 };
    }

    public void RaiseUpCakeCntForEndBoard(int idx)
    {
        cnt[idx]++;
    }
}

