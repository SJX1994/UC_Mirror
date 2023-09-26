using UnityEngine;
using UC_PlayerData;
using DG.Tweening;

public class UnitSimple : Unit
{
# region 数据对象
    public float durationRunning = 1f;
    public Material mat;
    public TetriUnitSimple tetriUnitSimple;
    float runningSpeed = 3f;
    float runningValue = 0f;
    float runningDelay = 1.0f;
    Tween runningTween;
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
    // 可视化
    Vector3 startPoint;
    Vector3 endPoint;
    public LineRenderer lineRenderer;
    int faceDirection = 1;
    ParticlesController bloodEffectLoad;
    ParticlesController bloodEffect;
    Color bloodColor = Color.clear;
    
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
        if(!soldier.weakAssociation)soldier.Start();
        soldier.weakAssociation.WeakAssociationActive += BlocksMechanismDoing;
        DifferentPlayerInit();
    }
    public override void Update()
    {
        targetOfAttack = AttackChecker();
        startPoint = transform.position;
        endPoint = targetOfAttack ? targetOfAttack.transform.position : startPoint;
        Attack(targetOfAttack);
    }
# endregion 数据关系
# region 数据操作
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
        soldier.strengthBar.GetComponent<SpriteRenderer>().color = moraleColor;
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
    public void OnTetrisMoveing()
    {
        PropDoing();
        DrawLine();
        ResetRotation();
    }
    public PropsData.PropsState InitPropDoing()
    {
        PropsData.PropsState propState = tetriUnitSimple.Ray_PorpCollect();
        switch(propState)
        {
            case PropsData.PropsState.ChainBall:
                // 链式传递
                Soldier.Behaviors_ChainTransfer();
                // 首个砖块获得加成
                animator.SetTrigger("DoWin");
                SufferAddHealthSimple((int)maxHealth);
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                // 移动方向改变
                tetriUnitSimple.TetrisBlockSimple.MoveUp = tetriUnitSimple.MoveDirectionCatch == PropsData.MoveDirection.Up ? true : false;
                tetriUnitSimple.MoveDirectionCatch=PropsData.MoveDirection.NotReady;
                // 首个砖块获得加成
                animator.SetTrigger("DoWin");
            break;
        }
        OnTetrisMoveing();
        return propState;
    }
    // blocks 机制行为表现
    public void BlocksMechanismDoing(BlocksData.BlocksMechanismType MechNeedUnit)
    {
        switch(MechNeedUnit)
        {
            case BlocksData.BlocksMechanismType.WeakAssociation:
                animator.SetTrigger("DoWin");
                SufferAddHealthSimple((int)maxHealth);
            break;
            case BlocksData.BlocksMechanismType.FullRows:
                Display_onFullRows();
            break;
        }
    }
    // 道具行为
    void PropDoing()
    {
        PropsData.PropsState propState = tetriUnitSimple.Ray_PorpCollect();
        if(propState == PropsData.PropsState.None)return;
        switch(propState)
        {
            case PropsData.PropsState.ChainBall:
                // 链式传递
                Soldier.Behaviors_ChainTransfer();
                // 首个砖块获得加成
                animator.SetTrigger("DoWin");
                SufferAddHealthSimple((int)maxHealth);
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                // 移动方向改变
                tetriUnitSimple.TetrisBlockSimple.MoveUp = tetriUnitSimple.MoveDirectionCatch == PropsData.MoveDirection.Up ? true : false;
                tetriUnitSimple.MoveDirectionCatch=PropsData.MoveDirection.NotReady;
                // 首个砖块获得加成
                animator.SetTrigger("DoWin");
            break;
        }
    }
    // 满行增强士气
    void Display_onFullRows()
    {
        ResetRotation();    
        runningValue = 0f;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("DoWin");
        soldier.Behaviors_OnFullRows();
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
        DOVirtual.DelayedCall(runningDelay, Display_delayCallRunning);
    }
    void Display_delayCallRunning()
    {
        float endValue = runningSpeed;
        float duration = durationRunning - runningDelay;
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
    
# endregion 数据操作
}