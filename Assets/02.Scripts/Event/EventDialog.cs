using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //for DataTable
using UnityEngine.UI;
using UnityEngine.SceneManagement;//씬 관련 라이브러리
using System.IO; // 파일, 폴더 생성을 위해 

public class EventDialog : MonoBehaviour
{
    public EventDialogs eventDialogs; 
    private string DialogjsonPath = Application.dataPath + "/02.Scripts/Event/DialogDB.json"; //dataPath

    public Text dialogText;
    public string[] currntDialog;
    private int idx=0;

    void Start()
    {
        LoadEventDialogJson();
        dialogText = GameObject.Find("DialogText").GetComponent<Text>();//대사 출력을 위한 텍스트 오브젝트 셋팅

        //int level = ExpManager.instance.level;
        int level = 2;
        if(level == 2)
        {
            currntDialog = eventDialogs.Level2;
        }else if (level == 4)
        {
            currntDialog = eventDialogs.Level4;
        }
        else if (level == 4)
        {
            currntDialog = eventDialogs.Level6;
        }
        else if (level == 4)
        {
            currntDialog = eventDialogs.Level8;
        }
        else
        {
            currntDialog = eventDialogs.Level10;
        }

        dialogText.text = currntDialog[idx];
    }

    public void PassDialog()
    {
        idx++;

        if (idx >= currntDialog.Length)
        {
            SceneManager.LoadScene("CakeStore 1");
            QuestManager.instance.GenMainQuest(ExpManager.instance.level);
            return;
        }
        dialogText.text = currntDialog[idx];
    }



    public void LoadEventDialogJson()
    {
        string DialogJsonText = File.ReadAllText(DialogjsonPath);
        Debug.Log("DialogJsonText : " + DialogJsonText);
        eventDialogs = JsonUtility.FromJson<EventDialogs>(DialogJsonText);
        Debug.Log("이벤트 대사 json에서 불러오기 완료");
    }

    [System.Serializable]
    public class EventDialogs
    {
        public string[] Level2;
        public string[] Level4;
        public string[] Level6;
        public string[] Level8;
        public string[] Level10;
    }
}
