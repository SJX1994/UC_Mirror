
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisClass
{
    // 砖块ID
    public int UnitIndexId;

    // 方块横坐标
    public int posx;

    // 方块纵坐标
    public int posy;

    // 是否为中心旋转点
    public bool rotepoint = false;

    // Unit颜色
    //  黄色：e89d26
    //，红色：ff0e0e
    //，蓝色：272fd5
    //，绿色：3bd218
    //，天蓝色：b0ffde
    //，紫色：da0eff
    //，橙色：ff900e
    public EventType.UnitColor Color;

    // 砖块等级
    public int BlockLevel;

    // 砖块GameObj
    public GameObject BlocksGameObject;

    // 砖块类型
    public EventType.UnitType unitType = EventType.UnitType.Cell;

    // 砖块整体ID
    public int TetrisGroupId;

    // 是否在交战区
    public bool IsBattlefieldBlock;

    // 是否在建筑编组状态上
    public bool IsBuildingBlock;

}

