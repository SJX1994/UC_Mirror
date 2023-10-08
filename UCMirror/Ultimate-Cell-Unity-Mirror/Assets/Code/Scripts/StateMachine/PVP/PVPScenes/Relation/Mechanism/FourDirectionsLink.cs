
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
using UC_PlayerData;
public class FourDirectionsLink: MonoBehaviour, ISoldierRelation
{
      bool active = true;
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
      private SoldierBehaviors north,east,south,west;
      public SoldierBehaviors North
      {
            get
            {
                  return north;
            }
            set
            {
                  if(value!=null)
                  {
                        if(value.unitBase.unitTemplate.player != Self.unitBase.unitTemplate.player)
                        { value = null; }
                  }
                  if(north == value)return;
                  north = value;
                  Level_DataEffect();
            }
      }
      public SoldierBehaviors East
      {
            get
            {
                  return east;
            }
            set
            {
                  if(value!=null)
                  {
                        if(value.unitBase.unitTemplate.player != Self.unitBase.unitTemplate.player)
                        { value = null; }
                  }
                  if(east == value)return;
                  east = value;
                  Level_DataEffect();
            }
      }
      public SoldierBehaviors South
      {
            get
            {
                  return south;
            }
            set
            {
                  if(value!=null)
                  {
                        if(value.unitBase.unitTemplate.player != Self.unitBase.unitTemplate.player)
                        { value = null; }
                  }
                  if(south == value)return;
                  south = value;
                  Level_DataEffect();
            }
      }
      public SoldierBehaviors West
      {
            get
            {
                  return west;
            }
            set
            {
                  if(value!=null)
                  {
                        if(value.unitBase.unitTemplate.player != Self.unitBase.unitTemplate.player)
                        { value = null; }
                  }
                  if(west == value)return;
                  west = value;
                  Level_DataEffect();
            }
      }

      private BlockDisplay northBlock,eastBlock,southBlock,westBlock;
      public BlockDisplay NorthBlock
      {
            get
            {
                  Vector2 posId = Self.UnitSimple.PosId;
                  Vector2 checkerPosId = new Vector2(posId.x,posId.y+1);
                  if( checkerPosId.x >= Dispaly.MaxWidth || checkerPosId.x < 0 || checkerPosId.y >= Dispaly.MaxHeight || checkerPosId.y < 0)return null;
                  if(!northBlock || northBlock.posId != checkerPosId )
                  {
                        northBlock = Self.TetriMechanism.FindBlockWithId(checkerPosId);
                  }
                  return northBlock;
            }
      }
      public BlockDisplay EastBlock
      {
            get
            {
                  Vector2 posId = Self.UnitSimple.PosId;
                  Vector2 checkerPosId = new Vector2(posId.x+1,posId.y);
                  if( checkerPosId.x >= Dispaly.MaxWidth || checkerPosId.x < 0 || checkerPosId.y >= Dispaly.MaxHeight || checkerPosId.y < 0)return null;
                  if(!eastBlock || eastBlock.posId != checkerPosId )
                  {
                        eastBlock = Self.TetriMechanism.FindBlockWithId(checkerPosId);
                  }
                  return eastBlock;
            }
      }
      public BlockDisplay SouthBlock
      {
            get
            {
                  Vector2 posId = Self.UnitSimple.PosId;
                  Vector2 checkerPosId = new Vector2(posId.x,posId.y-1);
                  if( checkerPosId.x >= Dispaly.MaxWidth || checkerPosId.x < 0 || checkerPosId.y >= Dispaly.MaxHeight || checkerPosId.y < 0)return null;
                  if(!southBlock || southBlock.posId != checkerPosId )
                  {
                        southBlock = Self.TetriMechanism.FindBlockWithId(checkerPosId);
                  }
                  return southBlock;
            }
      }
      public BlockDisplay WestBlock
      {
            get
            {
                  Vector2 posId = Self.UnitSimple.PosId;
                  Vector2 checkerPosId = new Vector2(posId.x-1,posId.y);
                  if( checkerPosId.x >= Dispaly.MaxWidth || checkerPosId.x < 0 || checkerPosId.y >= Dispaly.MaxHeight || checkerPosId.y < 0)return null;
                  if(!westBlock || westBlock.posId != checkerPosId )
                  {
                        westBlock = Self.TetriMechanism.FindBlockWithId(checkerPosId);
                  }
                  return westBlock;
            }
      }
   
