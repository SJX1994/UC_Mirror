using System.Collections.Generic;
using UnityEngine;
using static ResManager;

public class CommunicationInteractionManager : MonoBehaviour
{
    /// <summary>
    /// 数据管理类
    /// </summary>
    private BroadcastClass broadcastClass;

    /// <summary>
    /// 服务状态
    /// </summary>
    /// 0 服务状态为 Client
    /// 1 服务状态为 Server
    /// 2 服务状态为 Local
    public int ServerState = 0;

    private void Awake()
    {
        broadcastClass = this.transform.gameObject.AddComponent<BroadcastClass>();
    }

    /// <summary>
    /// 砖块创建方法
    /// </summary>
    /// <param name="info"></param>
    public void TetrisInfoCreate(List<UnitInfoClass> info)
    {
        var TetrisInfoCreate = broadcastClass.TetrisInfoCreate;

        if (TetrisInfoCreate != null)
        {
            TetrisInfoCreate(info);
        }
    }

    /// <summary>
    /// Unit信息更新方法
    /// </summary>
    /// <param name="info"></param>
    public void TetrisInfoUpdate(List<UnitInfoClass> info)
    {
        var TetrisPosUpdate = broadcastClass.TetrisInfoUpdate;

        if (TetrisPosUpdate != null)
        {
            TetrisPosUpdate(info);
        }
    }

    /// <summary>
    /// 砖块信息整体更新
    /// </summary>
    /// <param name="info"></param>
    public void UnitInfoAllUpdate(List<UnitInfoClass> info)
    {
        var UnitInfoAllUpdate = broadcastClass.UnitInfoAllUpdate;

        if (UnitInfoAllUpdate != null)
        {
            UnitInfoAllUpdate(info);
        }

    }

    /// <summary>
    /// Unit死亡信息更新方法
    /// </summary>
    /// <param name="info"></param>
    public void UnitDieInfoProcess(int info)
    {
        var OnUnitDie = broadcastClass.OnUnitDie;

        if (OnUnitDie != null)
        {
            OnUnitDie(info);
        }
    }

    /// <summary>
    /// 外尔战士生成方法
    /// </summary>
    /// <param name="info"></param>
    public void VirusInfoCreate(List<UnitInfoClass> info)
    {
        var TetrisInfoCreate = broadcastClass.VirusInfoCreate;

        if (TetrisInfoCreate != null)
        {
            TetrisInfoCreate(info);
        }

    }

    /// <summary>
    /// 外尔战士信息更新方法
    /// </summary>
    /// <param name="info"></param>
    public void VirusInfoUpdate(List<UnitInfoClass> info)
    {
        var TetrisInfoUpdate = broadcastClass.VirusInfoUpdate;

        if (TetrisInfoUpdate != null)
        {
            TetrisInfoUpdate(info);
        }
    }

    /// <summary>
    /// UI位置更新方法
    /// </summary>
    /// <param name="info"></param>
    public void OnUIPosUpdate(Vector3 info) 
    {
        var onUIPosChange = broadcastClass.OnUIPosChange;

        if (onUIPosChange != null) 
        {
            onUIPosChange(info);
        }
    }

    /// <summary>
    /// 方块移动、消除方法
    /// </summary>
    /// <param name="info"></param>
    public void BlockUpdate(List<UnitInfoClass> info, bool destory) 
    {
        if (destory)
        {
            var onBlockDestory = broadcastClass.BlocksInfoDestory;

            if (onBlockDestory != null)
            {
                onBlockDestory(info);
            }
        }
        else 
        {
            var onBlockDestory = broadcastClass.BlocksInfoMove;

            if (onBlockDestory != null)
            {
                onBlockDestory(info);
            }
        }
    }

    /// <summary>
    /// 英雄信息构筑触发方法
    /// </summary>
    /// <param name="info"></param>
    public void CreateHero(HeroCreateClass info) 
    {
        var onHeroCreate = broadcastClass.OnHeroCreate;

        if (onHeroCreate != null) 
        {
            onHeroCreate(info);
        }
    }

    /// <summary>
    /// 英雄行为改变触发方法
    /// </summary>
    /// <param name="info"></param>
    public void HeroActionChange(List<HeroActionClass> info) 
    {
        var OnHeroAction = broadcastClass.OnHeroAction;

        if (OnHeroAction != null) 
        {
            OnHeroAction(info);
        }
    }

