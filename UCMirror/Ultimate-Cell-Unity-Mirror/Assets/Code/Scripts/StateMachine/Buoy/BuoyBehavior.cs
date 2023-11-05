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
# endregion 数据对象
# region 联网数据对象
    [Header("UC_PVP:")]
    public List<TetrisBuoySimple> tetrisBuoySimpleList = new();
    [SyncVar(hook = nameof(ClientGetTetrisGroupID))]
    public int tsBuoyControledId = -1;
    [SyncVar(hook = nameof(ClientGetTetrisTempGroupID))]
    public int tetrisBuoySimpleIdTemp = -1;
    [SyncVar]
    Vector3 originPos_PVP;
    NetworkManagerUC_PVP networkManagerUC_PVP;
    NetworkManagerUC_PVP NetworkManagerUC_PVP
    {
        get{
            if(!isServer)return null;
            if(!networkManagerUC_PVP)networkManagerUC_PVP = FindObjectOfType<NetworkManagerUC_PVP>();
            return networkManagerUC_PVP;
        }
    }
    struct ServerToClient_SetCorrect
    {
        public uint netId;
        public float rotationDifferentLocal;
        public Vector3 localPosition;
        public Quaternion localRotation; 
        public Player player;
        public UserAction.State userState;
    }
# endregion 联网数据对象
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
            Reset();
        }else
        {
            if(!isLocalPlayer)return;
            Client_Behavior_OnMouseUp();
            Cmd_Reset();
        }
    }