      public enum Direction
      {
            North,
            East,
            South,
            West
      }
      [HideInInspector]
      public SoldierBehaviors self;
      public SoldierBehaviors Self
      {
            get
            {
                  if(!self)self = GetComponent<SoldierBehaviors>();
                  return self;
            }
      }
      [HideInInspector]
      public float range = 3f;
      [HideInInspector]
      public Color rangeColor = Color.green;
      float linkMinStrengthIncrease = 0.2f; // 联结最小强度增加
      /// <summary>
      /// 表现
      /// </summary>
      public Material fourDirectionsLinkLinkMat;
      public List<Material> fourDirectionsLinkLinkMats;
      [HideInInspector]
      private SpriteRenderer sprit_North,sprit_East,sprit_South,sprit_West;
      public SpriteRenderer Sprit_North
      {
            get
            {
                  if(!sprit_North)sprit_North = Self.unitBase.SkeletonRenderer.transform.Find("Level_N").GetComponent<SpriteRenderer>();
                  return sprit_North;
            }
      }
      public SpriteRenderer Sprit_East
      {
            get
            {
                  if(!sprit_East)sprit_East = Self.unitBase.SkeletonRenderer.transform.Find("Level_E").GetComponent<SpriteRenderer>();
                  return sprit_East;
            }
      }
      public SpriteRenderer Sprit_South
      {
            get
            {
                  if(!sprit_South)sprit_South = Self.unitBase.SkeletonRenderer.transform.Find("Level_S").GetComponent<SpriteRenderer>();
                  return sprit_South;
            }
      }
      public SpriteRenderer Sprit_West
      {
            get
            {
                  if(!sprit_West)sprit_West = Self.unitBase.SkeletonRenderer.transform.Find("Level_W").GetComponent<SpriteRenderer>();
                  return sprit_West;
            }
      }
      private bool died = false;