    /// <summary>
    /// 动画播放触发方法
    /// </summary>
    /// <param name="info"></param>
    public void OnStoryPlay(int info) 
    {
        var OnStoryPlay = broadcastClass.OnStoryPlay;

        if (OnStoryPlay != null) 
        {
            OnStoryPlay(info);
        }
    }

    /// <summary>
    /// 动画播放完成触发事件
    /// </summary>
    /// <param name="info"></param>
    public void OnStoryEnd (int info)
    {
        var OnStoryEnd = broadcastClass.OnStoryEnd;

        if (OnStoryEnd != null)
        {
            OnStoryEnd(info);
        }
    }

    /// <summary>
    /// 故事系统加载完成
    /// </summary>
    /// <param name="info"></param>
    public void OnStoryLoadingComplete(int info) 
    {
        var OnStoryLoadingComplete = broadcastClass.OnStoryLoadingComplete;

        if (OnStoryLoadingComplete != null) 
        {
            OnStoryLoadingComplete(info);
        }
    }

    /// <summary>
    /// 士兵池回收事件
    /// </summary>
    public void OnSoldierPoolBack(List<UnitInfoClass> info)
    {
        var OnSoldierPoolBack = broadcastClass.OnSoldierPoolBack;

        if (OnSoldierPoolBack != null) 
        {
            OnSoldierPoolBack(info);
        }
    }

    /// <summary>
    /// 生成第一个砖块的事件
    /// </summary>
    /// <param name="info"></param>
    public void OnBlockCreate(int info) 
    {
        var OnBlockCreate = broadcastClass.OnBlockCreate;

        if (OnBlockCreate != null)
        {
            OnBlockCreate(info);
        }
    }

    /// <summary>
    /// 外尔战士到达触发事件
    /// </summary>
    /// <param name="info"></param>
    public void OnVirusArrive(int info) 
    {
        var OnVirusArrive = broadcastClass.OnVirusArriveLine;

        if (OnVirusArrive != null) 
        {
            OnVirusArrive(info);
        }
    }

    /// <summary>
    /// UI控制组件触发事件
    /// </summary>
    /// <param name="info"></param>
    public void OnUIControl(EventType.UIControl info) 
    {
        var OnUIControl = broadcastClass.OnUIControl;

        if (OnUIControl != null) 
        {
            OnUIControl(info);
        }
    }

    /// <summary>
    /// 兵线移动触发事件
    /// </summary>
    /// <param name="info"></param>
    public void OnFireLineChange(FireLineInfoClass info) 
    {
        var OnFireLineChange = broadcastClass.OnFireLineChange;

        if (OnFireLineChange != null) 
        {
            OnFireLineChange(info);
        }
    }

    /// <summary>
    /// 战场生成想法触发方法
    /// </summary>
    /// <param name="info"></param>
    public void OnCreateNewIdea(int info) 
    {
        var OnCreateNewIdea = broadcastClass.OnCreateNewIdea;

        if (OnCreateNewIdea != null) 
        {
            OnCreateNewIdea(info);
        }
    }

    /// <summary>
    /// 战场想法生成返回值
    /// </summary>
    /// <param name="info"></param>
    public void ResponseCreateNewIdea(List<IdeaClass> info, EventType.BlocksGrade Level) 
    {
        var OnResponse = broadcastClass.CreateNewIdea;

        if (OnResponse != null) 
        {
            OnResponse(info, Level);
        }
    }

    /// <summary>
    /// 砖块准备完成进入预留区
    /// </summary>
    /// <param name="info"></param>
    public void BlocksReady(Dictionary<int, TetrisClass> info, string heroName = "Blocks") 
    {
        var BlocksReady = broadcastClass.BlocksReady;

        if (BlocksReady != null) 
        {
            BlocksReady(info, heroName);
        }
    }

    /// <summary>
    /// 砖块移动
    /// </summary>
    /// <param name="info"></param>
    public void OnBlocksMove(Vector3 info) 
    {
        var OnBlocksMove = broadcastClass.OnBlocksMove;

        if (OnBlocksMove != null) 
        {
            OnBlocksMove(info);
        }
    }

