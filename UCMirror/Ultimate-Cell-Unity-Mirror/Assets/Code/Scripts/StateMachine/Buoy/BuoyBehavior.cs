using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using Mirror;
using UC_PlayerData;


public class BuoyBehavior : NetworkBehaviour
{
# region 数据对象
    BuoyInfo buoyInfo;
    TetrisBuoySimple tsBuoyControled;
    List<TetrisBuoySimple> tsBuoyControleds = new();
    TetrisBuoySimple tetrisBuoySimpleTemp;
    List<TetrisBuoySimple> tetrisBuoySimpleTemps = new();
    List<TetriBuoySimple> checkSelfTetris = new(); // 多砖块自碰撞检测
    LayerMask blockTargetMask;
    Vector3 originPos;
    BuoyTurnHandle.TurnHandleControlState state = BuoyTurnHandle.TurnHandleControlState.None;
    [Header("UC_PVP:")]
    public List<TetrisBuoySimple> tetrisBuoySimpleList = new();
    [SyncVar(hook = nameof(ClientGetTetrisGroupID))]
    public int tsBuoyControledId = -1;
    [SyncVar(hook = nameof(ClientGetTetrisTempGroupID))]
    public int tetrisBuoySimpleIdTemp = -1;
    [SyncVar]
    Vector3 originPos_PVP;
    
# endregion 数据对象
# region 数据关系
    void Start()
    {
        buoyInfo = transform.parent.GetComponent<BuoyInfo>();
        if(buoyInfo.Local())
        {
            Init();
        }else
        {
            if(!isLocalPlayer)return;
            Init();
        }
    }
    void Init()
    {
        blockTargetMask = buoyInfo.blockTargetMask;
        // GetComponent<SortingGroup>().sortingOrder = UC_PlayerData.Dispaly.NotFlowOrder;
    }
    private void OnMouseDown()
    {
        
        if(buoyInfo.Local())
        {
            // 单机
            originPos = transform.parent.localPosition;
            if(!buoyInfo.MouseButtonDown(false))return;
            tsBuoyControled = buoyInfo.blockBuoyHandler.GetTetrisBuoy();
            tsBuoyControleds = buoyInfo.buoyTurnHandle.GetControledTetris();
            state = buoyInfo.buoyTurnHandle.GetControlState();
            Behavior_OnMouseDown();
            
        }else
        {
            // 联网
            if(!isLocalPlayer)return;
            Client_Behavior_OnMouseDown();
        }
    }
    private void OnMouseDrag()
    {
        if(buoyInfo.Local())
        {
            Behavior_OnMouseDrag();
        }else
        {
            if(!isLocalPlayer)return;
            Client_Behavior_OnMouseDrag();
        }
        
    }
    private void OnMouseUp()
    {
        if(buoyInfo.Local())
        {
            Behavior_OnMouseUp();
        }else
        {
            if(!isLocalPlayer)return;
            Client_Behavior_OnMouseUp();
        }
        // 重置
        Reset();
        if(buoyInfo.Local())return;
        Cmd_Reset();
        
    }
# endregion 数据关系
# region 数据操作
    void Behavior_OnMouseDown()
    {
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            if(!tsBuoyControled)return;
            if(!buoyInfo.Local() && tsBuoyControled.tetrisBlockSimple.player != buoyInfo.player)return;
            if(tsBuoyControled.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal) return;
            tsBuoyControled.tetrisBlockSimple.Stop();
            // Unit预示暂存
            TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControled.NewTetrisUnit = false;
            List<KeyValuePair<int, UnitData.Color>> indexPairColors = tetrisUnitControled.GetUnitsData();
            tetrisUnitControled.CheckUnitTag(false); // 不可以战斗
            // 俄罗斯方块组 预示
            tetrisBuoySimpleTemp = Instantiate(tsBuoyControled, transform.parent);
            tetrisBuoySimpleTemp.name = tsBuoyControled.name.Replace("(Clone)",UnitData.Temp);
            tetrisBuoySimpleTemp.tetrisBuoyDragged = tsBuoyControled; // 自碰撞检测:自己可以被覆盖
            if(!tetrisBuoySimpleTemp.tetrisBuoyDragged)return;
            // Unit预示加载
            tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>().LoadUnits(indexPairColors);
            // 转成Buoy坐标系
            tetrisBuoySimpleTemp.transform.localPosition = tsBuoyControled.transform.localPosition -transform.parent.localPosition;
            tetrisBuoySimpleTemp.Display_OnDragBuoy();
            // 监听Unit死亡事件
            tsBuoyControled.TetrisBuoyTemp = tetrisBuoySimpleTemp; 
            // 进入心流状态
            buoyInfo.OnBuoyDrag?.Invoke();
            
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            AddScanedTetrisToTemp();
        }
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
            if(!tsBuoyControled || !tetrisBuoySimpleTemp)return;
            // Unit处理
            TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControled.CheckUnitTag(true); // 可以战斗
            // 放置检测
            bool canPutChecker = CanPutChecker();
            if(!canPutChecker){CantPutAction();return;}
            PutAction();// 可以放置
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
    void AddScanedTetrisToTemp()
    {   
        if(tsBuoyControleds.Count == 0)return;
        foreach(var tetrisBuoy in tsBuoyControleds)
        {
            if(!tetrisBuoy)continue;
            if(!buoyInfo.Local() && tetrisBuoy.tetrisBlockSimple.player != buoyInfo.player)continue;
            TetrisBuoySimple tetrisBuoySimple = tetrisBuoy;
            if(tetrisBuoySimple.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal)continue;
            tetrisBuoySimple.tetrisBlockSimple.Stop();
            // Unit预示暂存
            TetrisUnitSimple tetrisUnitControled = tetrisBuoy.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControled.NewTetrisUnit = false;
            List<KeyValuePair<int, UnitData.Color>> indexPairColors = tetrisUnitControled.GetUnitsData();
            tetrisUnitControled.CheckUnitTag(false);// 不可以战斗
            // 俄罗斯方块组 预示
            TetrisBuoySimple tetrisControledTemp = Instantiate(tetrisBuoySimple, transform.parent);
            tetrisControledTemp.tetrisBuoyDragged = tetrisBuoySimple; // 自己可以被覆盖
            tetrisBuoySimple.TetrisBuoyTemp = tetrisControledTemp; // 监听Unit死亡事件
            if(!tetrisControledTemp.tetrisBuoyDragged)continue;
            // Unit预示加载
            tetrisControledTemp.transform.GetComponent<TetrisUnitSimple>().LoadUnits(indexPairColors);
            // 俄罗斯方块组 收集
            if(tetrisBuoySimpleTemps.Contains(tetrisControledTemp))continue;
            tetrisBuoySimpleTemps.Add(tetrisControledTemp);
            // 转成Buoy坐标系
            Vector2 idChanger = tetrisBuoySimple.tetrisBlockSimple.posId - buoyInfo.CurrentPosID;
            tetrisControledTemp.transform.localPosition = new Vector3(idChanger.x,0,idChanger.y);
            tetrisControledTemp.Display_OnDragBuoy();
        }
        if(tetrisBuoySimpleTemps.Count==0)return;
        // 进入心流状态
        buoyInfo.OnBuoyDrag?.Invoke();
    }
    //--------------------联网--------------------
    [Client]
    void Client_Behavior_OnMouseDown()
    {
        state = buoyInfo.buoyTurnHandle.GetControlState();
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            Cmd_Behavior_OnMouseDown(Input.mousePosition);
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            Cmd_BehaviorMultiple_OnMouseDown(Input.mousePosition);
        }
    }
    [Command(requiresAuthority = false)]
    void Cmd_Behavior_OnMouseDown(Vector3 mousePos_Temp)
    {
        state = BuoyTurnHandle.TurnHandleControlState.Scaning_1;
        if(!buoyInfo.Server_MouseButtonDown(mousePos_Temp))return;
        originPos_PVP = transform.parent.localPosition;
        tsBuoyControled = buoyInfo.blockBuoyHandler.GetTetrisBuoy();
        if(!tsBuoyControled)return;
        if(!buoyInfo.Local() && tsBuoyControled.tetrisBlockSimple.player != buoyInfo.player_local)return;
        if(tsBuoyControled.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal) return;
        tsBuoyControled.tetrisBlockSimple.Stop();
        // --服务器生成预示砖块--
        tetrisBuoySimpleTemp = MakeOneTetris(tsBuoyControled.tetrisBlockSimple.type,tsBuoyControled);
        if(!tetrisBuoySimpleTemp.tetrisBuoyDragged)return;
        // 转成Buoy坐标系
        tetrisBuoySimpleTemp.transform.localPosition = tsBuoyControled.transform.localPosition - transform.parent.localPosition;
        // 刷新砖块表现
        tetrisBuoySimpleTemp.tetrisBlockSimple.Player = buoyInfo.player_local;
        tetrisBuoySimpleTemp.Display_OnDragBuoy();
        // 服务器回调
        tetrisBuoySimpleIdTemp = ServerLogic.GetTetrisGroupIDTemp(1);
        tsBuoyControledId = tsBuoyControled.tetrisBlockSimple.serverID;
        Rcp_AddScanedTetriToTemp(tsBuoyControled.tetrisBlockSimple.type,tetrisBuoySimpleTemp.transform.localPosition,tsBuoyControled.rotateTimes);
    }
    [Command(requiresAuthority = false)]
    void Cmd_BehaviorMultiple_OnMouseDown(Vector3 mousePos_Temp)
    {
        state = BuoyTurnHandle.TurnHandleControlState.Scaning_9;
        if(!buoyInfo.Server_MouseButtonDown(mousePos_Temp))return;
        originPos_PVP = transform.parent.localPosition;
        tsBuoyControleds = buoyInfo.buoyTurnHandle.GetControledTetris();
        Server_AddScanedTetrisToTemp();
        List<TetrisBuoySimple> tsBuoyControledsTemp = new(tsBuoyControleds);
        foreach(var temp in tsBuoyControledsTemp)
        {
            if(!temp)continue;
            if(!temp.tetrisBlockSimple)temp.Init();
            if(buoyInfo.player_local!=temp.tetrisBlockSimple.player)continue;
            Vector2 idChanger = temp.tetrisBlockSimple.posId - buoyInfo.CurrentPosID;
            Vector3 pos = new Vector3(idChanger.x,0,idChanger.y);
            Rcp_AddScanedTetrisToTemp(temp.tetrisBlockSimple.type,pos,temp.rotateTimes);
        }
        
    }
    [TargetRpc]
    void Rcp_AddScanedTetriToTemp(string type,Vector3 pos,int roteteTimes)
    {
        state = BuoyTurnHandle.TurnHandleControlState.Scaning_1;
        int index = tetrisBuoySimpleList.FindIndex(item => item.GetComponent<TetrisBlockSimple>().type ==  type);
        var tetrominoe = Instantiate(tetrisBuoySimpleList[index].gameObject,transform.parent);
        TetrisBlockSimple tetrisBlockSimple = tetrominoe.GetComponent<TetrisBlockSimple>();
        for(int i = 0; i < roteteTimes; i++)
        {
            tetrisBlockSimple.Rotate(tetrisBlockSimple.transform.forward);
        }
        tetrisBuoySimpleTemp = tetrominoe.GetComponent<TetrisBuoySimple>();
        if(!tetrisBuoySimpleTemp)return;
        tetrisBuoySimpleTemp.transform.parent = transform.parent;
        tetrisBuoySimpleTemp.transform.localPosition = pos;
        tetrisBuoySimpleTemp.transform.localScale = Vector3.one;
        tetrisBuoySimpleTemp.Display_OnDragBuoy();
        tetrisBuoySimpleTemp.tetrisBlockSimple.Player = buoyInfo.player_local; // 刷新砖块表现
        // 进入心流状态
        buoyInfo.OnBuoyDrag?.Invoke();
    }
    [TargetRpc]
    void Rcp_AddScanedTetrisToTemp(string type,Vector3 pos,int roteteTimes)
    {
        state = BuoyTurnHandle.TurnHandleControlState.Scaning_9;
        int index = tetrisBuoySimpleList.FindIndex(item => item.GetComponent<TetrisBlockSimple>().type ==  type);
        var tetrominoe = Instantiate(tetrisBuoySimpleList[index].gameObject,transform.parent);
        TetrisBlockSimple tetrisBlockSimple = tetrominoe.GetComponent<TetrisBlockSimple>();
        for(int i = 0; i < roteteTimes; i++)
        {
            tetrisBlockSimple.Rotate(tetrisBlockSimple.transform.forward);
        }
        TetrisBuoySimple temp = tetrominoe.GetComponent<TetrisBuoySimple>();
        if(!temp)return;
        temp.transform.parent = transform.parent;
        temp.transform.localPosition = pos;
        temp.transform.localScale = Vector3.one;
        temp.Display_OnDragBuoy();
        temp.tetrisBlockSimple.Player = buoyInfo.player_local; // 刷新砖块表现
        // 加入检测组
        if(tetrisBuoySimpleTemps.Contains(temp))return;
        tetrisBuoySimpleTemps.Add(temp);
        // 进入心流状态
        buoyInfo.OnBuoyDrag?.Invoke();
    }
    [Server]
    void Server_AddScanedTetrisToTemp()
    {   
        
        if(tsBuoyControleds.Count == 0)return;
        foreach(var tetrisBuoy in tsBuoyControleds)
        {
            if(!tetrisBuoy)continue;
            if(!buoyInfo.Local() && tetrisBuoy.tetrisBlockSimple.player != buoyInfo.player_local)continue;
            if(tetrisBuoy.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal)continue;
            tetrisBuoy.tetrisBlockSimple.Stop();
            // --服务器生成预示砖块--
            TetrisBuoySimple tetrisControledTemp = MakeOneTetris(tetrisBuoy.tetrisBlockSimple.type,tetrisBuoy);
            // 加入检测组
            if(!tetrisControledTemp.tetrisBuoyDragged)continue;
            if(tetrisBuoySimpleTemps.Contains(tetrisControledTemp))continue;
            tetrisBuoySimpleTemps.Add(tetrisControledTemp);
            // 转成Buoy坐标系
            Vector2 idChanger = tetrisBuoy.tetrisBlockSimple.posId - buoyInfo.CurrentPosID;
            tetrisControledTemp.transform.localPosition = new Vector3(idChanger.x,0,idChanger.y);
            // 刷新砖块表现
            tetrisControledTemp.tetrisBlockSimple.Player = buoyInfo.player_local;
            tetrisControledTemp.Display_OnDragBuoy();
        }
        if(tetrisBuoySimpleTemps.Count==0)return;
        // 进入心流状态
        buoyInfo.OnBuoyDrag?.Invoke();
    }
    TetrisBuoySimple MakeOneTetris(string typeIn,TetrisBuoySimple Controled)
    {
        if(buoyInfo.Local())return null;
        int index = tetrisBuoySimpleList.FindIndex(item => item.GetComponent<TetrisBlockSimple>().type ==  typeIn);
        if(tetrisBuoySimpleList.Count==0){Debug.LogError("没给浮标配置砖块"); return null;}
        var tetrominoe = Instantiate(tetrisBuoySimpleList[index].gameObject,transform.parent);
        tetrominoe.name = tetrominoe.name.Replace("(Clone)","(Temp)");
        TetrisBlockSimple tetrisBlockSimple = tetrominoe.GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.Init();
        tetrisBlockSimple.autoID = false;
        for(int i = 0; i < Controled.rotateTimes; i++)
        {
            tetrisBlockSimple.Rotate(tetrisBlockSimple.transform.forward);
        }
        TetrisBuoySimple tetrisControledTemp = tetrominoe.GetComponent<TetrisBuoySimple>();
        tetrisControledTemp.Init();
        tetrisControledTemp.tetrisBuoyDragged = Controled; // 自碰撞检测:自己可以被覆盖
        // tetrisControledTemp.tetrisBlockSimple.ServerID = ServerLogic.GetTetrisGroupIDTemp(1);
        return tetrisControledTemp;
    }
    [Client]
    void Client_Behavior_OnMouseDrag()
    {
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            Cmd_Behavior_OnMouseDrag(block.posId);
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            // 自碰撞检测
            Cmd_BehaviorMultiple_OnMouseDrag(block.posId);
        }
        
    }
    [Command]
    void Cmd_Behavior_OnMouseDrag(Vector2 blockposId)
    {
        transform.parent.localPosition = new Vector3( blockposId.x, 0, blockposId.y);
        // 不能放置的表现
        if(!tetrisBuoySimpleTemp)return;
        bool canPut = tetrisBuoySimpleTemp.DoDropDragingCheck();
        Rpc_Behavior_OnMouseDrag(canPut);
    }
    [Command]
    void Cmd_BehaviorMultiple_OnMouseDrag(Vector2 blockposId)
    {
        if(tetrisBuoySimpleTemps.Count == 0)return;
        transform.parent.localPosition = new Vector3( blockposId.x, 0, blockposId.y);
        foreach(var tetrisBuoySimpleTemp in tetrisBuoySimpleTemps)
        {
            if(!tetrisBuoySimpleTemp)continue;
            this.tetrisBuoySimpleTemp = tetrisBuoySimpleTemp;
            this.tsBuoyControled = tetrisBuoySimpleTemp.tetrisBuoyDragged;
            checkSelfTetris.AddRange(this.tsBuoyControled.childTetris);
        }
        List<bool> checker = new();
        foreach(var tsBuoyControledTemp in tetrisBuoySimpleTemps)
        {
            if(!tsBuoyControledTemp)continue;
            bool check = tsBuoyControledTemp.DoDropDragingCheck(checkSelfTetris);
            checker.Add(check);
        }
        bool allTrue = checker.All(b => b);
        Rpc_BehaviorMultiple_OnMouseDrag(allTrue);
    }
    [TargetRpc]
    void Rpc_Behavior_OnMouseDrag(bool canPut)
    {
        if(!tetrisBuoySimpleTemp)return;
        if(canPut)tetrisBuoySimpleTemp.Display_OnDragBuoy();
        else if(!canPut)tetrisBuoySimpleTemp.Display_OnCantDragBuoy();
    }
    [TargetRpc]
    void Rpc_BehaviorMultiple_OnMouseDrag(bool canPut)
    {
        if(tetrisBuoySimpleTemps.Count == 0)return;
        if(canPut)
        {
            foreach(var tsBuoyControledTemp in tetrisBuoySimpleTemps)
            {
                if(!tsBuoyControledTemp)continue;
                tsBuoyControledTemp.Display_OnDragBuoy();
            }
        }else
        {
            foreach(var tsBuoyControledTemp in tetrisBuoySimpleTemps)
            {
                if(!tsBuoyControledTemp)continue;
                tsBuoyControledTemp.Display_OnCantDragBuoy();
            }
        }
    }
    [Client]
    void Client_Behavior_OnMouseUp()
    {
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            Cmd_Behavior_OnMouseUp();
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            Cmd_BehaviorMultiple_OnMouseUp();
        }
        
    }
    [Command]
    void Cmd_Behavior_OnMouseUp()
    {
        state = BuoyTurnHandle.TurnHandleControlState.None;
        Rcp_Destory();
        if(!tsBuoyControled || !tetrisBuoySimpleTemp)return;
        bool canPutChecker = Server_CanPutChecker();
        if(!canPutChecker){CantPutAction();return;}
        // 可以放置
        PutAction();
    }
    [Command]
    void Cmd_BehaviorMultiple_OnMouseUp()
    {
        if(tetrisBuoySimpleTemps.Count == 0)return;
        if(tsBuoyControleds.Count == 0)return;
        state = BuoyTurnHandle.TurnHandleControlState.None;
        Rcp_Destory();
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
            bool canPutChecker = Server_MultipleCanPutChecker();
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
    public void ClientGetTetrisTempGroupID(int oldValue,int newValue)
    { 
        if(!isClient)return;
        // Debug.Log("ClientGetTetrisTempGroupID" + newValue);
    }
    public void ClientGetTetrisGroupID(int oldValue,int newValue)
    {   
        if(!isClient)return;
        // Debug.Log("ClientGetTetrisGroupID" + newValue);
    }
    
    [TargetRpc]
    void Rcp_Destory()
    {
        if(!isClient)return;
        List<TetrisBuoySimple> tetrisBuoySimpleTemps = new();
        tetrisBuoySimpleTemps = transform.parent.GetComponentsInChildren<TetrisBuoySimple>().ToList();
        if(tetrisBuoySimpleTemps.Count == 0)return;
        foreach(var tetris in tetrisBuoySimpleTemps)
        {
            if(!tetris)return;
            DestroyImmediate(tetris.gameObject);
        }
    }
    [Server]
    bool Server_CanPutChecker()
    {
        List<bool> condition = new();
        List<TetrisBuoySimple> tetrisBuoySimpleTemps = new();
        tetrisBuoySimpleTemps = transform.parent.GetComponentsInChildren<TetrisBuoySimple>().ToList();
        foreach(var tetris in tetrisBuoySimpleTemps)
        {
            if(!tetris)condition.Add(false);
            bool putChecker = tetris.DoDropCanPutCheck();
            condition.Add(putChecker);
        }
        bool putCheckerControled = tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putCheckerControled);
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    [Server]
    bool Server_MultipleCanPutChecker()
    {
        if(checkSelfTetris.Count == 0)return false;
        List<bool> condition = new();
        bool putChecker = tetrisBuoySimpleTemp.DoDropCanPutCheck(checkSelfTetris);
        condition.Add(putChecker);
        bool putCheckerControled = tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putCheckerControled);
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    
    // ------------------工具--------------------
    [Command]
    void Cmd_Reset()
    {
        Reset();
    }
    void Reset()
    {
        if(buoyInfo.Local())
        {
            originPos = Vector3.zero;
            tsBuoyControled = null;
            tetrisBuoySimpleTemp = null;
            tsBuoyControleds.Clear();
            tetrisBuoySimpleTemps.Clear();
            checkSelfTetris.Clear();
        }
        else
        {
            originPos_PVP = Vector3.zero;
            if(tetrisBuoySimpleTemp)tetrisBuoySimpleTemp = null;
            if(tsBuoyControled)tsBuoyControled = null;
            if(tsBuoyControleds.Count!=0)tsBuoyControleds.Clear();
            if(tetrisBuoySimpleTemps.Count!=0)tetrisBuoySimpleTemps.Clear();
            if(checkSelfTetris.Count!=0)checkSelfTetris.Clear();
        }
        // 退出心流状态
        buoyInfo.OnBuoyEndDrag?.Invoke();
    }
    void CantPutAction()
    {
        if(!tsBuoyControled || !tetrisBuoySimpleTemp)return;
        if(buoyInfo.Local())
        {
            // Unit相关
            TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControled.CheckUnitTag(true);
            // 放置相关
            transform.parent.localPosition = originPos;
            tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
            tsBuoyControled.tetrisBlockSimple.Move();
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }else
        {
            if(!isServer)return;
            transform.parent.localPosition = originPos_PVP;
            tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
            tsBuoyControled.tetrisBlockSimple.Move();
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }
        
    }
    void PutAction()
    {
        if(!tsBuoyControled || !tetrisBuoySimpleTemp)return;
        // Unit相关
        TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
        tetrisUnitControled.CheckUnitTag(true); // 可以战斗
        // 放置相关
        tsBuoyControled.tetrisBlockSimple.Reset();
        tsBuoyControled.transform.parent = buoyInfo.transform.parent;
        tsBuoyControled.transform.localPosition = tetrisBuoySimpleTemp.transform.localPosition + transform.parent.localPosition;
        tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
        tsBuoyControled.tetrisBlockSimple.SuccessToCreat();
        tsBuoyControled.tetrisBlockSimple.Active();
        // 机制检测
        TetrisBlockSimple tetrisBlockSimple = tsBuoyControled.TetrisBlockSimple;
        tetrisBlockSimple.BlocksCreator.BlocksCounterInvoke();
        
        if(buoyInfo.Local())
        {
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }else
        {
            if(!isServer)return;
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }
        
       
    }
    bool CanPutChecker()
    {
        List<bool> condition = new();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        condition.Add(hitBlock);
        if(!tsBuoyControled){DestroyImmediate(tetrisBuoySimpleTemp.gameObject);return false;}
        bool putChecker = tetrisBuoySimpleTemp.DoDropCanPutCheck() && tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putChecker);
        bool allTrue = condition.All(b => b);
        tsBuoyControled.TetrisBuoyTemp = null; // 释放死亡监听
        return allTrue;
    }
    bool CanPutChecker(List<TetriBuoySimple> checkSelfTetris)
    {
        List<bool> condition = new();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        condition.Add(hitBlock);
        if(!tsBuoyControled || !tsBuoyControled.tetrisBlockSimple){DestroyImmediate(tetrisBuoySimpleTemp.gameObject);return false;}
        bool putChecker = tetrisBuoySimpleTemp.DoDropCanPutCheck(checkSelfTetris) && tsBuoyControled.tetrisBlockSimple.OnBuoyDrop();
        condition.Add(putChecker);
        bool allTrue = condition.All(b => b);
        tsBuoyControled.TetrisBuoyTemp = null; // 释放死亡监听
        return allTrue;
    }
#endregion 数据操作
}
