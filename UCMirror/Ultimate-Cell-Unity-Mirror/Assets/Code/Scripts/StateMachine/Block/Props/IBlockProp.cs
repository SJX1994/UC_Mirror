using UnityEngine;
using System.Collections.Generic;

public interface IBlockProp
{
    public Vector2 PosId{get;set;} // 砖块位置
    public bool MoveCollect{get;set;} // 通过移动收集道具
    public BlockPropsState BlockPropsState{get;} // 砖块道具状态
    public BlockDisplay BlockDisplay{get;} // 砖块显示
    public void Collect(); // 收集接收
}