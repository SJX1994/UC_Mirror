using System.Collections.Generic;
using UnityEngine;

public class UnitActionManager : SingTon<UnitActionManager>
{
    /// <summary>
    /// 角色死亡时的监听方法
    ///     播放死亡信息
    ///     存入本地数据表中
    ///     销毁俄罗斯方块逻辑信息
    ///     销毁细胞Unit
    /// </summary>
    /// <param name="unitDieClass"></param>
    public static void UnitDieHandler(int UnitId)
    {
        UnitActionService.UnitDieHandler(UnitId);
    }
}
