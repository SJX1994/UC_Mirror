using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroUpgradePage : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.HeroUpgradePage;
    }

    #region UI控件
    public Button CloseBtn;

    public Image HeroImg;
    public Image ActiveSkillImg;
    public Image PassiveSkillImg;

    public TextMeshProUGUI HeroName;
    public TextMeshProUGUI ActiveSkillName;
    public TextMeshProUGUI ActiveSkillProfile;
    public TextMeshProUGUI ActiveSkillLevelText;
    public TextMeshProUGUI PassiveSkillName;
    public TextMeshProUGUI PassiveSkillProfile;
    public TextMeshProUGUI PassiveSkillLevelText;

    public GameObject HeroTag;
    #endregion

    public override void OnAwake()
    {
        //优先相机处理
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        canvas.planeDistance = 10;

        Animation openAni = this.transform.gameObject.GetComponent<Animation>();
        openAni.Play("A_HeroUpgradePageOpen");

        //点击监听
        BtnEvent.RigisterButtonClickEvent(CloseBtn.transform.gameObject, p => { OnCloseBtnClick(); });
        base.OnAwake();
    }

    public override void OnLoadData(params object[] param)
    {
        //获取Data
        ResManager.Hero heroTemp = (ResManager.Hero)param[0];
        //判断Data数据是否存在
        if(heroTemp != null)
        {
            //判断当前英雄是否已经被解锁
            if (heroTemp._unlock)//已解锁状态
            {
                HeroName.text = heroTemp._name;
                ActiveSkillImg.sprite = heroTemp._activeSkillImg;
                PassiveSkillImg.sprite = heroTemp._passiveSkillImg;
                ActiveSkillName.text = heroTemp._activeSkillName;
                ActiveSkillProfile.text = heroTemp._activeSkillProfile;
                PassiveSkillName.text = heroTemp._passiveSkillName;
                PassiveSkillProfile.text = heroTemp._passiveSkillProfile;
                //添加已解锁英雄标签
                for(int i = 0; i < heroTemp._heroTags.Length; i++)
                {
                    string[] tag = new string[1] { heroTemp._heroTags[i] };
                    UIManager.Instance.OpenSubUI(UIType.SubHeroTag, tag, HeroTag);
                }
            }
            else//未解锁状态
            {
                HeroImg.color = Color.black;
                ActiveSkillImg.sprite = heroTemp._unlockSkillImg;
                PassiveSkillImg.sprite = heroTemp._unlockSkillImg;
                HeroName.text = heroTemp._unlockName;
                ActiveSkillLevelText.text = null;
                PassiveSkillLevelText.text = null;
                ActiveSkillName.text = "???";
                ActiveSkillProfile.text = "???";
                PassiveSkillName.text = "???";
                PassiveSkillProfile.text = "???";
                //添加未解锁英雄标签
                for (int i = 0; i < heroTemp._unlockHeroTags.Length; i++)
                {
                    string[] tag = new string[1] { heroTemp._unlockHeroTags[i] };
                    UIManager.Instance.OpenSubUI(UIType.SubHeroTag, tag, HeroTag);
                }
            }
        }

        base.OnLoadData(param);
    }

    #region 点击事件

    //关闭英雄升级页面
    void OnCloseBtnClick()
    {
        UIManager.Instance.CloseUI(UIType.HeroUpgradePage);
    }

    #endregion
}
