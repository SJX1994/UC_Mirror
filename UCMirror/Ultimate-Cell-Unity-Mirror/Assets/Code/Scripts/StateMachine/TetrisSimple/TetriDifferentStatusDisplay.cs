using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UC_PlayerData;
using Spine.Unity;
using System.Linq;
using Mirror;

public class TetriDifferentStatusDisplay : NetworkBehaviour
{
#region 数据对象
    Sprite player1ActionBox;
    Sprite player2ActionBox;
    Sprite Player1ActionBox
    {
        get
        {
            if(player1ActionBox)return player1ActionBox;
            player1ActionBox = Resources.Load<Sprite>("Foreshadow/BrightRed");
            return player1ActionBox;
        }
    }
    Sprite Player2ActionBox
    {
        get
        {
            if(player2ActionBox)return player2ActionBox;
            player2ActionBox = Resources.Load<Sprite>("Foreshadow/LakeBlue");
            return player2ActionBox;
        }
    }
    Sprite originSprite;

    Player player;
    Player Player
    {
        get
        {
            // if(!IdelBox)return Player.NotReady;
            // if(!TetriUnitSimple)return IdelBox.player;
            // if(!TetriUnitSimple.TetriBlock)return IdelBox.player;
            if(player != Player.NotReady)return player;
            player = TetriUnitSimple.TetriBlock.Player;
            return player;
        }
    }
    IdelBox idelBox;
    IdelBox IdelBox
    {
        get
        {
            if(idelBox)return idelBox;
            List<IdelBox> idelBoxes = new();
            idelBoxes = FindObjectsOfType<IdelBox>().ToList();
            if(idelBoxes.Count == 0)return null;
            for(int i = 0;i < idelBoxes.Count;i++)
            {
                if(idelBoxes[i].player != player)continue;
                idelBox = idelBoxes[i];
            }
            return idelBox;

        }
    }
    Transform cube;
    Transform Cube
    {
        get
        {
            if (!cube)cube = transform.Find("Cube");
            return cube;
        }
    }
    TetriUnitSimple tetriUnitSimple;
    TetriUnitSimple TetriUnitSimple
    {
        get
        {
            if (!tetriUnitSimple)tetriUnitSimple = transform.GetComponent<TetriUnitSimple>();
            return tetriUnitSimple;
        }
    }
    TetriDisplayRange tetriDisplayRange;
    TetriDisplayRange TetriDisplayRange
    {
        get
        {
            if (!tetriDisplayRange)tetriDisplayRange = transform.Find("Display_Range").GetComponent<TetriDisplayRange>();
            return tetriDisplayRange;
        }
    }
    TetriBuoySimple tetriBuoySimple;
    TetriBuoySimple TetriBuoySimple
    {
        get
        {
            if (!tetriBuoySimple)tetriBuoySimple = transform.GetComponent<TetriBuoySimple>();
            return tetriBuoySimple;
        }
    }
#endregion 数据对象
#region 数据关系
    void Start()
    {
        Event_Register();
    }
    void OnDisable()
    {
        UserAction.OnPlayer1UserActionStateChanged -= Event_OnUserActionStateChanged;
        UserAction.OnPlayer2UserActionStateChanged -= Event_OnUserActionStateChanged;
        if(!IdelBox)return;
        IdelBox.OnTetriBeginDrag -= Event_Display_OnBeginDrag;
        IdelBox.OnTetriEndDrag -= Event_Display_OnEndDrag;
        IdelBox.OnTheCheckerboard -= Event_Display_OnTheCheckerboard;
    }
#endregion 数据关系
#region 数据操作
    void Event_Register()
    {
        if(Local())
        {
            if(TetriUnitSimple.TetriBlock.Player == Player.Player1)
            {
                UserAction.OnPlayer1UserActionStateChanged += Event_OnUserActionStateChanged;
                player1ActionBox = Resources.Load<Sprite>("Foreshadow/BrightRed");
            }
            if(TetriUnitSimple.TetriBlock.Player == Player.Player2)
            {
                UserAction.OnPlayer2UserActionStateChanged += Event_OnUserActionStateChanged;
                player2ActionBox = Resources.Load<Sprite>("Foreshadow/LakeBlue");
            }
        }else
        {
            if(!isServer)return;
            if(TetriUnitSimple.TetriBlock.Player == Player.Player1)
            {
                UserAction.OnPlayer1UserActionStateChanged += Server_Event_OnUserActionStateChanged;
                player1ActionBox = Resources.Load<Sprite>("Foreshadow/BrightRed");
            }
            if(TetriUnitSimple.TetriBlock.Player == Player.Player2)
            {
                UserAction.OnPlayer2UserActionStateChanged += Server_Event_OnUserActionStateChanged;
                player2ActionBox = Resources.Load<Sprite>("Foreshadow/LakeBlue");
            }
        }
        
        float waitLoading = 0.1f;
        Invoke(nameof(Init),waitLoading);
    }
    void Init()
    {
        TryGetComponent<TetriUnitSimple>(out tetriUnitSimple);
        transform.Find("Display_Range").TryGetComponent<TetriDisplayRange>(out tetriDisplayRange);
        transform.TryGetComponent<TetriBuoySimple>(out tetriBuoySimple);
        
        if(Local())
        {
            Dispaly_UserWatchingFight();
            player = Player.NotReady;
            idelBox = null;
            player = Player;
            if(!IdelBox)return;
            IdelBox.OnTetriBeginDrag += Event_Display_OnBeginDrag;
            IdelBox.OnTetriEndDrag += Event_Display_OnEndDrag;
            IdelBox.OnTheCheckerboard += Event_Display_OnTheCheckerboard;
        }else
        {
            if(!isServer)return;
            Server_Init();
        }
        
    }
    public void Event_OnUserActionStateChanged(UserAction.State UserActionStateChanged)
    {
        switch (UserActionStateChanged)
        {
            case UserAction.State.CommandTheBattle_IdeaBox:
                Display_UserCommandTheBattle();
                break;
            case UserAction.State.CommandTheBattle_Buoy:
                Display_UserCommandTheBattle();
                break;
            case UserAction.State.WatchingFight:
                Dispaly_UserWatchingFight();
                break;
            case UserAction.State.Loading:
                Dispaly_UserLoading();
                break;
            default:
                break;
        }

    }
    [Server]
    public void Server_Event_OnUserActionStateChanged(UserAction.State UserActionStateChanged)
    {
        switch (UserActionStateChanged)
        {
            case UserAction.State.CommandTheBattle_IdeaBox:
                Server_Display_UserCommandTheBattle();
                break;
            case UserAction.State.CommandTheBattle_Buoy:
                Server_Display_UserCommandTheBattle();
                break;
            case UserAction.State.WatchingFight:
                Server_Dispaly_UserWatchingFight();
                break;
            case UserAction.State.Loading:
                Dispaly_UserLoading();
                break;
            default:
                break;
        }
    }
    void Event_Display_OnTheCheckerboard()
    {
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        transform.GetComponent<SpriteRenderer>().sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
        originSprite = transform.GetComponent<SpriteRenderer>().sprite;
        transform.GetComponent<SpriteRenderer>().sprite = Player == Player.Player1?Player1ActionBox:Player == Player.Player2 ? Player2ActionBox : null;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        SetSpriteAlpha(1.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
    }
    void Event_Display_OnBeginDrag()
    {
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_HideMorale();
    }
    
    void Event_Display_OnEndDrag()
    {
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        transform.GetComponent<SpriteRenderer>().sprite = originSprite;
        SetSpriteAlpha(0.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
    }
    
    void Display_UserCommandTheBattle()
    {
        if(!tetriUnitSimple)return;
        if(!TetriUnitSimple.TetrisBlockSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.SetUnitSortingOrderToFlow();
        bool isInIdeaBox = TetriUnitSimple.TetrisBlockSimple.transform.parent == null;
        // if(isInIdeaBox)return;
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserCommandTheBattle();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.FlowOrder+1);
            TetriDisplayRange.SetAlpha(1f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(1.0f);
        }
        
    }
    void Dispaly_UserWatchingFight()
    {
        if(!tetriUnitSimple)return;
        if(!tetriUnitSimple.TetrisBlockSimple)return;
        if(!tetriUnitSimple.HaveUnit)return;
        tetriUnitSimple.SetUnitSortingOrderToNormal();
        bool isInIdeaBox = TetriUnitSimple.TetrisBlockSimple.transform.parent == null;
        // if(isInIdeaBox)return;
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserWatchingFight();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.NotFlowOrder+1);
            TetriDisplayRange.SetAlpha(0f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }
    }
    void Dispaly_UserLoading()
    {
        
    }
    void SetCubeAlpha(float alpha)
    {
        if(!Cube)return;
        float cubeCommandTheBattle_Alpha = alpha;
        MeshRenderer cubeMeshRenderer = Cube.GetComponent<MeshRenderer>();
        Color cubeBaseColor = cubeMeshRenderer.sharedMaterial.color;
        cubeMeshRenderer.sharedMaterial.color = new Color(cubeBaseColor.r,cubeBaseColor.g,cubeBaseColor.b,cubeCommandTheBattle_Alpha);
    }
    void SetSpriteAlpha(float alpha)
    {
        float spriteCommandTheBattle_Alpha = alpha;
        SpriteRenderer tetriSpriteRenderer = transform.GetComponent<SpriteRenderer>();
        Color tetriBaseColor = tetriSpriteRenderer.color;
        tetriSpriteRenderer.color = new Color(tetriBaseColor.r,tetriBaseColor.g,tetriBaseColor.b,spriteCommandTheBattle_Alpha);
        tetriSpriteRenderer.sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [Server]
    void Server_Init()
    {
        
        player = Player.NotReady;
        idelBox = null;
        player = Player;
        if(!IdelBox)return;
        IdelBox.OnTetriBeginDrag += Server_Event_Display_OnBeginDrag;
        IdelBox.OnTetriEndDrag += Server_Event_Display_OnEndDrag;
        IdelBox.OnTheCheckerboard += Server_Event_Display_OnTheCheckerboard;
        Clinet_Init(idelBox.netId,player);
        Server_Dispaly_UserWatchingFight();
    }
    [Server]
    void Server_Display_UserCommandTheBattle()
    {
        if(!tetriUnitSimple)return;
        if(!TetriUnitSimple.TetrisBlockSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.SetUnitSortingOrderToFlow();
        bool isInIdeaBox = TetriUnitSimple.TetrisBlockSimple.transform.parent == null;
        // if(isInIdeaBox)return;
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserCommandTheBattle();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.FlowOrder+1);
            TetriDisplayRange.SetAlpha(1f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(1.0f);
        }
        Client_Display_UserCommandTheBattle(isInIdeaBox,Player);
    }
    [ClientRpc]
    void Client_Display_UserCommandTheBattle(bool isInIdeaBox,Player playerIn)
    {
        if(ServerLogic.Local_palayer != playerIn)
        {   
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserCommandTheBattle();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetAlpha(0f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
            return;
        }
        if(!TetriUnitSimple)return;
        TetriUnitSimple.SetUnitSortingOrderToFlow();
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserCommandTheBattle();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.FlowOrder+1);
            TetriDisplayRange.SetAlpha(1f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(1.0f);
        }
    }
    [Server]
    void Server_Dispaly_UserWatchingFight()
    {
        if(!tetriUnitSimple)return;
        if(!tetriUnitSimple.TetrisBlockSimple)return;
        if(!tetriUnitSimple.HaveUnit)return;
        tetriUnitSimple.SetUnitSortingOrderToNormal();
        bool isInIdeaBox = TetriUnitSimple.TetrisBlockSimple.transform.parent == null;
        // if(isInIdeaBox)return;
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserWatchingFight();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.NotFlowOrder+1);
            TetriDisplayRange.SetAlpha(0f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }
        Client_Dispaly_UserWatchingFight(isInIdeaBox,Player);
    }
    [ClientRpc]
    void Client_Dispaly_UserWatchingFight(bool isInIdeaBox,Player playerIn)
    {
        if(ServerLogic.Local_palayer != playerIn)
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserWatchingFight();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.NotFlowOrder+1);
            TetriDisplayRange.SetAlpha(0f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
            return;
        }
        if(!TetriUnitSimple)return;
        TetriUnitSimple.SetUnitSortingOrderToNormal();
        if(isInIdeaBox)
        {
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }else
        {
            if(!TetriUnitSimple)return;
            TetriUnitSimple.Display_UserWatchingFight();
            if(!TetriDisplayRange)return;
            TetriDisplayRange.SetSortingOrder(UC_PlayerData.Dispaly.NotFlowOrder+1);
            TetriDisplayRange.SetAlpha(0f);
            if(!TetriBuoySimple)return;
            TetriBuoySimple.Event_Display_Evaluate();
            SetCubeAlpha(0.0f);
            SetSpriteAlpha(0.0f);
        }
    }
    [Server]
    void Server_Event_Display_OnTheCheckerboard()
    {
        if(!tetriUnitSimple)return;
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        
        transform.GetComponent<SpriteRenderer>().sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
        originSprite = transform.GetComponent<SpriteRenderer>().sprite;
        transform.GetComponent<SpriteRenderer>().sprite = Player == Player.Player1?Player1ActionBox:Player == Player.Player2 ? Player2ActionBox : null;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        SetSpriteAlpha(1.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
        Clinet_Event_Display_OnTheCheckerboard(Player);
    }
    [ClientRpc]
    void Clinet_Event_Display_OnTheCheckerboard(Player playerIn)
    {
        if(ServerLogic.Local_palayer != playerIn)return;
        transform.GetComponent<SpriteRenderer>().sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
        originSprite = transform.GetComponent<SpriteRenderer>().sprite;
        transform.GetComponent<SpriteRenderer>().sprite = playerIn == Player.Player1?Player1ActionBox:playerIn == Player.Player2 ? Player2ActionBox : null;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        SetSpriteAlpha(1.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
    }
    [Server]
    void Server_Event_Display_OnEndDrag()
    {
        if(!tetriUnitSimple)return;
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        transform.GetComponent<SpriteRenderer>().sprite = originSprite;
        SetSpriteAlpha(0.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
        Clinet_Event_Display_OnEndDrag(Player);
    }
    [ClientRpc]
    void Clinet_Event_Display_OnEndDrag(Player playerIn)
    {
        if(ServerLogic.Local_palayer != playerIn)return;
        transform.GetComponent<SpriteRenderer>().sprite = originSprite;
        SetSpriteAlpha(0.0f);
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_ShowMorale();
    }
    [Server]
    void Server_Event_Display_OnBeginDrag()
    {
        if(!IdelBox)return;
        if(IdelBox.player != Player)return;
        if(!tetriUnitSimple)return;
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_HideMorale();
        Clinet_Event_Display_OnBeginDrag(Player);
    }
    [ClientRpc]
    void Clinet_Event_Display_OnBeginDrag(Player playerIn)
    {
        if(ServerLogic.Local_palayer != playerIn)return;
        if(!TetriUnitSimple)return;
        if(!TetriUnitSimple.HaveUnit)return;
        TetriUnitSimple.HaveUnit.Display_HideMorale();
    }
    [ClientRpc]
    void Clinet_Init(uint serverIdelBoxUint,Player serverPlayer)
    {
        Dispaly_UserWatchingFight();
        if(!idelBox)idelBox = FindObjectsOfType<IdelBox>().ToList().Find(idelBox => idelBox.netId == serverIdelBoxUint);
        if(!idelBox)return;
        idelBox.player = serverPlayer;
        player = serverPlayer;
    }
#endregion 联网数据操作
}