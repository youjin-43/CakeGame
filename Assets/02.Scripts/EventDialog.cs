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

        //경험치 실험 중

        Debug.Log("GameManager.instance.date : " + GameManager.instance.date);


        if(GameManager.instance == null)
        {
            Debug.Log("GameManager.instance is null");
        }
        else
        {
            Debug.Log("GameManager.instance is not null");
        }

        Debug.Log("GameManager.instance.date : " + GameManager.instance.date);

        Debug.Log(GameManager.tmp);

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
