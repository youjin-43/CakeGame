using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public Image seedImage;
    public Text seedName;
    public GameObject BuySlot;
    public Text totalPrice;
    public Text seedCountText;
    public Button leftButton;
    public Button rightButton;

    public int seedCount = 1;
    public int maxCount = 64;
    public int minCount = 1;

    public void minusSeedCount()
    {
        if (seedCount <= minCount) {
            // ���� �����Ϸ��� �ϴ� ���� ������ ���� ���� �ּ� �������� �۾����� ���� �ִ����� �Ѿ����
            seedCount = maxCount;
            return; 
        } 

        seedCount--;
    }

    public void plusSeedCount()
    {
        // ���� �����Ϸ��� �ϴ� ���� ������ ���� ���� �ִ� �������� Ŀ���� ���� �ּڰ����� �Ѿ����
        if (seedCount >= maxCount) {
            seedCount = minCount;
            return;
        }

        seedCount++;
    }

    private void Update()
    {
        // UI �� �ƴ� �κ��� Ŭ���ϸ� �׳� ��������..
        if (IsPointerOverUIObject()) return; 
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuySlot.SetActive(false);
            }
        }
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    public void ResetData()
    {
        seedCount = 1;
    }
}
