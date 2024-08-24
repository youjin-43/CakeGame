using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routine : MonoBehaviour
{
    private float timer;
    public float prepareTime;
    public float openTime;
    public RoutineState routineState;
    public static Routine instance;
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            routineState = RoutineState.Prepare;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록

        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Update()
    {
        //timer += Time.deltaTime;
        //if(timer > prepareTime)
        //{
        //    routineState = RoutineState.Open;
        //}
        //if(timer > prepareTime + openTime)
        //{
        //    routineState = RoutineState.Close;
        //}
        
        switch (routineState)
        {
            case RoutineState.Prepare:
                break;
            case RoutineState.Open:
                break;
            case RoutineState.Close:
                break;
        }
    }
    public void SetPrepare()
    {
        routineState = RoutineState.Prepare;
    }
}
public enum RoutineState
{
    None,
    Prepare,
    Open,
    Close
}
