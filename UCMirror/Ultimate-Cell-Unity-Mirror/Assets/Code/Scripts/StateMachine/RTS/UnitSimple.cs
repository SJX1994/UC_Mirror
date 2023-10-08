using UnityEngine;
using UC_PlayerData;
using DG.Tweening;

public class UnitSimple : Unit
{
# region 数据对象
    public float durationRunning = 1f;
    public float DurationRunning
    {
        get
        {
            if(!tetriUnitSimple.TetrisBlockSimple)return 0f;
            if(durationRunning!=tetriUnitSimple.TetrisBlockSimple.occupyingTime)durationRunning = tetriUnitSimple.TetrisBlockSimple.occupyingTime;
            return durationRunning;
        }
        set
        {
            durationRunning = value;
        }
    }
    public Material mat;
    public TetriUnitSimple tetriUnitSimple;
    float runningSpeed = 3f;
    public float RunningSpeed
    {
        get
        {
            if(!tetriUnitSimple.TetrisBlockSimple)return 0f;
            if(runningSpeed!=tetriUnitSimple.TetrisBlockSimple.occupyingTime)runningSpeed = 1/tetriUnitSimple.TetrisBlockSimple.occupyingTime;
            return runningSpeed;
        }
    }
    float runningValue = 0f;
    float runningDelay = 1.0f;
    float RunningDelay
    {
        get
        {
            return runningDelay;
        }
    }
    public Tween runningTween;
    // 机制相关
    SoldierBehaviors soldier;
    public SoldierBehaviors Soldier
    {
        get
        {
            if(!soldier)soldier = GetComponent<SoldierBehaviors>();
            return soldier;
        }
    }
    public Vector2 PosId
    {
        get
        {
            return tetriUnitSimple.PosId;
        }
    }
    // 可视化
    Vector3 startPoint;
    Vector3 endPoint;
    public LineRenderer lineRenderer;
    int faceDirection = 1;
    ParticlesController bloodEffectLoad;
    ParticlesController bloodEffect;
    Color bloodColor = Color.clear;
    Vector3 localScale;
    Vector3 LocalScale
    {
        get
        {
            return localScale;
        }
    }
    float originalScaleweapon;
    int originalAttackPower;
    int originalHealth;
# endregion 数据对象
# region 数据关系
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start() {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        soldier = GetComponent<SoldierBehaviors>();
        if(!soldier.WeakAssociation)soldier.Start();
        soldier.WeakAssociation.weakAssociationActive += BlocksMechanismDoing;
        originalScaleweapon = Weapon.WeaponTemplate.scale_weapon;
        originalAttackPower = Weapon.WeaponTemplate.attackPowerBonus;
        originalHealth = Weapon.WeaponTemplate.healthBonus;
        DifferentPlayerInit();
        ShaderInit();
    }
    public override void Update()
    {
        targetOfAttack = AttackChecker();
        startPoint = transform.position;
        endPoint = targetOfAttack ? targetOfAttack.transform.position : startPoint;
        Attack(targetOfAttack);
    }
    // blocks 机制行为表现
    public void BlocksMechanismDoing(BlocksData.BlocksMechanismType MechNeedUnit)
    {
        switch(MechNeedUnit)
        {
            case BlocksData.BlocksMechanismType.WeakAssociation:
                Display_onWeakAssociation();
            break;
            case BlocksData.BlocksMechanismType.FullRows:
                Display_onFullRows();
            break;
            case BlocksData.BlocksMechanismType.ReachBottomLine:
                Display_onReachBottomLine();
            break;
            case BlocksData.BlocksMechanismType.ReachBottomLineGain:
                Display_onReachBottomLineGain();
            break;
        }
    }
    // 道具收集初始化
    public PropsData.PropsState InitPropDoing()
    {
        PropsData.PropsState propState = tetriUnitSimple.Ray_PorpCollect();
        switch(propState)
        {
            case PropsData.PropsState.ChainBall:
                Display_onChainBall();
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                Display_onMoveDirectionChanger();
            break;
        }
        OnTetrisMoveing();
        return propState;
    }
    // 道具行为表现
    void PropDoing()
    {
        PropsData.PropsState propState = tetriUnitSimple.Ray_PorpCollect();
        if(propState == PropsData.PropsState.None)return;
        switch(propState)
        {
            case PropsData.PropsState.ChainBall:
                Display_onChainBall();
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                Display_onMoveDirectionChanger();
            break;
            case PropsData.PropsState.Obstacle:
                Display_onObstacle();
            break;
        }
    }
    
# endregion 数据关系
# region 数据操作
   
