using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G2C_TetrisBlocksManager : MonoBehaviour 
{
    // 最上级砖块生成概率
    [Header("最上级砖块生成概率")]
    public int TopGrade = 1;

    // 中级砖块生成概率
    [Header("中级砖块生成概率")]
    public int MiddleGrade = 2;

    // 低级砖块生成概率
    [Header("基础砖块生成概率")]
    public int BottomGrade = 7;

    // 将配置高度导入方块中
    private static int _height = 10;

    // 将配置宽度导入方块中
    private static int _width = 20;

    // 碰撞检测模拟器
    private string[,] _grid;

    // 砖块整体ID管理
    private int BlockIndex = 2000;

    // 砖块整体信息管理
    private Dictionary<int, TetrisClass> _AllTetrisDic = new();

    // 砖块整体组ID存储区
    private Dictionary<int, List<int>> GroupIdDic = new Dictionary<int, List<int>>();

    // 玩家一建筑当前位置
    private Vector2 BuildingPosPlayerOne = new Vector2(8f, 4f);

    // 玩家一当前建筑状态
    private int playerOneBuildingState = 2;

    // 玩家二建筑当前位置
    private Vector2 BuildingPosPlayerTwo = new Vector2(11f, 4f);

    // 玩家二当前建筑状态
    private int playerTwoBuildingState = 2;

    // 通信管理器物体
    private GameObject sceneLoader;

    // Unit管理类
    private G2C_UnitInfoManager unitManager;

    // 房间类
    private G2C_PlayerRoom playerRoomManager;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    public CommunicationInteractionManager CommunicationManager;

    private void Start()
    {
        // 暂时获取方式
        if (GameObject.Find("LanNetWorkManager") == null)
        {
            return;
        }
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        _grid = new string[_width, _height];

        unitManager = this.gameObject.GetComponent<G2C_UnitInfoManager>();

        playerRoomManager = this.gameObject.GetComponent<G2C_PlayerRoom>();

        // 砖块前进事件 3S触发一次
        InvokeRepeating(nameof(BroundForward), 5, 5);

        // 五分钟倒计时

        // 砖块三消合成计算
    }

    private void Update()
    {
    }

    /// <summary>
    /// 建筑变更事件
    /// </summary>
    /// <param name="info"></param>
    public void GetBuildingChangeState(BuildingStateClass info) 
    {
        // 建筑变更位置
        var BuildingPos = new Vector2(info.BuildingPosX, info.BuildingPosY);

        // 建筑变更状态
        var FunctionHitState = info.FunctionHitState;

        // 玩家区分/0玩家一/1玩家二
        var player = info.conn.connectionId == playerRoomManager.playerOne.connectionId ? 0 : 1;

        // 如果是玩家一的情况
        if (player == 0)
        {
            // 设置建筑当前状态
            playerOneBuildingState = FunctionHitState == 0 ? playerOneBuildingState : FunctionHitState;

            // 建筑位置发生了改变
            if (BuildingPos != BuildingPosPlayerOne && BuildingPos != new Vector2(-1, -1))
            {
                // 判断是否有编组的请况
                // 有编组的情况 建筑周围砖块与建筑一起移动
                if (playerOneBuildingState == 1)
                {
                    // 获取建筑周围砖块
                    var goupIdList = this.GetAroundBlocks((int)BuildingPosPlayerOne.x, (int)BuildingPosPlayerOne.y);

                    // 建筑移动事件
                    this.BuildingMove(BuildingPosPlayerOne, BuildingPos, goupIdList, player);

                    BuildingPosPlayerOne = BuildingPos;

                    Debug.Log("玩家一的建筑编组移动");
                }
                // 无编组的情况 建筑单独移动
                else
                {
                    BuildingPosPlayerOne = BuildingPos;

                    Debug.Log("玩家一建筑发生了移动：" + BuildingPosPlayerOne);
                }
            }
        }
        // 如果是玩家二的情况
        else
        {
            // 设置建筑当前状态
            playerTwoBuildingState = FunctionHitState == 0 ? playerTwoBuildingState : FunctionHitState;

            // 建筑位置发生了改变
            if (BuildingPos != BuildingPosPlayerTwo && BuildingPos != new Vector2(-1, -1))
            {

                // 设置建筑位置镜像翻转
                BuildingPos = new Vector2(19 - BuildingPos.x, BuildingPos.y);

                // 判断是否有编组的请况
                // 有编组的情况 建筑周围砖块与建筑一起移动
                if (playerTwoBuildingState == 1)
                {
                    var goupIdList = this.GetAroundBlocks((int)BuildingPosPlayerTwo.x, (int)BuildingPosPlayerTwo.y);

                    // 建筑移动事件
                    this.BuildingMove(BuildingPosPlayerTwo, BuildingPos, goupIdList, player);

                    BuildingPosPlayerTwo = BuildingPos;

                    Debug.Log("玩家二的建筑编组移动");
                }
                // 无编组的情况 建筑单独移动
                else
                {
                    BuildingPosPlayerTwo = BuildingPos;

                    Debug.Log("玩家二建筑发生了移动：" + BuildingPosPlayerTwo);
                }
            }
        }
    }

    /// <summary>
    /// 本地建筑变更事件
    /// </summary>
    /// <param name="info"></param>
    public void GetBuildingChangeLocal(BuildingStateClass info)
    {
        // 建筑变更位置
        var BuildingPos = new Vector2(info.BuildingPosX, info.BuildingPosY);

        // 建筑变更状态
        var FunctionHitState = info.FunctionHitState;

        // 设置建筑当前状态
        playerOneBuildingState = FunctionHitState == 0 ? playerOneBuildingState : FunctionHitState;

        // 建筑位置发生了改变
        if (BuildingPos != BuildingPosPlayerOne && BuildingPos != new Vector2(-1, -1))
        {
            // 判断是否有编组的请况
            // 有编组的情况 建筑周围砖块与建筑一起移动
            if (playerOneBuildingState == 1)
            {
                // 获取建筑周围砖块
                var goupIdList = this.GetAroundBlocks((int)BuildingPosPlayerOne.x, (int)BuildingPosPlayerOne.y);

                // 建筑移动事件
                this.BuildingMove(BuildingPosPlayerOne, BuildingPos, goupIdList, 0);

                BuildingPosPlayerOne = BuildingPos;

                Debug.Log("玩家一的建筑编组移动");
            }
            // 无编组的情况 建筑单独移动
            else
            {
                BuildingPosPlayerOne = BuildingPos;

                Debug.Log("玩家一建筑发生了移动：" + BuildingPosPlayerOne);
            }
        }

    }

    /// <summary>
    /// 获取建筑周围砖块
    /// </summary>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <returns></returns>
    private List<int> GetAroundBlocks(int posx, int posy)
    {
        List<int> returnList = new List<int>();

        if (posx < 0 || posy < 0) return returnList;

        List<int> Blocks = new List<int>();

        if (_grid[posx + 1, posy] != null) 
        {
            Blocks.Add(int.Parse(_grid[posx + 1, posy]));
        }

        if (_grid[posx - 1, posy] != null)
        {
            Blocks.Add(int.Parse(_grid[posx - 1, posy]));
        }

        if (_grid[posx, posy + 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx, posy + 1]));
        }

        if (_grid[posx, posy - 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx, posy - 1]));
        }

        if (_grid[posx + 1, posy + 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx + 1, posy + 1]));
        }

        if (_grid[posx - 1, posy - 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx - 1, posy - 1]));
        }

        if (_grid[posx + 1, posy - 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx + 1, posy - 1]));
        }

        if (_grid[posx - 1, posy + 1] != null)
        {
            Blocks.Add(int.Parse(_grid[posx - 1, posy + 1]));
        }

        foreach (int blocksId in Blocks) 
        {
            if (_AllTetrisDic.ContainsKey(blocksId)) 
            {
                var groupsId = _AllTetrisDic[blocksId].TetrisGroupId;

                if (!returnList.Contains(groupsId)) 
                {
                    returnList.Add(groupsId);
                }
            }
        }

        return returnList;
    }
    
    /// <summary>
    /// 建筑移动事件
    /// </summary>
    /// <param name="BuildingPosBefore"></param>
    /// <param name="BuildingPosAfter"></param>
    /// <param name="GroupIds"></param>
    /// <param name="player"></param>
    private void BuildingMove(Vector2 BuildingPosBefore, Vector2 BuildingPosAfter, List<int> GroupIds, int player) 
    {
        // 计算统一性差值
        var DifferenceVector = BuildingPosAfter - BuildingPosBefore;

        List<int> TetrisListAll = new List<int>();

        foreach (int groupId in GroupIds) 
        {
            if (GroupIdDic.ContainsKey(groupId)) 
            {
                var TetrisList = GroupIdDic[groupId];

                foreach (int tetrisId in TetrisList) 
                {
                    if (player == 0 && tetrisId > 0) 
                    {
                        TetrisListAll.Add(tetrisId);
                    }

                    if (player == 1 && tetrisId < 0)
                    {
                        TetrisListAll.Add(tetrisId);
                    }
                }
            }
        }

        var TetrisDelete = this.ChangeTetrisIntoUnitInfo(_AllTetrisDic, false);

        // 发送删除事件
        CommunicationManager.BlockUpdate(TetrisDelete, true);

        // 发送至客户端
        CommunicationManager.SendTetrisUpdateToClient(TetrisDelete, true);

        foreach (int id in TetrisListAll) 
        {
            if (_AllTetrisDic.ContainsKey(id)) 
            {
                _AllTetrisDic[id].posx += (int)DifferenceVector.x;

                _AllTetrisDic[id].posy += (int)DifferenceVector.y;
            }
        }

        // 发送碰撞管理器更新事件 -> 客户端
        CommunicationManager.G2C_GridInfoUpdate(_grid);

        var TetrisMove = this.ChangeTetrisIntoUnitInfo(_AllTetrisDic, false);

        // 发送新建事件
        CommunicationManager.BlockUpdate(TetrisMove, false);

        // 发送至客户端
        CommunicationManager.SendTetrisUpdateToClient(TetrisMove, false);

        // 服务器更新Unit信息
        unitManager.UpdateUnit(TetrisMove);
    }

    /// <summary>
    /// 砖块前进事件
    /// </summary>
    private void BroundForward() 
    {
        if (_AllTetrisDic.Count == 0) return;

        var TetrisDelete = this.ChangeTetrisIntoUnitInfo(_AllTetrisDic, false);

        // 发送删除事件
        CommunicationManager.BlockUpdate(TetrisDelete, true);

        // 发送至客户端
        CommunicationManager.SendTetrisUpdateToClient(TetrisDelete, true);

        // 计算需要前进的砖块
        this.CountTetris(_AllTetrisDic);

        // 更新碰撞管理器
        this.UpdateGrid(_AllTetrisDic);

        // 更新已经在建筑上的砖块
        this.UpdateBuilding(_AllTetrisDic);

        // 发送碰撞管理器更新事件 -> 客户端/本地
        CommunicationManager.G2C_GridInfoUpdate(_grid);

        var TetrisMove = this.ChangeTetrisIntoUnitInfo(_AllTetrisDic, false);

        // 发送新建事件
        CommunicationManager.BlockUpdate(TetrisMove, false);

        // 发送至客户端
        CommunicationManager.SendTetrisUpdateToClient(TetrisMove, false);

        // 服务器更新Unit信息
        unitManager.UpdateUnit(TetrisMove);

        Invoke(nameof(CreateTetrisBlocksCount), (float)1.5);
    }

    /// <summary>
    /// 计算砖块合成
    /// </summary>
    public void CreateTetrisBlocksCount()
    {
        // 计算砖块合成
        CountGridClass countGrid = this.OnCreateUnitButtonClick();

        // 删除显示砖块
        var deleteInfo = this.OnCreateUnitInfo(countGrid.tetrisDelete, false);

        CommunicationManager.BlockUpdate(deleteInfo, true);

        // 回收士兵
        CommunicationManager.OnUnitBack(deleteInfo);

        // 新建Unit
        var createInfo = this.OnCreateUnitInfo(countGrid.tetrisUpdate, false);

        foreach (var info in createInfo)
        {
            info.CreateUnit = true;
        }

        // 新建砖块
        CommunicationManager.BlockUpdate(createInfo, false);

        // 新建Unit
        CommunicationManager.OnCellUnitCreate(createInfo);

    }

    /// <summary>
    /// 计算砖块合成
    /// </summary>
    /// <returns></returns>
    private CountGridClass OnCreateUnitButtonClick()
    {
        // 砖块合成计算
        var returnInfo = TetrisService.Instance.CountBlocks(_grid, _AllTetrisDic, _width, _height);

        _grid = returnInfo._grid;

        _AllTetrisDic = returnInfo._AllTetrisDic;

        return returnInfo;
    }

    /// <summary>
    /// 更新是否有砖块在建筑上
    /// </summary>
    /// <param name="_AllTetris"></param>
    private void UpdateBuilding(Dictionary<int, TetrisClass> _AllTetris) 
    {
        // 获取建筑周边砖块
        var goupIdListPlayerOne = this.GetAroundBlocks((int)BuildingPosPlayerOne.x, (int)BuildingPosPlayerOne.y);

        foreach (int groupid in goupIdListPlayerOne)
        {
            if (GroupIdDic.ContainsKey(groupid))
            {
                foreach (int tetrisid in GroupIdDic[groupid])
                {
                    if (_AllTetrisDic.ContainsKey(tetrisid))
                    {
                        _AllTetrisDic[tetrisid].IsBuildingBlock = playerOneBuildingState == 1? true : false;
                    }
                }
            }
        }

        // 获取建筑周边砖块
        var goupIdListPlayerTwo = this.GetAroundBlocks((int)BuildingPosPlayerTwo.x, (int)BuildingPosPlayerTwo.y);

        foreach (int groupid in goupIdListPlayerTwo)
        {
            if (GroupIdDic.ContainsKey(groupid))
            {
                foreach (int tetrisid in GroupIdDic[groupid])
                {
                    if (_AllTetrisDic.ContainsKey(tetrisid))
                    {
                        _AllTetrisDic[tetrisid].IsBuildingBlock = playerTwoBuildingState == 1 ? true : false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 所有碰撞管理器更新
    /// </summary>
    /// <param name="_AllTetris"></param>
    private void UpdateGrid(Dictionary<int, TetrisClass> _AllTetris) 
    {
        var newGrid = new string[_width, _height];

        foreach (TetrisClass info in _AllTetris.Values) 
        {
            var posx = info.posx;

            var posy = info.posy;

            newGrid[posx, posy] = info.UnitIndexId + "";
        }

        _grid = newGrid;
    }

    /// <summary>
    /// 计算砖块交战
    /// </summary>
    /// <param name="info"></param>
    private void CountTetris(Dictionary<int, TetrisClass> info) 
    {
        // 所有砖块预计的前进位置
        List<string>  tetrisClassList = new List<string>();

        foreach (TetrisClass tetris in info.Values) 
        {
            string posstr = "";

            if (tetris.UnitIndexId > 0)
            {
                posstr = posstr + (tetris.posx + 1);
            }
            else
            {
                posstr = posstr + (tetris.posx - 1);
            }

            posstr += ",";

            posstr += tetris.posy;

            tetrisClassList.Add(posstr);
        }

        List<int> groupids = new();

        // 计算砖块是否即将进入交战区
        foreach (List<int> groupIds in GroupIdDic.Values) 
        {
            foreach (int tetrisId in groupIds) 
            {
                if (info.ContainsKey(tetrisId)) 
                {
                    var contx = tetrisId > 0 ? (info[tetrisId].posx + 1) : (info[tetrisId].posx - 1);

                    var temStr = contx + "," + info[tetrisId].posy;

                    // 找到所有可能前进到这个砖块的格子
                    List<string> ints = tetrisClassList.FindAll(x => x == temStr);
                    
                    // 格子产生重复的话设置整体砖块进入交战区
                    if (ints.Count >= 2)
                    {
                        var groupId = info[tetrisId].TetrisGroupId;

                        groupids.Add(groupId);

                        break;
                    }
                }
            }
        }

        // 前进事件
        foreach (TetrisClass tetrisinfo in info.Values) 
        {
            // 如果已经在交战区则不进行前进计算
            if (tetrisinfo.IsBattlefieldBlock) continue;

            // 如果已经在建筑上并且建筑处于编组状态则不前进
            var playerBuilding = tetrisinfo.UnitIndexId > 0 ? playerOneBuildingState : playerTwoBuildingState;

            if (tetrisinfo.IsBuildingBlock && playerBuilding == 1) continue;

            // 前进一格
            if (tetrisinfo.UnitIndexId > 0)
            {
                tetrisinfo.posx += 1;
            }
            else
            {
                tetrisinfo.posx -= 1;
            }
        }

        // 设置砖块已经进入交战区
        foreach (int groupId in groupids) 
        {
            var idList = GroupIdDic[groupId];

            foreach (int id in idList)
            {
                if (info.ContainsKey(id))
                {
                    info[id].IsBattlefieldBlock = true;
                }
            }
        }
    }

    /// <summary>
    /// 连接通讯管理器
    /// </summary>
    private bool FindSceneLoader()
    {
        sceneLoader = GameObject.Find("LanNetWorkManager");

        if (sceneLoader != null) 
        {
            broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

            CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

            return true;
        }

        return false;
    }

    /// <summary>
    /// 砖块放置
    /// </summary>
    public bool BlocksSet(Dictionary<int, TetrisClass> infoBlocks) 
    {
        // 如果是player Two的数据 先镜像再set
        var first = infoBlocks.FirstOrDefault();

        if (first.Key < 0) 
        {
            infoBlocks = BlocksMirrorSetLogic(infoBlocks);
        }

        // 检测砖块是否合理
        if (!ValidMove(_grid, infoBlocks, _width, _height)) 
        {
            return true;
        }

        // 碰撞模拟器更新
        this.AddToGrid(infoBlocks, _grid);

        // 连接通讯管理器
        if (!FindSceneLoader()) 
        {
            Debug.LogError("Please Check Logic Comm");
        }

        CommunicationManager.TetrisInfoCreate(this.ChangeTetrisIntoUnitInfo(infoBlocks, true));

        // 砖块整体信息更新
        this.AddTetris(infoBlocks);

        // 砖块整体组ID信息更新
        this.AddGroupDIc(infoBlocks);

        // 计算砖块合成
        Invoke(nameof(CreateTetrisBlocksCount), (float)1.5);

        return false;
    }

    /// <summary>
    /// 本地砖块放置
    /// </summary>
    /// <param name="infoBlocks"></param>
    public void BlockSetLocal(Dictionary<int, TetrisClass> infoBlocks)
    {
        // 砖块整体信息更新
        this.AddTetris(infoBlocks);

        // 砖块整体组ID信息更新
        this.AddGroupDIc(infoBlocks);

        // 计算砖块合成
        Invoke(nameof(CreateTetrisBlocksCount), (float)1.5);
    }

    /// <summary>
    /// 创建砖块整体信息组
    /// </summary>
    /// <param name="infoBlocks"></param>
    void AddGroupDIc(Dictionary<int, TetrisClass> infoBlocks) 
    {
        List<int> GroupList = new();

        foreach (int id in infoBlocks.Keys) 
        {
            infoBlocks[id].TetrisGroupId = BlockIndex;

            GroupList.Add(id);
        }

        GroupIdDic.Add(BlockIndex, GroupList);

        BlockIndex++;
    }

    /// <summary>
    /// 添加砖块信息进入整体信息管理
    /// </summary>
    /// <param name="infoBlocks"></param>
    void AddTetris(Dictionary<int, TetrisClass> infoBlocks) 
    {
        foreach (int id in infoBlocks.Keys) 
        {
            if (!_AllTetrisDic.ContainsKey(id)) 
            {
                _AllTetrisDic.Add(id, infoBlocks[id]);
            }
        }
    }

    /// <summary>
    /// 设置砖块镜像 -> 返回值
    /// </summary>
    public List<TetrisClass> BlocksMirrorSet(Dictionary<int, TetrisClass> infoBlocks) 
    {
        foreach (TetrisClass blocks in infoBlocks.Values) 
        {
            var infox = blocks.posx;

            blocks.posx = (19 - infox * 2) + infox;
        }

        return infoBlocks.Values.ToList();
    }

    /// <summary>
    /// 设置砖块镜像 -> 逻辑值
    /// </summary>
    public Dictionary<int, TetrisClass> BlocksMirrorSetLogic(Dictionary<int, TetrisClass> infoBlocks)
    {
        foreach (TetrisClass blocks in infoBlocks.Values)
        {
            var infox = blocks.posx;

            blocks.posx = (19 - infox * 2) + infox;
        }

        return infoBlocks;
    }


    /// <summary>
    /// 砖块位置合法性判断方法
    /// </summary>
    /// <returns></returns>
    private bool ValidMove(string[,] _grid, Dictionary<int, TetrisClass> _TetrisDic, int _width, int _height)
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
    /// 碰撞模拟器更新方法
    /// </summary>
    /// <param name="_TetrisDic"></param>
    /// <param name="_grid"></param>
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
    /// 将传输数据转化为 UnitInfoClass
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private List<UnitInfoClass> ChangeTetrisIntoUnitInfo(Dictionary<int, TetrisClass> info, bool CreateInfo)
    {
        List<UnitInfoClass> BlocksReadyUnit = new List<UnitInfoClass>();

        foreach (int key in info.Keys)
        {
            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = info[key].UnitIndexId;

            unitInfoClass.UnitPos = new Vector2(info[key].posx, info[key].posy);

            if (CreateInfo)
            {
                unitInfoClass.UnitPosUse = PositionTools.GetZeroPos();
            }
            else
            {
                unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);

            }

            unitInfoClass.color = info[key].Color;

            unitInfoClass.CreateUnit = true;

            BlocksReadyUnit.Add(unitInfoClass);
        }

        return BlocksReadyUnit;
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
}