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

    #region UI�ؼ�
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
        //�����������
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        canvas.planeDistance = 10;

        Animation openAni = this.transform.gameObject.GetComponent<Animation>();
        openAni.Play("A_HeroUpgradePageOpen");

        //�������
        BtnEvent.RigisterButtonClickEvent(CloseBtn.transform.gameObject, p => { OnCloseBtnClick(); });
        base.OnAwake();
    }

    public override void OnLoadData(params object[] param)
    {
        //��ȡData
        ResManager.Hero heroTemp = (ResManager.Hero)param[0];
        //�ж�Data�����Ƿ����
        if(heroTemp != null)
        {
            //�жϵ�ǰӢ���Ƿ��Ѿ�������
            if (heroTemp._unlock)//�ѽ���״̬
            {
                HeroName.text = heroTemp._name;
                ActiveSkillImg.sprite = heroTemp._activeSkillImg;
                PassiveSkillImg.sprite = heroTemp._passiveSkillImg;
                ActiveSkillName.text = heroTemp._activeSkillName;
                ActiveSkillProfile.text = heroTemp._activeSkillProfile;
                PassiveSkillName.text = heroTemp._passiveSkillName;
                PassiveSkillProfile.text = heroTemp._passiveSkillProfile;
                //����ѽ���Ӣ�۱�ǩ
                for(int i = 0; i < heroTemp._heroTags.Length; i++)
                {
                    string[] tag = new string[1] { heroTemp._heroTags[i] };
                    UIManager.Instance.OpenSubUI(UIType.SubHeroTag, tag, HeroTag);
                }
            }
            else//δ����״̬
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
                //���δ����Ӣ�۱�ǩ
                for (int i = 0; i < heroTemp._unlockHeroTags.Length; i++)
                {
                    string[] tag = new string[1] { heroTemp._unlockHeroTags[i] };
                    UIManager.Instance.OpenSubUI(UIType.SubHeroTag, tag, HeroTag);
                }
            }
        }

        base.OnLoadData(param);
    }

    #region ����¼�

    //�ر�Ӣ������ҳ��
    void OnCloseBtnClick()
    {
        UIManager.Instance.CloseUI(UIType.HeroUpgradePage);
    }

    #endregion
}
