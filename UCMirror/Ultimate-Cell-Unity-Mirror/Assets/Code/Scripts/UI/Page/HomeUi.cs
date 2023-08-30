using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUi : MonoBehaviour
{
    private void Start()
    {
        ExcelLoadManager.Instance.Load();

        //获取首页的所有数据
        List<UnitViewClass> _beConfigHerosInfo = UnitInfoManager.GetBeConfigUnit();

        //将所有数据汇总成Object[]
        object[] data = new object[] { _beConfigHerosInfo };

        // 刷新页面
        UIManager.Instance.RefreshUI();

        //这里可能要Loading页面
        //将Data数据传入首页
        UIManager.Instance.OpenUI(UIType.HomePage2D, data);

        //将玩家信息和背景故事传入首页
        UIManager.Instance.OpenUI(UIType.HomePage3D, null);
    }
}
