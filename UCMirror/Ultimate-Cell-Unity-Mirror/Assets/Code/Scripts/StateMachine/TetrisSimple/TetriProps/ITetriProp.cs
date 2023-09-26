using UnityEngine;
using System.Collections.Generic;

public interface ITetriProp
{
    public BlocksCreator BlocksCreator {get;set;} // 全部砖块信息
    public bool MoveCollect{get;set;} // 通过移动收集道具
    bool Ray_TetriPairBlock(); // 向下检测双重标记
    void ResetRotation(); // 看向摄像机
    public bool Generate(); // 生成道具
    public void Collect(); // 收集道具
    
}