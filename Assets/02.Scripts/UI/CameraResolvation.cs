using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;

    void Awake()
    {
        RescaleCamera();
    }

    void Start()
    {
        
    }

    void Update()
    {
        RescaleCamera();
    }

    void OnPreCull()
    {
        if (Application.isEditor) return;

        RescaleCamera();
    }

    private void RescaleCamera()
    {
        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

        float targetAspect = 16.0f / 9.0f;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }

        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }
}
