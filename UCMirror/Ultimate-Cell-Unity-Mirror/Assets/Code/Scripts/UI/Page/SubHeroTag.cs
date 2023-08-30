using GameFrameWork;
using UnityEngine.UI;

public class SubHeroTag : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.SubHeroTag;
    }

    #region 交互组件
    public Text HeroTagText;
    #endregion

    #region 数据加载
    public override void OnLoadData(params object[] param)
    {
        HeroTagText.text = param[0] as string;
        base.OnLoadData(param);
    }
    #endregion
}
