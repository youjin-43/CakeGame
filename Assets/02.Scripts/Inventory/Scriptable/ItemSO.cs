using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable { get; set; } // 아이템이 인벤토리 칸에 누적 저장될 수 있는지 여부 판단.

    // 각각의 스크립터블 오브젝트의 인스턴스는 자신만의 고유 instance id 를 가지고 있음. 즉, 이걸로 같은 아이템인지 아닌지 판단할 것임..
    public int ID => GetInstanceID();

    [field: SerializeField]
    public int MaxStackSize { get; set; } = 1;

    [field: SerializeField]
    public string Name { get; set; } // 아이템 이름

    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; } // 아이템 설명


    [field: SerializeField]
    public Sprite itemImage { get; set; } // 아이템 이미지

    [field: SerializeField]
    public int itemType; // 아이템 타입(0: 씨앗, 1: 과일, 2: 보석, 3: 케이크, ...)
}
