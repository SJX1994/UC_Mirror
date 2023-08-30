using GameFrameWork;
using System.Diagnostics;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
public class SubHeroItem : BaseUI
{

    /// <summary>
    /// ������Ӣ����ҳ
    /// ���ڼ̳�BaseUIʵ�ֵĳ�����
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.SubHeroItem;
    }

    public Image HeroImage;
    public Button HeroBtn;
    public TextMeshProUGUI HeroName;
    public TextMeshProUGUI Probability;

    #region PrivateValue
    private object[] hero = null;
    #endregion
    public override void OnLoadData(params object[] param)
    {
        hero = param;
        ResManager.Hero heroTemp = (ResManager.Hero)hero[0];
        if(heroTemp != null)
        {
            if (heroTemp._unlock && heroTemp._beConfig == false)
            {
                HeroImage.sprite = heroTemp._image;
                HeroName.text = heroTemp._name;
                heroTemp._beConfig = true;
            }
            else
            {

            }
        }
    }
}
