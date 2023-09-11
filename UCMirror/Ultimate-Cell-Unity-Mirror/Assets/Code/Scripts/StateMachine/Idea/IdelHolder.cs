using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UC_PlayerData;
using UnityEngine.Events;
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
    [Header("UC_PVP_IdelBox:")]
    [SyncVar]
    public int RamdomNumb;
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
        Debug.Log("OnStartClient"+playerPVP_local);
        Invoke(nameof(HideOther),0.1f);
    }
    void HideOther()
    {
        foreach(var idealHolder in FindObjectsOfType<IdelHolder>())
        {
            if(idealHolder.player == playerPVP)continue;
            idealHolder.idelUI.Hide();
            idealHolder.gameObject.SetActive(false);
            // DestroyImmediate(idealHolder.gameObject);
        }
        foreach(var tetrominoe in FindObjectsOfType<TetrisBlockSimple>())
        {
            if(!tetrominoe.idelBox)
            {
                tetrominoe.gameObject.SetActive(false);
                continue;
            }
            if(!tetrominoe.idelBox.idelHolder)
            {
                tetrominoe.gameObject.SetActive(false);
                continue;
            }
            if(tetrominoe.idelBox.idelHolder.player != playerPVP)
            {
                tetrominoe.gameObject.SetActive(false);
                continue;
            }
            
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
