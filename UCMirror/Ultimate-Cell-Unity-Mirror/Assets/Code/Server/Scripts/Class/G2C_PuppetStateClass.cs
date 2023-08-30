using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class G2C_PuppetStateClass : MonoBehaviour
{
    public List<NetworkConnection> players;

    public int UnitId;

    // 0 新建
    // 1 销毁
    public int UnitState;

}