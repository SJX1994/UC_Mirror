using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using DG.Tweening;
using Mirror;
public class AvatarUI_Main : NetworkBehaviour
{
#region 数据对象
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
        float localModeCreatDelay = 0.1f;
        Invoke(nameof(Init),localModeCreatDelay);
    }
    void Init()
    {
        UIData.OnPlayer1MoraleAccumulationMaxed += MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed += MoraleAccumulationMaxed;
        UIData.Player1MoraleAccumulation = 0;
        UIData.Player2MoraleAccumulation = 0;
    }
    void OnDisable() {
        UIData.OnPlayer1MoraleAccumulationMaxed -= MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed -= MoraleAccumulationMaxed;
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
    void MoraleAccumulationMaxed(Player player)
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
        }else if(player == Player.Player2)
        {
            Invoke(nameof(Player2MoraleAccumulationFinished),UIData.player2MoraleAccumulationAdditionContinuedTime);
            Player2AvatarSlider.ShakingAnimation(UIData.player2MoraleAccumulationAdditionContinuedTime);
            player2MaxFadeAway = DOVirtual.Float(UIData.MAX_MORALE, 0, UIData.player2MoraleAccumulationAdditionContinuedTime, 
            (TweenCallback<float>)((float value) =>
            {
                UIData.Player2MoraleAccumulation = value;
            }));
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
    public void Client_UpdateMoraleAccumulation(float player1MoraleAccumulation,float player2MoraleAccumulation)
    {
        Player2AvatarSlider.SetSliderValue(player2MoraleAccumulation);
        Player1AvatarSlider.SetSliderValue(player1MoraleAccumulation);
    }
#endregion 联网数据操作
}
