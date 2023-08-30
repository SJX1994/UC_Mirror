using Common;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelLoadManager : SingTon<ExcelLoadManager>
{
    public Dictionary<string, Dictionary<int, List<string>>> dict = new();

    public Dictionary<int, FileInfo> fileInfos = new();

    // Start is called before the first frame update
    public void Load()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        string path = Application.streamingAssetsPath;
#elif UNITY_ANDROID
        string path = Application.persistentDataPath;

        List<string> strings1 = new List<string>() { "HeroInfoConfig.xlsx", "NumericInfoConfig.xlsx", "SoulInfoConfig.xlsx", "UnitInfoConfig.xlsx", "WeaponInfoConfig.xlsx" };

        foreach (string s in strings1)
        {
            string pathInfo = "jar:file://" + Application.dataPath + "!/assets/" + s;
            // Application.streamingAssetsPath + "/" + s;

            UnityWebRequest request = UnityWebRequest.Get(pathInfo);

            request.SendWebRequest();//读取数据

            var info = true;

            while (info)
            {
                if (request.downloadHandler.isDone)//是否读取完数据
                {
                    var retur = request.downloadHandler.data;

                    File.WriteAllBytes(Application.persistentDataPath + "/" + s, retur);

                    info = false;
                }
            }
        }
        
#endif
        DirectoryInfo root = new DirectoryInfo(path);

        FileInfo[] files = root.GetFiles();

        foreach (FileInfo fileInfo in files)
        {
            var fileExtension = fileInfo.Extension;

            if (fileExtension == ".xlsx")
            {
                try
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];

                        string className = fileInfo.Name.Split(".xlsx")[0] + "Category";

                        int colCount = worksheet.Dimension.End.Column;

                        int rowCount = worksheet.Dimension.End.Row;

                        Dictionary<int, List<string>> addStr = new();

                        for (int row = 5; row <= rowCount; row++)
                        {
                            List<string> strings = new List<string>();

                            for (int col = 2; col <= colCount; col++)
                            {
                                string text = worksheet.Cells[row, col].Text;

                                strings.Add(text);
                            }

                            addStr.Add(int.Parse(strings[0]), strings);
                        }

                        if (!dict.ContainsKey(className))
                        {
                            dict.Add(className, addStr);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(fileInfo.Name + "表格配置有误，请进行检查" + e);
                }
            }
        }
    }
}
