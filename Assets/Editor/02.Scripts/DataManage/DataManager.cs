using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //for DataTable
using System.IO; //for StreamReader




public class DataManager : MonoBehaviour
{
    //싱글턴으로
    public static DataManager instance; // 싱글톤을 할당할 전역 변수

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
            Debug.Log("DataManager가 생성됐습니다");
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 삭제되지 않도록

            Debug.Log("csv파일을 데이터 데이블 딕셔너리로 저장(in Awake();) ");
            //tableDic.Add(CSVDatas.QuestTable, CSVReader(Application.dataPath + "/StreamingAssets/Quest.csv"));
            tableDic.Add(CSVDatas.EventTable, CSVReader(Application.dataPath + "/StreamingAssets/DialogTmp.csv"));
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 DataManager가 존재합니다!");
            Destroy(gameObject);
            Debug.Log("DataManager를 죽입니다");
        }

    }

    public enum CSVDatas
    {
        EventTable
    }

    public Dictionary<CSVDatas, DataTable> tableDic = new Dictionary<CSVDatas, DataTable>();

	// csv의 행과 열을 각각 읽어와 리턴
    public DataTable CSVReader(string path)
    {
        DataTable dt = new DataTable();

        StreamReader sr = new StreamReader(path); //C#에서 파일을 읽고 쓰기 위한 스트림으로 StreamReader와 StreamWriter를 사용할 수 있다.

        string[] headers = sr.ReadLine().Split(',');
        foreach (string header in headers)
        {
            dt.Columns.Add(header);
        }

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            string[] data = line.Split(',');

            DataRow row = dt.NewRow();
            for (int i = 0; i < headers.Length; i++)
            {
                row[i] = data[i];
            }
            dt.Rows.Add(row);
        }

        return dt;
    }
}
