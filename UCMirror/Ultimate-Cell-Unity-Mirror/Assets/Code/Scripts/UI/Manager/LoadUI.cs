using GameFrameWork;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI启动器
/// </summary>

public class LoadUI : MonoBehaviour
{
    //添加一个Start Action / Ready Action / Error Action
    //给郑乃久订阅

    private void Start()
    {
        
        UIManager.Instance.OpenUI(UIType.VictoryPage, null);
    }

}
