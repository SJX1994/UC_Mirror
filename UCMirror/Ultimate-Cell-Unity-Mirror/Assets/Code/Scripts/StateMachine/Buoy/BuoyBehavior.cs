using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class BuoyBehavior : MonoBehaviour
{
    
    BuoyInfo buoyInfo;
    TetrisBuoySimple tsBuoyControled;
    List<TetrisBuoySimple> tsBuoyControleds = new();
    TetrisBuoySimple tetrisBuoySimpleTemp;
    List<TetrisBuoySimple> tetrisBuoySimpleTemps = new();
    List<TetriBuoySimple> checkSelfTetris = new(); // 多砖块自碰撞检测
    LayerMask blockTargetMask;
    Vector3 originPos;
    BuoyTurnHandle.TurnHandleControlState state = BuoyTurnHandle.TurnHandleControlState.None;
    void Start()
    {
        buoyInfo = transform.parent.GetComponent<BuoyInfo>();
        blockTargetMask = buoyInfo.blockTargetMask;
        GetComponent<SortingGroup>().sortingOrder = PlayerData.Dispaly.NotFlowOrder;
    }
    private void OnMouseDown()
    {
        originPos = transform.parent.localPosition;
        if(!buoyInfo.blockBuoyHandler)return;
        tsBuoyControled = buoyInfo.blockBuoyHandler.GetTetrisBuoy();
        tsBuoyControleds = buoyInfo.buoyTurnHandle.GetControledTetris();
        state = buoyInfo.buoyTurnHandle.GetControlState();
        Behavior_OnMouseDown();
        // 进入心流状态
        buoyInfo.OnBuoyDrag?.Invoke();
    }

    private void OnMouseDrag()
    {
        Behavior_OnMouseDrag();
    }
    private void OnMouseUp()
    {
        Behavior_OnMouseUp();
        // 重置
        Reset();
        // 推出心流状态
        // 进入心流状态
        buoyInfo.OnBuoyEndDrag?.Invoke();
    }
    void Reset()
    {
        originPos = Vector3.zero;
        tsBuoyControled = null;
        tetrisBuoySimpleTemp = null;
        tsBuoyControleds.Clear();
        tetrisBuoySimpleTemps.Clear();
        checkSelfTetris.Clear();
    }
    void CantPutAction()
    {
        transform.parent.localPosition = originPos;
        tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
        tsBuoyControled.tetrisBlockSimple.Move();
        DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        
    }
    void PutAction()
    {
        tsBuoyControled.tetrisBlockSimple.Reset();
        tsBuoyControled.transform.parent = buoyInfo.transform.parent;
        tsBuoyControled.transform.localPosition = tetrisBuoySimpleTemp.transform.localPosition + transform.parent.localPosition;
        tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
        tsBuoyControled.tetrisBlockSimple.SuccessToCreat();
        tsBuoyControled.tetrisBlockSimple.Active();
        DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
       
    }
    bool CanPutChecker()
    {
        List<bool> condition = new();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        condition.Add(hitBlock);
        bool putChecker = tetrisBuoySimpleTemp.DoDropCanPutCheck() && tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putChecker);
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    bool CanPutChecker(List<TetriBuoySimple> checkSelfTetris)
    {
        List<bool> condition = new();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        condition.Add(hitBlock);
        bool putChecker = tetrisBuoySimpleTemp.DoDropCanPutCheck(checkSelfTetris) && tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putChecker);
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    void Behavior_OnMouseDrag()
    {
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        transform.parent.localPosition = new Vector3( block.posId.x, 0, block.posId.y);
        // 不能放置的表现
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            if(!tetrisBuoySimpleTemp)return;
            tetrisBuoySimpleTemp.DoDropDragingCheck();
        }
        else if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25)
        {
            // 自碰撞检测
            foreach(var tetrisBuoySimpleTemp in tetrisBuoySimpleTemps)
            {
                this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
                this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
                checkSelfTetris.AddRange(this.tsBuoyControled.childTetris);
            }
            foreach(var tsBuoyControledTemp in tetrisBuoySimpleTemps)
            {
                tsBuoyControledTemp.DoDropDragingCheck(checkSelfTetris);
            }
        }
    }
    void Behavior_OnMouseUp()
    {
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            if(!tsBuoyControled)return;
            bool canPutChecker = CanPutChecker();
            if(!canPutChecker){CantPutAction();return;}
            // 可以放置
            PutAction();
        }else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25)
        {
            if(tetrisBuoySimpleTemps.Count == 0)return;
            if(tsBuoyControleds.Count == 0)return;
            // 自碰撞检测
            foreach(var tetrisBuoySimpleTemp in tetrisBuoySimpleTemps)
            {
                this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
                this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
                checkSelfTetris.AddRange(this.tsBuoyControled.childTetris);
            }
            List<bool> condition = new();
            foreach(var tetrisBuoySimpleTemp in tetrisBuoySimpleTemps)
            {
                this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
                this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
                bool canPutChecker = CanPutChecker(checkSelfTetris);
                condition.Add(canPutChecker);
            }
            bool allTrue = condition.All(b => b);
            if(!allTrue)
            {
                List<TetrisBuoySimple> tempList0 = new(tetrisBuoySimpleTemps);
                foreach(var tetrisBuoySimpleTemp in tempList0)
                {
                    this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
                    this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
                    CantPutAction();
                }
                return;
            }
            List<TetrisBuoySimple> tempList1 = new(tetrisBuoySimpleTemps);
            foreach(var tetrisBuoySimpleTemp in tempList1)
            {
                this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
                this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
                PutAction();
            }

        }
    }
    void Behavior_OnMouseDown()
    {
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            if(!tsBuoyControled)return;
            if(tsBuoyControled.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal) return;
            tsBuoyControled.tetrisBlockSimple.Stop();
            tetrisBuoySimpleTemp = Instantiate(tsBuoyControled, transform.parent);
            tetrisBuoySimpleTemp.tetrisBuoyDragged = tsBuoyControled; // 自碰撞检测:自己可以被覆盖
            if(!tetrisBuoySimpleTemp.tetrisBuoyDragged)return;
            // 转成Buoy坐标系
            tetrisBuoySimpleTemp.transform.localPosition = tsBuoyControled.transform.localPosition -transform.parent.localPosition;
            tetrisBuoySimpleTemp.Display_OnDragBuoy();
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            AddScanedTetrisToTemp();
        }
    }
    void AddScanedTetrisToTemp()
    {   
        if(tsBuoyControleds.Count == 0)return;
        foreach(var tetrisBuoy in tsBuoyControleds)
        {
            if(!tetrisBuoy)return;
            TetrisBuoySimple tetrisBlockSimple = tetrisBuoy;
            if(tetrisBlockSimple.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal)continue;
            tetrisBlockSimple.tetrisBlockSimple.Stop();
            TetrisBuoySimple tetrisControledTemp = Instantiate(tetrisBlockSimple, transform.parent);
            tetrisControledTemp.tetrisBuoyDragged = tetrisBlockSimple; // 自己可以被覆盖
            if(!tetrisControledTemp.tetrisBuoyDragged)continue;
            if(tetrisBuoySimpleTemps.Contains(tetrisControledTemp))continue;
            tetrisBuoySimpleTemps.Add(tetrisControledTemp);
            // 转成Buoy坐标系
            Vector2 idChanger = tetrisBlockSimple.tetrisBlockSimple.posId - buoyInfo.CurrentPosID;
            tetrisControledTemp.transform.localPosition = new Vector3(idChanger.x,0,idChanger.y);
            tetrisControledTemp.Display_OnDragBuoy();
        }
    }
}
