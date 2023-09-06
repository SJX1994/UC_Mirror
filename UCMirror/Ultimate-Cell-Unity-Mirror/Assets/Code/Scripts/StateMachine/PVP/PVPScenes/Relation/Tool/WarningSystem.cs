using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class WarningSystem : Singleton<WarningSystem>
{
    public WarningType changeWarningTypes = WarningType.None;
    public UnityAction<WarningType> warningChangeAction;
    public string inText1,inText2,inText3,inText4 = "";
    Text warningText;
    Image warningImage;
    Color warningImageColor;
    Vector3 warningTextScale;
    public bool reday = true;
    bool play2EndFadeDuration = false;
    public bool Play2EndFadeDuration
    {
          get { return play2EndFadeDuration; }
          set
          {
                if (value == play2EndFadeDuration)return;
                {
                    play2EndFadeDuration = value;
                    if(play2EndFadeDuration)
                    {
                        Invoke(nameof(play2EndFadeDurationFinsh),1.5f);
                    }
                }
          }
    }
    public enum WarningType
    {
        None,
        Placeholder, // 占位符
        FourDirectionsLink, // 提示 参与方位联结 的士兵数
        WeakAssociation, // 提示 参与联结的 所有兵种 以及士兵数
        ChainTransfer, // 提示 参与传递的 兵种 和 数量
        Morale, // 提示 平均士气
        Reflash, // 提示 刷新
        BuildingMarshalling,// 建筑 操作：编组
        BuildingUnmarshal,// 建筑 操作：解组
        BuildingKeep,// 建筑 操作：保持
        CancelOperation,// 取消 操作
        BuoyInfo,// 浮标 控制
        Custom_Emergency, /// 紧急自定义显示
    }
    WarningType warningTypes;
    public WarningType WarningTypes
    {
          get { return warningTypes; }
          set
          {
                if (value != warningTypes)
                {
                    warningTypes = value;
                    
                    OnWarningChanged();
                }
          }
    }
    void Start()
    {
        warningImage = transform.GetComponent<Image>();
        warningText = transform.Find("WarningText").GetComponent<Text>();
        warningImageColor = warningImage.color;
        warningTextScale = warningText.transform.localScale;
        changeWarningTypes = WarningType.Placeholder;
        reday = true;
        play2EndFadeDuration = false;
    }
    void Update()
    {
        WarningTypes = changeWarningTypes;
    }
    void OnWarningChanged()
    {
        
        
        float fadeDuration = 1.5f;
        if (warningChangeAction != null)
        {
            warningChangeAction(warningTypes);
        }
        
        switch(warningTypes)
        {
            case WarningType.None:
            break;
            case WarningType.Placeholder:
                if(reday==false) return;
                string[] temp0 = {
                    "策无遗策！",
                    "足智多谋！",
                    "简拔无遗！",
                    "才高八斗！",
                    "谋定后动！",
                    "智勇双全!",
                };
                TextDispaly(temp0, fadeDuration);
                
            break;
            case WarningType.FourDirectionsLink:
                if(reday==false) return;
                string[] temp1 = {
                    "<b>参与联结的士兵</b> 数量有" + inText1 + "/" + inText2 + "名, 齐心协力!",
                    "<b>参与联结的士兵</b> 数量有" + inText1 + "/" + inText2 + "名, 如臂使指!",
                    "<b>参与联结的士兵</b> 数量有" + inText1 + "/" + inText2 + "名, 众志成城!",
                    "<b>参与联结的士兵</b> 数量有" + inText1 + "/" + inText2 + "名, 和衷共济!",
                };
                TextDispaly(temp1, fadeDuration);
            break;
            case WarningType.WeakAssociation:
                string[] temp2 = {
                    "<b><color=red>红色</color></b>" + inText1 + "名;" + "<b><color=green>绿色</color></b>" + inText2 + "名;" + "<b><color=blue>蓝色</color></b>" + inText3 + "名;" + "<b><color=purple>紫色</color></b>" + inText4 + "名， 逆水行舟!",
                    "<b><color=red>红色</color></b>" + inText1 + "名;" + "<b><color=green>绿色</color></b>" + inText2 + "名;" + "<b><color=blue>蓝色</color></b>" + inText3 + "名;" + "<b><color=purple>紫色</color></b>" + inText4 + "名，指日可待!",
                    "<b><color=red>红色</color></b>" + inText1 + "名;" + "<b><color=green>绿色</color></b>" + inText2 + "名;" + "<b><color=blue>蓝色</color></b>" + inText3 + "名;" + "<b><color=purple>紫色</color></b>" + inText4 + "名， 一日千里!",
                    "<b><color=red>红色</color></b>" + inText1 + "名;" + "<b><color=green>绿色</color></b>" + inText2 + "名;" + "<b><color=blue>蓝色</color></b>" + inText3 + "名;" + "<b><color=purple>紫色</color></b>" + inText4 + "名， 焕然一新!",
                };
                TextDispaly(temp2, fadeDuration);
            break;
            case WarningType.ChainTransfer:
                string[] temp3 = {
                    "<b>参与传递</b>的 " + inText2 + " 士兵为 " + inText1 + " 名, 可以造成" + inText3 + "伤害,势如破竹!",
                    "<b>参与传递</b>的 " + inText2 + " 士兵为 " + inText1 + " 名, 可以造成" + inText3 + "伤害,势不可挡!",
                    "<b>参与传递</b>的 " + inText2 + " 士兵为 " + inText1 + " 名, 可以造成" + inText3 + "伤害,气势磅礴!",
                };
                TextDispaly(temp3, fadeDuration);
            break;
            case WarningType.Morale:
                if(reday==false) return;
                string[] temp4 = {
                    "士气高涨",
                    "斗志昂扬",
                    "奋发图强",
                    "精神抖擞",
                    "振奋人心",
                };
                TextDispaly(temp4, fadeDuration);
            break;
            case WarningType.Reflash:
                string[] temp5 = {
                    "改弦更张",
                    "弃旧图新",
                    "转变思路",
                    "刷新观念",
                    "开拓新路",
                };
                TextDispaly(temp5, fadeDuration);
            break;
            case WarningType.CancelOperation:
                string[] temp6 = {
                    "迷途知返",
                    "步步为营",
                    "进退两难",
                    "前路茫然",
                };
                TextDispaly(temp6, fadeDuration);
            break;
            case WarningType.BuildingMarshalling:
                string[] temp7 = {
                    "纪律森严",
                    "铁规硬矩",
                    "严守纪律",
                    "严明纪纲",
                };
                TextDispaly(temp7, fadeDuration);
            break;
            case WarningType.BuildingUnmarshal:
                string[] temp8 = {
                    "随机应变",
                    "能屈能伸",
                    "依势变通",
                    "屈能伸缩",
                    "灵活应对",
                };
                TextDispaly(temp8, fadeDuration);
            break;
            case WarningType.BuildingKeep:
                string[] temp9 = {
                    "巍然不动",
                    "坚定不移",
                    "坚壁清野",
                    "屹立不倒",
                    "铁心铜壁",
                };
                TextDispaly(temp9, fadeDuration);
            break;
            case WarningType.Custom_Emergency:
                string[] temp10 = {
                    inText1 + "果实能力生效",
                    inText1 + "果实能力生效",
                };
                TextDispaly(temp10, fadeDuration);
                Play2EndFadeDuration = true;
            break;
            case WarningType.BuoyInfo:
                string[] temp11 = {
                    "<b>控制</b> 砖块数量有" + inText1 + "格, 齐心协力!",
                    "<b>控制</b> 砖块数量有" + inText1 + "格, 如臂使指!",
                    "<b>控制</b> 砖块数量有" + inText1 + "格, 众志成城!",
                    "<b>控制</b> 砖块数量有" + inText1 + "格, 和衷共济!",
                };
                TextDispaly(temp11, fadeDuration);
            break;
            
        }
    }
    void TextDispaly(string[] temp = null, float fadeDuration = 1.5f)
    {
        // if(play2EndFadeDuration)return;
        reday = false;
        warningText.text = temp[Random.Range(0, temp.Length)];
        warningImage.DOFade(warningImageColor.a+0.1f, fadeDuration).onComplete = () => {
            warningImage.DOFade(0f, fadeDuration);
        };
        warningText.transform.DOScale(warningTextScale.x+0.1f,fadeDuration).onComplete = () => {
            warningText.transform.DOScale(0, fadeDuration).SetEase(Ease.InBack).onComplete = () => {
                changeWarningTypes = WarningType.None;
                inText1 = "";
                inText2 = "";
                inText3 = "";
                inText4 = "";
                reday = true;
            };
        };
    }
    void play2EndFadeDurationFinsh()
    {
        play2EndFadeDuration = false;
    }
}
