using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Update()
    {
        // A 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.A))
        {
            // 현재 마우스 위치를 world 좌표로 변환
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f; // 오브젝트의 Z 축을 고정시키거나 필요에 따라 조정
            
            // 오브젝트를 마우스 위치로 이동
            transform.position = targetPosition;
        }
    }
}