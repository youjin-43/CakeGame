using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//씬 관련 라이브러리

public class GoFarmButton : MonoBehaviour
{
    public void SceneChange()
    {

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CakeStore 2"))
        {
            SceneManager.LoadScene("Farm");
        }
        else
        {
            SceneManager.LoadScene("CakeStore 2");
        }
    }
}
