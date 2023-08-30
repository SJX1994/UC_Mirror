
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2C_CountGridClass
{
    // 删除砖块
    public Dictionary<int, TetrisClass> tetrisDelete;

    // 新建Unit
    public Dictionary<int, TetrisClass> tetrisUpdate;

    // 整体信息偏移
    public string[,] _grid;

    public Dictionary<int, TetrisClass> _AllTetrisDic;

}

