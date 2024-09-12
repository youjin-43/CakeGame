using Inventory.Model;
using UnityEngine;

public class Showcase : MonoBehaviour
{
    public CakeManager cakeManager;
    public GameObject[] cakeImages;
    public bool[] isCakeSelected;
    public int[] cakeType;
    public int showcaseIndex;



    void OnMouseDown()
    {
        if (!cakeManager.IsPointerOverUI_Mobile(Input.touches[0]))
        {
            cakeManager.cakeUIController.OpenShowcaseUI(0, showcaseIndex);
            cakeManager.CallOpenAudio();
        }
    }



    /// <summary>
    /// 쇼케이스 UI를 업데이트하는 메서드
    /// </summary>
    /// <param name="index"></param>
    public void UpdateShowcaseImg()
    {
        // 쇼케이스 내 각 위치의 케이크 이미지 상태를 업데이트
        for (int i = 0; i < isCakeSelected.Length; i++)
        {
            // 케이크가 있으면 이미지 활성화
            cakeImages[i].SetActive(isCakeSelected[i]);
            if (isCakeSelected[i]) cakeImages[i].GetComponent<SpriteRenderer>().sprite = CakeManager.instance.cakeDataList[cakeType[i]].itemImage;
        }
    }



    /// <summary>
    /// 쇼케이스 내의 케이크 모두 반환
    /// </summary>
    public void GetBack()
    {
        for (int i = 0; i < cakeType.Length; i++)
        {
            if (isCakeSelected[i])
            {
                InventoryItem tempItem = new InventoryItem()
                {
                    item = UIInventoryManager.instance.cakeItems[cakeType[i]],
                    quantity = 1,
                };
                UIInventoryManager.instance.AddItem(tempItem);
                isCakeSelected[i] = false;
                cakeType[i] = -1;
            }
        }
    }
}