using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//씬 관련 라이브러리

public class GoFarmButton : MonoBehaviour
{
    //private void Start()
    //{
    //    this.gameObject.SetActive(false);
    //}

    public void ChageSceneToStore()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Farm"))
        {
            SceneManager.LoadScene("CakeStore 1");
        }
    }

    public void ChageSceneToFarm()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CakeStore 1"))
        {
            SceneManager.LoadScene("Farm");
            Routine.instance.SetPrepare();
        }
    }
}
