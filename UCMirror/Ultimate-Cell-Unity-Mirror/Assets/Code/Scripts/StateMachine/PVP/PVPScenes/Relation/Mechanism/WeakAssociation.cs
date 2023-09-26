using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using UC_PlayerData;
public class WeakAssociation : MonoBehaviour, ISoldierRelation
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
      //通讯
      public UnityAction<PuppetEffectDataStruct> OnPlayEffect;

      MechanismInPut mechanismInPut;
      public List<SoldierBehaviors> soldiers = new();
      public SoldierBehaviors self;
      MechanismInPut.ModeTest modeTest;
      // 弱势关联表现
      
      // 设置要使用的 Sorting Layer 名称
      public string sortingLayerName = "YourSortingLayerName";

      // 设置要使用的 Sorting Order
      public int sortingOrder = 0;

      public Material weakAssociationMat;
      public List<Material> weakAssociationMats;
     
      public LineRenderer lineRenderer; // 组件
      public SoldierBehaviors associationTarget; // 终点
      public SoldierBehaviors associationSource; // 起点
      public int numberOfPoints = 21; // 抛物线上的点数
      
      private Vector3[] points; // 抛物线上的所有点
      public bool active = false;
      public UnityAction<BlocksData.BlocksMechanismType> WeakAssociationActive;
      public void Start()
      {
            self = transform.GetComponent<SoldierBehaviors>();
            if(self.unitBase!=null)
            {
                  self.unitBase.OnStartCollect += UnitBaseCollected;
                  self.unitBase.OnDie += UnitBaseCollected;
            }
            Invoke(nameof(SetSkine), 0.1f);
            mechanismInPut = FindObjectOfType<MechanismInPut>();
            if(!mechanismInPut)return;
            // mechanismInPut.modeChangeAction += ModeChangedAction;
            // mechanismInPut.allSoldiers += AllSoldiers;
      }
      void Update()
      {
            if(active)SoldiersUpdateRelation(associationSource,associationTarget);
      }
      public void Active()
      {
            SoldierBehaviors to =  GetClosestSoldier();
            SoldiersStartRelation(self,to);
            active = true;
      }
      public void Stop()
      {
            active = false;
            if(lineRenderer ==null)return;
            SoldiersEndRelation(associationSource,associationTarget);
      }
      // void ModeChangedAction(MechanismInPut.ModeTest modeTest)
      // {
      //       this.modeTest = modeTest;
            
      //       if(modeTest != MechanismInPut.ModeTest.WeakAssociation)
      //       {
      //             if(lineRenderer!=null)
      //             {
      //                   SoldiersEndRelation(associationSource,associationTarget);
      //             }
                  
      //       }else
      //       {
      //             SoldierBehaviors to =  GetClosestSoldier();
      //             SoldiersStartRelation(self,to);
      //       }
      // }

      public void SoldiersStartRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            needRender = true;
            if(from == null || to == null) return;
            to.positiveEffect.Play();
            from.positiveEffect.Play();
            associationSource = from;
            associationTarget = to;
            // UnitSimple 需要处理的事情
            // to.weakAssociation.WeakAssociationActive?.Invoke(BlocksData.BlocksMechanismType.WeakAssociation);
            // from.weakAssociation.WeakAssociationActive?.Invoke(BlocksData.BlocksMechanismType.WeakAssociation);
            // 玩偶表现
            // PuppetEffectDataStruct p = new (PuppetEffectDataStruct.EffectType.Positive);
            // from.weakAssociation.OnPlayEffect?.Invoke(p);
            // to.weakAssociation.OnPlayEffect?.Invoke(p);
            
            // Line表现
            GameObject lineObject = new("LineRenderer");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2; // 设置线段的顶点数
            lineRenderer.SetPosition(0, from.transform.position); // 设置起点坐标
            lineRenderer.SetPosition(1, to.transform.position); // 设置终点坐标
            lineRenderer.startColor = Color.clear + Color.red * 0.9f; // 设置线段起点颜色
            lineRenderer.endColor =  Color.clear + Color.blue* 0.9f; // 设置线段终点颜色
            lineRenderer.startWidth = 0.3f; // 设置线段起点宽度
            lineRenderer.endWidth = 0.3f; // 设置线段终点宽度
            lineRenderer.numCapVertices = 6; // 设置线段端点的圆滑度
            if(SortingLayer.NameToID(sortingLayerName)!=0)
            {
                  int sortingLayerID = SortingLayer.NameToID(sortingLayerName);
                  lineRenderer.sortingLayerID = sortingLayerID;
                  lineRenderer.sortingOrder = sortingOrder;
            }
            
            Renderer lineRendererRenderer = lineRenderer.GetComponent<Renderer>();
            lineRendererRenderer.material = weakAssociationMat;
            lineRendererRenderer.enabled = needRender;
            this.lineRenderer = lineRenderer;
            // DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
            // {
            //       PuppetEffectDataStruct p2 = new (PuppetEffectDataStruct.EffectType.WeakAssociationStart,to.transform.position,sortingLayerName);
            //       from.weakAssociation.OnPlayEffect?.Invoke(p2);
            // });
            
      }

      public void SoldiersUpdateRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(from == null || to == null) return;
            // 每当位置改变，更新抛物线
            if(from.transform.hasChanged || to.transform.hasChanged)
            {
                  CreatePoints();
            }
            
      }
      public void SoldiersEndRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            to.positiveEffect.Play();
            from.positiveEffect.Play();
            // SoldierBehaviors 需要处理的事情
            to.morale.AddMorale(to, 0.6f, true);
            to.morale.EffectByMorale(to,ref to.strength);
            from.morale.AddMorale(to, 0.6f, true);
            from.morale.EffectByMorale(to,ref to.strength);
            // UnitSimple 需要处理的事情
            to.weakAssociation.WeakAssociationActive?.Invoke(BlocksData.BlocksMechanismType.WeakAssociation);
            from.weakAssociation.WeakAssociationActive?.Invoke(BlocksData.BlocksMechanismType.WeakAssociation);
            Destroy(lineRenderer.gameObject);
      }
