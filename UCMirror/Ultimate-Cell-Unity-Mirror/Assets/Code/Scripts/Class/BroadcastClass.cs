using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BroadcastClass : MonoBehaviour
{
    /// <summary>
    /// 事件发布例子
    /// </summary>
    public UnityAction<int> onDataPass;

    /// <summary>
    /// 砖块新建事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> TetrisInfoCreate;

    /// <summary>
    /// 砖块更新事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> TetrisInfoUpdate;

    /// <summary>
    /// 砖块/Unit整体更新事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> UnitInfoAllUpdate;

    /// <summary>
    /// 砖块整体销毁事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> BlocksInfoDestory;

    /// <summary>
    /// 砖块整体移动事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> BlocksInfoMove;

    /// <summary>
    /// 敌人生成事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> VirusInfoCreate;

    /// <summary>
    /// 敌人信息更新事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> VirusInfoUpdate;

    /// <summary>
    /// 单位死亡事件
    /// </summary>
    public UnityAction<int> OnUnitDie;

    /// <summary>
    /// 控制砖块的UI的位置移动事件
    /// </summary>
    public UnityAction<Vector3> OnUIPosChange;

    /// <summary>
    /// 英雄信息构筑触发方法
    /// </summary>
    public UnityAction<HeroCreateClass> OnHeroCreate;

    /// <summary>
    /// 英雄行为触发方法
    /// </summary>
    public UnityAction<List<HeroActionClass>> OnHeroAction;

    /// <summary>
    /// 动画播放监听事件
    /// </summary>
    public UnityAction<int> OnStoryPlay;

    /// <summary>
    /// 动画播放完成触发事件
    /// </summary>
    public UnityAction<int> OnStoryEnd;

    /// <summary>
    /// 故事系统加载完成
    /// </summary>
    public UnityAction<int> OnStoryLoadingComplete;

    /// <summary>
    /// 士兵池回收
    /// </summary>
    public UnityAction<List<UnitInfoClass>> OnSoldierPoolBack;

    /// <summary>
    /// 战场加载完成后生成砖块
    /// </summary>
    public UnityAction<int> OnBlockCreate;

    /// <summary>
    /// 外尔战士到达事件
    /// </summary>
    public UnityAction<int> OnVirusArriveLine;

    /// <summary>
    /// ui控制组件触发事件
    /// </summary>
    public UnityAction<EventType.UIControl> OnUIControl;

    /// <summary>
    /// 战场想法触发方法
    /// int 生成想法数量
    /// </summary>
    public UnityAction<int> OnCreateNewIdea;

    /// <summary>
    /// 战场想法接收方法
    /// </summary>
    public UnityAction<List<IdeaClass>, EventType.BlocksGrade> CreateNewIdea;

    /// <summary>
    /// 砖块移动前存入缓冲区
    /// </summary>
    public UnityAction<Dictionary<int, TetrisClass>, string> BlocksReady;

    /// <summary>
    /// 砖块移动
    /// </summary>
    public UnityAction<Vector3> OnBlocksMove;

    /// <summary>
    /// 砖块结束移动
    /// </summary>
    public UnityAction<int> BlocksMoveEnd;

    /// <summary>
    /// 砖块取消操作
    /// </summary>
    public UnityAction<int> OnBlocksCancellation;

    /// <summary>
    /// 点击按钮生成Unit
    /// </summary>
    public UnityAction<List<UnitInfoClass>> OnCellUnitCreate;

    /// <summary>
    /// Unit回收事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> OnUnitBack;

    /// <summary>
    /// 兵线移动触发事件
    /// </summary>
    public UnityAction<FireLineInfoClass> OnFireLineChange;

    /// <summary>
    /// 生成英雄信息
    /// </summary>
    public UnityAction<HeroInfoClass> OnHeroCreateResponse;

    /// <summary>
    /// 英雄死亡信息接收
    /// </summary>
    public UnityAction<int> OnHeroDie;

    /// <summary>
    /// 需要统一隐藏的UI
    /// </summary>
    public UnityAction<GameObject> StoryHide;

    /// <summary>
    /// 砖块生成结束
    /// </summary>
    public UnityAction<int> BlockCreateDone;

    /// <summary>
    /// 英雄方块取消生成操作
    /// </summary>
    public UnityAction<string> OnHeroUICancel;

    /// <summary>
    /// 塞尔到达指定位置
    /// </summary>
    public UnityAction<int> OnCellArrive;

    /// <summary>
    /// Unit死亡事件
    /// </summary>
    public UnityAction<List<int>> SetUnitDie;

    /// <summary>
    /// 触发Boss技能
    /// </summary>
    public UnityAction<int> FireSkillBoss;

    /// <summary>
    /// 新建Boss方法
    /// </summary>
    public UnityAction<int> CreateBoss;

    /// <summary>
    /// 请求战场数据方法
    /// </summary>
    public UnityAction<int> RequestBattlefieldInfo;

    /// <summary>
    /// 返回战场数据方法
    /// </summary>
    public UnityAction<BattledInfoClass> ResponseBaattledInfo;

    /// <summary>
    /// 仅点亮砖块事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> TetrisBright;

    /// <summary>
    /// 仅点亮砖块事件
    /// </summary>
    public UnityAction<List<UnitInfoClass>> TetrisNotBright;

    /// <summary>
    /// 进入心流事件
    /// </summary>
    public UnityAction<int> HeartFlowInit;

    /// <summary>
    /// 退出心流事件
    /// </summary>
    public UnityAction<int> HeartFlowOut;

    /// <summary>
    /// 心流背景变更事件
    /// </summary>
    public UnityAction<int> OnBackGroundChange;

    /// <summary>
    /// 服务器通讯测试
    /// </summary>
    public UnityAction<int> MessageSendTest;

    /// <summary>
    /// 开始游戏按钮点击
    /// </summary>
    public UnityAction<int> StartGameButtonAction;

    /// <summary>
    /// 开始验证
    /// </summary>
    public UnityAction<int> StratCheckInfo;

    /// <summary>
    /// 登陆通讯
    /// </summary>
    public UnityAction<StartCheckClass> StrartCheck;

    /// <summary>
    /// 版本信息验证返回值
    /// </summary>
    public UnityAction<int> StratCheckReturn;

    /// <summary>
    /// 开始匹配
    /// </summary>
    public UnityAction<int> StartMatching;

    /// <summary>
    /// 开始游戏
    /// </summary>
    public UnityAction<int> StartGamePVP;

    /// <summary>
    /// 发送砖块信息
    /// </summary>
    public UnityAction<Dictionary<int, TetrisClass>> C2G_SendBlocksInfo;

    /// <summary>
    /// 接收对手砖块信息
    /// </summary>
    public UnityAction<List<TetrisClass>> OpponBlocks;

    /// <summary>
    /// 未成功连接服务器
    /// </summary>
    public UnityAction<int> ServerStoped;

    /// <summary>
    /// 新建木偶操作
    /// </summary>
    public UnityAction<int> CreatePuppet;

    /// <summary>
    /// 更新木偶固定信息
    /// </summary>
    public UnityAction<PuppetFixedInfoClass> UpdatePuppetFixedInfo;

    /// <summary>
    /// 木偶位置更新
    /// </summary>
    public UnityAction<UnitInfoUpdateStruct> PuppetUnitPosUpdateClass;

    /// <summary>
    /// 建筑更新状态事件
    /// </summary>
    public UnityAction<BuildingStateClass> BuildingUpdateFunc;

    /// <summary>
    /// 砖块更新事件
    /// </summary>
    public UnityAction<G2C_TetrisUpdateClass> G2C_TetrisUpdateClass;

    /// <summary>
    /// 碰撞盒更新事件 - 本地
    /// </summary>
    public UnityAction<string[,]> G2C_GridInfoUpdate;

    /// <summary>
    /// 本地模式砖块新建
    /// </summary>
    public UnityAction<Dictionary<int, TetrisClass>> LocalTetrisCreate;

    /// <summary>
    /// 向客户端发送砖块新建
    /// </summary>
    public UnityAction<List<UnitInfoClass>, bool> SendTetrisUpdateToClient;

    public void Reload()
    {
        onDataPass = null;
        TetrisInfoCreate = null;
        TetrisInfoUpdate = null;
        UnitInfoAllUpdate = null;
        BlocksInfoDestory = null;
        BlocksInfoMove = null;
        VirusInfoCreate = null;
        VirusInfoUpdate = null;
        OnUnitDie = null;
        OnUIPosChange = null;
        OnHeroCreate = null;
        OnHeroAction = null;
        OnStoryPlay = null;
        OnStoryEnd = null;
        OnStoryLoadingComplete = null;
        OnSoldierPoolBack = null;
        OnBlockCreate = null;
        OnVirusArriveLine = null;
        OnUIControl = null;
        OnCreateNewIdea = null;
        CreateNewIdea = null;
        BlocksReady = null;
        OnBlocksMove = null;
        BlocksMoveEnd = null;
        OnBlocksCancellation = null;
        OnCellUnitCreate = null;
        OnUnitBack = null;
        OnFireLineChange = null;
        OnHeroCreateResponse = null;
        OnHeroDie = null;
        StoryHide = null;
        BlockCreateDone = null;
        OnHeroUICancel = null;
        OnCellArrive = null;
        SetUnitDie = null;
        FireSkillBoss = null;
        CreateBoss = null;
        RequestBattlefieldInfo = null;
        ResponseBaattledInfo = null;
        TetrisBright = null;
        TetrisNotBright = null;
        HeartFlowInit = null;
        HeartFlowOut = null;
        OnBackGroundChange = null;
    }
}

