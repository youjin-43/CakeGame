using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록 s
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
