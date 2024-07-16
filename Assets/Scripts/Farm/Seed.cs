using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    // ������ �� ���������� ����� ���� ��
    // �������� �������� SeedContainer �� ������ ��..

    public string seedName;
    public float seedPrice;
    public float growTime; // �����ϴµ� �ɸ��� �ð�
    public float currentTime; // ���� �ĺ��� ������� �ð�

    public bool isGrown = false; // �� �ڶ����� ���� Ȯ�ο� ����

    public int seedIdx; // ���� �ε���


    private void OnEnable()
    {
        isGrown = false;
        currentTime = 0;
        Debug.Log("������ �ɾ���!");
    }

    private void Update()
    {
        if (currentTime >= growTime) {
            isGrown = true;
            Debug.Log("�� �ڶ���!");
            transform.gameObject.SetActive(!isGrown);
        }
        currentTime += Time.deltaTime;
    }
}
