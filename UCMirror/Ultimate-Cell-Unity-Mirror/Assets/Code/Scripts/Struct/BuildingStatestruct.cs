using Mirror;
using UnityEngine;

public struct BuildingStatestruct : NetworkMessage 
{
    public Vector2 BuildingPos;

    public bool FunctionHit;

    public bool FunctionExit;
}