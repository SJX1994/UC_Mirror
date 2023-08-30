using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.AI;
using Unity.VisualScripting;

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
      List<Soldier> soldiers = new();
      MechanismInPut.ModeTest modeTest;
      SkillCooldownTimer skillCooldownTimer;
      SkillCooldown skillCooldown;
      
      Soldier self;
      Soldier next;
      bool counted = false;
      public bool collected = false;
      public bool theFirst = false;
      CapsuleCollider capsuleCollider;
      [HideInInspector]
      public float duration = 0.1f;
      [HideInInspector]
      public float originDuration;
      int gloableIndex = 0;
      Soldier[] gloableSoldiers;
      [HideInInspector]
      public bool bombIsMoving = false;
      bool chainTransfering = false; // 正在传递 碰撞伤害计算
      List<Soldier> dealAttackSoldiers = new(); // 需要处理伤害的士兵们
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
      
      void Start()
      {
            needRender = true;
            chainDamage = 10;
            chainDamageTotal = 0;
            Invoke(nameof(Init), 0.1f);
      }
      void AllSoldiers(List<Soldier> soldiers)
      {
            this.soldiers = soldiers;
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
            skillCooldown.NotActive();
            theFirst = true;
            this.soldiers = GetSameTypeSoldier(self);
           
            foreach(var soldier in this.soldiers)
            {
                  soldier.chainTransfer.bombIsMoving = true;
            }
            gloableSoldiers = new Soldier[this.soldiers.Count];
            gloableIndex = 0;
            StartCoroutine(Forward_ExecuteEveryFewSeconds());
      }
      public IEnumerator LastChain(Soldier[] soldiers, int tempIndex)
      {
            
            for(int i = soldiers.Length-1; i >= 0; i--)
            {
                  tempIndex += 1;
                  if(soldiers[i]==null) continue;
                  if(soldiers[i].chainTransfer.theFirst == true)
                  {
                        float tempDuration = soldiers[i].chainTransfer.duration;
                        soldiers[i].chainTransfer.duration *= tempDuration/tempIndex;
                        soldiers[i].chainTransfer.SoldiersEndRelation(soldiers[i+1],soldiers[i]);
                  }else
                  {
                        float tempDuration = soldiers[i].chainTransfer.duration;
                        soldiers[i].chainTransfer.duration *= tempDuration/tempIndex;
                        float modifyDuration = soldiers[i].chainTransfer.duration;
                        soldiers[i].chainTransfer.SoldiersUpdateRelation(soldiers[i],soldiers[i-1]);
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
                  gloableIndex+=1;
                  gloableSoldiers[0] = self;
                  counted = true;
                  next = self.chainTransfer.GetClosestSoldier();
                  chainDamageTotal += chainDamage;
                  SoldiersStartRelation(self,next);
                  soldiers.Remove(self);
                  yield return new WaitForSeconds(duration);
            }
            
            
            foreach (var soldier in soldiers)
            {     
                  gloableSoldiers[gloableIndex] = soldier;
                  gloableIndex += 1;
                  float tempDuration = soldier.chainTransfer.duration;
                  soldier.chainTransfer.duration *= tempDuration/gloableIndex;
                  float modifyDuration = soldier.chainTransfer.duration;
                  
                  if(gloableIndex-1 == soldiers.Count)
                  {
                       StartCoroutine(soldier.chainTransfer.LastChain(gloableSoldiers,gloableIndex));
                       continue; 
                       
                  }
                  soldier.chainTransfer.next = soldiers[gloableIndex-1];
                  soldier.chainTransfer.SoldiersStartRelation(soldier,soldier.chainTransfer.next);
                  soldier.chainTransfer.duration = tempDuration;
                  chainDamageTotal += soldier.chainTransfer.chainDamage;
                  // 在指定的间隔后继续执行下一次
                  // Debug.Log("modifyDuration"+modifyDuration);
                  yield return new WaitForSeconds(modifyDuration);
            }
            
            

            
      }
   
      public void SoldiersStartRelation(Soldier from, Soldier to)
      {
            if(from == null || to == null) return;
            to.chainTransfer.counted = true;
            Vector3 backPosition = from.transform.position;
            Vector3 targetPosition = to.transform.position;

            // 停止状态机的移动
            NavMeshAgent agent;
            agent = from.transform.TryGetComponent<NavMeshAgent>(out agent) ? agent : null;
            if(agent != null)
            {
                  // AICommand stop = new AICommand(AICommand.CommandType.Stop);
                  // from.unitBase.ExecuteCommand(stop);
                  // from.unitBase.controled = true;
            }
            
            from.transform.DOMove(targetPosition, from.chainTransfer.duration/2).onComplete = () =>
            {
                  to.positiveEffect.Play();
                  PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.Positive);
                  to.chainTransfer.OnPlayEffect?.Invoke(p); // 玩偶事件

                  from.transform.DOMove(backPosition, from.chainTransfer.duration/2).onComplete = () =>
                  {  
                  };
            };
      }

      public void SoldiersUpdateRelation(Soldier from, Soldier to)
      {
            if(from == null || to == null) return;
            to.chainTransfer.counted = false;
            Vector3 backPosition = from.transform.position;
            Vector3 targetPosition = to.transform.position;
            
            from.transform.DOMove(targetPosition, from.chainTransfer.duration/2).onComplete = () =>
            {
                  
                  to.positiveEffect.Play();
                  PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.Positive);
                  to.chainTransfer.OnPlayEffect?.Invoke(p); // 玩偶事件
                  from.transform.DOMove(backPosition, from.chainTransfer.duration/2).onComplete = () =>
                  {  
                        // 允许状态机的移动
                        NavMeshAgent agent;
                        agent = from.transform.TryGetComponent<NavMeshAgent>(out agent) ? agent : null;
                        if(agent != null)
                        {
                              // AICommand guard = new AICommand(AICommand.CommandType.GoToAndGuard, backPosition);
                              // from.unitBase.ExecuteCommand(guard);
                              // from.unitBase.controled = false;
                        }
                  };
            };
      }
      public void SoldiersEndRelation(Soldier from, Soldier to)
      {
           
       // 发出射线
            targetPosition = Vector3.zero;
        if (Physics.Raycast(from.transform.position, (to.transform.position - from.transform.position), out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            // 如果射线与物体相交，则打印相交点和相交物体的名称
            // Debug.Log("Intersection point: " + hit.point);
            // Debug.Log("Intersected object: " + hit.collider.gameObject.name);
            targetPosition = hit.point;
            bombIsMoving = true;

        }
        else
        {
            //TODO 增强士气？
        }
        if (targetPosition == Vector3.zero)
            {
                  // 计算延长线上的点
                  Vector3 direction = (to.transform.position - from.transform.position).normalized;
                  Vector3 extendedPoint = to.transform.position + (direction * extendDistance);
                  targetPosition = extendedPoint;
                  
            }
            if(bombIsMoving)
            {
                  Vector3 backPos = to.transform.position;
                  Vector3 targetPos = this.targetPosition;
                  //大炮发射表现
                  PuppetEffectDataStruct ped = new(PuppetEffectDataStruct.EffectType.ChainTransferTrail,targetPos,duration/2);
                  to.chainTransfer.OnPlayEffect?.Invoke(ped);

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

                  to.chainTransfer.ChainTransfering(originDuration);

                  float ortemp = originDuration;
                  to.transform.DOMove(targetPos,to.chainTransfer.duration/2).onComplete = () =>
                  { 
                        obj.transform.position = targetPos;
                        
                        // 使用DOTween.To方法来改变颜色
                        Color targetColor = new(trail.endColor.r,trail.endColor.g,trail.endColor.b,0f);
                        DOTween.To(() => trail.startColor, color => trail.startColor = color, targetColor, originDuration/2);
                        DOTween.To(() => trail.endColor, color => trail.endColor = color, targetColor, originDuration/2);
                        GameObject boonEffect = Instantiate(boonEffectPrefab.gameObject);
                        boonEffect.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = needRender; // 是否需要渲染
                        boonEffect.transform.position = targetPos;
                        boonEffect.GetComponent<ParticleSystem>().Play();
                        PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.ChainTransferBoom,targetPos,ortemp);
                        to.chainTransfer.OnPlayEffect?.Invoke(p); // 玩偶事件
                        Destroy(obj,originDuration);
                        Destroy(boonEffect,originDuration);
                        to.transform.DOMove(targetPos,to.chainTransfer.originDuration/2).onComplete = () =>
                        {
                              to.transform.DOMove(backPos,to.chainTransfer.originDuration).onComplete = () =>
                              {
                                    // 公告
                                    mechanismInPut.warningSystem.inText1 = (soldiers.Count+1).ToString();
                                    mechanismInPut.warningSystem.inText2 = self.morale.soldierType.ToString();
                                    mechanismInPut.warningSystem.inText3 = chainDamageTotal.ToString();
                                    mechanismInPut.warningSystem.changeWarningTypes = WarningSystem.WarningType.ChainTransfer;
                                    //复原
                                    foreach(Soldier soldier in GetSameTypeSoldier())
                                    {
                                          soldier.chainTransfer.counted = false;
                                          soldier.chainTransfer.theFirst = false;
                                          soldier.chainTransfer.gloableIndex = 0;
                                          soldier.chainTransfer.duration = originDuration;
                                          // soldier.chainTransfer.bombIsMoving = false;
                                          soldier.chainTransfer.preview = false;
                                          skillCooldown.NotActive(true);
                                    }
                                    
                                    
                                    
                              };
                                    
                              
                              
                        };
                        
                  };
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
	    selectionCircle.gameObject.SetActive(chainTransfering);
          // 碰撞扣血
          capsuleCollider.radius = 3.9f;
          
	    DOVirtual.DelayedCall(duration, () =>
          {
            chainTransfering = false;
	  	selectionCircle.gameObject.SetActive(chainTransfering);
            
            foreach(Soldier ss in dealAttackSoldiers)
            {
                 
                 if(ss.chainTransfer.boonEffectPrefab)
                 {
                     
                     if(!self.unitBase.IsDeadOrNull(ss.unitBase))
                     {
                        GameObject boonEffect = Instantiate(ss.chainTransfer.boonEffectPrefab.gameObject);
                        boonEffect.transform.position = ss.transform.position;
                        ss.unitBase.SufferAttack(chainDamageTotal);
                        
                        
                        StartCoroutine(WaitParticleDestory(ss,boonEffect.GetComponent<ParticleSystem>()));
                     }
                     
                 }
                 
            }
            capsuleCollider.radius = colliderRadiusTemp;
            dealAttackSoldiers.Clear();
            
          });
      }
      void OnTriggerEnter(Collider other)
      {
           
            if(chainTransfering)
            {
                  // 判断进入碰撞的物体
                  if (other.transform.TryGetComponent(out Soldier soldierAttack))
                  {
                        

                        if(self.unitBase.unitTemplate.GetOtherUnitType() == soldierAttack.unitBase.unitTemplate.unitType)
                        {
                              // 在此处编写碰撞逻辑
                              
                              if(!dealAttackSoldiers.Contains(soldierAttack)) 
                              {
                                    dealAttackSoldiers.Add(soldierAttack);
                              }
                              
                        }
                  } 
            }
           
      }
      private IEnumerator WaitParticleDestory(Soldier soldier,ParticleSystem particleSystem)
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
      public Soldier GetClosestSoldier()
      {
            Soldier closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            
            foreach (Soldier soldier in soldiers)
            {
                  if(soldier.chainTransfer.collected) continue;
                  if(soldier.chainTransfer.counted) continue;
                  if(soldier == self) continue;
                  if(soldier.morale.soldierType != self.morale.soldierType) continue;

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
      public List<Soldier> GetSameTypeSoldier()
      {
            List<Soldier> tempSoldiers = new();
            
            foreach(Soldier soldier in FindObjectsOfType<Soldier>())
            {
                if(soldier.morale.soldierType != self.morale.soldierType) continue;
                if(soldier.unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)continue;
                tempSoldiers.Add(soldier);
            }

            List<Soldier> tempS = new(tempSoldiers);
            
            foreach(Soldier s in tempS)
            {
                  if(s.chainTransfer.collected == true || s.fourDirectionsLinks.forceBreakLink == true)
                  {
                        tempSoldiers.Remove(s);
                  }
            }
            
            return tempSoldiers;
      }
      List<Soldier> GetSameTypeSoldier(Soldier firstSoldier)
      {
            soldiers = GetSameTypeSoldier();
            List<Soldier> tempSoldiers = new List<Soldier>();

            firstSoldier.chainTransfer.counted = true;
            tempSoldiers.Add(firstSoldier);

            Soldier nextSoldier = null;

            for(int i = 0; i < this.soldiers.Count-1; i++)
            {
                  if(nextSoldier == null)
                  {
                        nextSoldier = firstSoldier.chainTransfer.GetClosestSoldier();
                        if(nextSoldier!=null)
                        {
                              nextSoldier.chainTransfer.counted = true;
                              tempSoldiers.Add(nextSoldier);
                        }
                  }else
                  {
                        nextSoldier = nextSoldier.chainTransfer.GetClosestSoldier();
                        if(nextSoldier!=null)
                        {
                              nextSoldier.chainTransfer.counted = true;
                              tempSoldiers.Add(nextSoldier);
                        }     
                        
                  }
                  
            }
            foreach(Soldier s in tempSoldiers)
            {
                s.chainTransfer.counted = false;
            }

            return tempSoldiers;
      }
      void SetSkine()
      {
            
            if(self.skinName!="")
            {
                  switch(self.morale.soldierType)
                  {
                        case MoraleTemplate.SoldierType.Red:
                             boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_red");
                             boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_red");
                             skillCooldown = skillCooldownTimer.countersRedUI;
                             skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Blue:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_blue");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_blue");
                              skillCooldown = skillCooldownTimer.counterBlueUI;
                              skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Green:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_green");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_green");
                              skillCooldown = skillCooldownTimer.countersGreenUI;
                              skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                        case MoraleTemplate.SoldierType.Purple:
                              boonMat = boonMats.Find(x=>x.name == "FourDirLinkEffect_purple");
                              boonEffectPrefab = boonEffectPrefabs.Find(x=>x.name == "BoomEffect_purple");
                              skillCooldown = skillCooldownTimer.counterPurpleUI;
                              skillCooldown.CooldownReady += x =>{if(x == self.morale.soldierType)bombIsMoving = false;};
                        break;
                  }
            }     
            
      }
      void Init()
      {
            self = transform.GetComponent<Soldier>();
            if(self.unitBase!=null)
            {
                  self.unitBase.OnStartCollect += UnitBaseCollected;
                  self.unitBase.OnDie += UnitBaseCollected;
            }
            mechanismInPut = FindObjectOfType<MechanismInPut>();
            mechanismInPut.modeChangeAction += ModeChangedAction;
            mechanismInPut.allSoldiers += AllSoldiers;
            
            skillCooldownTimer = FindObjectOfType<SkillCooldownTimer>();
            

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
            this.self.morale.soldierType =  transform.GetComponent<Soldier>().morale.soldierType;
            capsuleCollider = GetComponent<CapsuleCollider>();
            Invoke(nameof(SetSkine), 0.1f);
            selectionCircle = self.unitBase.selectionCircle; 
            
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
            if (capsuleCollider != null)
            {
                  Gizmos.color = Color.red;
                  Gizmos.DrawWireSphere(transform.position + capsuleCollider.center, capsuleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z));
            }
      }
}