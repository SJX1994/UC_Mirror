using Mirror;

public struct PuppetUnitStateStruct : NetworkMessage
{
    public int UnitId;

    // 0 新建
    // 1 销毁
    public int UnitState;
}