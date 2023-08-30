using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineUIcontrolExample : MonoBehaviour
{
    public Spine.Unity.SkeletonGraphic spineGraphic;
    public void UIIndexExample(int index)
    {
        switch(index)
        {
            case 0:
                Debug.Log("UI::Idle");
                spineGraphic.AnimationState.SetAnimation(0, "idle", true);
            break;
            case 1:
                Debug.Log("UI::Move");
                spineGraphic.AnimationState.SetAnimation(0, "move", true);
            break;
            case 2:
                Debug.Log("UI::Attack");
                spineGraphic.AnimationState.SetAnimation(0, "attack", true);
            break;
            case 3:
                Debug.Log("UI::Die");
                spineGraphic.AnimationState.SetAnimation(0, "die", false);
            break;
        }
    }
}