    // 着色器初始化
    void ShaderInit()
    {
        localScale = transform.localScale;
        UpdateMatForeshadow(0);
    }
    // 玩家差异
    public void DifferentPlayerInit()
    {
        if(!lineRenderer)lineRenderer = GetComponent<LineRenderer>();
        if(!soldier)soldier = GetComponent<SoldierBehaviors>();
        Color lineRendererColor = Color.clear;
        Color moraleColor = Color.clear;
        
        // 差异
        if(unitTemplate.player == Player.Player1)
        {
            // 指向标渲染
            lineRendererColor = Color.red + Color.white * 0.3f;
            // 朝向赋值
            faceDirection = 1;
            // 士气颜色
            moraleColor = Color.red + Color.white * 0.3f;
            // 流血颜色
            bloodColor = Color.red + Color.white * 0.2f;
            

        }else if(unitTemplate.player == Player.Player2)
        {
            // 指向标渲染
            lineRendererColor = Color.blue + Color.white * 0.3f;
            // 朝向赋值
            faceDirection = -1;
            // 士气颜色
            moraleColor = Color.blue + Color.white * 0.3f;
            // 流血颜色
            bloodColor = Color.blue + Color.white * 0.2f;
        }
        // 共有
        lineRenderer.startColor = lineRendererColor;
        lineRenderer.endColor = lineRendererColor;
        lineRenderer.endWidth = 0.0f;
        moraleColor.a = 0.666f;
        soldier.StrengthBar.GetComponent<SpriteRenderer>().color = moraleColor;
        if(!bloodEffectLoad)bloodEffectLoad = Resources.Load<ParticlesController>("Effect/BeenAttackBlood");
        OnBeenAttacked += OnBeenAttackedBlood;
    }
    // 被攻击
    void OnBeenAttackedBlood(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            bloodEffect = Instantiate(bloodEffectLoad,transform); 
            bloodEffect.paintColor = bloodColor;
            var main = bloodEffect.GetComponent<ParticleSystem>().main;
            main.startColor = bloodColor;
            var particleSystemMain = bloodEffect.GetComponent<ParticleSystem>().main;
            particleSystemMain.startColor = bloodColor;
        }
    }
    // 攻击
    Unit AttackChecker()
    {
        if (transform.tag == "Untagged") return null;
        if (!animator) return null;
        if (Time.time < lastGuardCheckTime + guardCheckInterval) return null;
        lastGuardCheckTime = Time.time;
        Unit t = GetNearestPVPHostileUnit();
        return t;
    }
    public void Attack(Unit target)
    {
        if (!target)return;       
        if (IsDeadOrNull(target))return;
        DrawLine();
        if (runningTween != null) runningTween.Kill();
        animator.speed = Random.Range(0.95f, 1.15f);
        StartCoroutine(DealAttackSimple());
    }
    public void OnTetriPosIdChanged(Vector2 posId)
    {
        Invoke(nameof(DrawLine), 0.3f);
    }
    private void DrawLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);
        if(!targetOfAttack)return;
        lineRenderer.SetPosition(1, endPoint);
    }
    // 旋转
    public void OnRotate(bool Reverse)
    {
        ResetRotationRot();
    }
    // 移动监听
    public void OnTetrisMoveing()
    {
        PropDoing();
        DrawLine();
        ResetRotation();
    }
    // 被玩家从培养皿
    // 夹起的表现
    public void Display_OnBeginDragDisplay()
    {
        UpdateMatForeshadow(1);
        transform.localScale = LocalScale + Vector3.one * 0.15f;
    }
    // 放下的表现
    public void Display_OnEndDragDisplay()
    {
        UpdateMatForeshadow(0);
        transform.localScale = LocalScale;
    }
    // 当砖块Z向移动到顶后的表现
    public void Display_OnTetriStuck(TetriBlockSimple tetriBlock)
    {
        Display_AllWin(tetriBlock);
    }
    // 抵达对方底线后的表现
    void Display_onReachBottomLine()
    {
        tetriUnitSimple.TetrisSpeedNormal();
        Display_AllWin(tetriUnitSimple.TetriBlock);
        Soldier.Behaviors_onReachBottomLine();
        SufferAddHealthSimple((int)maxHealth);
        SkeletonRenderer.transform.DOScale(Vector3.one * 1.2f, tetriUnitSimple.TetrisBlockSimple.occupyingTime).SetEase(Ease.OutExpo).onComplete = () => {
            tetriUnitSimple.TetrisUnitSimple.KillAllUnits();
        };
    }
    void Display_onReachBottomLineGain()
    {
        Display_AllWin(tetriUnitSimple.TetriBlock);
        
        // 整组增强
        tetriUnitSimple.TetrisBlockSimple.childTetris.ForEach((tetri) => {
            tetri.TetriUnitSimple.PlayBlockEffect();
            tetri.TetriUnitSimple.haveUnit.Soldier.Behaviors_onReachBottomLine();
            tetri.TetriUnitSimple.haveUnit.SufferAddHealthSimple((int)maxHealth);
        });
    }
    // 障碍物表现
    void Display_onObstacle()
    {
        tetriUnitSimple.PlayBlockEffect();
        tetriUnitSimple.TetrisSpeedDown(1.5f);
        SufferAttackSimple((int)currentHP/2);
        Soldier.Behaviors_onObstacle();
    }
    // 移动方向改变
    void Display_onMoveDirectionChanger()
    {
        // 整组播放特效
        tetriUnitSimple.TetrisBlockSimple.childTetris.ForEach((tetri) => {
            tetri.TetriUnitSimple.PlayBlockEffect();
        });
        // 移动方向改变
        tetriUnitSimple.TetrisBlockSimple.MoveUp = tetriUnitSimple.MoveDirectionCatch == PropsData.MoveDirection.Up ? true : false;
        tetriUnitSimple.MoveDirectionCatch=PropsData.MoveDirection.NotReady;
        // 首个砖块获得加成
        Soldier.Behaviors_onMoveDirectionChanger();
        animator.SetTrigger("DoWin");
        SufferAddHealthSimple((int)maxHealth);
        unitTemplate.attackSpeed *= soldier.strength;
    }
    // 链式传递表现
    public void Display_onChainBall()
    {
        // 链式传递
        Soldier.Behaviors_ChainTransfer();
        // 首个砖块获得加成
        animator.SetTrigger("DoWin");
        tetriUnitSimple.PlayBlockEffect();
        SufferAddHealthSimple((int)maxHealth);
    }
    // 满行增强士气
    void Display_onFullRows()
    {
        ResetRotation();
        tetriUnitSimple.PlayBlockEffect();
        runningValue = 0f;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("DoWin");
        Soldier.Behaviors_OnFullRows();
        SufferAddHealthSimple((int)maxHealth);
        unitTemplate.attackSpeed *= soldier.strength;
        if(runningTween != null)runningTween.Kill();
    }
    // 弱势关联表现
    void Display_onWeakAssociation()
    {
        ResetRotation();
        tetriUnitSimple.PlayBlockEffect();
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("DoWin");
        Soldier.Behaviors_WeakAssociation();
        SufferAddHealthSimple((int)maxHealth);
        unitTemplate.attackSpeed *= soldier.strength;
        if(runningTween != null)runningTween.Kill();
    }
    // 跑步动画
    public void Display_StartRunning()
    {
        ResetRotation();
        animator.speed = Random.Range(1.05f, 1.15f);
        runningValue = 0f;
        animator.SetFloat("Speed", 0f);
        if(runningTween != null)runningTween.Kill();
        DOVirtual.DelayedCall(RunningDelay, Display_delayCallRunning);
    }
    void Display_delayCallRunning()
    {
        float endValue = RunningSpeed;
        float duration = DurationRunning - RunningDelay;
        if(!animator){runningTween.Kill(); return;}
        runningTween = DOVirtual.Float(0, animator.speed + endValue, duration, (float value) =>
        {
            runningValue = value;
            if(!animator)return;
            animator.speed = value;
            animator.SetFloat("Speed", runningValue);
        });
        runningTween.onComplete=() =>
        {
            runningValue = 0f;
            if(!animator)return;
            animator.speed = Random.Range(1.05f, 1.15f);
            animator.SetFloat("Speed", 0f);
            ResetRotation();
        };
    }
    // 全体庆祝
    void Display_AllWin(TetriBlockSimple tetriBlock)
    {
        foreach(var display in tetriBlock.tetrisBlockSimple.childTetris)
        {
            if(!display)continue;
            if(!display.GetComponent<TetriUnitSimple>().haveUnit)continue;
            display.GetComponent<TetriUnitSimple>().haveUnit.animator.SetTrigger("DoWin");
        }
    }
    // 朝向
    public void SetFlip()
    {
        DifferentPlayerInit();
        SetFlip(faceDirection);
    } 
    // 看向摄像机
    public void ResetRotation()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        directionToCamera.x = 0f;
        directionToCamera.z = -1f;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        transform.rotation = rotationToCamera;
    }
    // 看向摄像机
    public void ResetRotationRot()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        directionToCamera.x = -1f;
        directionToCamera.z = 0f;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        transform.rotation = rotationToCamera;
    }
    // 升级表现
    public void LevelUpDisplay(int level,float growth = 0.21f)
    {   
        Weapon.WeaponTemplate.scale_weapon = originalScaleweapon + growth * level;
        Weapon.ResetWeaponDispaly();
        Weapon.WeaponTemplate.attackPowerBonus = originalAttackPower + level;
        Weapon.WeaponTemplate.healthBonus = originalHealth + level * 2;
        animator.SetTrigger("DoWin");
        Soldier.Behaviors_onLevelUp(level);
    }
# endregion 数据操作
}