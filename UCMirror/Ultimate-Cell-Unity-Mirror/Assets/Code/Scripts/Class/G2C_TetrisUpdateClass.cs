using System.Collections.Generic;
using UnityEngine;

public class G2C_TetrisUpdateClass : MonoBehaviour
{
    // 需要更新的砖块
    public List<UnitInfoClass> UpdateClass;

    // 更新方式
    // true update
    // false delete
    public bool state;
}