    /// <summary>
    /// 砖块移动结束
    /// </summary>
    /// <param name="info"></param>
    public void BlocksMoveEnd(int info) 
    {
        var BlocksMoveEnd = broadcastClass.BlocksMoveEnd;

        if (BlocksMoveEnd != null) 
        {
            BlocksMoveEnd(info);
        }
    }

    /// <summary>
    /// 砖块取消操作
    /// </summary>
    /// <param name="info"></param>
    public void OnBlocksCancellation(int info) 
    {
        var OnBlocksCancellation = broadcastClass.OnBlocksCancellation;

        if (OnBlocksCancellation != null) 
        {
            OnBlocksCancellation(info);
        }
    }

    /// <summary>
    /// 点击按钮生成Unit
    /// </summary>
    /// <param name="info"></param>
    public void OnCellUnitCreate(List<UnitInfoClass> info) 
    {
        var OnCellUnitCreate = broadcastClass.OnCellUnitCreate;

        if (OnCellUnitCreate != null) 
        {
            OnCellUnitCreate(info);
        }
    }

    /// <summary>
    /// Unit回收
    /// </summary>
    /// <param name="info"></param>
    public void OnUnitBack(List<UnitInfoClass> info)
    {
        var OnCellUnitCreate = broadcastClass.OnUnitBack;

        if (OnCellUnitCreate != null)
        {
            OnCellUnitCreate(info);
        }
    }

    /// <summary>
    /// 英雄生成时的返回信息
    /// </summary>
    /// <param name="info"></param>
    public void OnHeroCreateResponse(HeroInfoClass info)
    {
        var OnHeroCreateResponse = broadcastClass.OnHeroCreateResponse;

        if (OnHeroCreateResponse != null) 
        {
            OnHeroCreateResponse(info);
        }
    }
    /// <summary>
    /// 英雄死亡信息更细触发方法
    /// </summary>
    /// <param name="info"></param>
    public void OnHeroDie(int info)
    {
        var OnHeroDie = broadcastClass.OnHeroDie;

        if (OnHeroDie != null)
        {
            OnHeroDie(info);
        }
    }

    /// <summary>
    /// 加入肯德基豪华午餐
    /// </summary>
    /// <param name="info"></param>
    public void AddHideGameObjectList(GameObject info) 
    {
        var StoryHide = broadcastClass.StoryHide;

        if (StoryHide != null)
        {
            StoryHide(info);
        }

    }

    /// <summary>
    /// 取消生成英雄操作
    /// </summary>
    /// <param name="info"></param>
    public void OnHeroUICancel(string info) 
    {
        var OnHeroUICancel = broadcastClass.OnHeroUICancel;

        if (OnHeroUICancel != null) 
        {
            OnHeroUICancel(info);
        }
    }

    /// <summary>
    /// 塞尔到达指定位置事件
    /// </summary>
    /// <param name="info"></param>
    public void OnCellArrive(int info) 
    {
        var OnCellArrive = broadcastClass.OnCellArrive;

        if (OnCellArrive != null) 
        {
            OnCellArrive(info);
        }
    }

    /// <summary>
    /// 砖块新建完成
    /// </summary>
    /// <param name="info"></param>
    public void BlockCreateDone(int info) 
    {
        var BlockCreateDone = broadcastClass.BlockCreateDone;

        if (BlockCreateDone != null) 
        {
            BlockCreateDone(info);
        }
    }

    /// <summary>
    /// 设置Unit死亡事件
    /// </summary>
    /// <param name="info"></param>
    public void SetUnitDie(List<int> info) 
    {
        var SetUnitDie = broadcastClass.SetUnitDie;

        if (SetUnitDie != null) 
        {
            SetUnitDie(info);
        }
    }

    /// <summary>
    /// 释放Boss技能
    /// </summary>
    /// <param name="info"></param>
    public void FireSkillBoss (int info) 
    {
        var SkillBoss = broadcastClass.FireSkillBoss;

        if (SkillBoss != null)  
        {
            SkillBoss(info);
        }

    }

    /// <summary>
    /// 新建Boss触发方法
    /// </summary>
    /// <param name="info"></param>
    public void CreateBoss(int info) 
    {
        var CreateBoss = broadcastClass.CreateBoss;

        if (CreateBoss != null) 
        {
            CreateBoss(info);
        }
    }

