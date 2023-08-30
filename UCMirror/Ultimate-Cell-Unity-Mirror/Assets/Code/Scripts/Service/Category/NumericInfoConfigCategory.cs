using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericInfoConfigCategory : SingTon<NumericInfoConfigCategory>
{
    private Dictionary<int, NumericInfoCategory> dict = new();

    /// <summary>
    /// 根据ID获取对应值
    /// </summary>
    /// <param name="ConfigId"></param>
    /// <returns></returns>
    public NumericInfoCategory GetOne(int ConfigId)
    {
        if (dict.Count == 0)
        {
            Load();
        }

        if (dict.ContainsKey(ConfigId))
        {
            return dict[ConfigId];
        }
        else
        {
            Debug.LogError("配置表：NumericInfoConfigCategory 中不包含ID为：" + ConfigId + "的数据");
            return null;
        }
    }

    /// <summary>
    /// 获取全部值
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, NumericInfoCategory> GetAll()
    {
        if (dict.Count == 0)
        {
            Load();
        }

        return dict;
    }

    private void Load()
    {
        if (dict.Count != 0) return;

        else
        {
            var allInfo = ExcelLoadManager.Instance.dict;

            if (allInfo.ContainsKey("NumericInfoConfigCategory"))
            {
                // 获取全部值
                var unitInfoAll = allInfo["NumericInfoConfigCategory"];

                foreach (int key in unitInfoAll.Keys)
                {
                    NumericInfoCategory NumericInfoCategory = new();

                    NumericInfoCategory.ConfigId = int.Parse(unitInfoAll[key][0]);

                    NumericInfoCategory.ConfigString = unitInfoAll[key][1];

                    NumericInfoCategory.ConfigNumeric = float.Parse(unitInfoAll[key][2]);

                    NumericInfoCategory.Notes = unitInfoAll[key][3];

                    dict.Add(key, NumericInfoCategory);

                }
            }
            else
            {
                Debug.LogError("配置表：NumericInfoConfigCategory 没有加载");
                return;
            }
        }
    }

    public class NumericInfoCategory
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int ConfigId;

        /// <summary>
        /// 配置内容
        /// </summary>
        public string ConfigString;

        /// <summary>
        /// 配置数值
        /// </summary>
        public float ConfigNumeric;

        /// <summary>
        /// 备注
        /// </summary>
        public string Notes;
    }
}

