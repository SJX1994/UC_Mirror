using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class BuoyBehavior : MonoBehaviour
{
    
    BuoyInfo buoyInfo;
    TetrisBlockSimple tetrisControled;
    TetrisBlockSimple tetrisControledTemp;
    LayerMask blockTargetMask;
    Vector3 originPos;
    void Start()
    {
        buoyInfo = transform.parent.GetComponent<BuoyInfo>();
        blockTargetMask = buoyInfo.blockTargetMask;
        GetComponent<SortingGroup>().sortingOrder = PlayerData.Dispaly.NotFlowOrder;
    }
    private void OnMouseDown()
    {
        originPos = transform.parent.localPosition;
        tetrisControled = buoyInfo.blockBuoyHandler.GetTetris();
        if(tetrisControled && tetrisControled.tetrisCheckMode == TetrisBlockSimple.TetrisCheckMode.Normal)
        {
            tetrisControled.Stop();
            tetrisControledTemp = Instantiate(tetrisControled, transform.parent);
            // 转成Buoy坐标系
            tetrisControledTemp.transform.localPosition = tetrisControled.transform.localPosition - transform.parent.localPosition;
            foreach(var tetri in tetrisControledTemp.childTetris)
            {
                tetri.transform.localScale = Vector3.one * 0.5f;
            }
            
        }
    }

    private void OnMouseDrag()
    {
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
        {
            if(hit.collider.transform.TryGetComponent(out BlockBuoyHandler block))
            {
                transform.parent.localPosition = new Vector3( block.posId.x, 0, block.posId.y);
            }
        }
    }
    private void OnMouseUp()
    {
        if(!tetrisControled)return;
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
        {
           if(tetrisControledTemp.BuoyValidDrop() && tetrisControled.OnBuoyDrop())
           {
                // 可以放置
                tetrisControled.Reset();
                tetrisControled.transform.parent = buoyInfo.transform.parent;
                tetrisControled.transform.localPosition = tetrisControledTemp.transform.localPosition + transform.parent.localPosition;
                tetrisControled.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
                tetrisControled.Active();
                DestroyImmediate(tetrisControledTemp.gameObject);
           }else
           {
                // 不能放置
                transform.parent.localPosition = originPos;
                tetrisControled.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
                tetrisControled.Move();
                DestroyImmediate(tetrisControledTemp.gameObject);
           }
        }else
        {
            // 不能放置
            transform.parent.localPosition = originPos;
            tetrisControled.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
            tetrisControled.Move();
            DestroyImmediate(tetrisControledTemp.gameObject);
        }

        originPos = Vector3.zero;
        tetrisControled = null;
        
    }

}
