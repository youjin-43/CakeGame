using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data; //for DataTable
using System.IO; //for StreamReader




public class DataManager
{
    public enum CSVDatas
    {
        QuestTable,
        SeedTable,
        EventTable
    }

    public Dictionary<CSVDatas, DataTable> tableDic = new Dictionary<CSVDatas, DataTable>();
	
    // 생성자 호출 시 자동으로 csv를 불러와 딕셔너리에 저장
    public DataManager()
    {
        tableDic.Add(CSVDatas.QuestTable, CSVReader(Application.dataPath + "/StreamingAssets/Quest.csv"));
    }
	

	// csv의 행과 열을 각각 읽어와 리턴
    public DataTable CSVReader(string path)
    {
        DataTable dt = new DataTable();

        StreamReader sr = new StreamReader(path); //C#에서 파일을 읽고 쓰기 위한 스트림으로 StreamReader와 StreamWriter를 사용할 수 있다.

        string[] headers = sr.ReadLine().Split(',');
        foreach (string header in headers)
        {
            dt.Columns.Add(header);
            //Debug.Log(header);
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
