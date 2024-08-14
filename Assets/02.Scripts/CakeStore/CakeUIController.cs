using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeUIController : MonoBehaviour
{
    public static CakeUIController instance;

    public Collider2D[] colliders;
    public GameObject spritesToDisable;  // 케이크 제작 중 비활성화할 스프라이트 그룹
    
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void CloseMenu(GameObject menu)
    {
        DisableSprites(false);
        menu.SetActive(false);
    }
    
    // 스프라이트의 콜라이더를 활성화/비활성화
    public void DisableSprites(bool isActive)
    {
        colliders = spritesToDisable.transform.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = !isActive;
                if(colliders[i].GetComponent<CakeMaker>() != null && !isActive)
                {
                    colliders[i].GetComponent<CakeMaker>().UpdateColliders();
                }
            }
            
        }
    }
}
