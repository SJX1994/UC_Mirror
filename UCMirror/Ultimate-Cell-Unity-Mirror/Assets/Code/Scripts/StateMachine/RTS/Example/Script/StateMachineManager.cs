using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;
using UnityEngine.Events;
public class StateMachineManager : Singleton<StateMachineManager>
{
    #region 数据对象
    // 通讯对象
    // private GameObject sceneLoader;
    // private CommunicationInteractionManager CommunicationManager;
    // private BroadcastClass broadcastClass;
    private Dictionary<int, UnitInfoClass> unitInfoState = new();
    private Dictionary<int, UnitInfoClass> VirusInfoState = new();
    public UnityAction<SoldierBehaviors> OnSyntheticEvent;// 合成事件
    public UnityAction<SoldierBehaviors> OnUnitCreat;// 单位创建事件
    public UnityAction<SoldierBehaviors> OnUnitDied;// 单位死亡事件
    public UnityAction<SoldierBehaviors, int> OnUnitDying;// 单位濒死事件
    MechanismInPut mechanismInPut;
    // 逻辑对象
    [Header("战士单位")]
    public Unit cellUnit;
    public Unit virusUnit;
    private Dictionary<int, Unit> cellUnits = new();
    private Dictionary<int, Unit> virusUnits = new();
    private Dictionary<int, Unit> heroUnitsInfo = new();
    [Header("英雄单位")]
    DragUIItem[] heroUI;
    public GameObject CreatFail;
    public List<GameObject> heroUnits;
    public RectTransform heroPanel, backgroundPanel, heroName, sloganPanel, warningPanel;
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    float m_displayTime = 1.5f;
    GameObject bgTemp;
    GameObject heroTemp;
    Tween tweenAnimation;
    GameObject heroCreating;
    [Header("敌方头目")]
    public List<GameObject> bossUnits;
    [Header("灵魂管理")]
    public TextMeshProUGUI soulTextUI;
    public TextMeshProUGUI exTextUI;
    #endregion 数据对象
    #region 数据关系

    void Start()
    {
        // // 暂时获取方式
        // if (GameObject.Find("LanNetWorkManager") == null)
        // {
        //     return;
        // }
        // sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;
        // // 全局通信方法管理
        // CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();
        // // 全局通信事件注册类
        // broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // // 生成砖块信息监听
        // broadcastClass.OnCellUnitCreate += OnListenCellsUnitCreate;

        // // 更新砖块信息监听
        // // broadcastClass.TetrisInfoUpdate += OnListenCellUnitUpdate;

        // // Unit信息整体更新监听
        // broadcastClass.UnitInfoAllUpdate += OnListenCellUnitUpdate;

        // // 外尔新建监听
        // broadcastClass.VirusInfoCreate += OnlistenVirusCreate;

        // // 外尔信息更新监听
        // broadcastClass.VirusInfoUpdate += OnlistenVirusUpdate;

        // // 英雄生成监听
        // // broadcastClass.OnHeroCreate += OnHeroCreate;

        // // Unit回退事件
        // broadcastClass.OnUnitBack += OnUnitBack;

        // // 删除 - 兵线向左/向右移动删除一整列Unit事件
        // // broadcastClass.SetUnitDie += SetUnitDie;

        // // 获取所有英雄UI
        // heroUI = transform.GetComponentsInChildren<DragUIItem>();
        // foreach (DragUIItem item in heroUI)
        // {
        //     item.OnCancel += OnHeroUICancel;
        // }
        // 触发刷新
        mechanismInPut = FindObjectOfType<MechanismInPut>();
    }

