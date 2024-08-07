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
        DialogDT = DataManager.instance.tableDic[DataManager.CSVDatas.EventTable];
        Debug.Log("데이터에서 EventTable 할당받음");

        dialogText.text = DialogDT.Rows[idx][1].ToString();
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
