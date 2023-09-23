using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;
using DG.Tweening;
public class PuppetUnit : MonoBehaviour
{
    // 启动模式
    public bool ClientStart;

    // 管理类
    public PuppetLineManager pupperManager;
    // 果实组件
    public List<FruitDataStructureTemplate> fruits;
    readonly List<FruitDataStructureTemplate> fruitsTemp;
    // 通讯组件
    public int puppetId; // 配对ID
    [HideInInspector]
    public PuppetMouseState puppetMouseState;
    // [HideInInspector]
    public PuppetLine puppetLine;
    [HideInInspector]
    public Vector3 puppetTargetPosition;
    // 表现组件
    // 特效
    public ParticleSystem positiveEffect;
    public ParticleSystem nagetiveEffect;
    public List<ParticleSystem> baseChainTransferboonEffectPrefabs;
    public List<Material> baseChainTransferboonMats;
    Material baseChainTransferboonMat;
    ParticleSystem baseChainTransferboonEffectPrefab;
    public List<Material> fourDirectionsLinkLinkMats;
    Material puppetFourDirectionsLinkLinkMat;
    LineRenderer NorthPuppet, EastPuppet, SouthPuppet, WestPuppet; // 用于存储连线
    public List<Material> weakAssociationMats;
    Material weakAssociationMat;
    LineRenderer weakAssociationLineRenderer;
    Vector3[] weakAssociationLineRendererPoints; // 抛物线上的所有点
    Vector3 associationTarget; // 抛物线 终点
    Vector3 associationSource; // 抛物线 起点

    public Color colorsCell = Color.white;
    public Color colorsOpponent = Color.white;
    string skinName;
    SkeletonRenderer skeletonRenderer;
    SpriteRenderer selectionCircle;
    Animator animator;
    Renderer spineRenderer;
    Weapon weapon;
    GameObject shooter;
    Vector3 targetPosition;
    MaterialPropertyBlock mpb;
    MaterialPropertyBlock spinePropertyBlock;
    MechanismInPut.ModeTest puppetUnitModeTest;
    public UnitTemplate.UnitType puppetUnitType;

