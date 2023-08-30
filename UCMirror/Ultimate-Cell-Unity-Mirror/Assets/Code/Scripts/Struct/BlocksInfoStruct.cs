using Mirror;
using System;
using System.Collections.Generic;

public struct BlocksInfoStruct : NetworkMessage 
{
    // public Dictionary<int, TetrisClass> blocksInfo;

    public List<TetrisClass> blocksInfo;
}