    /// <summary>
    /// 请求战场数据方法
    /// </summary>
    /// <param name="info"></param>
    public void RequestBattlefieldInfo(int info) 
    {
        var RequestBattlefieldInfo = broadcastClass.RequestBattlefieldInfo;

        if (RequestBattlefieldInfo != null) 
        {
            RequestBattlefieldInfo(info);
        }
    }

    /// <summary>
    /// 返回战场数据方法
    /// </summary>
    /// <param name="info"></param>
    public void ResponseBaattledInfo(BattledInfoClass info) 
    {
        var ResponseBaattledInfo = broadcastClass.ResponseBaattledInfo;

        if (ResponseBaattledInfo != null) 
        {
            ResponseBaattledInfo(info);
        }
    }

    /// <summary>
    /// 仅点亮砖块
    /// </summary>
    /// <param name="info"></param>
    public void OnTetrisBright(List<UnitInfoClass> info)
    {
        var OnTetrisBrightInfo = broadcastClass.TetrisBright;

        if (OnTetrisBrightInfo != null) 
        {
            OnTetrisBrightInfo(info);
        }
    }

    /// <summary>
    /// 仅熄灭砖块
    /// </summary>
    /// <param name="info"></param>
    public void OnTetrisNotBright(List<UnitInfoClass> info)
    {
        var OnTetrisNotBright = broadcastClass.TetrisNotBright;

        if (OnTetrisNotBright != null)
        {
            OnTetrisNotBright(info);
        }

    }

    /// <summary>
    /// 进入心流模式方法
    /// </summary>
    /// <param name="info"></param>
    public void OnInitHeartFlow(int info) 
    {
        var OnInitHeartFlow = broadcastClass.HeartFlowInit;

        if (OnInitHeartFlow != null) 
        {
            OnInitHeartFlow(info);
        }
    }

    /// <summary>
    /// 退出心流模式的方法
    /// </summary>
    /// <param name="info"></param>
    public void OnOutHeartFlow(int info) 
    {
        var OnOutHeartFlow = broadcastClass.HeartFlowOut;

        if (OnOutHeartFlow != null) 
        {
            OnOutHeartFlow(info);
        }
    }

    /// <summary>
    /// 心流背景变更
    /// </summary>
    /// <param name="info"></param>
    public void OnBackGroundChange(int info) 
    {
        var OnBackGroundChange = broadcastClass.OnBackGroundChange;

        if (OnBackGroundChange != null) 
        {
            OnBackGroundChange(info);
        }
    }

    /// <summary>
    /// 服务器通讯测试 发送消息
    /// </summary>
    public void OnMessageSendToServer(int info) 
    {
        var OnMessageSendToServer = broadcastClass.MessageSendTest;

        if (OnMessageSendToServer != null)
        {
            OnMessageSendToServer(info);
        }
    }

    /// <summary>
    /// 开始登陆版本检查验证程序
    /// </summary>
    /// <param name="info"></param>
    public void StartCheck(int info) 
    {
        var StartCheck = broadcastClass.StratCheckInfo;

        if (StartCheck != null) 
        {
            StartCheck(info);
        }
    }

    /// <summary>
    /// 版本检查更新
    /// </summary>
    public void C2G_StartCheck(StartCheckClass info) 
    {
        var C2G_StartCheck = broadcastClass.StrartCheck;

        if (C2G_StartCheck != null) 
        {
            C2G_StartCheck(info);
        }
    }

    /// <summary>
    /// 版本验证返回值信息
    /// </summary>
    /// <param name="info"></param>
    public void G2C_StratCheckReturn(int info) 
    {
        var G2C_StratCheckReturn = broadcastClass.StratCheckReturn;

        if (G2C_StratCheckReturn != null) 
        {
            G2C_StratCheckReturn(info);
        }
    }