      void Start()
      {
            active = false;
            died = false;
            NeedRender = true;
            Self.TetriMechanism.TetrisBlockSimple.OnUpdatDisplay += ()=>{ if(!active)active = true;};
            Self.unitBase.OnDie += (Unit unit)=>{this.StopAllCoroutines();died = true;};
            InitLevel();
            linkMinStrengthIncrease = 0.2f;
            // 联结表现          
            Invoke(nameof(SetSkine), 0.1f);
            
      }
      public void LateUpdate()
      {
           if(active)Active();
      }
      public void Active()
      {
            LinkChecker();
            UpdateLevel();
      }
      void LinkChecker()
      {   
            if(died || !Self || !Self.UnitSimple )return;
            // 获取单位的坐标
            Vector2 posId = Self.UnitSimple.PosId;
            // 方位检测
            var NorthTetri = NorthBlock ? NorthBlock.BlockBuoyHandler.tetriBuoySimple : null;
            var EastTetri = EastBlock ? EastBlock.BlockBuoyHandler.tetriBuoySimple : null;
            var SouthTetri = SouthBlock ? SouthBlock.BlockBuoyHandler.tetriBuoySimple : null;
            var WestTetri = WestBlock ? WestBlock.BlockBuoyHandler.tetriBuoySimple : null;
            if (NorthTetri)
            {
                  if(North != NorthTetri.TetriUnitSimple.haveUnit.Soldier)
                  {
                        North = NorthTetri.TetriUnitSimple.haveUnit.Soldier;
                        SoldiersStartRelation(Self,North);
                  }else
                  {
                        SoldiersUpdateRelation(Self,North);
                  }
                  
            }else
            {
                  SoldiersEndRelation(Self,North);
                  North = null;
            }
            if (EastTetri)
            {
                  if(East != EastTetri.TetriUnitSimple.haveUnit.Soldier)
                  {
                        East = EastTetri.TetriUnitSimple.haveUnit.Soldier;
                        SoldiersStartRelation(Self,East);
                  }else
                  {
                        SoldiersUpdateRelation(Self,East);
                  }
            }else
            {
                  SoldiersEndRelation(Self,East);
                  East = null;
            }
            if (WestTetri)
            {
                  if(West != WestTetri.TetriUnitSimple.haveUnit.Soldier)
                  {
                        West = WestTetri.TetriUnitSimple.haveUnit.Soldier;
                        SoldiersStartRelation(Self,West);
                  }else
                  {
                        SoldiersUpdateRelation(Self,East);
                  }
            }else
            {
                  SoldiersEndRelation(Self,West);
                  West = null;
            }
            if (SouthTetri)
            {
                  if(South != SouthTetri.TetriUnitSimple.haveUnit.Soldier)
                  {
                        South = SouthTetri.TetriUnitSimple.haveUnit.Soldier;
                        SoldiersStartRelation(Self,South);
                  }else
                  {
                        SoldiersUpdateRelation(Self,South);
                  }
            }else
            {
                  SoldiersEndRelation(Self,South);
                  South = null;
            }
      }
      void InitLevel()
      {
            Sprit_North.enabled = false;
            Sprit_East.enabled = false;
            Sprit_South.enabled = false;
            Sprit_West.enabled = false;
      }
      void UpdateLevel()
      {
            Level_Display();
      }
      void Level_Display()
      {
            if(North)
            {
                  if(Self.unitBase.unitTemplate.player != North.unitBase.unitTemplate.player)North = null;
                  Sprit_North.enabled = North;
            }else
            {
                  StopCoroutine(WaitParticleAddMorale(Self,Self.PositiveEffect));
                  StopCoroutine(WaitParticleReduceMorale(Self,Self.NegativeEffect));
                  Sprit_North.enabled = false;
            }
            if(East)
            {
                  if(Self.unitBase.unitTemplate.player != East.unitBase.unitTemplate.player)East = null;
                  Sprit_East.enabled = East;
            }else
            {
                  StopCoroutine(WaitParticleAddMorale(Self,Self.PositiveEffect));
                  StopCoroutine(WaitParticleReduceMorale(Self,Self.NegativeEffect));
                  Sprit_East.enabled = false;
            }
            if(South)
            {
                  if(Self.unitBase.unitTemplate.player != South.unitBase.unitTemplate.player)South = null;
                  Sprit_South.enabled = South;
            }else
            {
                  StopCoroutine(WaitParticleAddMorale(Self,Self.PositiveEffect));
                  StopCoroutine(WaitParticleReduceMorale(Self,Self.NegativeEffect));
                  Sprit_South.enabled = false;
            }
            if(West)
            {
                  if(Self.unitBase.unitTemplate.player != West.unitBase.unitTemplate.player)West = null;
                  Sprit_West.enabled = West;
            }else
            {
                  StopCoroutine(WaitParticleAddMorale(Self,Self.PositiveEffect));
                  StopCoroutine(WaitParticleReduceMorale(Self,Self.NegativeEffect));
                  Sprit_West.enabled = false;
            }
            
      }
      void Level_DataEffect()
      {
            if(!Self)return;
            if(North || East || South || West)
            {
                  StartCoroutine(WaitParticleAddMorale(Self,Self.PositiveEffect));
            }else if( !North || !East || !South || !West)
            {
                  StartCoroutine(WaitParticleReduceMorale(Self,Self.NegativeEffect));
            }
            // Debug.Log("Level_DataEffect++" + Self.morale.morale);
      }

