using System;
using UnityEngine;

public class SpawnRemoveController : MonoBehaviour
{
    [Header("方块横坐标")]
    public int posx;

    [Header("方块纵坐标")]
    public int posy;

    [Header("执行操作")]
    public bool RemoveSwapn;

    private void Update()
    {
        if (RemoveSwapn) 
        {
            RemoveSwapn = false;

            var removeInfo = TetrisBlock.DeleteAlone(posx, posy);

            if (!removeInfo) 
            {
                Debug.Log("目标位置不存在砖块");
            }
        }
    }

}
