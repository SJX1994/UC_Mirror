using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetails : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.None;
    }

    private void Awake()
    {
        Animation _uiAni = this.gameObject.GetComponent<Animation>();
        _uiAni.Play("A_LevelDetailsOpen");
    }

    public override void OnRelease()
    {
        Debug.Log("细节界面关闭啦");
        base.OnRelease();
    }
}
