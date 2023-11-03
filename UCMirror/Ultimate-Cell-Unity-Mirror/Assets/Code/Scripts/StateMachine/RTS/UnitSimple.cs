using UnityEngine;
using UC_PlayerData;
using DG.Tweening;
using Spine.Unity;
using Mirror;
public class UnitSimple : Unit
{
#region 数据对象
    public float durationRunning = 1f;
    public float DurationRunning
    {
        get
        {
            if(!tetriUnitSimple.TetrisBlockSimple)return 0f;
            if(durationRunning!=tetriUnitSimple.TetrisBlockSimple.OccupyingTime)durationRunning = tetriUnitSimple.TetrisBlockSimple.OccupyingTime;
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
            if(runningSpeed!=tetriUnitSimple.TetrisBlockSimple.OccupyingTime)runningSpeed = 1/tetriUnitSimple.TetrisBlockSimple.OccupyingTime;
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
    public Tween tween_running;
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
    public LineRenderer attackDirectionLineRenderer;
    LineRenderer AttackDirectionLineRenderer
    {
        get
        {
            if(!attackDirectionLineRenderer)attackDirectionLineRenderer = GetComponent<LineRenderer>();
            return attackDirectionLineRenderer;
        }
    }
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
    public Tween tween_OnEndDragDisplay;
    public Tween tween_DieScale;
    public Tween tween_BeenAttack;
    int foreshadowOpen = 0;
    int ForeshadowOpen
    {
        get
        {
            return foreshadowOpen;
        }
        set
        {
            foreshadowOpen = value;
            if(foreshadowOpen>1)foreshadowOpen = 1;
            UpdateMatForeshadow(foreshadowOpen);
        }
    }
    bool enableSelectEffect = false;
    public bool EnableSelectEffect
    {
        get
        {
            return enableSelectEffect;
        }
        set
        {
            
            if(enableSelectEffect == value)return;
            enableSelectEffect = value;
            if(enableSelectEffect)
            {
                Color enablePlayer1 = new Color32(29, 255, 254, 255);
                Color enablePlayer2 = new Color32(29, 255, 254, 255);
                Color enableColor = Soldier.Player == Player.Player1 ? Color.white :Soldier.Player == Player.Player2 ? Color.white : Color.white;
                UpdateMatSelectEffect(enableColor);
            }
            else
            {
                UpdateMatSelectEffect(Color.clear);
            }
        }
    }
    Vector3 playerLineRendererOffset = new Vector3(0,1,0);
# endregion 数据对象
# region 数据关系
    public override void Awake()
    {
        if(Local())
        {
            if(transform.TryGetComponent(out NetworkTransformBase networkTransform))
            networkTransform.enabled = false;
            if(transform.TryGetComponent(out NetworkIdentity networkIdentity))
            networkIdentity.enabled = false;
            DestroyImmediate(networkIdentity,true);
        }else
        {
            transform.TryGetComponent(out NetworkTransformBase networkTransform);
            if(!networkTransform.enabled)
            networkTransform.enabled = true;
            if(!transform.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                NetworkIdentity networkIdentityTemp = gameObject.AddComponent<NetworkIdentity>();
                networkIdentityTemp.serverOnly = false;
                networkIdentityTemp.visible = Visibility.Default;
            }
            
        }
        base.Awake();
        // ShaderInit();
    }
    public override void Start() {
        base.Start();
        attackDirectionLineRenderer = GetComponent<LineRenderer>();
        soldier = GetComponent<SoldierBehaviors>();
        if(!soldier.WeakAssociation)soldier.Start();
        soldier.WeakAssociation.weakAssociationActive += Event_BlocksMechanismDoing;
        originalScaleweapon = Weapon.WeaponTemplate.scale_weapon;
        originalAttackPower = Weapon.WeaponTemplate.attackPowerBonus;
        originalHealth = Weapon.WeaponTemplate.healthBonus;
        DifferentPlayerInit();
    }
    public override void Update()
    {
        if(Local())
        {
            targetOfAttack = AttackChecker();
            startPoint = transform.position + playerLineRendererOffset;
            endPoint = targetOfAttack ? new( targetOfAttack.transform.position.x - playerLineRendererOffset.x,targetOfAttack.transform.position.y + playerLineRendererOffset.y,targetOfAttack.transform.position.z+ playerLineRendererOffset.z) : startPoint;
            Attack(targetOfAttack);
        }else
        {
            if(!isServer)return;
            targetOfAttack = AttackChecker();
            startPoint = transform.position + playerLineRendererOffset;
            endPoint = targetOfAttack ? new( targetOfAttack.transform.position.x - playerLineRendererOffset.x,targetOfAttack.transform.position.y + playerLineRendererOffset.y,targetOfAttack.transform.position.z+ playerLineRendererOffset.z) : startPoint;
            Attack(targetOfAttack);
        }
        
    }

    public void Event_BlocksMechanismDoing(BlocksData.BlocksMechanismType MechNeedUnit)
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
            case PropsData.PropsState.Obstacle:
                Display_onObstacle();
            break;
        }
        Event_OnTetrisMoveing();
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
    public void SetUnitSortingOrderToFlow()
    {
        Renderer sotingOrderRender = SkeletonRenderer.gameObject.GetComponent<SkeletonMecanim>().GetComponent<Renderer>();
        if(sotingOrderRender.sortingOrder == UC_PlayerData.Dispaly.FlowOrder)return;
        sotingOrderRender.sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
    }

    public void SetUnitSortingOrderToNormal()
    {
        Renderer sotingOrderRender =  SkeletonRenderer.gameObject.GetComponent<SkeletonMecanim>().GetComponent<Renderer>();
        if(sotingOrderRender.sortingOrder == UC_PlayerData.Dispaly.NormalUnitOrder)return;
        sotingOrderRender.sortingOrder = UC_PlayerData.Dispaly.NormalUnitOrder;
        
    }
   
    // 着色器初始化
    public void ShaderInit()
    {
        localScale = transform.localScale;
        float disable = 0f;
        float enable = 1f;
        Color disableColor = Color.clear;
        Speechless.color = disableColor;
        UpdateBeenAttackedEffect(disableColor);
        UpdateMatHealth(enable);
        UpdateMatAlpha(enable);
        UpdateMatForeshadow(disable);
        EnableSelectEffect = true;
        Display_HideUnit();
        Display_HideMorale();
        HideForPlayerScreen();
        AudioSystemManager.Instance.PlaySound("Sound_FrequentBubbles");
    }
    // 玩家差异
    public void DifferentPlayerInit()
    {
        if(Local())
        {
            if(!attackDirectionLineRenderer)attackDirectionLineRenderer = GetComponent<LineRenderer>();
            if(!soldier)soldier = GetComponent<SoldierBehaviors>();
            Color attackDirectionLineRendererColor = Color.clear;
            Color moraleColor = Color.clear;
            // 差异
            if(unitTemplate.player == Player.Player1)
            {
                attackDirectionLineRendererColor = new Color32(208,101,83,255); 
                faceDirection = 1;
                moraleColor = Color.red + Color.white * 0.3f;
                bloodColor = Color.red + Color.white * 0.2f;
                bloodColor = new Color(bloodColor.r,bloodColor.g,bloodColor.b,0.5f);
                playerLineRendererOffset = new Vector3(0,1,0);
            }else if(unitTemplate.player == Player.Player2)
            {
                attackDirectionLineRendererColor = new Color32(83,115,208,255);
                faceDirection = -1;
                moraleColor = Color.blue + Color.white * 0.3f;
                bloodColor = Color.blue + Color.white * 0.2f;
                bloodColor = new Color(bloodColor.r,bloodColor.g,bloodColor.b,0.5f);
                playerLineRendererOffset = new Vector3(-0,1,0);
                
            }
            attackDirectionLineRenderer.startColor = attackDirectionLineRendererColor;
            attackDirectionLineRenderer.endColor = attackDirectionLineRendererColor;
            float width = 0.21f;
            attackDirectionLineRenderer.startWidth = width;
            attackDirectionLineRenderer.endWidth = width;
            moraleColor.a = 0.666f;
            soldier.StrengthBar.GetComponent<SpriteRenderer>().color = moraleColor;
            if(!bloodEffectLoad)bloodEffectLoad = Resources.Load<ParticlesController>("Effect/BeenAttackBlood");
            OnBeenAttacked += Event_OnBeenAttackedBlood;
            SetFlip();
        }else
        {
            if(!isServer)return;
            Server_DifferentPlayerInit();
        }
        
    }
    // 被攻击
    void Event_OnBeenAttackedBlood(int damage)
    {
        if(Local())
        {
            tween_BeenAttack?.Kill();
            Color startColor = bloodColor;
            Color endColor = Color.clear;
            Color beenAttackColor = bloodColor;
            UpdateBeenAttackedEffect(endColor);
            tween_BeenAttack = DOVirtual.Color(startColor, endColor, 0.5f, (TweenCallback<Color>)((Color value) =>
            {
                beenAttackColor = value;
                UpdateBeenAttackedEffect(beenAttackColor);
            }));
            
            for (int i = 0; i < damage; i++)
            {
                bloodEffect = Instantiate(bloodEffectLoad,transform); 
                bloodEffect.paintColor = bloodColor;
                // var main = bloodEffect.GetComponent<ParticleSystem>().main;
                // main.startColor = bloodColor;
                // var particleSystemMain = bloodEffect.GetComponent<ParticleSystem>().main;
                // particleSystemMain.startColor = bloodColor;
            }
        }else
        {
            if(!isServer)return;
            tween_BeenAttack?.Kill();
            Color startColor = bloodColor;
            Color endColor = Color.clear;
            Color beenAttackColor = bloodColor;
            UpdateBeenAttackedEffect(endColor);
            tween_BeenAttack = DOVirtual.Color(startColor, endColor, 0.5f, (TweenCallback<Color>)((Color value) =>
            {
                beenAttackColor = value;
                UpdateBeenAttackedEffect(beenAttackColor);
            }));
            
            for (int i = 0; i < damage; i++)
            {
                bloodEffect = Instantiate(bloodEffectLoad,transform); 
                bloodEffect.paintColor = bloodColor;
            }
            Client_OnBeenAttackedBlood();
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
        if(Local())
        {
            if (!target)return;       
            if (IsDeadOrNull(target))return;
            DrawLine();
            if (tween_running != null) tween_running.Kill();
            animator.speed = Random.Range(0.95f, 1.15f);
            StartCoroutine(DealAttackSimple());
        }else
        {
            if(!isServer)return;
            if (!target)return;       
            if (IsDeadOrNull(target))return;
            DrawLine();
            if (tween_running != null) tween_running.Kill();
            animator.speed = Random.Range(0.95f, 1.15f);
            StartCoroutine(Server_DealAttackSimple());
        }
        
    }
    public void Event_OnTetriPosIdChanged(Vector2 posId)
    {
        Invoke(nameof(DrawLine), 0.3f);
    }
    private void DrawLine()
    {
        attackDirectionLineRenderer.positionCount = 2;
        attackDirectionLineRenderer.SetPosition(0, startPoint);
        attackDirectionLineRenderer.SetPosition(1, startPoint);
        Vector3 lineRenderEndPoint = targetOfAttack ? endPoint : startPoint;
        attackDirectionLineRenderer.SetPosition(1, lineRenderEndPoint);
        if(Local())return;
        Client_DrawLine(startPoint,lineRenderEndPoint);
    }
    // 旋转
    public void Event_OnRotate(bool Reverse)
    {
        ResetRotationWhenPrussRot();
    }
    // 移动监听
    public void Event_OnTetrisMoveing()
    {
        PropDoing();
        DrawLine();
        ResetRotation();
        bool canAddToMoraleAccumulation = true;
        Soldier.morale.successCreated = canAddToMoraleAccumulation;
    }
    // 选中后的编辑状态表现
    public void Display_OnEditingStatusAfterSelection()
    {
        EnableSelectEffect = true;

        Soldier.Behaviors_onEditingStatusAfterSelection();
    }
    // 拖拽的表现
    public void Display_OnBeginDragDisplay()
    {
        ForeshadowOpen = 1;
        EnableSelectEffect = false;
        Soldier.Behaviors_OnBeginDragDisplay();
        transform.localScale = LocalScale + Vector3.one * 0.15f;
        SetUnitSortingOrderToFlow();
    }
    
    // 放下的表现
    public void Display_OnEndDragDisplay()
    {
        ForeshadowOpen = 0;
        tween_OnEndDragDisplay = transform.DOScale(LocalScale, 0.3f).SetEase(Ease.OutBounce);
        tween_OnEndDragDisplay.OnComplete(() => {
            animator.SetTrigger("DoWin");
            Soldier.Behaviors_OnEndDragDisplay();
            tween_OnEndDragDisplay.Kill();
        });
        SetUnitSortingOrderToNormal();
        // transform.localScale = LocalScale;
    }
    
    // 当砖块Z向移动到顶后的表现
    public void Event_Display_OnTetriStuck(TetriBlockSimple tetriBlock)
    {
        Display_AllWin(tetriBlock);
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
    public void Display_UserCommandTheBattle()
    {
        // SkeletonRenderer.transform.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", 0.6f);
        float commandTheBattle = 0.4f;
        UpdateMatAlpha(commandTheBattle);
        Weapon.spineSlot.A = commandTheBattle;
        Soldier.Behaviors_UserCommandTheBattle();
    }
    public void Display_UserWatchingFight()
    {
        // SkeletonRenderer.transform.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", 1f);
        float userWatchingFight = 1f;
        UpdateMatAlpha(userWatchingFight);
        Weapon.spineSlot.A = userWatchingFight;
        Soldier.Behaviors_UserWatchingFight();
    }
    public void Display_HideMorale()
    {
        Soldier.Behaviors_UserCommandTheBattle();
    }
    public void Display_ShowMorale()
    {
        Soldier.Behaviors_UserWatchingFight();
    }
    public void Display_HideUnit()
    {
        float hide = 0.0f;
        UpdateMatAlpha(hide);
        if(!Weapon)return;
        if(Weapon.spineSlot == null)return;
        Weapon.spineSlot.A = hide;
    }
    public void Display_ShowUnit()
    {
        float show = 1.0f;
        UpdateMatAlpha(show);
        if(!Weapon)return;
        if(Weapon.spineSlot == null)return;
        Weapon.spineSlot.A = show;
    }
    // 抵达对方底线后的表现
    void Display_onReachBottomLine()
    {
        tetriUnitSimple.TetrisSpeedNormal();
        Display_AllWin(tetriUnitSimple.TetriBlock);
        Soldier.Behaviors_onReachBottomLine();
        SufferAddHealthSimple((int)maxHealth);
        tween_DieScale = SkeletonRenderer.transform.DOScale(Vector3.one * 1.2f, 1f + 0.1f).SetEase(Ease.OutExpo);
        tween_DieScale.onComplete = () => {
            tetriUnitSimple.TetriBlock.tetrisBlockSimple.Stop();
            tetriUnitSimple.TetrisUnitSimple.KillAllUnits();
        };
    }
    void Display_onReachBottomLineGain()
    {
        Display_AllWin(tetriUnitSimple.TetriBlock);
        // 整组增强
        tetriUnitSimple.TetrisBlockSimple.ChildTetris.ForEach((System.Action<TetriBlockSimple>)((tetri) => {
            tetri.TetriUnitSimple.PlayBlockEffect();
            tetri.TetriUnitSimple.HaveUnit.Soldier.Behaviors_onReachBottomLine();
            tetri.TetriUnitSimple.HaveUnit.SufferAddHealthSimple((int)maxHealth);
        }));
    }
    // 障碍物表现
    void Display_onObstacle()
    {
        tetriUnitSimple.PlayBlockEffect();
        tetriUnitSimple.TetrisSpeedModify(1.5f);
        SufferAttackSimple((int)currentHP/2);
        Soldier.Behaviors_onObstacle();
    }
    // 移动方向改变
    void Display_onMoveDirectionChanger()
    {
        // 整组播放特效
        tetriUnitSimple.TetrisBlockSimple.ChildTetris.ForEach((tetri) => {
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
        if(tween_running != null)tween_running.Kill();
    }
    // 弱势关联表现
    void Display_onWeakAssociation()
    {
        if(Local())
        {
            ResetRotation();
            tetriUnitSimple.PlayBlockEffect();
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("DoWin");
            Soldier.Behaviors_WeakAssociation();
            SufferAddHealthSimple((int)maxHealth);
            unitTemplate.attackSpeed *= soldier.strength;
            if(tween_running != null)tween_running.Kill();
        }else
        {
            if(!isServer)return;
            Server_Display_onWeakAssociation();
            
        }
        
    }
    // 跑步动画
    public void Event_Display_StartRunning()
    {
        if(Local())
        {
            ResetRotation();
            animator.speed = Random.Range(1.05f, 1.15f);
            runningValue = 0f;
            animator.SetFloat("Speed", 0f);
            if(tween_running != null)tween_running.Kill();
            DOVirtual.DelayedCall(RunningDelay, Display_delayCallRunning);
        }else
        {
            if(!isServer)return;
            ResetRotation();
            animator.speed = Random.Range(1.05f, 1.15f);
            runningValue = 0f;
            animator.SetFloat("Speed", 0f);
            if(tween_running != null)tween_running.Kill();
            DOVirtual.DelayedCall(RunningDelay, Server_Display_delayCallRunning);
        }
        
    }
    void Display_delayCallRunning()
    {
        float endValue = RunningSpeed;
        float duration = DurationRunning - RunningDelay;
        if(!animator){tween_running.Kill(); return;}
        tween_running = DOVirtual.Float(0, animator.speed + endValue, duration, (float value) =>
        {
            runningValue = value;
            if(!animator)return;
            animator.speed = value;
            animator.SetFloat("Speed", runningValue);
        });
        tween_running.onComplete=() =>
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
        foreach(var display in tetriBlock.tetrisBlockSimple.ChildTetris)
        {
            if(!display)continue;
            if(!display.GetComponent<TetriUnitSimple>().HaveUnit)continue;
            display.GetComponent<TetriUnitSimple>().HaveUnit.animator.SetTrigger("DoWin");
        }
    }
    
    // 朝向
    public void SetFlip()
    {
        // DifferentPlayerInit();
        SetFlip(faceDirection);
    } 
    // 看向摄像机
    public void ResetRotation()
    {
        // transform.localRotation = Quaternion.Euler(Vector3.zero);
        // Vector3 CameraPos = Camera.main.transform.position;
        // Vector3 directionToCamera = CameraPos - transform.position;
        // directionToCamera.x = 0f;
        // directionToCamera.z = -1f;
        // float fixStretch = -13f;
        // directionToCamera.y += fixStretch;
        // Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        // transform.rotation = rotationToCamera;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 stretchCorrectionPrussRot = new Vector3(0,3f,-3f);
        Vector3 CameraPos = Camera.main.transform.position;
        Vector3 directionToCamera = transform.forward; //CameraPos - transform.position;
        directionToCamera.x = 0f;
        directionToCamera.z = -1f;
        float fixStretch = 6f;
        directionToCamera.y += fixStretch;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        transform.rotation = rotationToCamera;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + stretchCorrectionPrussRot);
        // skeletonRenderer.transform.LookAt(CameraPos);
        // skeletonRenderer.transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x,transform.localRotation.eulerAngles.y,0));
    }
    // 看向摄像机
    public void ResetRotationWhenPrussRot()
    {
        // transform.localRotation = Quaternion.Euler(Vector3.zero);
        // Vector3 stretchCorrectionPrussRot = new Vector3(0,3f,-6f);
        // // Vector3 CameraPos =  Camera.main.transform.position;
        // Vector3 directionToCamera = transform.forward; //CameraPos - transform.position;
        // directionToCamera.x = -1f;
        // directionToCamera.z = 0f;
        // float fixStretch = 4f;
        // directionToCamera.y += fixStretch;
        // Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        // transform.rotation = rotationToCamera;
        // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + stretchCorrectionPrussRot);
        
    }
    // 升级表现
    public void LevelUpDisplay(int level,float growth = 0.21f)
    {   
        switch(level)
        {
            case 1:
                skeletonRenderer.transform.localScale = Vector3.one * 0.6f;
            break;
            case 2:
                skeletonRenderer.transform.localScale = Vector3.one * 0.8f;
            break;
            case 3:
                skeletonRenderer.transform.localScale = Vector3.one * 1.0f;
            break;
        }
        // Weapon.WeaponTemplate.scale_weapon = originalScaleweapon - growth * level;
        Weapon.ResetWeaponDispaly();
        Weapon.WeaponTemplate.attackPowerBonus = originalAttackPower + level;
        Weapon.WeaponTemplate.healthBonus = originalHealth + level * 10;
        animator.SetTrigger("DoWin");
        Soldier.Behaviors_onLevelUp(level);
    }
# endregion 数据操作
# region 联网数据操作
    struct ServerToClient_Unit
    {
        public Color moraleColor;
        public Color attackDirectionLineRendererColor;
        public Color bloodColor;
        public int faceDirection;
        public Vector3 playerLineRendererOffset;
        public Player playerBelong;

    }
    [Server]
    void Server_DifferentPlayerInit()
    {
        if(!attackDirectionLineRenderer)attackDirectionLineRenderer = GetComponent<LineRenderer>();
        if(!soldier)soldier = GetComponent<SoldierBehaviors>();
        Color attackDirectionLineRendererColor = Color.clear;
        Color moraleColor = Color.clear;
        // 差异
        if(unitTemplate.player == Player.Player1)
        {
            attackDirectionLineRendererColor = new Color32(208,101,83,255); 
            faceDirection = 1;
            moraleColor = Color.red + Color.white * 0.3f;
            bloodColor = Color.red + Color.white * 0.2f;
            bloodColor = new Color(bloodColor.r,bloodColor.g,bloodColor.b,0.5f);
            playerLineRendererOffset = new Vector3(0,1,0);
        }else if(unitTemplate.player == Player.Player2)
        {
            attackDirectionLineRendererColor = new Color32(83,115,208,255);
            faceDirection = -1;
            moraleColor = Color.blue + Color.white * 0.3f;
            bloodColor = Color.blue + Color.white * 0.2f;
            bloodColor = new Color(bloodColor.r,bloodColor.g,bloodColor.b,0.5f);
            playerLineRendererOffset = new Vector3(-0,1,0);
            
        }
        attackDirectionLineRenderer.startColor = attackDirectionLineRendererColor;
        attackDirectionLineRenderer.endColor = attackDirectionLineRendererColor;
        float width = 0.21f;
        attackDirectionLineRenderer.startWidth = width;
        attackDirectionLineRenderer.endWidth = width;
        moraleColor.a = 0.666f;
        soldier.StrengthBar.GetComponent<SpriteRenderer>().color = moraleColor;
        if(!bloodEffectLoad)bloodEffectLoad = Resources.Load<ParticlesController>("Effect/BeenAttackBlood");
        OnBeenAttacked += Event_OnBeenAttackedBlood;
        ServerToClient_Unit serverToClient_Unit = new();
        serverToClient_Unit.moraleColor = moraleColor;
        serverToClient_Unit.attackDirectionLineRendererColor = attackDirectionLineRendererColor;
        serverToClient_Unit.bloodColor = bloodColor;
        serverToClient_Unit.faceDirection = faceDirection;
        serverToClient_Unit.playerLineRendererOffset = playerLineRendererOffset;
        serverToClient_Unit.playerBelong = unitTemplate.player;
        ShaderInit();
        ResetRotation();
        SetFlip();
        Client_DifferentPlayerInit(serverToClient_Unit);
    }
    [ClientRpc]
    void Client_DifferentPlayerInit(ServerToClient_Unit serverToClient_Unit_In)
    {
        Color attackDirectionLineRendererColor = Color.clear;
        Color moraleColor = Color.clear;
        attackDirectionLineRendererColor = serverToClient_Unit_In.attackDirectionLineRendererColor; 
        faceDirection = serverToClient_Unit_In.faceDirection;
        moraleColor = serverToClient_Unit_In.moraleColor;
        bloodColor = serverToClient_Unit_In.bloodColor;
        playerLineRendererOffset = serverToClient_Unit_In.playerLineRendererOffset;
        AttackDirectionLineRenderer.startColor = attackDirectionLineRendererColor;
        AttackDirectionLineRenderer.endColor = attackDirectionLineRendererColor;
        moraleColor.a = 0.666f;
        Soldier.StrengthBar.GetComponent<SpriteRenderer>().color = moraleColor;
        ShaderInit();
        ResetRotation();
        SetFlip();
        unitTemplate.player = serverToClient_Unit_In.playerBelong;
        if(unitTemplate.player == ServerLogic.local_palayer)return;
        Color DistinguishBelongingColor = unitTemplate.player == Player.Player1 ? Color.red * 0.4f + Color.white*0.6f : Color.blue * 0.4f + Color.white * 0.6f;
        UpdateColorMultiplication(DistinguishBelongingColor);
    }
    [Server]
    void Server_Display_onWeakAssociation()
    {
        ResetRotation();
        tetriUnitSimple.PlayBlockEffect();
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("DoWin");
        Soldier.Behaviors_WeakAssociation();
        SufferAddHealthSimple((int)maxHealth);
        unitTemplate.attackSpeed *= soldier.strength;
        if(tween_running != null)tween_running.Kill();
        Client_Display_onWeakAssociation();
    }
    [ClientRpc]
    void Client_Display_onWeakAssociation()
    {
        ResetRotation();
        tetriUnitSimple.PlayBlockEffect();
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("DoWin");
        if(tween_running != null)tween_running.Kill();
    }
    [ClientRpc]
    void Client_OnBeenAttackedBlood()
    {
        tween_BeenAttack?.Kill();
        Color startColor = bloodColor;
        Color endColor = Color.clear;
        Color beenAttackColor = bloodColor;
        UpdateBeenAttackedEffect(endColor);
        tween_BeenAttack = DOVirtual.Color(startColor, endColor, 0.5f, (TweenCallback<Color>)((Color value) =>
        {
            beenAttackColor = value;
            UpdateBeenAttackedEffect(beenAttackColor);
        }));
    }
    [ClientRpc]
    void Client_DrawLine(Vector3 startPoint,Vector3 endPoint)
    {
        attackDirectionLineRenderer.positionCount = 2;
        attackDirectionLineRenderer.SetPosition(0, startPoint);
        attackDirectionLineRenderer.SetPosition(1, endPoint);
    }
    void HideForPlayerScreen()
    {
        SkeletonRenderer.transform.GetComponent<MeshRenderer>().enabled = false;
        soldier.StrengthBar.GetComponent<SpriteRenderer>().enabled = false;
        soldier.LevelUpEffect.GetComponent<Renderer>().enabled = false;
    }
    public void ShowForPlayerScreen()
    {
        SkeletonRenderer.transform.GetComponent<MeshRenderer>().enabled = true;
        Soldier.StrengthBar.GetComponent<SpriteRenderer>().enabled = true;
        Soldier.LevelUpEffect.GetComponent<Renderer>().enabled = true;
    }
    [Server]
    public void Server_Display_OnBeginDragDisplay()
    {
        ForeshadowOpen = 1;
        EnableSelectEffect = false;
        Soldier.Behaviors_OnBeginDragDisplay();
        transform.localScale = LocalScale + Vector3.one * 0.15f;
        SetUnitSortingOrderToFlow();
        Client_Display_OnBeginDragDisplay();
    }
    [ClientRpc]
    void Client_Display_OnBeginDragDisplay()
    {
        ForeshadowOpen = 1;
        EnableSelectEffect = false;
        transform.localScale = LocalScale + Vector3.one * 0.15f;
        SetUnitSortingOrderToFlow();
    }
    [Server]
    void Server_Display_delayCallRunning()
    {
        float endValue = RunningSpeed;
        float duration = DurationRunning - RunningDelay;
        if(!animator){tween_running.Kill(); return;}
        tween_running = DOVirtual.Float(0, animator.speed + endValue, duration, (float value) =>
        {
            runningValue = value;
            if(!animator)return;
            animator.speed = value;
            animator.SetFloat("Speed", runningValue);
            Client_Display_delayCallRunning_SetSpeed(runningValue);
        });
        tween_running.onComplete=() =>
        {
            runningValue = 0f;
            if(!animator)return;
            animator.speed = Random.Range(1.05f, 1.15f);
            float stop = 0;
            animator.SetFloat("Speed", stop);
            Client_Display_delayCallRunning_SetSpeed(stop);
            ResetRotation();
        };
    }
    [ClientRpc]
    void Client_Display_delayCallRunning_SetSpeed(float runningValueIn)
    {
        animator.SetFloat("Speed", runningValueIn);
    }
    [Server]
    public void Server_Display_OnEndDragDisplay()
    {
        ForeshadowOpen = 0;
        tween_OnEndDragDisplay = transform.DOScale(LocalScale, 0.3f).SetEase(Ease.OutBounce);
        tween_OnEndDragDisplay.OnComplete(() => {
            animator.SetTrigger("DoWin");
            Soldier.Behaviors_OnEndDragDisplay();
            tween_OnEndDragDisplay.Kill();
        });
        SetUnitSortingOrderToNormal();
        Client_Display_OnEndDragDisplay();
    }
    [ClientRpc]
    void Client_Display_OnEndDragDisplay()
    {
        ForeshadowOpen = 0;
        animator.SetTrigger("DoWin");
        SetUnitSortingOrderToNormal();
    }
# endregion 联网数据操作
}