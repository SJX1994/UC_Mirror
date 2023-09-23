using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ResManager;

public class BattlefieldControlManager : MonoBehaviour
{
    public Dictionary<int, UnitInfoClass> VirusInfo = new();

    public int CellsCountInfo = 0;

    public int VirusCountInfo = 0;

    public Dictionary<Transform, int> transInfo = new();

    private List<UnitInfoClass> deleteBlocksClassList = new();

    private Dictionary<int, int> ArriveUnit = new();

    [Header("外尔生成时间间隔")]
    public float VirusCreateTime = 10;

    // 外尔ID递增
    // 生成时间可调整
    private int VirusIndex = 4000;

    // 战场时间判断
    private float previousTime;

    // 外尔生成时间判断
    private float previousTimeV;

    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    [Header("初始化战场高度")]
    public int height = 10;

    [Header("初始化战场宽度(左侧)")]
    public int width  = 10;

    [Header("判断倒计时更新位置")]
    public GameObject JudgeTime;

    [Header("细胞数量更新")]
    public GameObject JudgeCells;

    [Header("病毒数量更新")]
    public GameObject JudgeVirus;

    [Header("判断倒计时")]
    public int timeinfo = 10;

    private GameObject sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        BattlefieldClass.height = height;

        BattlefieldClass.width = width;

        BattlefieldClass.grid = new string[width, height];

        // Debug.Log("战场生成：" + "height:" + height + " * " + "width :" + width);

        // Debug.Log("程序启动：生成砖块");

        // 通信获取
        // 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.OnUnitDie += OnUnitDie;

        broadcastClass.OnHeroDie += OnHeroDie;

        broadcastClass.OnVirusArriveLine += OnVirusArrive;

        broadcastClass.OnCellArrive += OnCellsArrive;

        broadcastClass.FireSkillBoss += FireSkillBoss;