    /// <summary>
    /// 死亡触发事件
    /// </summary>
    /// <param name="UnitId"></param>
    private void SetUnitDie(List<int> UnitIdList)
    {
        /*AICommand newCommand = new AICommand(AICommand.CommandType.Die);

        foreach (int UnitId in UnitIdList)
        {
            if (cellUnits.ContainsKey(UnitId))
            {
                cellUnits[UnitId].ExecuteCommand(newCommand);
            }
            else if (virusUnits.ContainsKey(UnitId))
            {
                virusUnits[UnitId].ExecuteCommand(newCommand);
            }
            else if (heroUnitsInfo.ContainsKey(UnitId))
            {
                heroUnitsInfo[UnitId].ExecuteCommand(newCommand);
            }

        }*/
    }
    private void OnBossCreate()
    {
        // 假数据
        Vector3 pos = Vector3.one;
        // 假数据

    }
    /// <summary>
    /// 英雄生成
    /// </summary>
    /// <param name="infoList"></param>
    public void OnHeroCreate(HeroCreateClass infoList, int indexId)
    {

        // 生成英雄
        string infoHeroName = infoList.HeroInfoClass.HeroName;

        foreach (GameObject heroUnit in heroUnits)
        {
            string heroNameInList = heroUnit.GetComponent<IHeroUnit>().OnCreating().heroName;

            if (infoHeroName == heroNameInList)
            {
                heroCreating = heroUnit;
                ShowHeroPanel();
                //heroCreating = heroUnits[1];
                HeroToCreate(heroCreating, infoList.CreatePos, indexId);
            }
            else
            {
                continue;
            }
        }

        // // 聚拢战士单位在英雄生成位置
        // foreach (UnitInfoClass info in infoList.CellIndexList)
        // {
        //     if (unitInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
        //     {
        //         unitInfoState[info.UnitIndexId] = info;
        //         StartCoroutine(MoveTo(info,new Vector3((float)-15, (float)0.6, (float)-8)));
        //     }
        // }



    }
    /// <summary>
    /// 外尔生成监听
    /// </summary>
    /// <param name="infoList"></param>
    void OnlistenVirusCreate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (!VirusInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                VirusInfoState.Add(info.UnitIndexId, info);

                // Debug.Log("外尔信息接收成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
                // 根据 位置创建 单位
                // CreateVirusUnit(info);
            }
            else
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }

    /// <summary>
    /// 外尔信息更新监听
    /// </summary>
    /// <param name="infoList"></param>
    void OnlistenVirusUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            // Debug.Log("外尔信息更新成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            // 根据 ID更新 单位
            UpdateVirusUnit(info);
        }

    }

    /// <summary>
    /// 砖块生成监听
    /// </summary>
    /// <param name="info"></param>
    void OnListenCellsUnitCreate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (!unitInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                unitInfoState.Add(info.UnitIndexId, info);

                //Debug.Log("信息接收成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
                // 根据 位置创建 单位
                CreateCellUnit(info);
            }
            else
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }
    /// <summary>
    /// 砖块信息更新监听
    /// </summary>
    /// <param name="info"></param>
    void OnListenCellUnitUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            //Debug.Log("信息接收成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "Tetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            // 根据 ID更新 单位
            UpdateCellUnit(info);
        }
    }


    /// <summary>
    /// 回收战士方法
    /// </summary>
    /// <param name="CellIndexList"></param>
    void OnUnitBack(List<UnitInfoClass> CellIndexList)
    {
        // 聚拢战士单位在英雄生成位置
        foreach (UnitInfoClass info in CellIndexList)
        {
            if (unitInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
            {
                unitInfoState[info.UnitIndexId] = info;
                StartCoroutine(MoveTo(info, new Vector3((float)-15, (float)0.6, (float)-8)));
            }
        }

    }

    /// <summary>
    /// Unit死亡时的数据通信
    /// </summary>
    /// <param name="UnitIndexId"></param>
    void OnUnitDie(int UnitIndexId)
    {
        // CommunicationManager.UnitDieInfoProcess(UnitIndexId);
    }
    #endregion 数据关系
    #region 数据操作
    IEnumerator MoveTo(UnitInfoClass info, Vector3 position)
    {
        // 更新单位
        if (cellUnits.TryGetValue(info.UnitIndexId, out Unit cell))
        {
            if (cell.OnStartCollect != null)
            {
                cell.OnStartCollect(cell);

            }
            yield return new WaitForSeconds(0.2f);
            cell.targetPos = position;
            AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndIdle, position);
            cell.ExecuteCommand(newCommand);
            yield return new WaitForSeconds(5);
            cell.unitSoul = null;
            AICommand newCommand_Collecting = new AICommand(AICommand.CommandType.Collecting);
            cell.ExecuteCommand(newCommand_Collecting);
            yield return new WaitForSeconds(1);

        }
    }
    /// <summary>
    /// 展示 英雄生成口号UI
    /// </summary>
    void ShowHeroPanel()
    {
        ClearHeroPanel();
        heroPanel.gameObject.SetActive(true);
        backgroundPanel.gameObject.SetActive(true);
        sloganPanel.gameObject.SetActive(true);
        heroName.gameObject.SetActive(true);
    }
    /// <summary>
    /// 隐藏 英雄生成口号UI
    /// </summary>
    void HideHeroPanel()
    {
        heroPanel.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
        sloganPanel.gameObject.SetActive(false);
        heroName.gameObject.SetActive(false);
        ClearHeroPanel();
    }
    /// <summary>
    /// 清理 英雄生成口号UI
    /// </summary>
    void ClearHeroPanel()
    {
        bgTemp = null;
        heroTemp = null;
        tweenAnimation.Kill();
        foreach (Transform item in heroPanel)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in backgroundPanel)
        {
            Destroy(item.gameObject);
        }
    }
    /// <summary>
    /// 生成英雄逻辑
    /// </summary>
    /// <param name="heroToCreate"></param>
    void HeroToCreate(GameObject heroToCreate, Vector3 createdPosition, int indexId)
    {
        if (heroToCreate.TryGetComponent(out IHeroUnit ihero))
        {
            UnitHeroTemplate uht = ihero.OnCreating();
            heroTemp = Instantiate(uht.hreoSkeletonGraphic.gameObject, heroPanel, false);
            bgTemp = Instantiate(uht.backgroundSkeletonGraphic.gameObject, backgroundPanel, false);
            sloganPanel.GetComponent<Text>().text = uht.slogan;
            heroName.GetComponent<Text>().text = uht.heroName;

            // 假数据
            Vector3 posCreateMoveTo = new Vector3(7, 0, 0);
            // createdPosition = new Vector3((float)-14.5 + createdPosition.x * (float)1.5, (float)0.6, (float)-8.5 + createdPosition.y * (float)1.5);
            DoAnimationHeroCreate(heroToCreate, heroTemp.GetComponent<RectTransform>(), createdPosition, posCreateMoveTo, indexId);
        }
    }
    /// <summary>
    /// 英雄战败逻辑
    /// </summary>
    /// <param name="heroToDefeated"></param>
    void HeroToDefeated(GameObject heroToDefeated)
    {
        if (heroToDefeated.TryGetComponent(out IHeroUnit ihero))
        {
            if (!ihero.OnDefeated().fininsh)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
    /// <summary>
    /// 英雄生成动画
    /// </summary>
    /// <param name="ihero"></param>
    void DoAnimationHeroCreate(GameObject heroWorldSpace, RectTransform ihero, Vector3 posCreate, Vector3 posCreateMoveTo, int indexId)
    {
        if (ihero)
        {
            Time.timeScale = 0;
            tweenAnimation = ihero.DOAnchorPos(new Vector2(1500f, 0f), m_displayTime)
                .ChangeStartValue(new Vector2(-1500f, 0f))
                .SetEase(curve);
            tweenAnimation.SetUpdate(true);
            tweenAnimation.onComplete = () =>
            {
                Time.timeScale = 1;
                HideHeroPanel();
                CreateHeroInWorldSpace(heroWorldSpace, posCreate, posCreateMoveTo, indexId);
            };
        }

    }
    /// <summary>
    /// 在世界空间下生成英雄
    /// </summary>
    /// <param name="posCreate"></param>
    /// <param name="posCreateMoveTo"></param>
    void CreateHeroInWorldSpace(GameObject heroWorldSpace, Vector3 posCreate, Vector3 posCreateMoveTo, int indexId)
    {
        GameObject hero = Instantiate(heroWorldSpace, posCreate, Quaternion.identity);
        if (hero.TryGetComponent<Unit>(out Unit ihero))
        {
            ihero.id = indexId;
            ihero.OnDie += HeroDie;
            heroUnitsInfo.Add(indexId, ihero);
        }
        // float diffectSize = Random.Range(-0.2f,0.01f);
        float diffectSize = 0f;
        hero.transform.localScale = new Vector3(hero.transform.localScale.x + diffectSize, hero.transform.localScale.y + diffectSize, hero.transform.localScale.z + diffectSize);
        hero.SetActive(true);
        // StartCoroutine(hero.GetComponent<IHeroUnit>().WhenCreatMoveTo(posCreateMoveTo));
    }

    /// <summary>
    /// 更新外尔战士生成事件
    /// </summary>
    /// <param name="info"></param>
    public void UpdateVirusUnit(UnitInfoClass info)
    {
        // 更新单位
        if (virusUnits.TryGetValue(info.UnitIndexId, out Unit virus))
        {
            AICommand newCommand = new AICommand(AICommand.CommandType.AttackTarget, info.UnitPosUse);
            virus.ExecuteCommand(newCommand);
        }
    }

    /// <summary>
    /// 外尔战士生成事件
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    // public Unit CreateVirusUnit(UnitInfoClass info)
    // {
    //     // 生成单位
    //     Unit virus = Instantiate(virusUnit, info.UnitPosUse, Quaternion.identity);

    //     virus.SkeletonRenderer.Skeleton.SetSkin(info.color.ToString());
    //     Weapon weapon;
    //     if (virus.SkeletonRenderer.transform.TryGetComponent(out weapon))
    //     {
    //         weapon.SetWeapon(info.color);
    //         weapon.OnChangeWeapon += CellWeaponChange;
    //     }

    //     virus.id = info.UnitIndexId;
    //     virus.OnDie += VirusDie;
    //     virus.OnArrive += VirusArrive;
    //     virusUnits.Add(info.UnitIndexId, virus);

    //     return virus;
    // }

    /// <summary>
    /// 生成外尔战士Boss
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public Unit CreateVirusUnitForBoss(UnitInfoClass info)
    {
        // 生成单位
        Unit virus = Instantiate(virusUnit, info.UnitPosUse, Quaternion.identity);
        virus.id = info.UnitIndexId;
        virusUnits.Add(info.UnitIndexId, virus);
        return virus;
    }

    /// <summary>
    /// 新建塞尔生成事件
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public Unit CreateCellUnit(UnitInfoClass info)
    {

        // 生成单位
        Unit cell = Instantiate(cellUnit, new(info.UnitPosUse.x - 3f, info.UnitPosUse.y, info.UnitPosUse.z), Quaternion.identity);
        // OnUnitCreat.Invoke(cell.GetComponent<Soldier>());
        // 假数据 ---
        // string[] skinName = {"red","green","blue","purple"};
        // cell.skinName = skinName[Random.Range(0, 4)];
        // info.color = EventType.UnitColor.red; // TODO 更多武器
        // 假数据 ---
        if (info.UnitLevel == 0)
        {
            info.UnitLevel = 1;
        }
        if (info.UnitLevel > 1)
        {
            SoldierBehaviors soldier = cell.GetComponent<SoldierBehaviors>();
            mechanismInPut.modeTest = MechanismInPut.ModeTest.Morale;
            soldier.morale.AddMorale(soldier, 3.2f, true);
            soldier.morale.EffectByMorale(soldier, ref soldier.strength);
            // AICommand move = new AICommand(AICommand.CommandType.GoToAndGuard, info.UnitPosUse);
            // cell.ExecuteCommand(move);
            // 机制事件刷新
            OnSyntheticEvent?.Invoke(soldier);
            Invoke(nameof(ReflashMechanismEnd), 0.1f);
        }
        cell.level = info.UnitLevel;

        cell.SkeletonRenderer.Skeleton.SetSkin(info.color.ToString());
        Weapon weapon;
        if (cell.SkeletonRenderer.transform.TryGetComponent(out weapon))
        {
            // weapon.SetWeapon(info.color);
            weapon.OnChangeWeapon += CellWeaponChange;
        }

        cell.id = info.UnitIndexId;
        cell.OnDie += CellDie;
        cell.OnArrive += CellArrive;
        cellUnits.Add(info.UnitIndexId, cell);

        return cell;
    }

    /// <summary>
    /// 更新塞尔事件
    /// </summary>
    /// <param name="info"></param>
    public void UpdateCellUnit(UnitInfoClass info)
    {
        // 更新单位
        if (cellUnits.TryGetValue(info.UnitIndexId, out Unit cell))
        {
            Vector3 temp_pos = info.UnitPosUse;
            cell.targetPos = temp_pos;
            AICommand newCommand = new(AICommand.CommandType.GoToAndGuard, temp_pos);
            cell.ExecuteCommand(newCommand);
        }
    }
    /// <summary>
    /// 外尔到达指定位置事件 订阅者
    /// </summary>
    /// <param name="whichVirus"></param>
    void VirusArrive(Unit whichVirus)
    {
        // CommunicationManager.OnVirusArrive(whichVirus.id);
    }
    /// <summary>
    /// 塞尔到达指定位置事件 订阅者
    /// </summary>
    /// <param name="whichCell">细胞到达事件</param>
    void CellArrive(Unit whichCell)
    {
        // CommunicationManager.OnCellArrive(whichCell.id);
    }
    /// <summary>
    /// 塞尔单位死亡事件 订阅者
    /// </summary>
    /// <param name="whoDied">塞尔单位</param>
    void CellDie(Unit whichCell)
    {
        OnUnitDie(whichCell.id);
    }
    /// <summary>
    /// 外尔单位死亡事件 订阅者
    /// </summary>
    /// <param name="whoDied">外尔单位</param>
    void VirusDie(Unit whichVirus)
    {
        OnUnitDie(whichVirus.id);
    }
    /// <summary>
    /// 英雄死亡事件 订阅者
    /// </summary>
    /// <param name="whichHero">英雄单位</param>
    void HeroDie(Unit whichHero)
    {
        // CommunicationManager.OnHeroDie(whichHero.id);

        if (heroUnitsInfo.ContainsKey(whichHero.id))
        {
            heroUnitsInfo.Remove(whichHero.id);
        }
    }
    /// <summary>
    /// UI取消事件 订阅者
    /// </summary>
    /// <param name="whichHeroUI">英雄对应的UI</param>
    void OnHeroUICancel(DragUIItem whichHeroUI)
    {
        // CommunicationManager.OnHeroUICancel(whichHeroUI.heroName);
    }
    /// <summary>
    /// 塞尔战士切换武器
    /// </summary>
    /// <param name="weapon"></param>
    void CellWeaponChange(Weapon weapon)
    {

    }
    void ReflashMechanismEnd()
    {
        mechanismInPut.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
        mechanismInPut.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }

    #endregion 数据操作
}
