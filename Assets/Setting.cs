using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Setting : MonoBehaviour
{
    public GameObject settingPanel;
    private bool isActive;

    void Start()
    {
        isActive = true;
        OnSettingPanel();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))

        {
            OnSettingPanel();
        }
    }
    public void OnSettingPanel()
    {
        isActive = !isActive;
        settingPanel.SetActive(isActive);
    }
}
