using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Linq;
using UC_PlayerData;

public class ChainTransfer : MonoBehaviour, ISoldierRelation
{
      private bool needRender;
      public bool NeedRender { 
            get {return needRender;}
            set {
                if (value != needRender)
                {
                    needRender = value;
                }
            } 
      }
      //数值
      [HideInInspector]
      public int chainDamage = 10;
      [HideInInspector]
      public int chainDamageTotal = 10;
      //通讯
      public UnityAction<PuppetEffectDataStruct> OnPlayEffect;

      public LayerMask targetLayer;
      MechanismInPut mechanismInPut;
      List<SoldierBehaviors> soldiers = new();
      MechanismInPut.ModeTest modeTest;
      // SkillCooldownTimer skillCooldownTimer;
      // SkillCooldown skillCooldown;
      
      SoldierBehaviors self;
      public SoldierBehaviors Self
      {
            get
            {
                  if(!self)TryGetComponent<SoldierBehaviors>(out self);
                  return self;
            }
      }
      SoldierBehaviors next;
      bool counted = false;
      public bool collected = false;
      public bool theFirst = false;
      CapsuleCollider capsuleCollider;
      [HideInInspector]
      public float duration = 0.1f;
      [HideInInspector]
      public float originDuration;
      int gloableIndex = 0;
      SoldierBehaviors[] gloableSoldiers;
      [HideInInspector]
      public bool bombIsMoving = false;
      bool chainTransfering = false; // 正在传递 碰撞伤害计算
      List<SoldierBehaviors> dealAttackSoldiers = new(); // 需要处理伤害的士兵们
      private Vector3 targetPosition;
      // 表现
      public Material boonMat;
      public List<Material> boonMats;
      public ParticleSystem boonEffectPrefab;

      public List<ParticleSystem> boonEffectPrefabs;
      [HideInInspector]
      public bool preview = false;
      float extendDistance = 3f;
      SpriteRenderer selectionCircle;
      Tween tween_SoldiersStartRelationFrom;
      Tween tween_SoldiersUpdateRelationFrom;
      Tween tween_SoldiersEndRelationTo;
      Tween tween_ChainTransfering;
      Vector3 originLocalPosition;
      