    // Start is called before the first frame update
    void Start()
    {

        Transform spine = transform.Find("Spine");
        weapon = spine.GetComponent<Weapon>();
        animator = spine.GetComponent<Animator>();
        skeletonRenderer = spine.GetComponent<SkeletonRenderer>();
        selectionCircle = transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
        spineRenderer = spine.GetComponent<Renderer>();
        positiveEffect = transform.Find("FourDirLinkGoodEffect").GetComponent<ParticleSystem>();
        nagetiveEffect = transform.Find("FourDirLinkBadEffect").GetComponent<ParticleSystem>();

        animator.speed += Random.Range(-0.15f, 0.15f);
        targetPosition = transform.position;

        // 接收事件
        Invoke(nameof(LateStart), 0.1f);
        if (!ClientStart)
        {
            puppetLine.OnPuppetTypeChanged += (UnitTemplate.UnitType type) => { puppetUnitType = type; };
            puppetLine.OnPuppetChangeWeapon += OnChangeWeapon;
            puppetLine.OnPuppetSkinChanged += OnChangeSkin;
            puppetLine.OnPuppetSide += OnSide;
            puppetLine.OnPuppetHealthChange += (float shaderHP) => { UpdateMatHealth(shaderHP); };
            puppetLine.OnPuppetMechModeChange += (MechanismInPut.ModeTest mode) => { puppetUnitModeTest = mode; };
            puppetLine.OnPuppetPlayEffect += OnPlayEffect;

        }
        // 发送事件
        puppetMouseState = transform.GetComponent<PuppetMouseState>();
        puppetMouseState.puppetId = this.puppetId;
        // 初始化
        Init();

    }
    void LateStart()
    {

        if (!ClientStart)
        {
            puppetId = puppetLine.id;
        }
        if (!weapon.shooter)
        {
            shooter = weapon.transform.Find("Shooter").gameObject;
            weapon.shooter = shooter.GetComponent<SpriteRenderer>();

        }
        else
        {
            shooter = weapon.shooter.gameObject;
        }
        shooter.SetActive(false);

        if (!ClientStart)
        {
            puppetLine.OnPuppetPositionChange += OnUnitPositionChanged;
            puppetLine.OnPuppetScaleChanged += OnUnitScaleChanged;
            puppetLine.OnPuppetDestory += OnDie;
            puppetLine.OnPuppetFilp += OnFilp;
            puppetLine.OnPuppetStateChanged += OnStateChanged;
            puppetLine.OnPuppetSpeedChange += OnSpeedChanged;
            puppetLine.OnPuppetTargetOfAttackChange += OnTargetOfAttackChange;
            puppetLine.OnPuppetAttacking += OnAttacking;
            puppetLine.OnPuppetAttackFinish += OnAttackFinish;
            puppetLine.OnPuppetShooting += OnShooting;
            // 初始位置
            transform.position = puppetLine.transform.position;
        }

        // 初始血量
        UpdateMatHealth(1.0f);

        if (ClientStart)
        {
            pupperManager.PuppetCreateDone(puppetId, this.gameObject);

            Debug.Log(this.puppetId + ": 木偶新建完成");
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.transform.rotation = Quaternion.Euler(new Vector3(80.00f, 0.00f, 0.00f));
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #region 数据操作
    void Init()
    {
        // 初始化果实
        if (fruits.Count > 0)
        {
            foreach (FruitDataStructureTemplate fruit in fruits)
            {
                FruitDataStructureTemplate fruitTemp;
                fruitTemp = Instantiate<FruitDataStructureTemplate>(fruit);
                fruitsTemp.Add(fruitTemp);
            }
            fruits.Clear();
            fruits = fruitsTemp;
        }
        // 初始化基础特效
        switch (skinName)
        {
            case "red":
                baseChainTransferboonMat = baseChainTransferboonMats.Find(x => x.name == "FourDirLinkEffect_red");
                baseChainTransferboonEffectPrefab = baseChainTransferboonEffectPrefabs.Find(x => x.name == "BoomEffect_red");
                puppetFourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x => x.name == "FourDirLinkLine_red");
                weakAssociationMat = weakAssociationMats.Find(x => x.name == "FourDirLinkEffect_red");
                break;
            case "blue":
                baseChainTransferboonMat = baseChainTransferboonMats.Find(x => x.name == "FourDirLinkEffect_blue");
                baseChainTransferboonEffectPrefab = baseChainTransferboonEffectPrefabs.Find(x => x.name == "BoomEffect_blue");
                puppetFourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x => x.name == "FourDirLinkLine_blue");
                weakAssociationMat = weakAssociationMats.Find(x => x.name == "FourDirLinkEffect_blue");
                break;
            case "green":
                baseChainTransferboonMat = baseChainTransferboonMats.Find(x => x.name == "FourDirLinkEffect_green");
                baseChainTransferboonEffectPrefab = baseChainTransferboonEffectPrefabs.Find(x => x.name == "BoomEffect_green");
                puppetFourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x => x.name == "FourDirLinkLine_green");
                weakAssociationMat = weakAssociationMats.Find(x => x.name == "FourDirLinkEffect_green");
                break;
            case "purple":
                baseChainTransferboonMat = baseChainTransferboonMats.Find(x => x.name == "FourDirLinkEffect_purple");
                baseChainTransferboonEffectPrefab = baseChainTransferboonEffectPrefabs.Find(x => x.name == "BoomEffect_purple");
                puppetFourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x => x.name == "FourDirLinkLine_purple");
                weakAssociationMat = weakAssociationMats.Find(x => x.name == "FourDirLinkEffect_purple");

