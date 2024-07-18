using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum States
    {
        moving, // 거리활보중 
        waiting,// 케이크 받길 기다리는 중 
        goback // 받고 돌아가는 중 
    }

    [SerializeField] private States currentState;

    [SerializeField] private Move move;

    void Start()
    {
        move = GetComponent<Move>();
        currentState = States.moving;
    }

    void Update()
    {

        if(currentState == States.waiting)
        {
            StartWaiting();

            if (Input.GetKeyDown(KeyCode.P))
            {
                endWaiting();
            }
        }
    }

    public void ChangeStatetoWating()
    {
        currentState = States.waiting;
    }

    public void StartWaiting()
    {
        transform.GetChild(0).gameObject.SetActive(true); //케이크 달라는말 활성화
    }

    public void endWaiting()
    {
        GameManager.instance.getMoney(); //돈 증가 
        transform.GetChild(0).gameObject.SetActive(false); //케이크 달라는말 비활성화
        currentState = States.moving;

        move.EndWaitingMove();
    }
}