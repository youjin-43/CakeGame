using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryDescription : MonoBehaviour
{
    [SerializeField]
    public Image descriptionItemImage;

    [SerializeField]
    public Text descriptionTitle;

    [SerializeField]
    public Text description;


    private void Awake()
    {
        ResetDescription(); // 맨 처음 시작할 때 아이템 설명 칸은 다 비운채로 시작함..
    }

    public void ResetDescription()
    {
        descriptionItemImage.gameObject.SetActive(false);
        descriptionTitle.text = "";
        description.text = "";
    }


    // 아이템을 클릭했을 때, 아이템에 맞는 설명을 띄우기 위한 함수..
    public void SetDescription(Sprite sprite, string itemName, string itemDescription)
    {
        descriptionItemImage.gameObject.SetActive(true);
        descriptionItemImage.sprite = sprite;
        descriptionTitle.text = itemName;
        description.text = itemDescription;
    }
}