                break;

        }

    }

    public void OnShooting(bool isShooting, Vector3 targetPosition)
    {
        shooter.SetActive(isShooting);
        if (isShooting)
        {
            // weapon.Shoot(targetPosition);
        }
    }

    public void UpdateMatHealth(float SetFloat)
    {
        if (spinePropertyBlock == null)
        {
            spinePropertyBlock = new MaterialPropertyBlock();
        }
        spineRenderer.GetPropertyBlock(spinePropertyBlock);
        spinePropertyBlock.SetFloat("_Porcess", SetFloat);
        spineRenderer.SetPropertyBlock(spinePropertyBlock);
    }

    public void OnSide(UnitTemplate.UnitType type)
    {
        bool isOpponent;
        if (type == UnitTemplate.UnitType.Cell)
        {
            isOpponent = false;
        }
        else
        {
            isOpponent = true;
        }
        Transform spine = transform.Find("Spine");
        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", isOpponent ? colorsOpponent : colorsCell);
        spine.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
    }
    public void OnTargetOfAttackChange(Vector3 attackPos)
    {
        puppetTargetPosition = attackPos;
    }

    public void OnChangeWeapon(WeaponTemplate.WeaponType weaponType)
    {
        weapon.SetWeapon(weaponType);
        weapon.Attach();
    }

    public void OnChangeSkin(string skinName)
    {
        this.skinName = skinName;
        skeletonRenderer.Skeleton.SetSkin(skinName);
    }

    public void OnSpeedChanged(float speed)
    {
        animator.SetFloat("Speed", speed * .05f);
    }

    public void OnStateChanged(Unit.UnitState state, Vector3 targetPos)
    {
        this.targetPosition = targetPos;

        switch (state)
        {
            case Unit.UnitState.Idle:

                break;
            case Unit.UnitState.Guarding:
                break;
            case Unit.UnitState.Attacking:
                // animator.SetTrigger("DoAttack");
                break;
            case Unit.UnitState.MovingToTarget:
                break;
            case Unit.UnitState.MovingToSpotIdle:
                break;
            case Unit.UnitState.MovingToSpotGuard:
                break;
            case Unit.UnitState.Dead:
                // animator.SetTrigger("DoDeath");
                break;
            case Unit.UnitState.MovingToSpotWithGuard:
                break;

        }
    }
    public void OnAttacking()
    {
        animator.SetTrigger("DoAttack");
    }
    public void OnAttackFinish()
    {
        animator.SetTrigger("InterruptAttack");
    }
    public void OnFilp(bool puppetFilp)
    {
        if (puppetFilp)
        {
            skeletonRenderer.skeleton.ScaleX = -1f;
        }
        else
        {
            skeletonRenderer.skeleton.ScaleX = 1f;
        }
    }
    public void OnDie(float delayTime)
    {
        animator.SetTrigger("DoDeath");
        if (!ClientStart)
        {
            puppetLine.OnPuppetPositionChange -= OnUnitPositionChanged;
            puppetLine.OnPuppetDestory -= OnDie;
            puppetLine.OnPuppetFilp -= OnFilp;
            puppetLine.OnPuppetStateChanged -= OnStateChanged;
            puppetLine.OnPuppetSpeedChange -= OnSpeedChanged;
            puppetLine.OnPuppetTargetOfAttackChange -= OnTargetOfAttackChange;
            puppetLine.OnPuppetAttacking -= OnAttacking;
            puppetLine.OnPuppetAttackFinish -= OnAttackFinish;
        }
        puppetMouseState.OnMouseEnterPuppet = null;
        puppetMouseState.OnMouseExitPuppet = null;
        if (NorthPuppet) Destroy(NorthPuppet);
        if (EastPuppet) Destroy(EastPuppet);
        if (SouthPuppet) Destroy(SouthPuppet);
        if (WestPuppet) Destroy(WestPuppet);
        Destroy(gameObject);
    }
    public void OnUnitScaleChanged(Vector3 puppetScale)
    {
        skeletonRenderer.transform.localScale = puppetScale;
    }
    public void OnUnitPositionChanged(Vector3 puppetPosition)
    {
        transform.position = puppetPosition;
        this.targetPosition = puppetPosition;
    }
    // 实现基础机制接口
    public void OnPlayEffect(PuppetEffectDataStruct data)
    {
        switch (puppetUnitModeTest)
        {
            case MechanismInPut.ModeTest.Morale:

                break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:

                break;
            case MechanismInPut.ModeTest.WeakAssociation:
                switch (data.effectType)
                {
                    case PuppetEffectDataStruct.EffectType.Positive:
                        positiveEffect.Play();
                        break;
                    case PuppetEffectDataStruct.EffectType.WeakAssociationStart:

                        // Line表现
                        if (!weakAssociationMat)
                        {
                            Init();
                        }
                        if (!weakAssociationMat) return;

                        GameObject lineObject = new("WeakAssociationLineRenderer_" + puppetId);
                        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.positionCount = 2; // 设置线段的顶点数
                        associationSource = transform.position;
                        associationTarget = data.targetPosition;
                        lineRenderer.SetPosition(0, transform.position); // 设置起点坐标
                        lineRenderer.SetPosition(1, data.targetPosition); // 设置终点坐标
                        lineRenderer.startColor = Color.red; // 设置线段起点颜色
                        lineRenderer.endColor = Color.blue; // 设置线段终点颜色
                        lineRenderer.startWidth = 0.1f; // 设置线段起点宽度
                        lineRenderer.endWidth = 0.1f; // 设置线段终点宽度
                        lineRenderer.numCapVertices = 6; // 设置线段端点的圆滑度
                        if (SortingLayer.NameToID(data.stringInfo) != 0)
                        {
                            int sortingLayerID = SortingLayer.NameToID(data.stringInfo);
                            lineRenderer.sortingLayerID = sortingLayerID;
                            lineRenderer.sortingOrder = 100;
                        }

                        Renderer lineRendererRenderer = lineRenderer.GetComponent<Renderer>();

                        lineRendererRenderer.material = weakAssociationMat;

                        weakAssociationLineRenderer = lineRenderer;
                        break;
                    case PuppetEffectDataStruct.EffectType.WeakAssociationUpdate:
                        if (!weakAssociationLineRenderer) return;

                        // 创建抛物线上的所有点
                        int numberOfPoints = 21;
                        weakAssociationLineRendererPoints = new Vector3[numberOfPoints];
                        for (int i = 0; i < numberOfPoints; i++)
                        {
                            float t = i / (float)(numberOfPoints - 1);
                            weakAssociationLineRendererPoints[i] = CalculatePoint(t);
                        }

                        // 更新 LineRenderer 组件
                        weakAssociationLineRenderer.positionCount = numberOfPoints;
                        weakAssociationLineRenderer.SetPositions(weakAssociationLineRendererPoints);
                        break;
                }
                break;
            case MechanismInPut.ModeTest.ChainTransfer:
                switch (data.effectType)
                {
                    case PuppetEffectDataStruct.EffectType.Positive:
                        positiveEffect.Play();
                        break;
                    case PuppetEffectDataStruct.EffectType.ChainTransferBoom:
                        if (baseChainTransferboonEffectPrefab == null)
                        {
                            Init();
                        }

                        // 粒子
                        GameObject boonEffect = Instantiate(baseChainTransferboonEffectPrefab.gameObject);
                        boonEffect.transform.position = data.targetPosition;
                        boonEffect.GetComponent<ParticleSystem>().Play();

                        Destroy(boonEffect, data.duration);
                        break;
                    case PuppetEffectDataStruct.EffectType.ChainTransferTrail:
                        if (baseChainTransferboonMat == null)
                        {
                            Init();
                        }
                        // 轨迹
                        GameObject obj = new("TrailObject");
                        TrailRenderer trail = obj.AddComponent<TrailRenderer>();
                        trail.startColor = Color.yellow;
                        trail.endColor = Color.red;
                        trail.startWidth = 0.6f;
                        trail.endWidth = 0.6f;
                        trail.time = 2.0f;
                        trail.material = baseChainTransferboonMat;
                        obj.SetActive(false);
                        obj.transform.position = transform.position;
                        obj.SetActive(true);
                        obj.transform.DOMove(data.targetPosition, data.duration).OnComplete(() =>
                        {
                            //obj.SetActive(false);
                        });
                        Destroy(obj, data.duration * 2);
                        break;
                    case PuppetEffectDataStruct.EffectType.FourDirectionsLinkerStart:
                        PuppetUnit linkPuppetUnit = FindObjectsOfType<PuppetUnit>().FirstOrDefault(linkPuppetUnit => data.id == linkPuppetUnit.puppetId);
                        if (linkPuppetUnit != null)
                        {
                            // Line表现
                            GameObject lineObject = new("LineRendererPuppet_" + puppetId + data.direction.ToString());
                            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                            lineRenderer.positionCount = 2; // 设置线段的顶点数
                            lineRenderer.SetPosition(0, transform.position); // 设置起点坐标
                            lineRenderer.SetPosition(1, linkPuppetUnit.transform.position); // 设置终点坐标
                            lineRenderer.startColor = Color.red; // 设置线段起点颜色
                            lineRenderer.endColor = Color.blue; // 设置线段终点颜色
                            lineRenderer.startWidth = 0.5f; // 设置线段起点宽度
                            lineRenderer.endWidth = 0.1f; // 设置线段终点宽度
                            lineRenderer.numCapVertices = 6; // 设置线段端点的圆滑度
                            if (SortingLayer.NameToID(data.stringInfo) != 0)
                            {
                                int sortingLayerID = SortingLayer.NameToID(data.stringInfo);
                                lineRenderer.sortingLayerID = sortingLayerID;
                                lineRenderer.sortingOrder = 99;
                            }
                            Renderer lineRendererRenderer = lineRenderer.GetComponent<Renderer>();
                            if (puppetFourDirectionsLinkLinkMat == null)
                            {
                                Init();
                            }
                            lineRendererRenderer.material = puppetFourDirectionsLinkLinkMat;
                            switch (data.direction)
                            {
                                case FourDirectionsLink.Direction.North:
                                    if (NorthPuppet)
                                    {
                                        Destroy(lineObject);
                                        return;
                                    }
                                    else
                                    {
                                        NorthPuppet = lineObject.GetComponent<LineRenderer>();
                                    }

                                    break;
                                case FourDirectionsLink.Direction.South:
                                    if (SouthPuppet)
                                    {
                                        Destroy(lineObject);
                                        return;
                                    }
                                    else
                                    {
                                        SouthPuppet = lineObject.GetComponent<LineRenderer>();
                                    }

                                    break;
                                case FourDirectionsLink.Direction.East:
                                    if (EastPuppet)
                                    {
                                        Destroy(lineObject);
                                        return;
                                    }
                                    else
                                    {
                                        EastPuppet = lineObject.GetComponent<LineRenderer>();
                                    }
                                    break;
                                case FourDirectionsLink.Direction.West:
                                    if (WestPuppet)
                                    {
                                        Destroy(lineObject);
                                        return;
                                    }
                                    else
                                    {
                                        WestPuppet = lineObject.GetComponent<LineRenderer>();
                                    }
                                    break;
                            }
                        }
                        break;
                    case PuppetEffectDataStruct.EffectType.FourDirectionsLinkerUpdate:
                        switch (data.direction)
                        {
                            case FourDirectionsLink.Direction.North:
                                if (NorthPuppet)
                                {
                                    NorthPuppet.SetPosition(0, transform.position);
                                    NorthPuppet.SetPosition(1, data.targetPosition);
                                }
                                break;
                            case FourDirectionsLink.Direction.South:
                                if (SouthPuppet)
                                {
                                    SouthPuppet.SetPosition(0, transform.position);
                                    SouthPuppet.SetPosition(1, data.targetPosition);
                                }
                                break;
                            case FourDirectionsLink.Direction.East:
                                if (EastPuppet)
                                {
                                    EastPuppet.SetPosition(0, transform.position);
                                    EastPuppet.SetPosition(1, data.targetPosition);
                                }
                                break;
                            case FourDirectionsLink.Direction.West:
                                if (WestPuppet)
                                {
                                    WestPuppet.SetPosition(0, transform.position);
                                    WestPuppet.SetPosition(1, data.targetPosition);
                                }
                                break;
                        }
                        break;
                    case PuppetEffectDataStruct.EffectType.FourDirectionsLinkerEnd:
                        switch (data.direction)
                        {
                            case FourDirectionsLink.Direction.North:
                                if (NorthPuppet)
                                {
                                    Destroy(NorthPuppet.gameObject);
                                    nagetiveEffect.Play();
                                }
                                break;
                            case FourDirectionsLink.Direction.South:
                                if (SouthPuppet)
                                {
                                    Destroy(SouthPuppet.gameObject);
                                    nagetiveEffect.Play();
                                }
                                break;
                            case FourDirectionsLink.Direction.East:
                                if (EastPuppet)
                                {
                                    Destroy(EastPuppet.gameObject);
                                    nagetiveEffect.Play();
                                }
                                break;
                            case FourDirectionsLink.Direction.West:
                                if (WestPuppet)
                                {
                                    Destroy(WestPuppet.gameObject);
                                    nagetiveEffect.Play();
                                }
                                break;
                        }
                        break;
                }

                break;
            case MechanismInPut.ModeTest.Reflash:

                break;
        }
    }
    private Vector3 CalculatePoint(float t)
    {
        float lineHeight = 1.8f; // 抛物线的高度
                                 // 计算抛物线上的单个点
        float x = Mathf.Lerp(associationSource.x, associationTarget.x, t);
        float y = Mathf.Lerp(associationSource.y, associationTarget.y, t);
        float z = Mathf.Lerp(associationSource.z, associationTarget.z, t);

        // 计算抛物线高度
        y += Mathf.Sin(t * Mathf.PI) * lineHeight;

        return new Vector3(x, y, z);
    }



    #endregion 数据操作
}
