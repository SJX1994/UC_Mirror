using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetrisBlocksManager : MonoBehaviour
{
    // 将配置高度导入方块中
    public int _height;

    // 将配置宽度导入方块中
    public int _width;

    // 碰撞检测模拟器
    private static string[,] _grid;

    // 砖块信息暂存缓冲区
    private Dictionary<int, TetrisClass> _TetrisDic = new();

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    // 砖块整体信息管理
    public Dictionary<int, TetrisClass> _AllTetrisDic = new();

    // 英雄生成暂存
    private string heroNameInfo;

    // 减少发送 节约内存
    private Vector2 tetrisPos;

    // 是否为新建砖块
    private bool CreateInfo = false;

    public int UnitInfoCount = 0;

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        // 初始化生成战场检测
        _height = BattlefieldClass.height;

        _width = BattlefieldClass.width;

        _grid = BattlefieldClass.grid;

        broadcastClass.BlocksReady += BlocksReady;

        broadcastClass.OnBlocksMove += OnBlocksMove;

        broadcastClass.BlocksMoveEnd += BlocksMoveEnd;

        broadcastClass.OnBlocksCancellation += OnBlocksCancellation;

        broadcastClass.OnHeroUICancel += BlocksCancel;

        broadcastClass.OpponBlocks += CreateOpponBlocks;

        broadcastClass.G2C_TetrisUpdateClass += TetrisUpdateFunc;

        broadcastClass.G2C_GridInfoUpdate += G2C_GridInfoUpdate;
    }

    private bool cencel = false;

    /// <summary>
    /// 碰撞盒更新事件 -> 客户端/服务器
    /// </summary>
    /// <param name=""></param>
    private void G2C_GridInfoUpdate(string[,] _newGrid)
    {
        _grid = _newGrid;

        Debug.Log("碰撞盒更新---> TetrisBlocks");
    }

    /// <summary>
    /// 砖块更新事件 -> 服务器
    /// </summary>
    /// <param name="info"></param>
    private void TetrisUpdateFunc(G2C_TetrisUpdateClass info)
    {
        CommunicationManager.BlockUpdate(info.UpdateClass, info.state);
    }


    /// <summary>
    /// 新建敌对势力砖块
    /// </summary>
    /// <param name="info"></param>
    private void CreateOpponBlocks(List<TetrisClass> info)
    {
        Dictionary<int, TetrisClass> infoClass = new Dictionary<int, TetrisClass>();

        foreach (TetrisClass t in info)
        {
            infoClass.Add(t.UnitIndexId, t);
        }

        // 生成通信类
        List<UnitInfoClass> unitInfo = OnCreateUnitInfo(infoClass, true);

        // 发送通信类
        onTransPos(unitInfo, true);

    }

    public void OnCencelBlock()
    {
        AudioSystemManager.Instance.PlaySound("Click_Once");

        FindObjectOfType<GlobalControlManager>().OnPause();

        cencel = true;
    }

    private void Update()
    {
    }


    /// <summary>
    /// 砖块放入暂存区
    /// </summary>
    private void BlocksReady(Dictionary<int, TetrisClass> info, string heroName)
    {
        // 砖块信息更新
        _TetrisDic = info;

        // 新建信息更新
        CreateInfo = true;

        // 判断是否为英雄砖块
        heroNameInfo = heroName;
    }

    /// <summary>
    /// 砖块移动触发事件
    /// </summary>
    /// <param name="info"></param>
    private void OnBlocksMove(Vector3 info)
    {
        // 将实际坐标转换为逻辑坐标
        var posx = (int)((info.x + 13.75) / 1.5 + 0.5);

        var posy = (int)((info.z + 8.6) / 1.5 + 0.5);

        var infoPos = new Vector2(posx, posy);

        // 判断位置是否发生了改变
        if (tetrisPos != infoPos)
        {
            // 赋值供下一次使用
            tetrisPos = infoPos;

            var DifferenceVector = new Vector2(0, 0);

            // 计算归一化差值
            foreach (int i in _TetrisDic.Keys)
            {
                if (_TetrisDic[i].rotepoint)
                {
                    DifferenceVector = new Vector2(_TetrisDic[i].posx, _TetrisDic[i].posy) - tetrisPos;
                }
            }

            // 全局归一化差值计算
            foreach (int j in _TetrisDic.Keys)
            {
                _TetrisDic[j].posx -= (int)DifferenceVector.x;

                _TetrisDic[j].posy -= (int)DifferenceVector.y;
            }

            // 砖块合法性判断
            bool ValidMove = TetrisService.Instance.ValidMove(_grid, _TetrisDic, _width, _height);

            if (!ValidMove)
            {
                foreach (int h in _TetrisDic.Keys)
                {
                    _TetrisDic[h].posx += (int)DifferenceVector.x;

                    _TetrisDic[h].posy += (int)DifferenceVector.y;
                }

                return;
            }

            // 生成通信类
            List<UnitInfoClass> unitInfo = OnCreateUnitInfo(_TetrisDic, CreateInfo);

            // 发送通信类
            onTransPos(unitInfo, CreateInfo);

            CreateInfo = false;
        }
    }

    /// <summary>
    /// 砖块停止移动触发事件
    /// </summary>
    private void BlocksMoveEnd(int info)
    {
        // 检测如果为英雄则不生成士兵
        var heroCheck = _TetrisDic.First().Value.unitType;

        if (heroCheck == EventType.UnitType.Hero)
        {
            HeroInfoClass HeroInfo = FindObjectOfType<HeroManager>().CreateHero(_TetrisDic, heroNameInfo);

            _TetrisDic = HeroInfo.HeroInfo;

            // 发送英雄生成信息
            CommunicationManager.OnHeroCreateResponse(HeroInfo);

            // 生成通信类
            List<UnitInfoClass> unitInfo = OnCreateUnitInfo(_TetrisDic, false);

            // 发送通信类
            onTransPos(unitInfo, false);
        }
        else
        {
            // 本地新建砖块与Unit发送方法
            CommunicationManager.LocalTetrisCreate(_TetrisDic);

            // 向服务器发送砖块信息
            CommunicationManager.SendBlocksInfo(_TetrisDic);

            /*// 计算Unit参数
            var UnitInfo = BlocksReadyUnitCreate(_TetrisDic);

            // 创建Unit
            this.CreateUnit(UnitInfo);*/
        }

        // 加入碰撞检测缓存区
        AddToGrid(_TetrisDic, _grid);

        // 加入整体信息管理
        AddToTetrisInfo(_TetrisDic);

        CommunicationManager.BlockCreateDone(0);
    }

    /// <summary>
    /// 砖块取消移动事件
    /// </summary>
    /// <param name="info"></param>
    private void BlocksCancel(string info)
    {
        // 生成通信类
        List<UnitInfoClass> unitInfo = OnCreateUnitInfo(_TetrisDic, false);

        BattleInfoSynchronization(unitInfo, true);

    }

    /// <summary>
    /// 从全部砖块信息中找出不包含英雄信息的砖块
    /// </summary>
    /// <param name="_AllTetrisDic"></param>
    /// <returns></returns>
    private Dictionary<int, TetrisClass> GetCellUnitFromList(Dictionary<int, TetrisClass> _AllTetrisDic)
    {
        Dictionary<int, TetrisClass> info = new();

        foreach (int key in _AllTetrisDic.Keys)
        {
            if (_AllTetrisDic[key].unitType != EventType.UnitType.Hero)
            {
                info.Add(key, _AllTetrisDic[key]);
            }
        }

        return info;
    }

    /// <summary>
    /// 全部细胞移动
    /// </summary>
    private void AllCellsMove()
    {
        // 获取细胞砖块
        var cellUnits = GetCellUnitFromList(_AllTetrisDic);

        // 生成通信类
        var unitInfo = OnCreateUnitInfo(cellUnits, false);

        UnitInfoCount = unitInfo.Count;

        // 更新细胞信息
        CommunicationManager.UnitInfoAllUpdate(unitInfo);
    }

    /// <summary>                                                                           
    /// 点击更新Unit信息
    /// </summary>
    public void CreateUnit(List<UnitInfoClass> info)
    {
        AudioSystemManager.Instance.PlaySound("Cell_Update1");

        AudioSystemManager.Instance.PlaySound("Cell_Update2");

        CommunicationManager.OnCellUnitCreate(info);

        Invoke(nameof(AllCellsMove), (float)0.5);
    }

    private CountGridClass OnCreateUnitButtonClick()
    {
        // 砖块合成计算
        var returnInfo = TetrisService.Instance.CountBlocks(_grid, _AllTetrisDic, _width, _height);

        _grid = returnInfo._grid;

        _AllTetrisDic = returnInfo._AllTetrisDic;

        return returnInfo;
    }

    /// <summary>
    /// 砖块取消移动事件
    /// </summary>
    /// <param name="info"></param>
    private void OnBlocksCancellation(int info)
    {
        // 将砖块现在位置发送至显示层

        // 缓存区清零
    }


    //砖块整体信息更新方法
    private void AddToTetrisInfo(Dictionary<int, TetrisClass> _TetrisDic)
    {
        foreach (int info in _TetrisDic.Keys)
        {
            if (!_AllTetrisDic.ContainsKey(info))
            {
                _AllTetrisDic.Add(info, _TetrisDic[info]);
            }
            else
            {
                Debug.LogError("Check Logic Error");
            }
        }
    }

    // 碰撞模拟器更新方法
    private void AddToGrid(Dictionary<int, TetrisClass> _TetrisDic, string[,] _grid)
    {
        foreach (int keys in _TetrisDic.Keys)
        {
            int roundedX = _TetrisDic[keys].posx;

            int roundedY = _TetrisDic[keys].posy;

            if (_grid[roundedX, roundedY] == null)
            {
                _grid[roundedX, roundedY] = _TetrisDic[keys].UnitIndexId + "";
            }
        }
    }

    /// <summary>
    /// 砖块准备UnitList
    /// </summary>
    /// <param name="info"></param>
    public List<UnitInfoClass> BlocksReadyUnitCreate(Dictionary<int, TetrisClass> info)
    {
        List<UnitInfoClass> returnList = new();

        foreach (int key in info.Keys)
        {
            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = info[key].UnitIndexId;

            unitInfoClass.UnitPos = new Vector2(info[key].posx, info[key].posy);

            unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);

            unitInfoClass.color = info[key].Color;

            unitInfoClass.CreateUnit = true;

            returnList.Add(unitInfoClass);
        }

        return returnList;
    }

    /// <summary>
    /// 将unit dic转化为unit list的方法
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public List<UnitInfoClass> OnCreateUnitInfo(Dictionary<int, TetrisClass> info, bool CreateInfo)
    {
        List<UnitInfoClass> unitInfoClassList = new();

        foreach (int key in info.Keys)
        {
            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = info[key].UnitIndexId;

            unitInfoClass.UnitPos = new Vector2(info[key].posx, info[key].posy);

            unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);

            if (CreateInfo)
            {
                unitInfoClass.UnitPosUse = PositionTools.GetZeroPos();
            }

            unitInfoClass.color = info[key].Color;

            unitInfoClass.CreateUnit = CreateInfo;

            unitInfoClass.UnitLevel = info[key].BlockLevel;

            unitInfoClassList.Add(unitInfoClass);

        }

        return unitInfoClassList;

    }

    /// <summary>
    /// 发送砖块位置信息方法
    /// </summary>
    /// <param name="trans"></param>
    public void onTransPos(List<UnitInfoClass> unitInfoClassList, bool CreateInfo)
    {
        if (CreateInfo)
        {
            CommunicationManager.TetrisInfoCreate(unitInfoClassList);
        }
        else
        {
            CommunicationManager.TetrisInfoUpdate(unitInfoClassList);
        }
    }

    /// <summary>
    /// 向左破坏性移动
    /// </summary>
    public void RowLeft()
    {
        //List<UnitInfoClass> deleteClass = new List<UnitInfoClass>();

        // 生成新的碰撞模拟器
        string[,] Grid_Change = new string[_width - 1, _height];

        //List<UnitInfoClass> moveClass = new List<UnitInfoClass>();

        for (int i = 0; i < _width - 1; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_grid[i, j] != null)
                {
                    //deleteClass.Add(CreateTetrisClass(_grid[i, j], i, j));

                    // Grid_Change[i - 1, j] = _grid[i, j];

                    Grid_Change[i, j] = _grid[i, j];

                    //moveClass.Add(CreateTetrisClass(_grid[i, j], i - 1, j));

                    //_AllTetrisDic[int.Parse(_grid[i, j])].posx -= 1;
                }
            }
        }

        _grid = Grid_Change;

        _width--;

        BattlefieldClass.width--;

        //this.BattleInfoSynchronization(deleteClass, true);

        //this.BattleInfoSynchronization(moveClass, false);

        //AllCellsMove();
    }

    /// <summary>
    /// 向右整体移动
    /// </summary>
    public void RowRight()
    {
        List<UnitInfoClass> deleteClass = new List<UnitInfoClass>();

        // 生成新的碰撞模拟器
        string[,] Grid_Change = new string[_width + 1, _height];

        List<UnitInfoClass> moveClass = new List<UnitInfoClass>();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_grid[i, j] != null)
                {
                    deleteClass.Add(CreateTetrisClass(_grid[i, j], i, j));

                    Grid_Change[i + 1, j] = _grid[i, j];

                    moveClass.Add(CreateTetrisClass(_grid[i, j], i + 1, j));

                    _AllTetrisDic[int.Parse(_grid[i, j])].posx += 1;
                }
            }
        }

        _grid = Grid_Change;

        _width++;

        BattlefieldClass.width++;

        this.BattleInfoSynchronization(deleteClass, true);

        this.BattleInfoSynchronization(moveClass, false);

        AllCellsMove();

        if (_width == 15)
        {
            CommunicationManager.CreateBoss(1);

        }
    }

    /// <summary>
    /// 将砖块显示发送至显示层
    /// </summary>
    /// <param name="infoList"></param>
    /// <param name="destory"></param>
    private void BattleInfoSynchronization(List<UnitInfoClass> infoList, bool destory)
    {
        if (destory)
        {
            CommunicationManager.BlockUpdate(infoList, true);
        }
        else
        {
            CommunicationManager.BlockUpdate(infoList, false);
        }
    }

    // 删除单个砖块
    public void DeleteAlone(int key)
    {
        var info = _AllTetrisDic[key];

        List<UnitInfoClass> deleteClass = new List<UnitInfoClass>();

        deleteClass.Add(CreateTetrisClass(info.UnitIndexId + "", info.posx, info.posy));

        _AllTetrisDic.Remove(info.UnitIndexId);

        _grid[(int)info.posx, (int)info.posy] = null;

        this.BattleInfoSynchronization(deleteClass, true);
    }

    /// <summary>
    /// 英雄销毁事件
    /// </summary>
    /// <param name="key"></param>
    public void DeleteHero(int key)
    {
        var allHeroInfo = FindObjectOfType<HeroManager>().HeroControl;

        if (allHeroInfo.ContainsKey(key))
        {
            var tetris = allHeroInfo[key].HeroInfo;

            List<UnitInfoClass> deleteClass = new List<UnitInfoClass>();

            foreach (int indexId in tetris.Keys)
            {
                var info = _AllTetrisDic[indexId];

                deleteClass.Add(CreateTetrisClass(info.UnitIndexId + "", info.posx, info.posy));

                _AllTetrisDic.Remove(indexId);

                _grid[(int)info.posx, (int)info.posy] = null;

            }

            this.BattleInfoSynchronization(deleteClass, true);

            allHeroInfo.Remove(key);
        }
    }

    private UnitInfoClass CreateTetrisClass(string id, int x, int y)
    {
        UnitInfoClass tetrisClass = new UnitInfoClass();

        tetrisClass.UnitIndexId = int.Parse(id);

        tetrisClass.UnitPos = new Vector2(x, y);

        tetrisClass.UnitPosUse =
                    new Vector3(
                        (float)-14.5 + x * (float)1.5
                            , (float)0.6
                                , (float)-8.5 + y * (float)1.5);

        tetrisClass.CreateUnit = false;

        tetrisClass.color = _AllTetrisDic[int.Parse(id)].Color;

        return tetrisClass;
    }
}

