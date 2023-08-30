using GameFrameWork;
using UnityEngine.UI;

public class SubHeroTag : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.SubHeroTag;
    }

    #region �������
    public Text HeroTagText;
    #endregion

    #region ���ݼ���
    public override void OnLoadData(params object[] param)
    {
        HeroTagText.text = param[0] as string;
        base.OnLoadData(param);
    }
    #endregion
}