      void Start()
      {
            needRender = true;
            chainDamage = 10;
            chainDamageTotal = 0;
            originLocalPosition = transform.localPosition;
            Invoke(nameof(Init), 0.1f);
      }
      public void AllSoldiers()
      {
            soldiers = new List<SoldierBehaviors>(FindObjectsOfType<SoldierBehaviors>().ToList().Where(x=>CheckSoldier(x)));
      }
      bool CheckSoldier(SoldierBehaviors soldierBehavior)
      {
            List<bool> condition = new();
            if(!soldierBehavior)return false;
            if(!Self)Init();
            if(soldierBehavior.UnitSimple.IsDeadOrNull(soldierBehavior.UnitSimple))return false;
            // 不包含培养皿中的砖块
            Transform p = soldierBehavior.UnitSimple.TetriUnitSimple.TetrisBlockSimple.transform.parent;
            if(p==null)return false;
            condition.Add(p);
            if(p)condition.Add(p.TryGetComponent<BlocksCreator_Main>(out BlocksCreator_Main bc));
            // 不包含不同类型的砖块
            condition.Add(soldierBehavior.morale.soldierType == Self.morale.soldierType);
            // 不包含不同玩家的砖块
            condition.Add(soldierBehavior.UnitSimple.unitTemplate.player == Self.UnitSimple.unitTemplate.player);
            // 不包含拖拽的临时砖块
            condition.Add(!soldierBehavior.UnitSimple.TetriUnitSimple.TetrisBlockSimple.name.Contains(UnitData.Temp));
            bool allTrue = condition.All(b => b);
            return allTrue;
      }
      private void ModeChangedAction(MechanismInPut.ModeTest modeTest)
      {
            this.modeTest = modeTest;
      }
      public bool CanDoChain()
      {
            if(GetClosestSoldier()!=null)
            {
                  return true;
            }else
            {
                  Debug.Log("少于一个单位，不能进行链式传递");
                  
                  return false;
            }
      }
      public void FirstChain(bool preview)
      {
            this.preview = preview;
            // skillCooldown.NotActive();
            theFirst = true;
            this.soldiers = GetSoldiersArranged(Self);
            foreach(var soldier in this.soldiers)
            {
                  soldier.ChainTransfer.bombIsMoving = true;
            }
            gloableSoldiers = new SoldierBehaviors[this.soldiers.Count];
            gloableIndex = 0;
            StartCoroutine(Forward_ExecuteEveryFewSeconds());
      }
      public IEnumerator LastChain(SoldierBehaviors[] soldiers, int tempIndex)
      {
            for(int i = soldiers.Length-1; i >= 0; i--)
            {
                  tempIndex += 1;
                  if(soldiers[i]==null) continue;
                  if(soldiers[i].ChainTransfer.theFirst == true)
                  {
                        float tempDuration = soldiers[i].ChainTransfer.duration;
                        soldiers[i].ChainTransfer.duration *= tempDuration/tempIndex;
                        soldiers[i].ChainTransfer.SoldiersEndRelation(soldiers[i+1],soldiers[i]);
                  }else
                  {
                        float tempDuration = soldiers[i].ChainTransfer.duration;
                        soldiers[i].ChainTransfer.duration *= tempDuration/tempIndex;
                        float modifyDuration = soldiers[i].ChainTransfer.duration;
                        soldiers[i].ChainTransfer.SoldiersUpdateRelation(soldiers[i],soldiers[i-1]);
                        // 在指定的间隔后继续执行下一次
                        yield return new WaitForSeconds(modifyDuration);
                        
                  }
                  
                  
            }
            
      }
      IEnumerator Forward_ExecuteEveryFewSeconds()
      {
            chainDamageTotal = chainDamage;
            if(theFirst)
            {
                  gloableIndex = 0;
                  gloableSoldiers[gloableIndex] = Self;
                  gloableIndex += 1;
                  counted = true;
                  next = Self.ChainTransfer.GetClosestSoldier();
                  chainDamageTotal += chainDamage;
                  SoldiersStartRelation(Self,next);
                  soldiers.Remove(Self);
                  yield return new WaitForSeconds(duration);
            }
            foreach (var soldier in soldiers)
            {     
                  if(!soldier)continue;
                  gloableSoldiers[gloableIndex] = soldier;
                  gloableIndex += 1;
                  float tempDuration = soldier.ChainTransfer.duration;
                  soldier.ChainTransfer.duration *= tempDuration/gloableIndex;
                  float modifyDuration = soldier.ChainTransfer.duration;
                  if(gloableIndex-1 == soldiers.Count)
                  {
                       StartCoroutine(soldier.ChainTransfer.LastChain(gloableSoldiers,gloableIndex));
                       continue;
                  }
                  soldier.ChainTransfer.next = soldiers[gloableIndex-1];
                  soldier.ChainTransfer.SoldiersStartRelation(soldier,soldier.ChainTransfer.next);
                  soldier.ChainTransfer.duration = tempDuration;
                  chainDamageTotal += soldier.ChainTransfer.chainDamage;
                  
                  // 在指定的间隔后继续执行下一次
                  // Debug.Log("modifyDuration"+modifyDuration);
                  yield return new WaitForSeconds(modifyDuration);
            }
            
            

            
      }
      public void ClearChainTransferBeforDie()
      {
            List<Tween> tweens = new List<Tween>();
            tweens.Add(tween_SoldiersStartRelationFrom);
            tweens.Add(tween_SoldiersUpdateRelationFrom);
            tweens.Add(tween_SoldiersEndRelationTo);
            tweens.Add(tween_ChainTransfering);
            for(int i = 0; i < tweens.Count; i++)
            {
                  if(tweens[i] == null)continue;
                  if(tweens[i]!=null){tweens[i]?.Kill(); tweens[i] = null;}
            }
      }
      public void SoldiersStartRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(from == null || to == null) return;
            if(!from.UnitSimple || !to.UnitSimple) return;
            to.ChainTransfer.counted = true;
            Vector3 backPosition = from.transform.position;
            Vector3 targetPosition = to.transform.position;

