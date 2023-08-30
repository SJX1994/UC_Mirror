using System.Collections.Generic;
using UnityEngine;

public class UnitActionService : SingTon<UnitActionService>
{
    /// <summary>
    /// 角色死亡时的监听方法
    /// 
    ///     播放死亡信息
    ///     
    ///     存入本地数据表中
    ///     
    ///     销毁俄罗斯方块逻辑信息
    ///     
    ///     销毁细胞Unit
    ///     
    /// </summary>
    /// <param name="unitDieClass"></param>
    public static void UnitDieHandler(int UnitId)
    {
        var unitInfo = UnitInfoService.GetUnitBasicInfoByUnitId(UnitId);

        Debug.Log("！！死亡提示！！");

        Debug.Log(message: $"{unitInfo.UnitName} 已死亡.属于：{unitInfo.unitcamp.ToString()} 阵营");

        Debug.Log(UnitId + ": 死亡信息已记录");
    }
}
