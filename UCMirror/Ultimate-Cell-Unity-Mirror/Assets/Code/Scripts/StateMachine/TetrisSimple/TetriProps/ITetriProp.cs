using UnityEngine;
using System.Collections.Generic;

public interface ITetriProp
{
    public BlocksCreator_Main BlocksCreator {get;set;} // 全部砖块信息
    public GameObject Icon{get;} // 道具图标
    public struct PropTimer // 道具计时器
    {
        public float currentTime;
        public float totalTime;
        public bool isTimerRunning;
    }
    public bool Locked{get;set;} // 道具锁定状态
    public bool MoveCollect{get;set;} // 通过移动收集道具
    bool Ray_TetriPairBlock(); // 向下检测双重标记
    void ResetRotation(); // 看向摄像机
    public bool Generate_ForPlayer(); // 生成道具
    public void Collect(); // 收集道具
    
}