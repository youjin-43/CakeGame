using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMouseDragItem : MonoBehaviour
{
    [SerializeField]
    public Canvas canvas;

    [SerializeField]
    public UIInventoryItem item;


    // 캔버스속 Mouse Follower 게임 오브젝트의 활성화가 꺼진채로 시작하면 오류남....
    // 켜진채로 시작해야 Awake 함수로 진입해서 레퍼런스 가져오는데 끈채로 시작하면 못가져옴..
    // 혹시라도 또 똑같은 이유로 헤맬까봐 적어놓음..
    private void Awake()
    {
        // 일반적인 경우 UI의 root 는 canvas 이므로..
        canvas = transform.root.GetComponent<Canvas>();

        // 내 밑에 UIInventoryItem 이 있으므로 GetCompoentInChildren 함수를 이용해서 가져옴
        item = GetComponentInChildren<UIInventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }

    private void Update()
    {
        // 마우스 포인터의 스크린 좌표(터치도 마우스 포인터로 작동 하니까.. 일단 마우스 포인터의 포지션 이용)
        // 를 캔버스 내의 로컬 좌표로 변환한 다음, 변환된 로컬 좌표를 이용하여 특정 UI 요소의 위치 설정
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
            );

        // 변환된 로컬 좌표를 월드 좌표로 변환하여 transform.position 에 설정해줌.
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        Debug.Log("Item toggled" + val);
        gameObject.SetActive(val);
    }
}
