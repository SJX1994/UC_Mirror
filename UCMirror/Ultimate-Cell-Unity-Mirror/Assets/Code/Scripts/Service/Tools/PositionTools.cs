using UnityEngine;

/// <summary>
/// 砖块工具类
/// </summary>
public static class PositionTools
{
    /// <summary>
    /// 计算砖块位置偏移量
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static Vector3 CountPosShifting(float posx, float posy) 
    {
        var returnVec =
                    new Vector3(
                        (float)-12.95 + posx * (float)1.5
                            , (float)0.6
                                , (float)-9.0 + posy * (float)1.5);

        return returnVec;
    }

    /// <summary>
    /// 计算兵线位置偏移量
    /// </summary>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <returns></returns>
    public static Vector3 CountFireLinePos(float posx, float posy)
    {
        var returnVec =
                    new Vector3(
                        (float)-15.0 + posx * (float)1.65
                            , (float)0.6
                                , (float)-9.0 + posy * (float)1.65);

        return returnVec;

    }

    /// <summary>
    /// 获取战场原点值
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetZeroPos() 
    {
        return new Vector3((float)-13.5, (float)0.6, (float)-8.5);
    }


    /// <summary>
    /// 获取左侧出兵地点
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetBattlefieldLeft()
    {
        return new Vector3(-15f, 0.6f, -3.5f);
    }

    /// <summary>
    /// 获取右侧出兵地点
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetBattlefieldRight()
    {
        return new Vector3(15f, 0.6f, -3.5f);
    }
}