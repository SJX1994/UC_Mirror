using Mirror;
using UnityEngine;

public struct G2C_SendUnitPos : NetworkMessage 
{
    public int UnitId;

    public Vector3 Pos;
}