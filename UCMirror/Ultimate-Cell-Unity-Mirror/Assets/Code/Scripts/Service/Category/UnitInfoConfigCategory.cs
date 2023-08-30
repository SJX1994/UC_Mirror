using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoConfigCategory : SingTon<UnitInfoConfigCategory>
{
    private Dictionary<int, UnitInfoCategory> dict = new();

    /// <summary>
    /// 根据ID获取对应值
    /// </summary>
    /// <param name="ConfigId"></param>
    /// <returns></returns>
    public UnitInfoCategory GetOne(int ConfigId)
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
            Debug.LogError("配置表：UnitInfoConfigCategory 中不包含ID为：" + ConfigId + "的数据");
            return null;
        }
    }

    /// <summary>
    /// 获取全部值
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, UnitInfoCategory> GetAll() 
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

            if (allInfo.ContainsKey("UnitInfoConfigCategory"))
            {
                // 获取全部值
                var unitInfoAll = allInfo["UnitInfoConfigCategory"];

                foreach (int key in unitInfoAll.Keys)
                {
                    UnitInfoCategory unitInfo = new UnitInfoCategory();

                    unitInfo.ConfigID = int.Parse(unitInfoAll[key][0]);

                    unitInfo.Camp = unitInfoAll[key][1];

                    unitInfo.Name = unitInfoAll[key][2];

                    unitInfo.Health = float.Parse(unitInfoAll[key][3]);

                    unitInfo.Aggressivity = float.Parse(unitInfoAll[key][4]);

                    unitInfo.Attack_Speed = float.Parse(unitInfoAll[key][5]);

                    unitInfo.Patrol_Range = float.Parse(unitInfoAll[key][6]);

                    unitInfo.Attack_Range = float.Parse(unitInfoAll[key][7]);

                    unitInfo.Destory_Delay = float.Parse(unitInfoAll[key][8]);

                    unitInfo.Move_Speed = float.Parse(unitInfoAll[key][9]);

                    dict.Add(key, unitInfo);
                }
            }
            else
            {
                Debug.LogError("配置表：UnitInfoConfigCategory 没有加载");
                return;
            }
        }
    }

    public class UnitInfoCategory
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int ConfigID;

        /// <summary>
        /// 阵营
        /// </summary>
        public string Camp;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 血量
        /// </summary>
        public float Health;

        /// <summary>
        /// 攻击
        /// </summary>
        public float Aggressivity;

        /// <summary>
        /// 攻速
        /// </summary>
        public float Attack_Speed;

        /// <summary>
        /// 巡逻范围
        /// </summary>
        public float Patrol_Range;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float Attack_Range;

        /// <summary>
        /// 销毁延迟
        /// </summary>
        public float Destory_Delay;

        /// <summary>
        /// 移速
        /// </summary>
        public float Move_Speed;

    }
}