    /// <summary>
    /// 开始匹配
    /// </summary>
    /// <param name="info"></param>
    public void C2G_StartMatching(int info) 
    {
        var C2G_StartMatching = broadcastClass.StartMatching;

        if (C2G_StartMatching != null) 
        {
            C2G_StartMatching(info);
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="info"></param>
    public void StartGamePVP(int info) 
    {
        var StartGame = broadcastClass.StartGamePVP;

        if (StartGame != null) 
        {
            StartGame(info);
        }
    }

    /// <summary>
    /// 发送砖块信息
    /// </summary>
    /// <param name="info"></param>
    public void SendBlocksInfo(Dictionary<int, TetrisClass> info) 
    {
        var C2G_SendBlocksInfo = broadcastClass.C2G_SendBlocksInfo;

        if (C2G_SendBlocksInfo != null) 
        {
            C2G_SendBlocksInfo(info);
        }
    }

    /// <summary>
    /// 获取对手的砖块信息
    /// </summary>
    /// <param name=""></param>
    public void GetOpponBlocks(List<TetrisClass> info) 
    {
        var GetOpponBlocks = broadcastClass.OpponBlocks;

        if (GetOpponBlocks != null) 
        {
            GetOpponBlocks(info);
        }
    }

    /// <summary>
    /// 未连接到服务器
    /// </summary>
    /// <param name="info"></param>
    public void ServerStoped(int info) 
    {
        var ServerStoped = broadcastClass.ServerStoped;

        if (ServerStoped != null) 
        {
            ServerStoped(info);
        }
    }

    /// <summary>
    /// 新建木偶信息
    /// </summary>
    /// <param name="info"></param>
    public void CreatePuppet(int info) 
    {
        var CreatePuppet = broadcastClass.CreatePuppet;

        if (CreatePuppet != null) 
        {
            CreatePuppet(info);
        }
    }

    /// <summary>
    /// 更新Puppet固有信息
    /// </summary>
    /// <param name="info"></param>
    public void UpdatePuppetFixedInfo(PuppetFixedInfoClass info) 
    {
        var UpdatePuppetFixedInfo = broadcastClass.UpdatePuppetFixedInfo;

        if (UpdatePuppetFixedInfo != null) 
        {
            UpdatePuppetFixedInfo(info);
        }
    }

    /// <summary>
    /// 更新木偶整体信息
    /// </summary>
    public void UpdatePuppetInfo(UnitInfoUpdateStruct info) 
    {
        var UpdatePuppetPos = broadcastClass.PuppetUnitPosUpdateClass;

        if (UpdatePuppetPos != null) 
        {
            UpdatePuppetPos(info);
        }
    }

    /// <summary>
    /// 建筑状态更新事件
    /// </summary>
    /// <param name="info"></param>
    public void BuildingUpdateFunc(BuildingStateClass info) 
    {
        var BuildingUpdateFunc = broadcastClass.BuildingUpdateFunc;

        if (BuildingUpdateFunc != null) 
        {
            BuildingUpdateFunc(info);
        }
    }

    /// <summary>
    /// 砖块更新方法
    /// </summary>
    /// <param name="info"></param>
    public void G2C_TetrisUpdateFunc(G2C_TetrisUpdateClass info) 
    {
        var G2C_TetrisUpdateFunc = broadcastClass.G2C_TetrisUpdateClass;

        if (G2C_TetrisUpdateFunc != null) 
        {
            G2C_TetrisUpdateFunc(info);
        }
    }

    /// <summary>
    /// 碰撞盒更新事件
    /// </summary>
    /// <param name="info"></param>
    public void G2C_GridInfoUpdate(string[,] info) 
    {
        var G2C_GridInfoUpdate = broadcastClass.G2C_GridInfoUpdate;

        if (G2C_GridInfoUpdate != null) 
        {
            G2C_GridInfoUpdate(info);
        }
    }

    /// <summary>
    /// 开始游戏按钮点击
    /// </summary>
    /// <param name="info"></param>
    public void StartGame(int info) 
    {
        var StartGame = broadcastClass.StartGameButtonAction;

        if (StartGame != null) 
        {
            StartGame(info);
        }
    }

    /// <summary>
    /// 本地砖块新建事件
    /// </summary>
    /// <param name="info"></param>
    public void LocalTetrisCreate(Dictionary<int, TetrisClass> info) 
    {
        var LocalTetrisCreate = broadcastClass.LocalTetrisCreate;

        if (LocalTetrisCreate != null) 
        {
            LocalTetrisCreate(info);
        }
    }

    /// <summary>
    /// 发送砖块更新事件 -> 客户端
    /// </summary>
    /// <param name="info"></param>
    /// <param name="Delete"></param>
    public void SendTetrisUpdateToClient(List<UnitInfoClass> info, bool Delete) 
    {
        var SendTetrisUpdateToClient = broadcastClass.SendTetrisUpdateToClient;

        if (SendTetrisUpdateToClient != null) 
        {
            SendTetrisUpdateToClient(info, Delete);
        }
    }
}
