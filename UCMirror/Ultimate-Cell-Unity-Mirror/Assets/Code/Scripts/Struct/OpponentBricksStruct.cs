using Mirror;
using System.Collections.Generic;
using UnityEngine;

public struct OpponentBricksStruct : NetworkMessage
{
    public List<TetrisClass> tetrisClaass;

    public int fromplayer;

    public int sendPlayer;
}