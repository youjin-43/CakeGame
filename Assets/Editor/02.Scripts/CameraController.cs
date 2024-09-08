using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 0.005f;
    [SerializeField] private float zoomSpeed = 0.01f;

    [Header("LIMIS")]
    [SerializeField] private float limitUp = 7.0f;
    [SerializeField] private float limitDown = -7.0f;
    [SerializeField] private float limitLeft = -10.0f;
    [SerializeField] private float limitRight = 10.0f;

    void Update()
    {
        //moveInCom();

        MoveCam();
        ZoomCam();
    }

    //컴으로 조작할때 사용 
    private void moveInCom()
    {
        panSpeed = 24f;
        zoomSpeed = 30f;

        //camera position - 상하좌우키로 
        float dx = panSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        float dy = panSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        Camera.main.transform.Translate(dx, dy, 0);

        //zoomin zoomout - qe를 누르면 줌인 줌아웃 됨
        float dz = zoomSpeed * Time.deltaTime * Input.GetAxis("Zoom");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + dz, 4f, 20f);
    }   


    //카메라 이동 
    private void MoveCam()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];

            float x = Camera.main.transform.position.x - touch.deltaPosition.x * panSpeed;
            x = (x < limitLeft) ? limitLeft : x;
            x = (limitRight < x) ? limitRight : x;

            float y = Camera.main.transform.position.y - touch.deltaPosition.y * panSpeed;
            y = (y < limitDown) ? limitDown : y;
            y = (limitUp < y) ? limitUp : y;

            Camera.main.transform.position = new Vector3(x,y,Camera.main.transform.position.z);
        }
    }

    //카메라 줌인아웃 
    private void ZoomCam()
    {
        if (Input.touchCount == 2)
        {
            Touch touch_1 = Input.touches[0];
            Touch touch_2 = Input.touches[1];

            //이전 프레임의 터치 좌표를 구한다.
            Vector2 t1PrevPos = touch_1.position - touch_1.deltaPosition;
            Vector2 t2PrevPos = touch_2.position - touch_2.deltaPosition;

            //이전 프레임과 현재 프레임 움직임 크기를 구함.
            float prevDeltaMag = (t1PrevPos - t2PrevPos).magnitude;
            float deltaMag = (touch_1.position - touch_2.position).magnitude;

            //두 크기값의 차를 구해 줌 인/아웃의 크기값을 구한다.
            float deltaMagDiff = prevDeltaMag - deltaMag;
            Camera.main.orthographicSize += deltaMagDiff * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 20f);
        }
    }
}