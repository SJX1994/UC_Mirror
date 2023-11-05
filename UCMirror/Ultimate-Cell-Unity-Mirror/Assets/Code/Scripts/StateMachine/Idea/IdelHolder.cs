using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UC_PlayerData;
using DG.Tweening;
public struct IdelBoxData
{
    public string name;
    public int idelId;
}
public class IdelHolder : NetworkBehaviour
{
    public IdelUI idelUI;
    [SyncVar]
    public Player playerPVP_Temp;
    public Player playerPVP_local;
    public Player player;
    public static HashSet<IdelHolder> ActiveIdelHolders = new();
    [SyncVar]
    public int IdelHolderId = -1;
    public override void OnStartClient()
    {
        base.OnStartClient();
        
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    // Start is called before the first frame update
    void Awake()
    {
        if(Local())return;
        ActiveIdelHolders.Add(this);
    }
    protected virtual void OnDestroy()
    {
        ActiveIdelHolders.Remove(this);
        
    }
    void Start()
    {
        // Active();
        playerPVP_local = playerPVP_Temp;
        RecordId();
     
        if(idelUI)
        {
            idelUI.player = player;
            idelUI.idelHolder = this;
            return;
        }
        idelUI.Active();
        if(ServerLogic.Local_palayer != Player.NotReady)return;
        ServerLogic.Local_palayer = playerPVP_local;
    }
    void RecordId()
    {
        if(Local())return;
        IdelHolderId = ActiveIdelHolders.Count;
    }
    [ClientRpc]
    public void Client_HideOther()
    {
        ServerLogic.Local_palayer = playerPVP_local;
        bool thisNeedHide = playerPVP_local == player ? false : true;
        if(!thisNeedHide)return;
        idelUI.Hide();
        foreach(var tetrominoe in FindObjectsOfType<TetrisBlockSimple>())
        {
            if(tetrominoe.player == player)continue;
            tetrominoe.DisPlayOnline(false); 
        }
        
    }
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }

}
