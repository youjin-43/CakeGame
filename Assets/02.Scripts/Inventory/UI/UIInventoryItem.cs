using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IDropHandler, IDragHandler
{
    [SerializeField]
    public Image itemImage;

    [SerializeField]
    public Text quantityText;

    [SerializeField]
    public Image itemBorderImage;


    [SerializeField]
    // delegate 선언
    // UIInventoryItem 타입의 매개변수를 받는 함수를 연결할 수 있음.
    public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag;

    [SerializeField]
    public bool empty = true;

    [SerializeField]
    public int touchCount = 0; // 두번 클릭하면 아이템 뒷창 뜨도록..


    /*
     PointerEventData는 다음과 같은 터치 관련 정보를 포함하고 있습니다:

    pointerId: 입력 장치의 ID입니다. 터치 장치에서는 각 터치마다 고유한 ID를 가집니다.
    position: 입력 포인터의 현재 스크린 좌표입니다.
    delta: 마지막 위치에서 현재 위치까지의 변경량입니다.
    pressPosition: 입력이 시작된 위치입니다.
    clickTime: 마지막 클릭이 발생한 시간입니다.
    clickCount: 클릭 횟수입니다.
     */


    // UI 는 매개변수로 넘겨줄 수 없음.
    // 그래서 아이템 이미지는 Sprite 로 넘겨줌
    public void SetData(Sprite itemImage, int quantity)
    {
        this.itemImage.sprite = itemImage; // 해당 아이템 이미지로 설정
        this.quantityText.text = quantity + ""; // 해당 아이템의 개수로 설정
        this.itemImage.gameObject.SetActive(true); // 이제 아이템 이미지 활성화..
        empty = false; // 이제 비어있지 않을테니..
    }

    public void Select()
    {
        // 경계 이미지 활성화
        itemBorderImage.enabled = true;
    }

    public void Deselect()
    {
        // 경계 이미지 활성화 끄기
        itemBorderImage.enabled = false;
    }

    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false); // 다시 아이템 이미지 비활성화..
        empty = true; // 다시 비어있도록..
        // 경계 이미지 활성화 끄기
        Deselect();
    }

    // 맨 처음 초기화시 호출 함수
    public void Initialize()
    {
        itemImage.gameObject.SetActive(false);
        quantityText.text = "";
        Deselect();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작!!");

        if (empty) return; // 만약 아이템 칸이 비어있으면 걍 빠져나가도록..

        OnItemBeginDrag?.Invoke(this);
    }


    // Drop 은 드래그가 종료된 위치에 드롭 가능한 오브젝트가 있는지 여부에 따라 호출이 결정됨.
    // 드롭 가능한 오브젝트가 있으면 호출됨(레이캐스트가 오브젝트를 제대로 감지해야함).
    // 드롭 가능한 오브젝트가 IDropHandler 인터페이스를 구현하고 있어야함.
    // 이건 드래그가 종료된 위치의 오브젝트에서 발생하는 이벤트임..
    // 즉, 0번 아이템을 드래그 시작해서 3번 아이템에 드래그 종료하면 3번 아이템에서 드롭 이벤트 발생!!
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("내려놨다!!");

        OnItemDroppedOn?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 끝!!");

        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 모든 터치는 고유의 아이디를 갖는다고 함.
        // 즉, 0보다 크거나 같으면 터치니까 조건문으로 터치인지 아닌지 판별
        if (eventData.pointerId >= 0) 
        {
            Debug.Log("Touch!");
            touchCount++;

            if (touchCount >= 2)
            {
                // 아이템을 파는 뒷면을 보여주는 로직이 들어갈 것..

                touchCount = 0;
            }
            Debug.Log(touchCount);
        } else
        {
            // 이건 마우스로 일단 확인하기 위함..
            Debug.Log("Touch!");
            touchCount++;
            Debug.Log(touchCount);

            if (touchCount >= 2)
            {
                // 아이템을 파는 뒷면을 보여주는 로직이 들어갈 것..

                touchCount = 0;
            }
            Debug.Log(touchCount);
        }

        OnItemClicked?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("움직이는 중~~");
    }
}