      /// <summary>
      /// 实现联结 关系 开始时的刹那间的表现
      /// 基于心理学的 互助和合作 增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersStartRelation(SoldierBehaviors from,SoldierBehaviors to)
      {
            if(!to || !from)return;
            if(!to.unitBase)return;
            if(to.unitBase.unitTemplate.player != from.unitBase.unitTemplate.player)return;
            // 特效表现
            ParticleSystem ps = to.PositiveEffect;
            if(!ps)return;
      }
      /// <summary>
      /// 实现联结 关系 持续的表现
      /// 给玩家体验到 互助合作的重要性，从而增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersUpdateRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(!to || !from)return;
            Vector3 startPosition = from.transform.position;
            Vector3 endPosition = to.transform.position;
      }
      /// <summary>
      /// 实现联结 关系 结束的表现
      /// 给玩家体验到 互助合作失去后的乏力，从而增强领悟感
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      public void SoldiersEndRelation(SoldierBehaviors from, SoldierBehaviors to)
      {
            if(!to)return;
            if(!Self)return;
            // 特效表现
            ParticleSystem ps = to.NegativeEffect;
            if(!ps)return;
            StartCoroutine(WaitParticleReduceMorale(from,from.NegativeEffect));
            StartCoroutine(WaitParticleReduceMorale(to,to.NegativeEffect));
      }
      private IEnumerator WaitParticleAddMorale(SoldierBehaviors soldier,ParticleSystem particleSystem)
      {
            if(!soldier || !particleSystem)yield return null;
            // 等待粒子特效开始播放
            particleSystem.Play();
            // while (particleSystem.isPlaying && !died)
            // {
            //       if(died)break;
            //       yield return null;
            // }

            // 执行你想要执行的操作
            // soldier.morale.ModifyBaseMinMorale(soldier,+linkMinStrengthIncrease);
            soldier.morale.AddMorale(soldier, 1.06f, false);
            soldier.morale.EffectByMorale(soldier,ref soldier.strength);
            
      }
      private IEnumerator WaitParticleReduceMorale(SoldierBehaviors soldier,ParticleSystem particleSystem)
      {
            if(!soldier || !particleSystem)yield return null;
            // 等待粒子特效开始播放
            particleSystem.Play();
            // while ( particleSystem.isPlaying && !died)
            // {
            //       if(died)break;
            //       yield return null;
            // }

            // 执行你想要执行的操作
            // soldier.morale.ModifyBaseMinMorale(soldier,-linkMinStrengthIncrease);
            soldier.morale.ReduceMorale(soldier, 1.06f, false);
            soldier.morale.EffectByMorale(soldier,ref soldier.strength);
      }
      void SetSkine()
      {
            if(!Self)return;
            if(Self.skinName!="")
            {
                  switch(Self.morale.soldierType)
                  {
                        case MoraleTemplate.SoldierType.Red:
                             // fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_red");
                        break;
                        case MoraleTemplate.SoldierType.Blue:
                              // fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_blue");
                        break;
                        case MoraleTemplate.SoldierType.Green:
                              // fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_green");
                        break;
                        case MoraleTemplate.SoldierType.Purple:
                              // fourDirectionsLinkLinkMat = fourDirectionsLinkLinkMats.Find(x=>x.name == "FourDirLinkLine_purple");
                        break;
                  }
            }     
            if(Self.unitBase.unitTemplate.player == Player.Player1)
            {
                  Sprit_North.color = Dispaly.Player1Color + Color.white*0.3f;
                  Sprit_East.color = Dispaly.Player1Color+ Color.white*0.3f;
                  Sprit_South.color = Dispaly.Player1Color+ Color.white*0.3f;
                  Sprit_West.color = Dispaly.Player1Color+ Color.white*0.3f;
            }else if(Self.unitBase.unitTemplate.player == Player.Player2)
            {
                  Sprit_North.color = Dispaly.Player2Color+ Color.white*0.3f;
                  Sprit_East.color = Dispaly.Player2Color+ Color.white*0.3f;
                  Sprit_South.color = Dispaly.Player2Color+ Color.white*0.3f;
                  Sprit_West.color = Dispaly.Player2Color+ Color.white*0.3f;
            }
      }
      
      /// <summary>
    /// 为机制 刷新战场信息
    /// </summary>
    void ReflashMechanism()
    {
          mechanismInPut.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
          mechanismInPut.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
}