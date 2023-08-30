using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局枚举类
/// </summary>
public class EventType
{
    /// <summary>
    /// Enum - Unit区分
    /// </summary>
    public enum UnitType 
    {
        Hero,
        Cell,
        Virus,
    }

    /// <summary>
    /// Enum - 角色阵营
    /// </summary>
    public enum UnitCamp
    {
        Cell,
        Virus,
    }


    /// <summary>
    /// Enum - 角色性别
    /// </summary>
    public enum Gender
    {
        Male,
        Famale,
    }

    public enum TetrisType
    {
        O = 0,
        I = 1,
        J = 2,
        L = 3,
        T = 4,
        S = 5,
        Z = 6,
    }

    public enum MoveType
    {
        Up,
        Down,
        Move,
        Back,
        Trans,
        BackTrans,
    }

    public enum UIControl
    {
        Up,
        Down,
        Trans,
        Acce,
        Moderate,

    }

    public enum UnitColor
    {
        red,
        green,
        blue,
        purple,
        yellow,
    }

    public enum IdeaType
    {
        Block,
        Skill,
    }

    public enum BlocksGrade
    {
        TopGrade,
        MiddleGrade,
        BottomGrade,
    }
}


