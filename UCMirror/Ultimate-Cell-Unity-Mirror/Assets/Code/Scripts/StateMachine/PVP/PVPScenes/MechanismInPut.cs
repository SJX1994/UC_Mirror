using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using UC_PlayerData;
public class MechanismInPut : Singleton<MechanismInPut>
{
      public ModeTest modeTest;
      public UnityAction<ModeTest> modeChangeAction;
      public UnityAction<List<Soldier>> allSoldiers;
      public List<Soldier> soldiers = new();
      public Text text;
      // 通讯
      public WarningSystem warningSystem;
      public enum ModeTest
      {
            Morale, /// 士气
            FourDirectionsLinks, /// 四向链接
            WeakAssociation,/// 弱势关联
            ChainTransfer,/// 链式传递
            ChainTransferAndFourDirectionsLinks,/// 链式传递和四向链接
            Reflash, /// 刷新
           
      }
      
      private ModeTest modeVariable;
      public ModeTest ModeVariable
      {
            get { return modeVariable; }
            set
            {
                  if (value != modeVariable)
                  {
                  modeVariable = value;
                  if(RunModeData.CurrentRunMode != RunMode.Local)return;
                  OnModeVariableChanged();
                  }
            }
      }
      private void OnModeVariableChanged()
      {
            // 获取所有士兵
            soldiers = new List<Soldier>(FindObjectsOfType<Soldier>());
            List<Soldier> tempS = new(soldiers);
            
            foreach(Soldier s in tempS)
            {
                  if(!s)continue;
                  if(s.chainTransfer.collected == true || s.fourDirectionsLinks.forceBreakLink == true)
                  {
                        soldiers.Remove(s);
                  }
                  if(s.unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)
                  {
                        soldiers.Remove(s);
                  }
            }

            allSoldiers?.Invoke(soldiers);
            // 模式改变事件发送
            modeChangeAction?.Invoke(modeTest);
            // 提示系统
            switch (modeTest)
            {
                  case ModeTest.Morale:
                        warningSystem.changeWarningTypes = WarningSystem.WarningType.Morale;
                  break;
                  case ModeTest.ChainTransfer:
                        
                        int all0 = tempS.Count;
                        int links = 0;
                        foreach(Soldier s in tempS)
                        {
                              if(s.fourDirectionsLinks.North!=null || s.fourDirectionsLinks.East!=null || s.fourDirectionsLinks.West!=null || s.fourDirectionsLinks.South!=null)
                              {
                                    links++;
                              }
                        }
                        warningSystem.inText1 = links.ToString();
                        warningSystem.inText2 = all0.ToString();
                        warningSystem.changeWarningTypes = WarningSystem.WarningType.FourDirectionsLink;
                  break;
                  case ModeTest.WeakAssociation:
                        int red = 0;
                        int blue = 0;
                        int green = 0;
                        int purple = 0;
                        foreach(Soldier s in tempS)
                        {
                              switch(s.morale.soldierType)
                              {
                                    case MoraleTemplate.SoldierType.Red:
                                          red++;
                                    break;
                                    case MoraleTemplate.SoldierType.Blue:
                                          blue++;
                                    break;
                                    case MoraleTemplate.SoldierType.Green:
                                          green++;
                                    break;
                                    case MoraleTemplate.SoldierType.Purple:
                                          purple++;
                                    break;
                              }
                        }
                        warningSystem.inText1 = red.ToString();
                        warningSystem.inText2 = green.ToString();
                        warningSystem.inText3 = blue.ToString();
                        warningSystem.inText4 = purple.ToString();
                        warningSystem.changeWarningTypes = WarningSystem.WarningType.WeakAssociation;
                  break;
            }
      }
      void Start()
      {
            Invoke(nameof(StartInvoke), 0.1f);
      }
      void StartInvoke()
      {
            warningSystem = FindObjectOfType<WarningSystem>();
      }
      
      void Update()
      {
      
           ModeVariable = modeTest;
           if(text == null)return;
           switch(modeTest)
           {
                  case ModeTest.Morale:
                        text.text = "士气增强";
                  break;
                  case ModeTest.FourDirectionsLinks:
                        text.text = "四向链接";
                  break;
                  case ModeTest.WeakAssociation:
                        text.text = "弱势关联";
                  break;
                  case ModeTest.ChainTransfer:
                        text.text = "链式传递";
                  break;
           }
      }

}      