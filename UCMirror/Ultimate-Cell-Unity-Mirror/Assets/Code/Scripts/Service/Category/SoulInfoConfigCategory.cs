using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulInfoConfigCategory : SingTon<SoulInfoConfigCategory>
{
    private Dictionary<int, SoulInfoCategory> dict = new();

    /// <summary>
    /// 根据ID获取对应值
    /// </summary>
    /// <param name="ConfigId"></param>
    /// <returns></returns>
    public SoulInfoCategory GetOne(int ConfigId)
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
            Debug.LogError("配置表：SoulInfoConfigCategory 中不包含ID为：" + ConfigId + "的数据");
            return null;
        }
    }

    /// <summary>
    /// 获取全部值
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, SoulInfoCategory> GetAll()
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

            if (allInfo.ContainsKey("SoulInfoConfigCategory"))
            {
                // 获取全部值
                var unitInfoAll = allInfo["SoulInfoConfigCategory"];

                foreach (int key in unitInfoAll.Keys)
                {
                    SoulInfoCategory soulInfoCategory = new();

                    soulInfoCategory.ConfigId = int.Parse(unitInfoAll[key][0]);

                    soulInfoCategory.Name = unitInfoAll[key][1];

                    soulInfoCategory.Existence_Time = float.Parse(unitInfoAll[key][2]);

                    soulInfoCategory.Soul_Value = int.Parse(unitInfoAll[key][3]);

                    soulInfoCategory.Patrol_Radius = float.Parse(unitInfoAll[key][4]);

                    dict.Add(key, soulInfoCategory);

                }
            }
            else
            {
                Debug.LogError("配置表：SoulInfoConfigCategory 没有加载");
                return;
            }
        }
    }

    public class SoulInfoCategory
    {
        // 配置ID
        public int ConfigId;

        // 灵魂名称
        public string Name;

        // 存在时间
        public float Existence_Time;

        // 灵魂价值
        public int Soul_Value;

        // 巡游半径
        public float Patrol_Radius;

    }
}

