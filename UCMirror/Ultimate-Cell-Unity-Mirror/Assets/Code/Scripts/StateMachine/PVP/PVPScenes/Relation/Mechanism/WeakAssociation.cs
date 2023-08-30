using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
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
      List<Soldier> soldiers = new();
      Soldier self;
      MechanismInPut.ModeTest modeTest;
      // 弱势关联表现
      
      // 设置要使用的 Sorting Layer 名称
      public string sortingLayerName = "YourSortingLayerName";

      // 设置要使用的 Sorting Order
      public int sortingOrder = 0;

      public Material weakAssociationMat;
      public List<Material> weakAssociationMats;
     
      public LineRenderer lineRenderer; // 组件
      public Soldier associationTarget; // 终点
      public Soldier associationSource; // 起点
      public int numberOfPoints = 21; // 抛物线上的点数
      
      private Vector3[] points; // 抛物线上的所有点
      
      
      void Start()
      {
            mechanismInPut = FindObjectOfType<MechanismInPut>();
            mechanismInPut.modeChangeAction += ModeChangedAction;
            mechanismInPut.allSoldiers += AllSoldiers;
            self = transform.GetComponent<Soldier>();
            if(self.unitBase!=null)
            {
                  self.unitBase.OnStartCollect += UnitBaseCollected;
                  self.unitBase.OnDie += UnitBaseCollected;
            }
            Invoke(nameof(SetSkine), 0.1f);
           
      }
      void Update()
      {
            if(modeTest == MechanismInPut.ModeTest.WeakAssociation) 
            {
                  SoldiersUpdateRelation(associationSource,associationTarget);
            }
            
      }
      void ModeChangedAction(MechanismInPut.ModeTest modeTest)
      {
            this.modeTest = modeTest;
            
            if(modeTest != MechanismInPut.ModeTest.WeakAssociation)
            {
                  if(lineRenderer!=null)
                  {
                        SoldiersEndRelation(associationSource,associationTarget);
                  }
                  
            }else
            {
                  Soldier to =  GetClosestSoldier();
                  SoldiersStartRelation(self,to);
            }
      }

      public void SoldiersStartRelation(Soldier from, Soldier to)
      {
            
            if(from == null || to == null) return;
            to.positiveEffect.Play();
            from.positiveEffect.Play();
            associationSource = from;
            associationTarget = to;
            // 玩偶表现
            PuppetEffectDataStruct p = new (PuppetEffectDataStruct.EffectType.Positive);
            from.weakAssociation.OnPlayEffect?.Invoke(p);
            to.weakAssociation.OnPlayEffect?.Invoke(p);
            
            // Line表现
            GameObject lineObject = new("LineRenderer");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2; // 设置线段的顶点数
            lineRenderer.SetPosition(0, from.transform.position); // 设置起点坐标
            lineRenderer.SetPosition(1, to.transform.position); // 设置终点坐标
            lineRenderer.startColor = Color.red; // 设置线段起点颜色
            lineRenderer.endColor = Color.blue; // 设置线段终点颜色
            lineRenderer.startWidth = 0.1f; // 设置线段起点宽度
            lineRenderer.endWidth = 0.1f; // 设置线段终点宽度
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
             DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
            {
                  PuppetEffectDataStruct p2 = new (PuppetEffectDataStruct.EffectType.WeakAssociationStart,to.transform.position,sortingLayerName);
                  from.weakAssociation.OnPlayEffect?.Invoke(p2);
            });
            
      }

      public void SoldiersUpdateRelation(Soldier from, Soldier to)
      {
            if(from == null || to == null) return;
            // 每当位置改变，更新抛物线
            if(from.transform.hasChanged || to.transform.hasChanged)
            {
                  CreatePoints();
            }
            
      }
      public void SoldiersEndRelation(Soldier from, Soldier to)
      {
            to.morale.AddMorale(to, 0.1f, true);
            to.morale.EffectByMorale(to,ref to.strength);
            Destroy(lineRenderer.gameObject);
      }
#region 数据操作
      void UnitBaseCollected(Unit u)
      {
            if(lineRenderer!=null)
            {
                  SoldiersEndRelation(associationSource,associationTarget);
            }
            mechanismInPut.modeChangeAction -= ModeChangedAction;
            mechanismInPut.allSoldiers -= AllSoldiers;
      }
      private void CreatePoints()
      {
            if(lineRenderer == null) return;

            PuppetEffectDataStruct p = new (PuppetEffectDataStruct.EffectType.WeakAssociationUpdate);
            self.weakAssociation.OnPlayEffect?.Invoke(p);

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
      void AllSoldiers(List<Soldier> soldiers)
      {
            this.soldiers = soldiers;
      }
      Soldier GetClosestSoldier()
      {
            Soldier closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Soldier soldier in soldiers)
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
            if(self.skinName!="")
            {
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
            
      }
#endregion 数据操作
}