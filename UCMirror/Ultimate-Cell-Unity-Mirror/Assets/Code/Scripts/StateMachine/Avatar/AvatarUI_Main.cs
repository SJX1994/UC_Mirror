using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using DG.Tweening;
using Mirror;
using UnityEngine.UI;
using System.Linq;
public class AvatarUI_Main : NetworkBehaviour
{
#region 数据对象
    BlocksCounter blocksCounter;
    public BlocksCounter BlocksCounter
    {
       get
        {
            if(!blocksCounter)blocksCounter = FindObjectOfType<BlocksCounter>();
            return blocksCounter;
        }
    }
    AvatarUI_Buff player1_Buff;
    public AvatarUI_Buff Player1_Buff
    {
        get
        {
            if(!player1_Buff)player1_Buff = Player1AvatarSet.GetComponentInChildren<AvatarUI_Buff>();
            return player1_Buff;
        }
    }
    AvatarUI_Buff player2_Buff;
    public AvatarUI_Buff Player2_Buff
    {
        get
        {
            if(!player2_Buff)player2_Buff = Player2AvatarSet.GetComponentInChildren<AvatarUI_Buff>();
            return player2_Buff;
        }
    }
    Tween player1MaxFadeAway;
    Tween player2MaxFadeAway;
    Transform player1AvatarSet;
    public Transform Player1AvatarSet
    {
        get
        {
            if(!player1AvatarSet)player1AvatarSet = transform.Find("UI_Panel_Player1");
            return player1AvatarSet;
        }
    }
    Transform player1BattleInfoSet;
    public Transform Player1BattleInfoSet
    {
        get
        {
            if(!player1BattleInfoSet)player1BattleInfoSet = Player1AvatarSet.Find("UI_Set_BattleInfo");
            return player1BattleInfoSet;
        }
    }
    AvatarSlider player1AvatarSlider;
    public AvatarSlider Player1AvatarSlider
    {
        get
        {
            if(!player1AvatarSlider)player1AvatarSlider = Player1BattleInfoSet.Find("UI_BattleInfoProgressBarFrame").GetComponent<AvatarSlider>();
            return player1AvatarSlider;
        }
    }
    Transform player2AvatarSet;
    public Transform Player2AvatarSet
    {
        get
        {
            if(!player2AvatarSet)player2AvatarSet = transform.Find("UI_Panel_Player2");
            return player2AvatarSet;
        }
    }
    Transform player2BattleInfoSet;
    public Transform Player2BattleInfoSet
    {
        get
        {
            if(!player2BattleInfoSet)player2BattleInfoSet = Player2AvatarSet.Find("UI_Set_BattleInfo");
            return player2BattleInfoSet;
        }
    }
    AvatarSlider player2AvatarSlider;
    public AvatarSlider Player2AvatarSlider
    {
        get
        {
            if(!player2AvatarSlider)player2AvatarSlider = Player2BattleInfoSet.Find("UI_BattleInfoProgressBarFrame").GetComponent<AvatarSlider>();
            return player2AvatarSlider;
        }
    }
#endregion 数据对象
#region 联网数据对象

#endregion 联网数据对象
#region 数据关系
    // Start is called before the first frame update
    void Start()
    {
        if(Local())
        {
            float localModeCreatDelay = 0.3f;
            Invoke(nameof(Init),localModeCreatDelay);
        }else
        {
            if(!isServer)return;
            float localModeCreatDelay = 0.3f;
            Invoke(nameof(Server_Init),localModeCreatDelay);
            
        }
        
    }
    void Server_Init()
    {
        
        UIData.Player1MoraleAccumulation = 0;
        UIData.Player2MoraleAccumulation = 0;
        UIData.player1isAdditioning = false;
        UIData.player2isAdditioning = false;
        // ServerLogic.OnServerLogicStart += Init;
        float localModeCreatDelay = 0.2f;
        Invoke(nameof(Init),localModeCreatDelay);
        
    }
    void Init()
    {
        UIData.OnPlayer1MoraleAccumulationMaxed += Event_MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed += Event_MoraleAccumulationMaxed;
        UIData.Player1MoraleAccumulation = 0;
        UIData.Player2MoraleAccumulation = 0;
        if(!BlocksCounter)return;
        BlocksCounter.OnPlayerNeedHelp_WeakAss += Event_WeakAssociationUI;
        BlocksCounter.OnPlayer_FullRows += Event_FullRowsUI;
        UnitData.OnUnitChainTransfer += Event_ChainTransfer;
        if(Local())return;
        Clinet_Init();
    }
    void OnDisable() {
        UIData.OnPlayer1MoraleAccumulationMaxed -= Event_MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed -= Event_MoraleAccumulationMaxed;
        if(!BlocksCounter)return;
        BlocksCounter.OnPlayerNeedHelp_WeakAss -= Event_WeakAssociationUI;
        BlocksCounter.OnPlayer_FullRows -= Event_FullRowsUI;
        UnitData.OnUnitChainTransfer -= Event_ChainTransfer;
        if(!Local())return;
        ServerLogic.OnServerLogicStart -= Init;
        ServerLogic.On_Local_palayer_ready -= HideOtherUI;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(Local())
        {
            Player2AvatarSlider.SetSliderValue(UIData.Player2MoraleAccumulation_Normalized);
            Player1AvatarSlider.SetSliderValue(UIData.Player1MoraleAccumulation_Normalized);
        }else
        {
            if(!isServer)return;
            float player1MoraleAccumulation = UIData.Player1MoraleAccumulation_Normalized;
            float player2MoraleAccumulation = UIData.Player2MoraleAccumulation_Normalized;
            Player2AvatarSlider.SetSliderValue(player2MoraleAccumulation);
            Player1AvatarSlider.SetSliderValue(player1MoraleAccumulation);
            Client_UpdateMoraleAccumulation(player1MoraleAccumulation,player2MoraleAccumulation);
        }
        
    }
#endregion 数据关系
#region 数据操作
    void Event_ChainTransfer(Player playerIn)
    {
        if(playerIn == Player.Player1)
        {
            Player1_Buff.AddBuff(AvatarUI_Buff.Buff.ChainTransfer);
        }
        if(playerIn == Player.Player2)
        {
            Player2_Buff.AddBuff(AvatarUI_Buff.Buff.ChainTransfer);
        }
        if(Local())return;
        Client_AddBuff(playerIn,AvatarUI_Buff.Buff.ChainTransfer);
    }
    void Event_FullRowsUI(Player playerIn)
    {
        if(playerIn == Player.Player1)
        {
            Player1_Buff.AddBuff(AvatarUI_Buff.Buff.FullRows);
        }
        if(playerIn == Player.Player2)
        {
            Player2_Buff.AddBuff(AvatarUI_Buff.Buff.FullRows);
        }
        if(Local())return;
        Client_AddBuff(playerIn,AvatarUI_Buff.Buff.FullRows);
    }
    void Event_WeakAssociationUI(Player playerIn)
    {
        if(playerIn == Player.Player1)
        {
            Player1_Buff.AddBuff(AvatarUI_Buff.Buff.WeakAssociation);
        }
        if(playerIn == Player.Player2)
        {
            Player2_Buff.AddBuff(AvatarUI_Buff.Buff.WeakAssociation);
        }
        if(Local())return;
        Client_AddBuff(playerIn,AvatarUI_Buff.Buff.WeakAssociation);
    }
    void Event_MoraleAccumulationMaxed(Player player)
    {
        if(player == Player.Player1)
        {
            Invoke(nameof(Player1MoraleAccumulationFinished),UIData.player1MoraleAccumulationAdditionContinuedTime);
            Player1AvatarSlider.ShakingAnimation(UIData.player1MoraleAccumulationAdditionContinuedTime);
            player1MaxFadeAway = DOVirtual.Float(UIData.MAX_MORALE, 0, UIData.player1MoraleAccumulationAdditionContinuedTime, 
            (TweenCallback<float>)((float value) =>
            {
                UIData.Player1MoraleAccumulation = value;
            }));
            Player1_Buff.AddBuff(AvatarUI_Buff.Buff.MoraleAccumulationMaxed);
            if(Local())return;
            Client_AddBuff(Player.Player1,AvatarUI_Buff.Buff.MoraleAccumulationMaxed);
        }else if(player == Player.Player2)
        {
            Invoke(nameof(Player2MoraleAccumulationFinished),UIData.player2MoraleAccumulationAdditionContinuedTime);
            Player2AvatarSlider.ShakingAnimation(UIData.player2MoraleAccumulationAdditionContinuedTime);
            player2MaxFadeAway = DOVirtual.Float(UIData.MAX_MORALE, 0, UIData.player2MoraleAccumulationAdditionContinuedTime, 
            (TweenCallback<float>)((float value) =>
            {
                UIData.Player2MoraleAccumulation = value;
            }));
            Player2_Buff.AddBuff(AvatarUI_Buff.Buff.MoraleAccumulationMaxed);
            if(Local())return;
            Client_AddBuff(Player.Player2,AvatarUI_Buff.Buff.MoraleAccumulationMaxed);
        }
    }
    void Player1MoraleAccumulationFinished()
    {
        if(player1MaxFadeAway!=null)
        {
            player1MaxFadeAway.Kill();
            player1MaxFadeAway = null;
        }
        UIData.Player1MoraleAccumulation = 0;
        UIData.OnPlayer1MoraleAccumulationAdditionFinished?.Invoke(Player.Player1);
        UIData.player1isAdditioning = false;
    }
    void Player2MoraleAccumulationFinished()
    {
        if(player2MaxFadeAway!=null)
        {
            player2MaxFadeAway?.Kill();
            player2MaxFadeAway = null;
        }
        UIData.Player2MoraleAccumulation = 0;
        UIData.OnPlayer2MoraleAccumulationAdditionFinished?.Invoke(Player.Player2);
        UIData.player2isAdditioning = false;
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [ClientRpc]
    void Clinet_Init()
    {
        UIData.Player1MoraleAccumulation = 0;
        UIData.Player2MoraleAccumulation = 0;
        UIData.player1isAdditioning = false;
        UIData.player2isAdditioning = false;
        ServerLogic.On_Local_palayer_ready += HideOtherUI;
    }
    void HideOtherUI()
    {
        if(ServerLogic.Local_palayer == Player.Player1)
        {
            player2AvatarSet = transform.Find("UI_Panel_Player2");
            if(!player2AvatarSet)return;
            player2AvatarSet.gameObject.SetActive(false);

            ServerLogic.On_Local_palayer_ready -= HideOtherUI;
        }
        else if(ServerLogic.Local_palayer == Player.Player2)
        {
            player1AvatarSet = transform.Find("UI_Panel_Player1");
            if(!player1AvatarSet)return;
            player1AvatarSet.gameObject.SetActive(false);

            ServerLogic.On_Local_palayer_ready -= HideOtherUI;
        }
    }
    [ClientRpc]
    void Client_AddBuff(Player playerIn,AvatarUI_Buff.Buff buffType)
    {
        if(ServerLogic.Local_palayer != playerIn)return;

        if(playerIn == Player.Player1)
        {
            Player1_Buff.AddBuff(buffType);
        }
        if(playerIn == Player.Player2)
        {
            Player2_Buff.AddBuff(buffType);
        }
    }
    
    [ClientRpc]
    public void Client_UpdateMoraleAccumulation(float player1MoraleAccumulation,float player2MoraleAccumulation)
    {
        Player2AvatarSlider.SetSliderValue(player2MoraleAccumulation);
        Player1AvatarSlider.SetSliderValue(player1MoraleAccumulation);
    }
#endregion 联网数据操作
}
