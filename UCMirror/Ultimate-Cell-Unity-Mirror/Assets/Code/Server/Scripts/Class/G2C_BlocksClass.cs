
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2C_BlocksClass
{
    // 整体砖块ID
    public int BlocksIndexId;

    // 砖块品质
    public EventType.BlocksGrade BlocksGrade;

    // 砖块类型
    public EventType.TetrisType TetrisType;

    // 砖块详细信息
    public Dictionary<int, TetrisClass> TetrisInfo;

}

