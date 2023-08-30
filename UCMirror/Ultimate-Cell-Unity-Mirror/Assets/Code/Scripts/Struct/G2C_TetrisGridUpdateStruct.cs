using Mirror;
using System.Collections.Generic;

public struct G2C_TetrisGridUpdateStruct : NetworkMessage
{
    public List<TetrisGridUpdateClass> playerGrid;
}