using GameFrameWork;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI������
/// </summary>

public class LoadUI : MonoBehaviour
{
    //���һ��Start Action / Ready Action / Error Action
    //��֣�˾ö���

    private void Start()
    {
        
        UIManager.Instance.OpenUI(UIType.VictoryPage, null);
    }

}
