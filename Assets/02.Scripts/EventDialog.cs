using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //for DataTable
using UnityEngine.UI;
using UnityEngine.SceneManagement;//씬 관련 라이브러리

public class EventDialog : MonoBehaviour
{
    public DataTable DialogDT;
    public Text dialogText;

    private int idx=0;

    // Start is called before the first frame update
    void Start()
    {
        DialogDT = GameManager.instance.dataManager.tableDic[DataManager.CSVDatas.EventTable];
        dialogText.text = DialogDT.Rows[idx][1].ToString();

        //경험치 실험 중


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            idx++;
            if (idx == 4)
            {
                SceneManager.LoadScene("CakeStore 2");
            }
            dialogText.text = DialogDT.Rows[idx][1].ToString();
        }
    }
}