#region 数据操作
      void UnitBaseCollected(Unit u)
      {
            if(lineRenderer!=null)
            {
                  SoldiersEndRelation(associationSource,associationTarget);
            }
            // mechanismInPut.modeChangeAction -= ModeChangedAction;
            // mechanismInPut.allSoldiers -= AllSoldiers;
      }
      private void CreatePoints()
      {
            if(lineRenderer == null) return;

            // PuppetEffectDataStruct p = new (PuppetEffectDataStruct.EffectType.WeakAssociationUpdate);
            // self.weakAssociation.OnPlayEffect?.Invoke(p);

            // 创建抛物线上的所有点
            points = new Vector3[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                  float t = i / (float)(numberOfPoints - 1);
                  points[i] = CalculatePoint(t);
            }

            // 更新 LineRenderer 组件
            lineRenderer.positionCount = numberOfPoints;
            lineRenderer.SetPositions(points);
      }
      private Vector3 CalculatePoint(float t)
      {
            float lineHeight = 1.8f; // 抛物线的高度
            // 计算抛物线上的单个点
            float x = Mathf.Lerp(associationSource.transform.position.x, associationTarget.transform.position.x, t);
            float y = Mathf.Lerp(associationSource.transform.position.y, associationTarget.transform.position.y, t);
            float z = Mathf.Lerp(associationSource.transform.position.z, associationTarget.transform.position.z, t);

            // 计算抛物线高度
            y += Mathf.Sin(t * Mathf.PI)* lineHeight;

            return new Vector3(x, y, z);
      }
      void AllSoldiers(List<SoldierBehaviors> soldiers)
      {
            this.soldiers = soldiers;
      }
      // void AllSoldiers()
      // {
      //       this.soldiers = new(FindObjectsOfType<SoldierBehaviors>().Where(x=>AllSoldiersChecker(x)).ToList());
      // }
      // bool AllSoldiersChecker(SoldierBehaviors soldier)
      // {
      //       List<bool> condition = new();
      //       if(!soldier)return false;
      //       if(!self)Start();
      //       if(soldier.UnitSimple.IsDeadOrNull(soldier.UnitSimple))return false;
      //       // 不包含培养皿中的砖块
      //       Transform p = soldier.UnitSimple.tetriUnitSimple.TetrisBlockSimple.transform.parent;
      //       if(p==null)return false;
      //       // 不包含同类玩家
      //       bool player = soldier.Player==self.Player;
      //       condition.Add(player);
      //       // 不包含拖拽的临时士兵
      //       bool draging = !soldier.UnitSimple.tetriUnitSimple.TetrisBlockSimple.name.Contains(UnitData.Temp);
      //       condition.Add(draging);
      //       bool allTrue = condition.All(b => b);
      //       return allTrue;
      // }
      SoldierBehaviors GetClosestSoldier()
      {     
            // AllSoldiers();
            SoldierBehaviors closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (SoldierBehaviors soldier in soldiers)
            {
                  if(soldier.weakAssociation.associationTarget!=null) continue;
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
      void SetSkine()
      {
            if(self.skinName=="")return;
            switch(self.morale.soldierType)
            {
                  case MoraleTemplate.SoldierType.Red:
                       weakAssociationMat = weakAssociationMats.Find(x=>x.name == "FourDirLinkEffect_red");
                  break;
                  case MoraleTemplate.SoldierType.Blue:
                        weakAssociationMat = weakAssociationMats.Find(x=>x.name == "FourDirLinkEffect_blue");
                  break;
                  case MoraleTemplate.SoldierType.Green:
                        weakAssociationMat = weakAssociationMats.Find(x=>x.name == "FourDirLinkEffect_green");
                  break;
                  case MoraleTemplate.SoldierType.Purple:
                        weakAssociationMat = weakAssociationMats.Find(x=>x.name == "FourDirLinkEffect_purple");
                  break;
            }
                 
            
      }
#endregion 数据操作
}