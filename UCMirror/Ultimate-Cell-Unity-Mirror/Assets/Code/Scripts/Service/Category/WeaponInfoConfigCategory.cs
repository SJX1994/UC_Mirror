using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfoConfigCategory : SingTon<WeaponInfoConfigCategory>
{
    private Dictionary<int, WeaponInfoCategory> dict = new();

    /// <summary>
    /// 根据ID获取对应值
    /// </summary>
    /// <param name="ConfigId"></param>
    /// <returns></returns>
    public WeaponInfoCategory GetOne(int ConfigId)
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
            Debug.LogError("配置表：WeaponInfoConfigCategory 中不包含ID为：" + ConfigId + "的数据");
            return null;
        }
    }

    /// <summary>
    /// 获取全部值
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, WeaponInfoCategory> GetAll()
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

            if (allInfo.ContainsKey("WeaponInfoConfigCategory"))
            {
                // 获取全部值
                var unitInfoAll = allInfo["WeaponInfoConfigCategory"];

                foreach (int key in unitInfoAll.Keys)
                {
                    WeaponInfoCategory weaponInfoCategory = new();

                    weaponInfoCategory.ConfigId = int.Parse(unitInfoAll[key][0]);

                    weaponInfoCategory.Name = unitInfoAll[key][1];

                    weaponInfoCategory.Level = int.Parse(unitInfoAll[key][2]);

                    weaponInfoCategory.Add_Attack = float.Parse(unitInfoAll[key][3]);

                    weaponInfoCategory.Add_Health = float.Parse(unitInfoAll[key][4]);

                    weaponInfoCategory.Add_AttackSpeed = float.Parse(unitInfoAll[key][5]);

                    weaponInfoCategory.Add_PatrolRange = float.Parse(unitInfoAll[key][6]);

                    weaponInfoCategory.Add_AttackRange = float.Parse(unitInfoAll[key][7]);

                    weaponInfoCategory.Add_MoveSpeed = float.Parse(unitInfoAll[key][8]);

                    weaponInfoCategory.Configuration_Location = int.Parse(unitInfoAll[key][9]);

                    weaponInfoCategory.Weapon_Style = unitInfoAll[key][10];

                    dict.Add(key, weaponInfoCategory);
                }
            }
            else
            {
                Debug.LogError("配置表：WeaponInfoConfigCategory 没有加载");
                return;
            }
        }
    }

    public class WeaponInfoCategory
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int ConfigId;

        /// <summary>
        /// 武器名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 武器等级
        /// </summary>
        public int Level;

        /// <summary>
        /// 增加攻击
        /// </summary>
        public float Add_Attack;

        /// <summary>
        /// 增加血量
        /// </summary>
        public float Add_Health;

        /// <summary>
        /// 增加攻速
        /// </summary>
        public float Add_AttackSpeed;

        /// <summary>
        /// 增加巡逻范围
        /// </summary>
        public float Add_PatrolRange;

        /// <summary>
        /// 增加攻击范围
        /// </summary>
        public float Add_AttackRange;

        /// <summary>
        /// 增加移速
        /// </summary>
        public float Add_MoveSpeed;

        /// <summary>
        /// 武器配置位置
        /// </summary>
        public int Configuration_Location;

        /// <summary>
        /// 武器样式
        /// </summary>
        public string Weapon_Style;

    }
}

