using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowtoUseSeedSO : MonoBehaviour
{
    Seedtmp apple;

    Seedtmp[] Seeds;

    void Start()
    {
        apple = (Seedtmp)Resources.Load("SeedSO/apple");
        Debug.Log("name: " + apple.name + ", idx: " + apple.seedIdx);

        Seeds = Resources.LoadAll<Seedtmp>("SeedSO");

        for (int i = 0; i < Seeds.Length; i++)
        {
            Debug.Log(Seeds[i].name + Seeds[i].seedIdx); // 왜 seedName만 디버깅이 안돼??? 
        }

    }

}