# endregion 数据关系
# region 数据操作
    void Behavior_OnMouseDown()
    {
        buoyInfo.OnBuoyDrag?.Invoke();
        UserAction.Player1UserState = UserAction.State.CommandTheBattle_Buoy;
        UserAction.Player2UserState = UserAction.State.CommandTheBattle_Buoy;
        state = buoyInfo.buoyTurnHandle.GetControlState();
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            tsBuoyControled = buoyInfo.blockBuoyHandler.GetTetrisBuoy();
            if(!tsBuoyControled)return;
            if(!buoyInfo.Local() && tsBuoyControled.tetrisBlockSimple.player != buoyInfo.player)return;
            if(tsBuoyControled.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal) return;
            tsBuoyControled.tetrisBlockSimple.Stop(false);
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
            // Units
            TetrisUnitSimple tetrisUnitControledTemp = tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControledTemp.LoadUnits(indexPairColors);
            // 转成Buoy坐标系
            tetrisBuoySimpleTemp.transform.localPosition = tsBuoyControled.transform.localPosition -transform.parent.localPosition;
            tetrisBuoySimpleTemp.Display_OnDragBuoy();
            tetrisBuoySimpleTemp.GetComponent<TetrisUnitSimple>().Display_ShowForPlayerScreen();
            tsBuoyControled.TetrisBuoyTemp = tetrisBuoySimpleTemp;
            
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            tsBuoyControleds = buoyInfo.buoyTurnHandle.GetControledTetris();
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
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            if(!tetrisBuoySimpleTemp)return;
            TetrisUnitSimple tetrisUnitControledTemp = tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>();
            tetrisUnitControledTemp.SetUnitSortingOrderToFlow();
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
        UserAction.Player1UserState = UserAction.State.WatchingFight;
        UserAction.Player2UserState = UserAction.State.WatchingFight;
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
        }else 
        if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25)
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
            float littleUpForRayCheck = 0.3f;
            tetrisControledTemp.transform.localPosition = new Vector3(idChanger.x,littleUpForRayCheck,idChanger.y);
            tetrisControledTemp.Display_OnDragBuoy();
        }
        if(tetrisBuoySimpleTemps.Count==0)return;
    }
    
    // ------------------工具--------------------
    
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
            tsBuoyControled.tetrisBlockSimple.Stop();
            tsBuoyControled.tetrisBlockSimple.Active_X();
            // tsBuoyControled.tetrisBlockSimple.Move();
            
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }else
        {
            if(!isServer)return;
            transform.parent.localPosition = originPos_PVP;
            tsBuoyControled.tetrisBlockSimple.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Drop;
            tsBuoyControled.tetrisBlockSimple.Stop();
            tsBuoyControled.tetrisBlockSimple.Active_X();
            // tsBuoyControled.tetrisBlockSimple.Move();
            
            // DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
            NetworkServer.Destroy(tetrisBuoySimpleTemp.gameObject);
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
        tetrisBlockSimple.BlocksCreator.Event_BlocksCounterInvoke();
        
        if(buoyInfo.Local())
        {
            DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
        }else
        {
            if(!isServer)return;
            // DestroyImmediate(tetrisBuoySimpleTemp.gameObject);
            NetworkServer.Destroy(tetrisBuoySimpleTemp.gameObject);
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
#region 联网数据操作
//--------------------联网--------------------
    Vector3 ClientMouseOffest()
    {
        Vector3 player1MousePosition = Input.mousePosition + new Vector3(0,0,100);
        Vector3 player2MousePosition = Input.mousePosition - new Vector3(0,0,100);
        Vector3 mousePosition = ServerLogic.Local_palayer == Player.Player1 ? player1MousePosition : ServerLogic.Local_palayer == Player.Player2 ? player2MousePosition : Vector3.zero;
        return mousePosition;
    }
    [Client]
    void Client_Behavior_OnMouseDown()
    {
        state = buoyInfo.buoyTurnHandle.GetControlState();
        
        if(state == BuoyTurnHandle.TurnHandleControlState.Scaning_1)
        {
            originPos_PVP = transform.parent.localPosition;
            Cmd_Behavior_OnMouseDown(Input.mousePosition,originPos_PVP);
        }
        else if (state == BuoyTurnHandle.TurnHandleControlState.Scaning_9 || state == BuoyTurnHandle.TurnHandleControlState.Scaning_25 )
        {
            // 过时，暂未启用
            // Cmd_BehaviorMultiple_OnMouseDown(Input.mousePosition);
        }
    }
    [Command(requiresAuthority = false)]
    void Cmd_Behavior_OnMouseDown(Vector3 mousePos_Temp,Vector3 buoyOriginPos_PVP)
    {
        originPos_PVP = buoyOriginPos_PVP;
        buoyInfo.OnBuoyDrag?.Invoke();
        // UserAction.Player1UserState = UserAction.State.CommandTheBattle_Buoy;
        // UserAction.Player2UserState = UserAction.State.CommandTheBattle_Buoy;
        state = buoyInfo.buoyTurnHandle.GetControlState();
        tsBuoyControled = buoyInfo.blockBuoyHandler.GetTetrisBuoy();
        if(!tsBuoyControled)return;
        if(!buoyInfo.Local() && tsBuoyControled.tetrisBlockSimple.player != buoyInfo.player)return;
        if(tsBuoyControled.tetrisBlockSimple.tetrisCheckMode != TetrisBlockSimple.TetrisCheckMode.Normal) return;
        if(buoyInfo.player_local == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.CommandTheBattle_Buoy;
        }else if(buoyInfo.player_local == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.CommandTheBattle_Buoy;
        }
        // 俄罗斯方块组 预示
            // tetrisBuoySimpleTemp = Instantiate(tsBuoyControled, transform.parent);
        Player whichPlayer = tsBuoyControled.TetrisBlockSimple.player;
        Server_OnGameObjCreate_tetrisBuoySimpleTemp(tsBuoyControled.name.Replace("(Clone)",""),whichPlayer);
        
        
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
        TetrisUnitSimple tetrisUnitControledTemp = tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>();
        tetrisUnitControledTemp.SetUnitSortingOrderToFlow();
        bool canPut = tetrisBuoySimpleTemp.DoDropDragingCheck();
        // Debug.Log(canPut);
        Player whichPlayer = tsBuoyControled.TetrisBlockSimple.player;
        Rpc_Behavior_OnMouseDrag(canPut,whichPlayer);
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
    void Rpc_Behavior_OnMouseDrag(bool canPut,Player whichPlayer)
    {
        if(!tetrisBuoySimpleTemp)return;
        if(whichPlayer!=ServerLogic.Local_palayer)return;
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
            // Cmd_BehaviorMultiple_OnMouseUp();
        }
        
    }
    [Command]
    void Cmd_Behavior_OnMouseUp()
    {
        if(buoyInfo.player_local == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.WatchingFight;
        }else if(buoyInfo.player_local == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.WatchingFight;
        }
        state = BuoyTurnHandle.TurnHandleControlState.None;
        // Rcp_Destory();
        if(!tsBuoyControled || !tetrisBuoySimpleTemp)return;
        // Unit处理
        TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
        bool canDoFight = true;
        tetrisUnitControled.CheckUnitTag(canDoFight);
        // 放置检测
        Player whichPlayer = tsBuoyControled.TetrisBlockSimple.player;
        bool canPutChecker = Server_CanPutChecker();
        if(!canPutChecker){
            CantPutAction();
            Client_CantPutAction(whichPlayer);
            return;
        }
        // 可以放置
        PutAction();
        Client_PutAction(whichPlayer);
    }
    [ClientRpc]
    void Client_CantPutAction(Player whichPlayer)
    {
        if(whichPlayer!=ServerLogic.Local_palayer)return;
        if(whichPlayer == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.WatchingFight;
        }else if(whichPlayer == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.WatchingFight;
        }
        Reset();
    }
    [ClientRpc]
    void Client_PutAction(Player whichPlayer)
    {
        if(whichPlayer!=ServerLogic.Local_palayer)return;
        if(whichPlayer == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.WatchingFight;
        }else if(whichPlayer == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.WatchingFight;
        }
        Reset();
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
    
    [Command]
    void Cmd_Reset()
    {
        Reset();
    }
    [Server]
    void Server_OnGameObjCreate_tetrisBuoySimpleTemp(string tetrominoeName,Player whichPlayer)
    {
        // Debug.Log("Server_OnGameObjCreate_tetrisBuoySimpleTemp----");
        tetrisBuoySimpleTemp = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == tetrominoeName),transform.parent).GetComponent<TetrisBuoySimple>();
        tetrisBuoySimpleTemp.Server_Init_TetriUnits();
        tetrisBuoySimpleTemp.player = whichPlayer;
        tetrisBuoySimpleTemp.TetrisBlockSimple.Player = whichPlayer;
        NetworkServer.Spawn(tetrisBuoySimpleTemp.gameObject);
        Client_OnGameObjCreate_tetrisBuoySimpleTemp(tetrisBuoySimpleTemp.netId,whichPlayer);
        Server_SetCorrect_tetrisBuoySimpleTemp();
    }
    [ClientRpc]
    void Client_OnGameObjCreate_tetrisBuoySimpleTemp(uint tetrisBuoySimpleTempNetID,Player whichPlayerDragging)
    {
        
        if(!tetrisBuoySimpleTemp)tetrisBuoySimpleTemp = FindObjectsOfType<TetrisBuoySimple>().ToList().Find(item => item.netId == tetrisBuoySimpleTempNetID);
        tetrisBuoySimpleTemp.transform.parent = transform.parent;
        if(whichPlayerDragging != ServerLogic.Local_palayer)return;
        buoyInfo.OnBuoyDrag?.Invoke();
        Invoke(nameof(Display_ShowForPlayerScreen),0.1f);
    }
    void Display_ShowForPlayerScreen()
    {
        if(!tetrisBuoySimpleTemp)return;
        tetrisBuoySimpleTemp.GetComponent<TetrisUnitSimple>().Display_ShowForPlayerScreen();
    }
    [Server]
    void Server_SetCorrect_tetrisBuoySimpleTemp()
    {
        tsBuoyControled.tetrisBlockSimple.Stop(false);
        // Unit预示暂存
        TetrisUnitSimple tetrisUnitControled = tsBuoyControled.transform.GetComponent<TetrisUnitSimple>();
        tetrisUnitControled.NewTetrisUnit = false;
        List<KeyValuePair<int, UnitData.Color>> indexPairColors = tetrisUnitControled.GetUnitsData();
        tetrisUnitControled.CheckUnitTag(false); // 不可以战斗

        tetrisBuoySimpleTemp.name = tsBuoyControled.name.Replace("(Clone)",UnitData.Temp);
        tetrisBuoySimpleTemp.tetrisBuoyDragged = tsBuoyControled; // 自碰撞检测:自己可以被覆盖
        if(!tetrisBuoySimpleTemp.tetrisBuoyDragged)return;
        // Units
        TetrisUnitSimple tetrisUnitControledTemp = tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>();
        var unitRotationDifferentLocal = Quaternion.Angle(tetrisBuoySimpleTemp.transform.localRotation, tsBuoyControled.transform.localRotation);
        tetrisUnitControledTemp.Server_LoadUnits(indexPairColors,unitRotationDifferentLocal);
        // 转成Buoy坐标系
        tetrisBuoySimpleTemp.transform.localPosition = tsBuoyControled.transform.localPosition -transform.parent.localPosition;
        tetrisBuoySimpleTemp.transform.localRotation = tsBuoyControled.transform.localRotation;
        tetrisBuoySimpleTemp.Display_OnDragBuoy();
        tsBuoyControled.TetrisBuoyTemp = tetrisBuoySimpleTemp;
        ServerToClient_SetCorrect serverToClient_SetCorrect = new();
        serverToClient_SetCorrect.netId = tetrisBuoySimpleTemp.netId;
        serverToClient_SetCorrect.rotationDifferentLocal = unitRotationDifferentLocal;
        serverToClient_SetCorrect.localRotation = tsBuoyControled.transform.localRotation;
        serverToClient_SetCorrect.localPosition = tsBuoyControled.transform.localPosition;
        serverToClient_SetCorrect.player = buoyInfo.player_local;
        serverToClient_SetCorrect.userState = UserAction.State.CommandTheBattle_Buoy;
        Client_SetCorrect_tetrisBuoySimpleTemp(serverToClient_SetCorrect);
    }
    [ClientRpc]
    void Client_SetCorrect_tetrisBuoySimpleTemp(ServerToClient_SetCorrect serverToClient_SetCorrect_In)
    {
        // if(serverToClient_SetCorrect_In.player == Player.Player1)
        // {
        //    TetriDifferentStatusDisplays.ForEach(x=>x.Event_OnUserActionStateChanged(serverToClient_SetCorrect_In.userState));
        // }else if(serverToClient_SetCorrect_In.player == Player.Player2)
        // {
        //     TetriDifferentStatusDisplays.ForEach(x=>x.Event_OnUserActionStateChanged(serverToClient_SetCorrect_In.userState));
        // }
        // if(serverToClient_SetCorrect_In.player != buoyInfo.player_local)return;
        if(serverToClient_SetCorrect_In.player != ServerLogic.Local_palayer)return;
        if(serverToClient_SetCorrect_In.player == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.CommandTheBattle_Buoy;

        }else if(serverToClient_SetCorrect_In.player == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.CommandTheBattle_Buoy;
        }
        if(!tetrisBuoySimpleTemp)tetrisBuoySimpleTemp = FindObjectsOfType<TetrisBuoySimple>().ToList().Find(item => item.netId == serverToClient_SetCorrect_In.netId);
        TetrisUnitSimple tetrisUnitControledTemp = tetrisBuoySimpleTemp.transform.GetComponent<TetrisUnitSimple>();
        tetrisUnitControledTemp.Client_LoadUnits(serverToClient_SetCorrect_In.rotationDifferentLocal);
        // 转成Buoy坐标系
        tetrisBuoySimpleTemp.transform.localPosition = serverToClient_SetCorrect_In.localPosition - transform.parent.localPosition;
        tetrisBuoySimpleTemp.transform.localRotation = serverToClient_SetCorrect_In.localRotation;
        tetrisBuoySimpleTemp.Display_OnDragBuoy();
    }
#endregion 联网数据操作
}
