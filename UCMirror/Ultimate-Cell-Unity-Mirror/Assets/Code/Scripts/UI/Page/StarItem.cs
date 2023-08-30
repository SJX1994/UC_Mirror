using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarItem : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.StarItem;
    }

    #region ½»»¥¿Ø¼þ

    public Image Obtained;

    #endregion

    public override void OnStart()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play("A_StarAni");
    }
}
