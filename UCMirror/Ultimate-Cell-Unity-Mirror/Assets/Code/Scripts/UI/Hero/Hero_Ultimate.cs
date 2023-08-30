using GameFrameWork;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 英雄_奥特米尔的UI事件
/// </summary>
public class Hero_Ultimate : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.Hero_Ultimate;
    }

    public Button btn;
    public Spine.Unity.SkeletonGraphic spineGraphic;

    public override void OnAwake()
    {   
        //BtnEvent.RigisterButtonClickEvent(btn.transform.gameObject, p => { ClikHero(); });
        //BtnEvent.RigisterButtonEnterEvent(btn.transform.gameObject, p => { EnterHero(); });
        //BtnEvent.RigisterButtonExitEvent(btn.transform.gameObject, p => { ExitHero(); });
        //base.OnAwake();
    }

    private void Update()
    {
        //Vector3 targetPostition = new Vector3(this.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        //this.transform.LookAt(targetPostition);
    }

    void ClikHero()
    {
        spineGraphic.AnimationState.SetAnimation(0, "beenAttack", false);
    }

    void EnterHero()
    {
        spineGraphic.AnimationState.SetAnimation(0, "move", true);
    }

    void ExitHero()
    {
        spineGraphic.AnimationState.SetAnimation(0, "idle", true);
    }
}
