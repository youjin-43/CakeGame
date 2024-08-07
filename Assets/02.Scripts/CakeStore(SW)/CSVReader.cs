using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class CSVReader
{
    public static List<Dictionary<string, string>> Read(string filePath)
    {
        var data = new List<Dictionary<string, string>>();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length == 0)
            return data;

        var headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var entry = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length; j++)
            {
                entry[headers[j].Trim()] = values[j].Trim();
            }

            data.Add(entry);
        }

        return data;
    }
}
