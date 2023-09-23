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
    public Player playerPVP;
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
        Active();
        RecordId();
     
        if(idelUI)
        {
            idelUI.player = player;
            idelUI.idelHolder = this;
            return;
        }
        idelUI.Active();
        
    }
    void RecordId()
    {
        if(Local())return;
        IdelHolderId = ActiveIdelHolders.Count;
    }
  
  
    public void Active()
    {
        if(Local())return;
        if(isServer)return;
        playerPVP_local = playerPVP;
        Invoke(nameof(HideOther),0.1f);
    }
    void HideOther()
    {
        foreach(var idealHolder in FindObjectsOfType<IdelHolder>())
        {
            if(idealHolder.player == playerPVP_local)continue;
            idealHolder.idelUI.Hide();
        }
        foreach(var tetrominoe in FindObjectsOfType<TetrisBlockSimple>())
        {
            if(tetrominoe.player == playerPVP_local)continue;
            tetrominoe.DisPlayOnline(false); 
            
        }
        
    }
   
    /// <summary>
    /// 检查是否是本地模式
    /// </summary>
    /// <returns></returns>
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }

}