            // 停止砖块移动
            bool notCancleOccupied = false;
            from.UnitSimple.TetriUnitSimple.TetrisBlockSimple.Stop(notCancleOccupied);
            to.UnitSimple.TetriUnitSimple.TetrisBlockSimple.Stop(notCancleOccupied);
            // 停止状态机的移动
            NavMeshAgent agent;
            agent = from.transform.TryGetComponent<NavMeshAgent>(out agent) ? agent : null;
            if(agent != null)
            {
                  // AICommand stop = new AICommand(AICommand.CommandType.Stop);
                  // from.unitBase.ExecuteCommand(stop);
                  // from.unitBase.controled = true;
            }
            tween_SoldiersStartRelationFrom = from.transform.DOMove(targetPosition, from.ChainTransfer.duration/2);
            tween_SoldiersStartRelationFrom.onComplete = () =>
            {
                  
                  if(to.PositiveEffect)to.PositiveEffect.Play();
                  tween_SoldiersStartRelationFrom.Kill();
                  tween_SoldiersStartRelationFrom = from.transform.DOLocalMove(originLocalPosition, from.ChainTransfer.duration/2);
                  tween_SoldiersStartRelationFrom.onComplete = () =>
                  {  
                        transform.localPosition = originLocalPosition;
                        Invoke(nameof(ResetPos),0.5f);
                        tween_SoldiersStartRelationFrom.Kill();
                  };
            };
            
      }
      
      public void SoldiersUpdateRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            
            if(from == null || to == null) return;
            to.ChainTransfer.counted = false;
            Vector3 backPosition = from.transform.position;
            Vector3 targetPosition = to.transform.position;
            // Unit 增强
            from.UnitSimple.Display_onChainBall();
            to.UnitSimple.Display_onChainBall();
            
            tween_SoldiersUpdateRelationFrom = from.transform.DOMove(targetPosition, from.ChainTransfer.duration/2);
            tween_SoldiersUpdateRelationFrom.onComplete = () =>
            {
                  
                  to.PositiveEffect.Play();
                  tween_SoldiersUpdateRelationFrom.Kill();
                  tween_SoldiersUpdateRelationFrom = from.transform.DOLocalMove(originLocalPosition, from.ChainTransfer.duration/2);
                  tween_SoldiersUpdateRelationFrom.onComplete = () =>
                  {  
                        // 允许状态机的移动
                        Invoke(nameof(ResetPos),0.5f);
                        NavMeshAgent agent;
                        agent = from.transform.TryGetComponent<NavMeshAgent>(out agent) ? agent : null;
                        if(agent != null)
                        {
                              // AICommand guard = new AICommand(AICommand.CommandType.GoToAndGuard, backPosition);
                              // from.unitBase.ExecuteCommand(guard);
                              // from.unitBase.controled = false;
                        }
                        from.UnitSimple.TetriUnitSimple.TetrisBlockSimple.Move();
                        tween_SoldiersUpdateRelationFrom.Kill();
                  };
            };
      }
      public void SoldiersEndRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            // 发出射线
            targetPosition = originLocalPosition;
            if (Physics.Raycast(from.transform.position, (to.transform.position - from.transform.position), out RaycastHit hit, Mathf.Infinity, targetLayer))
            {
                // 如果射线与物体相交，则打印相交点和相交物体的名称
                // Debug.Log("Intersection point: " + hit.point);
                // Debug.Log("Intersected object: " + hit.collider.gameObject.name);
                targetPosition = hit.point;
                bombIsMoving = true;
            }
            else if (targetPosition == originLocalPosition)
            {
                  // 计算延长线上的点
                  Vector3 direction = (to.transform.position - from.transform.position).normalized;
                  Vector3 extendedPoint = to.transform.position + (direction * extendDistance);
                  targetPosition = extendedPoint;
            }
            if(!bombIsMoving)return;
            Vector3 backPos = to.transform.position;
            Vector3 targetPos = this.targetPosition;
            //大炮发射表现
            PuppetEffectDataStruct ped = new(PuppetEffectDataStruct.EffectType.ChainTransferTrail,targetPos,duration/2);
            to.ChainTransfer.OnPlayEffect?.Invoke(ped);
            GameObject obj = new("TrailObject");
            TrailRenderer trail = obj.AddComponent<TrailRenderer>();
            trail.startColor = Color.red;
            trail.endColor = Color.yellow;
            trail.startWidth = 0.6f;
            trail.endWidth = 0.6f;
            trail.time = 2.0f;
            Material trailMaterial = boonMat;
            trail.material = trailMaterial;
            obj.SetActive(false);
            obj.transform.position = transform.position;
            obj.SetActive(true);
            trail.GetComponent<Renderer>().enabled = needRender; // 是否需要渲染
            to.ChainTransfer.ChainTransfering(originDuration);
            float ortemp = originDuration;
            // 发射
            tween_SoldiersEndRelationTo = to.transform.DOMove(targetPos,to.ChainTransfer.duration/2);
            tween_SoldiersEndRelationTo.onComplete = () =>
            {
            obj.transform.position = targetPos;
            Color targetColor = new(trail.endColor.r,trail.endColor.g,trail.endColor.b,0f);
            DOTween.To(() => trail.startColor, color => trail.startColor = color, targetColor, originDuration/2);
            DOTween.To(() => trail.endColor, color => trail.endColor = color, targetColor, originDuration/2);
            GameObject boonEffect = Instantiate(boonEffectPrefab.gameObject);
            boonEffect.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = needRender; 
            boonEffect.transform.position = targetPos;
            boonEffect.GetComponent<ParticleSystem>().Play();
            Destroy(obj,originDuration);
            Destroy(boonEffect,originDuration);
            tween_SoldiersEndRelationTo.Kill();
            tween_SoldiersEndRelationTo = to.transform.DOMove(targetPos,to.ChainTransfer.originDuration/2);
            tween_SoldiersEndRelationTo.onComplete = () =>
            {
                  tween_SoldiersEndRelationTo.Kill();
                  tween_SoldiersEndRelationTo = to.transform.DOLocalMove(originLocalPosition,to.ChainTransfer.originDuration);
                  tween_SoldiersEndRelationTo.onComplete = () =>
                  {   
                        //复原
                        Invoke(nameof(ResetPos),0.5f);
                        AllSoldiers();
                        foreach(SoldierBehaviors soldier in soldiers)
                        {
                              
                              if(!soldier)continue;
                              soldier.ChainTransfer.counted = false;
                              soldier.ChainTransfer.theFirst = false;
                              soldier.ChainTransfer.gloableIndex = 0;
                              soldier.ChainTransfer.duration = originDuration;
                              soldier.ChainTransfer.bombIsMoving = false;
                              soldier.ChainTransfer.preview = false;
                              // 允许砖块移动
                              soldier.UnitSimple.TetriUnitSimple.TetrisBlockSimple.Move();
                        }
                        // 公告
                        if(!mechanismInPut)return;
                        if(!mechanismInPut.warningSystem)return;
                        mechanismInPut.warningSystem.inText1 = (soldiers.Count+1).ToString();
                        mechanismInPut.warningSystem.inText2 = Self.morale.soldierType.ToString();
                        mechanismInPut.warningSystem.inText3 = chainDamageTotal.ToString();
                        mechanismInPut.warningSystem.changeWarningTypes = WarningSystem.WarningType.ChainTransfer;
                        tween_SoldiersEndRelationTo.Kill();
                        self.UnitSimple.TetriUnitSimple.TetrisBlockSimple.Move();
                  };
            };
                  
            };
            
      }
      void ResetPos()
      {
            while(transform.localPosition != originLocalPosition)
            {
                  transform.localPosition = originLocalPosition;
            }
      }
      void UnitBaseCollected(Unit u)
      {
            this.collected = true;
      }
      public void ChainTransfering(float duration)
      {
            float colliderRadiusTemp = capsuleCollider.radius;
            chainTransfering = true;
            // if(!selectionCircle)selectionCircle = self.unitBase.selectionCircle; 
	      // selectionCircle.gameObject.SetActive(chainTransfering);
            // capsuleCollider.enabled = chainTransfering;
            // 碰撞扣血
            capsuleCollider.radius = 3.9f;
            // GameObject visableCapsuleCollider = VisableCapsuleCollider();
            tween_ChainTransfering = DOVirtual.DelayedCall(duration, (TweenCallback)(() =>
            {
                  chainTransfering = false;
	            // selectionCircle.gameObject.SetActive(chainTransfering);
                  // capsuleCollider.enabled = chainTransfering;
                  foreach(SoldierBehaviors ss in dealAttackSoldiers)
                  {
                        if(!ss.ChainTransfer.boonEffectPrefab)continue;     
                        if(this.Self.unitBase.IsDeadOrNull(ss.unitBase))continue;
                        GameObject boonEffect = Instantiate(ss.ChainTransfer.boonEffectPrefab.gameObject);
                        boonEffect.transform.position = ss.transform.position;
                        // ss.unitBase.SufferAttack(chainDamageTotal);
                        ss.unitBase.SufferAttackSimple(150);
                        StartCoroutine(WaitParticleDestory(ss, boonEffect.GetComponent<ParticleSystem>()));
                  }
                capsuleCollider.radius = colliderRadiusTemp;
                dealAttackSoldiers.Clear();
            }));
      }
      void OnTriggerEnter(Collider other)
      {
            if(!chainTransfering)return;
            // 判断进入碰撞的物体
            if (other.transform.TryGetComponent(out SoldierBehaviors soldierAttack))
            {
                  if(Self.unitBase.unitTemplate.GetOtherPlayerType() != soldierAttack.unitBase.unitTemplate.player)return;
                  if(soldierAttack.UnitSimple.TetriUnitSimple.TetrisBlockSimple.name.Contains(UnitData.Temp))return;
                  if(soldierAttack.UnitSimple.TetriUnitSimple.TetrisBlockSimple.transform.parent == null)return;
                  // 在此处编写碰撞逻辑
                  if(!dealAttackSoldiers.Contains(soldierAttack)) 
                  {
                        dealAttackSoldiers.Add(soldierAttack);
                  }  
            } 
            
           
      }
      private IEnumerator WaitParticleDestory(SoldierBehaviors soldier,ParticleSystem particleSystem)
      {
            // 等待粒子特效开始播放
            particleSystem.Play();
            while (particleSystem.isPlaying)
            {
                  yield return null;
            }
            // 执行你想要执行的操作
            Destroy(particleSystem.gameObject);
            
      }
      public SoldierBehaviors GetClosestSoldier()
      {
            SoldierBehaviors closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            
            foreach (SoldierBehaviors soldier in soldiers)
            {
                  if(soldier.ChainTransfer.collected) continue;
                  if(soldier.ChainTransfer.counted) continue;
                  if(soldier == Self) continue;
                  if(soldier.morale.soldierType != Self.morale.soldierType) continue;

                  Vector3 distanceToEnemy = soldier.transform.position - currentPosition;
                  float distanceSqrToEnemy = distanceToEnemy.sqrMagnitude;

                  if (distanceSqrToEnemy < closestDistanceSqr)
                  {
                        closestDistanceSqr = distanceSqrToEnemy;
                        closest = soldier;
                  }
            }

            return closest;
      }
      public SoldierBehaviors GetClosestSoldier(List<SoldierBehaviors> soldiers)
      {
            SoldierBehaviors closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            
            foreach (SoldierBehaviors soldier in soldiers)
            {
                  if(soldier.ChainTransfer.collected) continue;
                  if(soldier.ChainTransfer.counted) continue;
                  if(soldier == Self) continue;
                  if(soldier.morale.soldierType != Self.morale.soldierType) continue;

                  Vector3 distanceToEnemy = soldier.transform.position - currentPosition;
                  float distanceSqrToEnemy = distanceToEnemy.sqrMagnitude;

                  if (distanceSqrToEnemy < closestDistanceSqr)
                  {
                        closestDistanceSqr = distanceSqrToEnemy;
                        closest = soldier;
                  }
            }

            return closest;
      }
      List<SoldierBehaviors> tempSoldiers = new();
      List<SoldierBehaviors> GetSoldiersArranged(SoldierBehaviors firstSoldier)
      {
            FindSoliderRecursion(firstSoldier,soldiers);
            tempSoldiers.ForEach(x=>x.ChainTransfer.counted = false);
            var tempSoldiersReturn = tempSoldiers;
            tempSoldiers = new();
            return tempSoldiersReturn;
      }
      void FindSoliderRecursion(SoldierBehaviors next,List<SoldierBehaviors> soldiers)
      {
            SoldierBehaviors soldier = next.ChainTransfer.GetClosestSoldier(soldiers);
            if(soldier == null)return;
            if(!tempSoldiers.Contains(soldier))tempSoldiers.Add(soldier);
            soldier.ChainTransfer.counted = true;
            FindSoliderRecursion(soldier,soldiers);
      }
      // List<SoldierBehaviors> GetSoldiersArranged(SoldierBehaviors firstSoldier)
      // {
      //       // soldiers = GetSameTypeSoldier();
      //       // AllSoldiers();
      //       List<SoldierBehaviors> tempSoldiers = new List<SoldierBehaviors>();

      //       firstSoldier.chainTransfer.counted = true;
      //       tempSoldiers.Add(firstSoldier);

      //       SoldierBehaviors nextSoldier = null;
      //       for(int i = 0; i < this.soldiers.Count-1; i++)
      //       {
      //             if(nextSoldier == null)
      //             {
      //                   nextSoldier = firstSoldier.chainTransfer.GetClosestSoldier();
      //                   if(nextSoldier!=null)
      //                   {
      //                         nextSoldier.chainTransfer.counted = true;
      //                         tempSoldiers.Add(nextSoldier);
      //                   }
      //             }else
      //             {
      //                   nextSoldier = nextSoldier.chainTransfer.GetClosestSoldier();
      //                   if(nextSoldier!=null)
      //                   {
      //                         nextSoldier.chainTransfer.counted = true;
      //                         tempSoldiers.Add(nextSoldier);
      //                   }     
                        
      //             }
                  
      //       }
      //       foreach(SoldierBehaviors s in tempSoldiers)
      //       {
      //           s.chainTransfer.counted = false;
      //       }

      //       return tempSoldiers;
      // }
      void Display_setSkine()
      {
            // if(!skillCooldown) skillCooldown = FindObjectOfType<SkillCooldown>();
            // if(!skillCooldownTimer) skillCooldownTimer = FindObjectOfType<SkillCooldownTimer>();
            if(Self.skinName!="")
            {
                  switch(Self.morale.soldierType)
                  {
                        case MoraleTemplate.SoldierType.Red:
                             boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_red");
                             boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_red");
                             // skillCooldown = skillCooldownTimer.countersRedUI;
                             // skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Blue:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_blue");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_blue");
                              // skillCooldown = skillCooldownTimer.counterBlueUI;
                              // skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Green:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_green");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_green");
                              // skillCooldown = skillCooldownTimer.countersGreenUI;
                              // skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Purple:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_purple");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_purple");
                              // skillCooldown = skillCooldownTimer.counterPurpleUI;
                              // skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                  }
            }     
            
      }
      void Init()
      {
            if(Self.unitBase!=null)
            {
                  Self.unitBase.OnStartCollect += UnitBaseCollected;
                  Self.unitBase.OnDie += UnitBaseCollected;
            }
            // skillCooldownTimer = FindObjectOfType<SkillCooldownTimer>();
            next = null;
            duration = 1.0f;
            extendDistance = 6f;
            originDuration = duration;
            counted = false;
            theFirst = false;
            gloableIndex = 0;
            duration = originDuration;
            bombIsMoving = false;
            preview = false;
            collected = false;
            this.Self.morale.soldierType =  transform.GetComponent<SoldierBehaviors>().morale.soldierType;
            capsuleCollider = GetComponent<CapsuleCollider>();
            Invoke(nameof(Display_setSkine), 0.1f);
            // selectionCircle = self.unitBase.selectionCircle; 
            mechanismInPut = FindObjectOfType<MechanismInPut>();
            if(!mechanismInPut)return;
            // mechanismInPut.modeChangeAction += ModeChangedAction;
            // mechanismInPut.allSoldiers += AllSoldiers;
            
      }
      void ReflashMechanismStart()
      {
            mechanismInPut.modeTest = MechanismInPut.ModeTest.Reflash;
      }
      void ReflashMechanismEnd()
      {
            mechanismInPut.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
            mechanismInPut.modeTest = MechanismInPut.ModeTest.ChainTransfer;
      }
      private void OnDrawGizmos()
      {
            if (capsuleCollider == null)return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + capsuleCollider.center, capsuleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z));
      }
}