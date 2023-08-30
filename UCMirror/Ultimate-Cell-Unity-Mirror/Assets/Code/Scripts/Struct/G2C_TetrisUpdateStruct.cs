using Mirror;
using System.Collections.Generic;

public struct G2C_TetrisUpdateStruct : NetworkMessage 
{
    // 需要更新的砖块
    public List<UnitInfoClass> UpdateClass;

    // 更新方式
    // true update
    // false delete
    public bool state;
}