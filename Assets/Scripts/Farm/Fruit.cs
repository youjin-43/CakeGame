using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    // ������ �� ���������� ����� ���� ��
    // �������� �������� FruitContainer �� ������ ��..

    public string fruitName;
    public float fruitPrice;
    public bool isEnabled = false;

    public int fruitIdx = 0; // ���� �ε���

    private void OnEnable()
    {
        Debug.Log("������ �����!");
    }
}