        CreateInvoke();

    }

    public void CreateInvoke()
    {
        // // 每隔1s执行一次时间更新
        // InvokeRepeating(nameof(UpdateTime), 1, 1);

        // // 每隔0.5执行一次Unit更新
        // InvokeRepeating(nameof(UpdateUnit), (float)0.5, (float)0.5);

        // // 每隔5s生成一次外尔战士
        // InvokeRepeating(nameof(CreateVirus), 1f,5f);

        // // 每隔5s更新一次外尔战士的移动
        // InvokeRepeating(nameof(UpdateVirusInfo), 5, 5);

    }

    /// <summary>
    /// 更新时间
    /// </summary>
    private void UpdateTime()
    {
        
    }

    /// <summary>
    /// 兵线控制事件
    /// </summary>
    public void FireLineControl() 
    {
        int VirusCount = VirusCountInfo;

        int CellsCount = CellsCountInfo;

        if (timeinfo != 0)
        {
            timeinfo--;
        }
        else
        {
            timeinfo = 10;

            if (VirusCount > CellsCount)
            {
                KillCells();
                FireLeft();
            }

            if (CellsCount > VirusCount)
            {
                KillVirus();
                FireRight();
            }
        }

        FireLineChange();
    }

    public void BoundForward() 
    {
        FindObjectOfType<GlobalControlManager>().OnAccelerate();
    }

    /// <summary>
    /// 获取砖块管理类
    /// </summary>
    /// <returns></returns>
    private TetrisBlocksManager GetTetrisBlocksManager() 
    {
        var blocks = FindObjectOfType<TetrisBlocksManager>();

        return blocks;
    }

    /// <summary>
    /// 细胞与病毒数量更新
    /// </summary>
    private void UpdateUnit()
    {
        var virusCount = 0;

        foreach (int key in VirusInfo.Keys) 
        {
            if (VirusInfo[key].UnitPos.x == this.GetTetrisBlocksManager()._width) 
            {
                if (ArriveUnit.ContainsKey(key))
                {
                    virusCount++;
                }
            }
        }

        VirusCountInfo = virusCount;

        JudgeVirus.gameObject.GetComponent<TextMeshProUGUI>().text = "Virus : " + virusCount;

        var count = 0;

        var allUnit = this.GetTetrisBlocksManager()._AllTetrisDic;

        var _width = this.GetTetrisBlocksManager()._width - 1;

        foreach (int info in allUnit.Keys) 
        {
            if (_width == allUnit[info].posx)
            {
                if (ArriveUnit.ContainsKey(info))
                {
                    count += allUnit[info].BlockLevel;
                }
                if (allUnit[info].BlockLevel != 1)
                {
                    count += allUnit[info].BlockLevel;
                }
            }
        }

        CellsCountInfo = count;

        JudgeCells.gameObject.GetComponent<TextMeshProUGUI>().text = "Cells : " + count;

    }

    /// <summary>
    /// 塞尔战士到达事件
    /// </summary>
    /// <param name="info"></param>
    private void OnCellsArrive(int info)
    {
    }

    /// <summary>
    /// 新建外尔战士方法
    /// </summary>
    private void CreateVirus()
    {
        int createCount = 1;

        for (int i = 0; i < createCount; i ++)
        {
            AudioSystemManager.Instance.PlaySound("Virus_Cteate");

            NewVirus();
        }
    }

    /// <summary>
    /// 更新外尔战士位置的方法
    /// </summary>
    private void UpdateVirusInfo() 
    {
        UpdateVirus();
    }

    /// <summary>
    /// 新建外尔战士方法
    /// </summary>
    private void NewVirus()
    {
        List<UnitInfoClass> unitInfoUpdateClassList = new();

        UnitInfoClass unitInfoClass = new();

        unitInfoClass.UnitIndexId = VirusIndex;

        var randomPosX = 19;

        var randomPosY = Random.Range(0, 10);

        // 生成外尔战士逻辑位置
        unitInfoClass.UnitPos = new Vector2(randomPosX, randomPosY);

        // 根据外尔战士的逻辑位置计算外尔战士的实际位置
        unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(randomPosX, randomPosY);

        unitInfoClass.CreateUnit = true;

        VirusInfo.Add(VirusIndex, unitInfoClass);

        unitInfoUpdateClassList.Add(unitInfoClass);

        CommunicationManager.VirusInfoCreate(unitInfoUpdateClassList);

        VirusIndex++;

    }

    /// <summary>
    /// 技能创建外尔战士
    /// </summary>
    public void SkillCreateNewVirus() 
    {
        for (int i = 0; i < 10; i ++) 
        {
            List<UnitInfoClass> unitInfoUpdateClassList = new();

            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = VirusIndex;

            if (!ArriveUnit.ContainsKey(VirusIndex))
            {
                ArriveUnit.Add(VirusIndex, VirusIndex);
            }

            var randomPosX = this.GetTetrisBlocksManager()._width;

            var randomPosY = i;

            // 生成外尔战士逻辑位置
            unitInfoClass.UnitPos = new Vector2(randomPosX, randomPosY);

            // 根据外尔战士的逻辑位置计算外尔战士的实际位置
            unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(randomPosX, randomPosY);

            unitInfoClass.CreateUnit = true;

            VirusInfo.Add(VirusIndex, unitInfoClass);

            unitInfoUpdateClassList.Add(unitInfoClass);

            CommunicationManager.VirusInfoCreate(unitInfoUpdateClassList);

            VirusIndex++;
        }
    }

    /// <summary>
    /// 更新外尔战士信息方法
    /// </summary>
    private void UpdateVirus()
    {
        List<UnitInfoClass> unitInfoUpdateClassList = new();

        foreach (int key in VirusInfo.Keys)
        {
            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = key;

            var randomPosX = VirusInfo[key].UnitPos.x;

            var target = this.GetTetrisBlocksManager()._width;

            // 在兵线位置上不前进
            if (VirusInfo[key].UnitPos.x != target)
            {
                if (VirusInfo[key].UnitPos.x - target < 2)
                {
                    // 每次前进两格
                    randomPosX = VirusInfo[key].UnitPos.x - 1;
                }
                else
                {
                    randomPosX = VirusInfo[key].UnitPos.x - 2;
                }
            }

            var randomPosY = VirusInfo[key].UnitPos.y;

            VirusInfo[key].UnitPos.x = randomPosX;

            // 更新外尔战士逻辑位置
            unitInfoClass.UnitPos = new Vector2(randomPosX, randomPosY);

            unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(randomPosX, randomPosY);

            VirusInfo[key].UnitPosUse = unitInfoClass.UnitPosUse;

            unitInfoClass.CreateUnit = false;

            unitInfoUpdateClassList.Add(unitInfoClass);
        }

        CommunicationManager.BlockUpdate(deleteBlocksClassList, true);

        deleteBlocksClassList = unitInfoUpdateClassList;

        CommunicationManager.VirusInfoUpdate(unitInfoUpdateClassList);

    }

    /// <summary>
    /// 外耳战士到达事件
    /// </summary>
    private void OnVirusArrive(int info)
    {
        if (!ArriveUnit.ContainsKey(info))
        {
            ArriveUnit.Add(info, info);
        }

        if (VirusInfo.ContainsKey(info)) 
        {
            List<UnitInfoClass> unitInfo = new();

            unitInfo.Add(VirusInfo[info]);

            CommunicationManager.BlockUpdate(unitInfo, false);
        }
    }

    /// <summary>
    /// 控制兵线向左
    /// </summary>
    public void FireLeft()
    {
        this.GetTetrisBlocksManager().RowLeft();

        WinJudge();
    }

    /// <summary>
    /// 兵线左侧士兵销毁事件
    /// </summary>
    private void KillCells() 
    {
        var allUnit = this.GetTetrisBlocksManager()._AllTetrisDic;

        var _width = this.GetTetrisBlocksManager()._width - 1;

        List<int> infoList = new();

        foreach (int info in allUnit.Keys)
        {
            if (_width <= allUnit[info].posx)
            {
                infoList.Add(info);
            }
        }

        CommunicationManager.SetUnitDie(infoList);
    }

    /// <summary>
    /// 控制兵线向右
    /// </summary>
    public void FireRight()
    {

        this.GetTetrisBlocksManager().RowRight();

        WinJudge();

    }

    /// <summary>
    /// 兵线右侧士兵销毁事件
    /// </summary>
    private void KillVirus()
    {
        List<int> infoList = new();

        foreach (int key in VirusInfo.Keys)
        {
            if (VirusInfo[key].UnitPos.x == this.GetTetrisBlocksManager()._width)
            {
                if (ArriveUnit.ContainsKey(key))
                {
                    infoList.Add(key);
                }
            }
        }
        CommunicationManager.SetUnitDie(infoList);
    }

    /// <summary>
    /// 兵线变更事件
    /// </summary>
    private void FireLineChange()
    {
        var lineInfoX = this.GetTetrisBlocksManager()._width;

        var lineInfoY = this.GetTetrisBlocksManager()._height;

        FireLineInfoClass fireLineInfoClass = new FireLineInfoClass();

        fireLineInfoClass.FireLinePosX = PositionTools.CountFireLinePos(lineInfoX, lineInfoY);

        float info;

        if (CellsCountInfo == 0 && VirusCountInfo == 0)
        {
            info = (float)0.5;
        }
        else
        {
            info = (float)CellsCountInfo / (float)(VirusCountInfo + CellsCountInfo);
        }

        fireLineInfoClass.FireLinePosY = info;

        fireLineInfoClass.FireLineCount = timeinfo;

        CommunicationManager.OnFireLineChange(fireLineInfoClass);
    }

    /// <summary>
    /// 输赢判定
    /// </summary>
    public void WinJudge() 
    {
        var lineInfo = this.GetTetrisBlocksManager()._width;

        if (lineInfo > 19) 
        {
            FindObjectOfType<GlobalControlManager>().OnPause();

            Debug.Log("Win");

            // UIManager.Instance.OpenUI(GameFrameWork.UIType.VictoryPage, null);

            // 场景管理类
            var sceneManager = sceneLoader.GetComponent<MainSceneControlManager>();

            // sceneManager.LoadMainBasicScene();
        }

        if (lineInfo < 4)
        {
            FindObjectOfType<GlobalControlManager>().OnPause();

            Debug.Log("Lose");

            // UIManager.Instance.OpenUI(GameFrameWork.UIType.FailPage, null);

            // 场景管理类
            var sceneManager = sceneLoader.GetComponent<MainSceneControlManager>();

            // sceneManager.LoadMainBasicScene();
        }
    }

    /// <summary>
    /// Unit死亡信息接收方法
    /// </summary>
    /// <param name="UnitIndexId"></param>
    private void OnUnitDie(int UnitIndexId)
    {
        if (VirusInfo.ContainsKey(UnitIndexId))
        {
            VirusInfo.Remove(UnitIndexId);
        }
        else
        {
            this.GetTetrisBlocksManager().DeleteAlone(UnitIndexId);
        }
    }

    /// <summary>
    /// 英雄死亡信息接收方法
    /// </summary>
    /// <param name="HeroIndexId"></param>
    private void OnHeroDie(int HeroIndexId)
    {
        this.GetTetrisBlocksManager().DeleteHero(HeroIndexId);
    }


    /// <summary>
    /// 触发boos技能
    /// </summary>
    private void FireSkillBoss(int info)
    {
        if (info == 3)
        {
            SkillCreateNewVirus();
        }
    }

    [Header("自动/手动切换按钮")]
    public GameObject HandAutoGameObject;

    [Header("手动按钮")]
    public GameObject HideButton;

    public bool hand = false;

    /// <summary>
    /// 自动模式与手动模式切换
    /// </summary>
    public void HandAutoChange()
    {
        AudioSystemManager.Instance.PlaySound("Button_Click");

        if (hand)
        {
            HandAutoGameObject.GetComponent<TextMeshProUGUI>().text = "Auto->Hand";

            hand = false;

            HideButton.SetActive(false);
        }
        else
        {
            HandAutoGameObject.GetComponent<TextMeshProUGUI>().text = "Hand->Auto";

            hand = true;

            HideButton.SetActive(true);
        }
        
    }
}
