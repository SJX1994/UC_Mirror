using System.Collections.Generic;
using UnityEngine;

public class G2C_TetrisService
{
    // 砖块ID管理
    public int sumIndex = 1000;

    /// <summary>
    /// 生成砖块方法
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public G2C_BlocksClass CreateTetris(G2C_BlocksClass blockClass, int random) 
    {
        Dictionary<int, TetrisClass> TetrisDic = new Dictionary<int, TetrisClass>();

        // ＴＯＤＯ　Load from Excel
        if (random == (int)EventType.TetrisType.O)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 0;

            tetrisClass1.posy = 1;

            tetrisClass1.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 1;

            tetrisClass2.posy = 0;

            tetrisClass2.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 1;

            tetrisClass3.posy = 1;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;

        }

        if (random == (int)EventType.TetrisType.I)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 1;

            tetrisClass.posy = 0;

            tetrisClass1.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 2;

            tetrisClass2.posy = 0;

            tetrisClass2.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 3;

            tetrisClass3.posy = 0;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;

        }

        if (random == (int)EventType.TetrisType.J)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 1;

            tetrisClass1.posy = 0;

            tetrisClass1.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 2;

            tetrisClass2.posy = 0;

            tetrisClass2.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 2;

            tetrisClass3.posy = 1;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;

        }

        if (random == (int)EventType.TetrisType.L)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 0;

            tetrisClass1.posy = 1;

            tetrisClass1.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 1;

            tetrisClass2.posy = 0;

            tetrisClass2.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 2;

            tetrisClass3.posy = 0;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;
        }

        if (random == (int)EventType.TetrisType.T)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 1;

            tetrisClass1.posy = 0;

            tetrisClass1.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 1;

            tetrisClass2.posy = 1;

            tetrisClass2.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 2;

            tetrisClass3.posy = 0;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;

        }

        if (random == (int)EventType.TetrisType.S)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 0;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 1;

            tetrisClass1.posy = 0;

            tetrisClass1.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 1;

            tetrisClass2.posy = 1;

            tetrisClass2.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 2;

            tetrisClass3.posy = 1;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;
        }

        if (random == (int)EventType.TetrisType.Z)
        {
            TetrisClass tetrisClass = new TetrisClass();

            tetrisClass.UnitIndexId = sumIndex;

            tetrisClass.posx = 0;

            tetrisClass.posy = 1;

            tetrisClass.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass);

            sumIndex++;

            TetrisClass tetrisClass1 = new TetrisClass();

            tetrisClass1.UnitIndexId = sumIndex;

            tetrisClass1.posx = 1;

            tetrisClass1.posy = 1;

            tetrisClass1.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass1);

            sumIndex++;

            TetrisClass tetrisClass2 = new TetrisClass();

            tetrisClass2.UnitIndexId = sumIndex;

            tetrisClass2.posx = 1;

            tetrisClass2.posy = 0;

            tetrisClass2.rotepoint = true;

            TetrisDic.Add(sumIndex, tetrisClass2);

            sumIndex++;

            TetrisClass tetrisClass3 = new TetrisClass();

            tetrisClass3.UnitIndexId = sumIndex;

            tetrisClass3.posx = 2;

            tetrisClass3.posy = 0;

            tetrisClass3.rotepoint = false;

            TetrisDic.Add(sumIndex, tetrisClass3);

            sumIndex++;
        }

        blockClass.TetrisInfo = TetrisDic;

        return blockClass;
    }

    /// <summary>
    /// 砖块整体计算方法
    /// </summary>
    /// <param name="_grid"></param>
    /// <returns></returns>
    public G2C_CountGridClass CountBlocks(string[,] _grid, Dictionary<int, TetrisClass> _AllTetrisDic, int _width, int _height)
    {
        // 新建计算栈
        Queue<TetrisClass> countStack = new();

        // 删除砖块
        Dictionary<int, TetrisClass> tetrisDelete = new Dictionary<int, TetrisClass>();

        // 新建Unit
        Dictionary<int, TetrisClass> tetrisCreate = new Dictionary<int, TetrisClass>();

        // 计算整体信息 竖行
        for (int i = 0; i < _width; i ++) 
        {
            countStack.Clear();

            for (int j = 0; j < _height; j++) 
            {
                if (_grid[i, j] != null)
                {
                    if (countStack.Count == 0)
                    {
                        if (!_AllTetrisDic.ContainsKey(int.Parse(_grid[i, j]))) continue;

                        countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[i, j])]);
                    }
                    else
                    {
                        var tetrisInfoLast = countStack.Peek();

                        var tetrisInfoFirst = _AllTetrisDic[int.Parse(_grid[i, j])];

                        // 颜色不同不合成
                        if (tetrisInfoLast.Color == tetrisInfoFirst.Color
                            // 等级不同不合成
                            && tetrisInfoLast.BlockLevel == tetrisInfoFirst.BlockLevel
                                // 等级超过三级不合成
                                && tetrisInfoLast.BlockLevel != 3
                                    // 英雄砖块不合成 
                                    && tetrisInfoLast.unitType != EventType.UnitType.Hero)
                        {
                            countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[i, j])]);
                        }
                        else
                        {
                            countStack.Clear();

                            countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[i, j])]);
                        }
                    }

                    if (countStack.Count == 3)
                    {
                        //Add
                        while (countStack.Count != 0)
                        {
                            var info = countStack.Dequeue();

                            tetrisDelete.Add(info.UnitIndexId, info);

                            _AllTetrisDic.Remove(info.UnitIndexId);

                            _grid[info.posx, info.posy] = null;

                            if (countStack.Count == 0)
                            {
                                if (info.BlockLevel != 3) info.BlockLevel++;

                                var addinfo = CreateAloneTetrisClass(info);

                                _AllTetrisDic.Add(addinfo.UnitIndexId, addinfo);

                                tetrisCreate.Add(addinfo.UnitIndexId, addinfo);

                                _grid[info.posx, info.posy] = addinfo.UnitIndexId + "";
                            }
                        }
                    }
                }
                else
                {
                    countStack.Clear();
                }
            }
        }

        countStack.Clear();

        // 计算整体信息 横行
        for (int i = 0; i < _height; i++)
        {
            countStack.Clear();

            for (int j = 0; j < _width; j++)
            {
                if (_grid[j, i] != null)
                {
                    if (countStack.Count == 0)
                    {
                        if (!_AllTetrisDic.ContainsKey(int.Parse(_grid[j, i]))) continue;

                        countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[j, i])]);
                    }
                    else
                    {
                        var tetrisInfoLast = countStack.Peek();

                        var tetrisInfoFirst = _AllTetrisDic[int.Parse(_grid[j, i])];

                        // 颜色不同不合成
                        if (tetrisInfoLast.Color == tetrisInfoFirst.Color
                            // 等级不同不合成
                            && tetrisInfoLast.BlockLevel == tetrisInfoFirst.BlockLevel
                                // 等级超过三级不合成
                                && tetrisInfoLast.BlockLevel != 3
                                    // 英雄砖块不合成 
                                    && tetrisInfoLast.unitType != EventType.UnitType.Hero)
                        {
                            countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[j, i])]);
                        }
                        else
                        {
                            countStack.Clear();

                            countStack.Enqueue(_AllTetrisDic[int.Parse(_grid[j, i])]);
                        }
                    }

                    if (countStack.Count == 3)
                    {
                        //Add
                        while (countStack.Count != 0)
                        {
                            var info = countStack.Dequeue();

                            tetrisDelete.Add(info.UnitIndexId, info);

                            _AllTetrisDic.Remove(info.UnitIndexId);

                            _grid[info.posx, info.posy] = null;

                            if (countStack.Count == 0)
                            {
                                if (info.BlockLevel != 3) info.BlockLevel++;

                                var addinfo = CreateAloneTetrisClass(info);

                                _AllTetrisDic.Add(addinfo.UnitIndexId, addinfo);

                                tetrisCreate.Add(addinfo.UnitIndexId, addinfo);

                                _grid[info.posx, info.posy] = addinfo.UnitIndexId + "";
                            }
                        }
                    }
                }
                else
                {
                    countStack.Clear();
                }
            }
        }

        G2C_CountGridClass countGrid = new();

        countGrid.tetrisUpdate = tetrisCreate;

        countGrid.tetrisDelete = tetrisDelete;

        countGrid._grid = _grid;

        countGrid._AllTetrisDic = _AllTetrisDic;

        return countGrid;
    }

    private TetrisClass CreateAloneTetrisClass(TetrisClass info) 
    {
        TetrisClass returnClass = new();

        returnClass.UnitIndexId = sumIndex;

        sumIndex++;

        returnClass.posx = info.posx;

        returnClass.posy = info.posy;

        returnClass.rotepoint= info.rotepoint;

        returnClass.Color = info.Color;

        returnClass.BlockLevel = info.BlockLevel;

        returnClass.BlocksGameObject = info.BlocksGameObject;

        returnClass.unitType = info.unitType;

        return returnClass;
    }


    /// <summary>
    /// 砖块形状变换
    /// </summary>
    public Dictionary<int, TetrisClass> TetrisTrans(Dictionary<int, TetrisClass> _TetrisDic)
    {
        var info = Random.Range(0, 3);

        for (int s = 0; s < info; s++)
        {
            var DifferenceVector = new Vector2(0, 0);

            // 计算归一化差值
            foreach (int i in _TetrisDic.Keys)
            {
                if (_TetrisDic[i].rotepoint)
                {
                    DifferenceVector = new Vector2(_TetrisDic[i].posx, _TetrisDic[i].posy) - new Vector2(2, 2);
                }
            }

            // 全局归一化
            foreach (int j in _TetrisDic.Keys)
            {
                _TetrisDic[j].posx -= (int)DifferenceVector.x;

                _TetrisDic[j].posy -= (int)DifferenceVector.y;
            }

            // 形状转换
            foreach (int k in _TetrisDic.Keys)
            {
                var posx = _TetrisDic[k].posx;

                var posy = _TetrisDic[k].posy;

                // 顺时针变形
                _TetrisDic[k].posx = posy;

                _TetrisDic[k].posy = 4 - posx;
            }

            // 归一化还原
            foreach (int l in _TetrisDic.Keys)
            {
                _TetrisDic[l].posx += (int)DifferenceVector.x;

                _TetrisDic[l].posy += (int)DifferenceVector.y;
            }
        }

        var ZeroVector = new Vector2();

        // 计算归一化差值
        foreach (int i in _TetrisDic.Keys)
        {
            if (_TetrisDic[i].rotepoint)
            {
                ZeroVector = new Vector2(_TetrisDic[i].posx, _TetrisDic[i].posy);
            }
        }

        // 全局归零
        foreach (int l in _TetrisDic.Keys)
        {
            _TetrisDic[l].posx -= (int)ZeroVector.x;

            _TetrisDic[l].posy -= (int)ZeroVector.y;
        }

        return _TetrisDic;

    }



    /// <summary>
    /// 砖块位置合法性判断方法
    /// </summary>
    /// <returns></returns>
    public bool ValidMove(string[,] _grid, Dictionary<int, TetrisClass> _TetrisDic, int _width, int _height)
    {
        // 获取children在碰撞模拟器中的坐标
        foreach (int i in _TetrisDic.Keys)
        {
            int roundedX = _TetrisDic[i].posx;

            int roundedY = _TetrisDic[i].posy;

            // 边界判断
            if (roundedX < 0 || roundedX >= _width || roundedY < 0 || roundedY >= _height)
            {
                return false;
            }

            // 砖块判断
            if (_grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 变形触发事件
    /// </summary>
    private void OnTransClick()
    {
        /*// 变形
        UpdateBlocksLogic(EventType.MoveType.Trans);

        Debug.Log("形状变换");

        if (!ValidMove())
        {

            UpdateBlocksLogic(EventType.MoveType.BackTrans);

            // TODO 智能化操作
            // TransMoveAndRoteUp(transform);
        }*/

    }

    /// <summary>
    /// 砖块逻辑位置更新方法
    /// </summary>
    /// <param name="infoType"></param>
    private void UpdateBlocksLogic(EventType.MoveType infoType)
    {
        /*switch (infoType)
        {
            case EventType.MoveType.Up:
                foreach (int i in _TetrisDic.Keys)
                {
                    var posInfo = _TetrisDic[i];

                    posInfo.posy += 1;
                }
                break;
            case EventType.MoveType.Down:
                foreach (int i in _TetrisDic.Keys)
                {
                    var posInfo = _TetrisDic[i];

                    posInfo.posy -= 1;
                }
                break;
            case EventType.MoveType.Move:
                foreach (int i in _TetrisDic.Keys)
                {
                    var posInfo = _TetrisDic[i];

                    posInfo.posx += 1;
                }
                break;
            case EventType.MoveType.Back:
                foreach (int i in _TetrisDic.Keys)
                {
                    var posInfo = _TetrisDic[i];

                    posInfo.posx -= 1;
                }
                break;
            case EventType.MoveType.Trans:
                TetrisTrans(EventType.MoveType.Trans);
                break;
            case EventType.MoveType.BackTrans:
                TetrisTrans(EventType.MoveType.BackTrans);
                break;
        }*/
    }

}
