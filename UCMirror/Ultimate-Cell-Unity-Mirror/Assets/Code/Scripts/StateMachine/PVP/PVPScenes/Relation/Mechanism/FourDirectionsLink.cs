
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
public class FourDirectionsLink: MonoBehaviour, ISoldierRelation
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
      MechanismInPut.ModeTest modeTest;
      /// <summary>
      /// 四个方向的士兵
      ///   N
      /// W + E
      ///   S
      /// </summary>
      public SoldierBehaviors North,East,South,West;
      public enum Direction
      {
            North,
            East,
            South,
            West
      }
      private readonly Dictionary< SoldierBehaviors,Direction> direction_Soldier_dir = new();
     

      [HideInInspector]
      public SoldierBehaviors self;
      [HideInInspector]
      public float range = 3f;
      [HideInInspector]
      public Color rangeColor = Color.green;
      float linkMinStrengthIncrease = 0.2f; // 联结最小强度增加
      /// <summary>
      /// 表现
      /// </summary>
      // 设置要使用的 Sorting Layer 名称
      public string sortingLayerName = "YourSortingLayerName";

      // 设置要使用的 Sorting Order
      public int sortingOrder = 0;
      
      public Material fourDirectionsLinkLinkMat;
      public List<Material> fourDirectionsLinkLinkMats;

      readonly Dictionary<SoldierBehaviors,LineRenderer> lineRenderers = new();  // Line Renderer组件
      [HideInInspector]
      public List<SpriteRenderer> levelSpriteRenders;
      public bool forceBreakLink;

    

    void Start()
      {
            linkMinStrengthIncrease = 0.2f;
            self = transform.GetComponent<SoldierBehaviors>();
            forceBreakLink = false;
            // 联结表现
            Transform spine = transform.Find("Spine");             
            foreach(SpriteRenderer spriteRender in levelSpriteRenders)
            {
                  if(spriteRender!=null)
                  {
                        spriteRender.color = new Color(1,1,1,0.5f);
                  }
                  
            }
            if(self.unitBase!=null)
            {
                  self.unitBase.OnStartCollect += UnitBaseCollected;
                  self.unitBase.OnDie += UnitBaseCollected;
            }
            Invoke(nameof(SetSkine), 0.1f);
            mechanismInPut = FindObjectOfType<MechanismInPut>();
            if(!self.unitBase)return;
            if(!self.unitBase.unitTemplate.levelSprite)return;
            for(int i = 1; i < self.unitBase.level+1; i++)
            {
                  levelSpriteRenders.Add(spine.Find("Level_"+i).GetComponent<SpriteRenderer>());
            }
            if(!mechanismInPut)return;
            mechanismInPut.modeChangeAction += ModeChangedAction;
            
      }
      void ModeChangedAction(MechanismInPut.ModeTest modeTest)
      {
            this.modeTest = modeTest;
            if(modeTest == MechanismInPut.ModeTest.FourDirectionsLinks || this.modeTest == MechanismInPut.ModeTest.ChainTransfer || modeTest == MechanismInPut.ModeTest.ChainTransferAndFourDirectionsLinks)
            {
                  StartLink();
            }else
            {
                  Dictionary<SoldierBehaviors, LineRenderer> tempDictionary = new(lineRenderers);
                  foreach (KeyValuePair<SoldierBehaviors,LineRenderer> kvp in tempDictionary)
                  {
                        if(kvp.Value == null)continue;
                        kvp.Key.morale.minMorale = kvp.Key.morale.baseminMorale;
                        GameObject remove = lineRenderers[kvp.Key].gameObject;
                        lineRenderers.Remove(kvp.Key);
                        direction_Soldier_dir.Remove(kvp.Key);
                        Destroy(remove);
                  }
            }
      }
      public void Update()
      {
            if(forceBreakLink || !self)return;
            if(this.modeTest == MechanismInPut.ModeTest.FourDirectionsLinks || this.modeTest == MechanismInPut.ModeTest.ChainTransfer || modeTest == MechanismInPut.ModeTest.ChainTransferAndFourDirectionsLinks)
            {
                  // 持续 + 断开
                  Dictionary<SoldierBehaviors, LineRenderer> tempDictionary = new(lineRenderers);
                  foreach (KeyValuePair<SoldierBehaviors,LineRenderer> kvp in tempDictionary)
                  {
                        if(kvp.Value == null)continue;
                        //if(self.transform.hasChanged || kvp.Key.transform.hasChanged)
                        {
                              SoldiersEndRelation(self, kvp.Key);
                              SoldiersUpdateRelation(self,kvp.Key);
                        }
                  }
                  // 重连
                  StartLink();
            }
      }
      void StartLink()
      {
            if(forceBreakLink)return;
            List<SoldierBehaviors> soldiers = FindObjectsOfType<SoldierBehaviors>().ToList();
      
            foreach (var s in soldiers)
            {
                  
                  if(s == self) continue;
                  if(self == null) continue;
                  if(s == null ) continue;
                  // 获取单位的坐标
                  Vector3 sPosition = self.transform.position;

                  // 计算单位与圆心的距离
                  float distance = Vector3.Distance(sPosition, s.transform.position);
                  if (distance <= range)
                  {
                        // 方位检测
                        Vector3 positionDifference = s.transform.localPosition - self.transform.localPosition;

                        if (positionDifference.x > 0 && East == null )
                        {
                              East = s;
                              if(!AddIfNotExists(direction_Soldier_dir,East,Direction.East))
                              {
                                    SoldiersStartRelation(self,East);
                              }else
                              {
                                    East = null;
                              }
                             
                              
                        }
                        else if (positionDifference.x < 0 && West == null )
                        {
                              West = s;
                              if(!AddIfNotExists(direction_Soldier_dir,West,Direction.West))
                              {
                                    SoldiersStartRelation(self,West);
                              }else
                              {
                                    West = null;
                              }
                             
                              
                              
                              
                        }
                        if (positionDifference.z > 0 && North == null )
                        {
                              North = s;
                              if(!AddIfNotExists(direction_Soldier_dir,North,Direction.North))
                              {
                                    SoldiersStartRelation(self,North);
                              }else
                              {
                                    North = null;
                              }
                             
                              
                              
                              
                        }
                        else if (positionDifference.z < 0 && South == null )
                        {
                              South = s;
                              if(!AddIfNotExists(direction_Soldier_dir,South,Direction.South))
                              {
                                    SoldiersStartRelation(self,South);
                              }else
                              {
                                    South = null;
                              }
                              
                              
                              
                              
                        }
                  }
            }
      }
      
      /// <summary>
      /// 实现联结 关系 开始时的刹那间的表现
      /// 基于心理学的 互助和合作 增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersStartRelation(SoldierBehaviors from,SoldierBehaviors to)
      {
            if(forceBreakLink)return;
            if(!to)return;
            if(!from)return;
            if(!to.unitBase)return;
            if(to.unitBase.unitTemplate.unitType != from.unitBase.unitTemplate.unitType) return;

            // 特效表现
            ParticleSystem ps = to.fourDirectionsLinkStartEffect;
            ps.GetComponent<Renderer>().enabled = needRender;

            // 玩偶表现
            PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.Positive);
            to.fourDirectionsLinks.OnPlayEffect?.Invoke(p); 

            if(ps!=null)
            {
                  StartCoroutine(WaitParticleAddMorale(to,ps));
            }
            

            if(levelSpriteRenders.Count > 0)
            {
                  if(!to || !from)return;
                  
                  foreach(SpriteRenderer spriteRender in from.fourDirectionsLinks.levelSpriteRenders)
                  {
                        if(spriteRender!=null)
                        {
                              spriteRender.color = new Color(1,1,1,1);
                        }
                  }
                  // foreach(SpriteRenderer spriteRender in to.fourDirectionsLinks.levelSpriteRenders)
                  // {
                  //       if(spriteRender!=null)
                  //       {
                  //             spriteRender.color = new Color(1,1,1,1);
                  //       }
                        
                  // }
            }
            
            
            // Line表现
            GameObject lineObject = new("LineRenderer");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderers.Add(to,lineRenderer);
            lineRenderer.positionCount = 2; // 设置线段的顶点数
            lineRenderer.SetPosition(0, from.transform.position); // 设置起点坐标
            lineRenderer.SetPosition(1, to.transform.position); // 设置终点坐标
            lineRenderer.startColor = Color.red; // 设置线段起点颜色
            lineRenderer.endColor = Color.blue; // 设置线段终点颜色
            lineRenderer.startWidth = 0.5f; // 设置线段起点宽度
            lineRenderer.endWidth = 0.1f; // 设置线段终点宽度
            lineRenderer.numCapVertices = 6; // 设置线段端点的圆滑度
            if(SortingLayer.NameToID(sortingLayerName)!=0)
            {
                  int sortingLayerID = SortingLayer.NameToID(sortingLayerName);
                  lineRenderer.sortingLayerID = sortingLayerID;
                  lineRenderer.sortingOrder = sortingOrder;
            }
            Renderer lineRendererRenderer = lineRenderer.GetComponent<Renderer>();
            lineRendererRenderer.material = fourDirectionsLinkLinkMat;
            // 玩偶表现
                  // PuppetEffectDataStruct p2 = new(PuppetEffectDataStruct.EffectType.FourDirectionsLinkerStart,to.unitBase.id,sortingLayerName,direction_Soldier_dir[to]);
                  // from.fourDirectionsLinks.OnPlayEffect?.Invoke(p2); 

            lineRendererRenderer.GetComponent<Renderer>().enabled = needRender;
            
            
      }
      /// <summary>
      /// 实现联结 关系 持续的表现
      /// 给玩家体验到 互助合作的重要性，从而增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersUpdateRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(!to)return;
        if(forceBreakLink)
        {
            SoldiersEndRelation(from,to);
            return;
        }
            Vector3 startPosition = from.transform.position;
            Vector3 endPosition = to.transform.position;
        foreach (KeyValuePair<SoldierBehaviors,Direction> kvp in direction_Soldier_dir)
        {
            if(kvp.Key == to)
            {
                  
                  LineRenderer lineRenderer = lineRenderers[to];
                  // 玩偶表现
                        PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.FourDirectionsLinkerUpdate,kvp.Value,endPosition);
                        from.fourDirectionsLinks.OnPlayEffect?.Invoke(p);
                  switch (kvp.Value)
                  {
                        case Direction.North:
                              lineRenderer.SetPosition(0, startPosition);
                              lineRenderer.SetPosition(1, endPosition);
                        break;
                        case Direction.East:
                              lineRenderer.SetPosition(0, startPosition);
                              lineRenderer.SetPosition(1, endPosition);
                        break;
                        case Direction.South:
                              lineRenderer.SetPosition(0, startPosition);
                              lineRenderer.SetPosition(1, endPosition);
                        break;
                        case Direction.West:
                              lineRenderer.SetPosition(0, startPosition);
                              lineRenderer.SetPosition(1, endPosition);
                        break;

                  }
                  
                 

            }
        }
      }
      /// <summary>
      /// 实现联结 关系 结束的表现
      /// 给玩家体验到 互助合作失去后的乏力，从而增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersEndRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(to == null)return;
            if(forceBreakLink)
            {
                  if(lineRenderers.ContainsKey(to))
                  {
                        PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.FourDirectionsLinkerEnd,direction_Soldier_dir[to]);
                        from.fourDirectionsLinks.OnPlayEffect?.Invoke(p);
                        
                        GameObject remove = lineRenderers[to].gameObject;
                        lineRenderers.Remove(to);
                        direction_Soldier_dir.Remove(to);
                        Destroy(remove);
                  }
                  
                  return;
            }
            if(!self)return;
            float distance = Vector3.Distance(self.transform.position, to.transform.position);
            if(distance > range)
            {
                  
                  switch (direction_Soldier_dir[to])
                  {
                        case Direction.North:
                              North = null;
                        break;
                        case Direction.East:
                              East = null;
                        break;
                        case Direction.South:
                              South = null;
                        break;
                        case Direction.West:
                              West = null;
                        break;
                  }
                  // 特效表现
                  ParticleSystem ps = to.fourDirectionsLinkEndEffect;
                  StartCoroutine(WaitParticleReduceMorale(to,ps));
                  
                  // 玩偶表现
                        PuppetEffectDataStruct p = new(PuppetEffectDataStruct.EffectType.FourDirectionsLinkerEnd,direction_Soldier_dir[to]);
                        from.fourDirectionsLinks.OnPlayEffect?.Invoke(p);

                  if(levelSpriteRenders.Count > 0)
                  {
                        foreach(SpriteRenderer spriteRender in from.fourDirectionsLinks.levelSpriteRenders)
                        {
                              if(spriteRender!=null)
                              {
                                    spriteRender.color = new Color(1,1,1,0.5f);
                              }
                        }
                        foreach(SpriteRenderer spriteRender in to.fourDirectionsLinks.levelSpriteRenders)
                        {
                              if(spriteRender!=null)
                              {
                                    spriteRender.color = new Color(1,1,1,0.5f);
                              }
                              
                        }
                  }
                  

                  GameObject remove = lineRenderers[to].gameObject;
                  lineRenderers.Remove(to);
                  direction_Soldier_dir.Remove(to);
                  Destroy(remove);
            }
            
      }
      /// <summary>
      /// 防止重复添加的检查
      /// </summary>
      /// <param name="dictionary"></param>
      /// <param name="key"></param>
      /// <param name="value"></param>
      /// <typeparam name="TKey"></typeparam>
      /// <typeparam name="TValue"></typeparam>
      /// <returns></returns>
      public static bool AddIfNotExists<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
      {
            if(dictionary.Count == 0) 
            {
                  dictionary.Add(key, value);
                  return false;
            }
            if (dictionary.ContainsKey(key))
            {
                  return true; // 存在重复的键
            }else
            {
                  dictionary.Add(key, value);
                  return false; // 添加成功，不存在重复的键
            }

            
      }
      private IEnumerator WaitParticleAddMorale(SoldierBehaviors soldier,ParticleSystem particleSystem)
      {
            // 等待粒子特效开始播放
            particleSystem.Play();
            while (particleSystem.isPlaying)
            {
                  yield return null;
            }

            // 执行你想要执行的操作
            soldier.morale.ModifyBaseMinMorale(soldier,+linkMinStrengthIncrease);
            soldier.morale.AddMorale(soldier, 0.06f, false);
            soldier.morale.EffectByMorale(soldier,ref soldier.strength);
            
      }
      private IEnumerator WaitParticleReduceMorale(SoldierBehaviors soldier,ParticleSystem particleSystem)
      {
            // 等待粒子特效开始播放
            particleSystem.Play();
            while (particleSystem.isPlaying)
            {
                  yield return null;
            }

            // 执行你想要执行的操作
            soldier.morale.ModifyBaseMinMorale(soldier,-linkMinStrengthIncrease);
            
      }
      void SetSkine()
      {
            if(!self)return;
            if(self.skinName!="")
            {
                  switch(self.morale.soldierType)
                  {
                        case MoraleTemplate.SoldierType.Red:
                             fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_red");
                        break;
                        case MoraleTemplate.SoldierType.Blue:
                              fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_blue");
                        break;
                        case MoraleTemplate.SoldierType.Green:
                              fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_green");
                        break;
                        case MoraleTemplate.SoldierType.Purple:
                              fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_purple");
                        break;
                  }
            }     
            
      }
      void UnitBaseCollected(Unit u)
      {
            
           forceBreakLink = true;

           if(North!=null)
           {
             SoldiersEndRelation(self,North);
           }
           if(East!=null)
           {
             SoldiersEndRelation(self,East);
           }
           if(South!=null)
           {
             SoldiersEndRelation(self,South);
           }
           if(West!=null)
           {
             SoldiersEndRelation(self,West);
           }
           self = null;

           this.enabled = false;
           
      }
      /// <summary>
    /// 为机制 刷新战场信息
    /// </summary>
    void ReflashMechanism()
    {
          mechanismInPut.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
          mechanismInPut.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
    /// <summary>
    /// 比较两个队列
    /// </summary>
    private bool ListEquals(List<SoldierBehaviors> list1, List<SoldierBehaviors> list2)
      {
            if (list1.Count != list2.Count)
            {
                  return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                  if (!list1[i].Equals(list2[i]))
                  {
                        return false;
                  }
            }

            return true;
      }
}
#if UNITY_EDITOR
[CustomEditor(typeof(FourDirectionsLink))]
public class FourDirectionsLinkEditor : Editor
{
    FourDirectionsLink myScript;
    FourDirectionsLinkEditor()
    {
        // 注册场景视图回调函数
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private  void OnSceneGUI(SceneView sceneView)
    {
        if (target == null)
        {
            return;
        }
        myScript = (FourDirectionsLink)target;
       
        // 绘制您的Handles内容
        DrawFourDirLinkRange(myScript.transform.position, myScript.range, myScript.rangeColor);
        // 刷新Scene视图
        sceneView.Repaint();
    }
    private  void OnSceneGUI()
    {
        if (target == null)
        {
            return;
        }
        myScript = (FourDirectionsLink)target;
        DrawFourDirLinkRange(myScript.transform.position, myScript.range, myScript.rangeColor);
    }
    public static void DrawFourDirLinkRange(Vector3 center, float radius, Color color, float duration = 0)
      {
            
            Handles.color = color;
            Handles.DrawWireCube(center, Vector3.one * radius * 2);
            
            
      }
}
#endif