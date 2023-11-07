using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
using Mirror;
public class BlocksEffects : NetworkBehaviour
{
#region 数据对象
    ParticleSystem blockAttention;
    ParticleSystem blockAttentionForUnit;
    public BlocksCreator_Main blocksCreator;
    public BlocksCreator_Main BlocksCreator { 
        get 
        {
            if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            return blocksCreator;
        }  
        set 
        {
            if(!value)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            blocksCreator = value;
        } 
    }
#endregion 数据对象
#region 数据关系
    void Start()
    {
        UIData.player1FloatingWordIDTemp = 0;
        UIData.player2FloatingWordIDTemp = 0;
        
    }
#endregion 数据关系
#region 数据操作
    public void LoadAttentionEffect(BlockDisplay blockDisplay, Player player = Player.NotReady,string floatingwordToShow = "增益",Color color32 = default)
    {
        Color colorIn = color32;
        blockAttentionForUnit = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttentionForUnit = Instantiate(blockAttentionForUnit,blockDisplay.transform);
        blockAttentionForUnit.transform.localPosition = Vector3.zero;
        blockAttentionForUnit.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttentionForUnit.transform.localRotation = Quaternion.Euler(Vector3.zero);
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().player = player;
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().Active();
        FloatingWord_FaceToCamera floatingWord_FaceToCamera = blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>();
        if(!floatingWord_FaceToCamera)return;
        floatingWord_FaceToCamera.TextMeshPro.fontSize = floatingWord_FaceToCamera.TextMeshPro.fontSize - 0.5f;
        floatingWord_FaceToCamera.TextMeshPro.fontStyle = TMPro.FontStyles.Normal;
        floatingWord_FaceToCamera.TextMeshPro.sortingOrder = 104;
        floatingWord_FaceToCamera.textToShow = floatingwordToShow;
        
        var main = blockAttentionForUnit.main;
        
        switch (player)
        {
            case Player.NotReady:
                color32 = Color.white;
                main.startColor = color32;
                break;
            case Player.Player1:
                color32 = Color.red;
                main.startColor = color32;
                break;
            case Player.Player2:
                color32 = Color.blue;
                main.startColor = color32;
                break;
        }
        if(colorIn != default)
        {
            main.startColor = colorIn;
            floatingWord_FaceToCamera.TextMeshPro.outlineColor = colorIn;
        }
        floatingWord_FaceToCamera.Active();
        Invoke(nameof(FloatingWordRecount),blockAttentionForUnit.main.duration/6f);
    }
    public void LoadAttentionEffect(BlockDisplay blockDisplay, PropsData.PropsState propsState = PropsData.PropsState.None,string floatingwordToShow = "...")
    {
        blockAttention = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttention = Instantiate(blockAttention,blockDisplay.transform);
        blockAttention.transform.localPosition = Vector3.zero;
        blockAttention.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttention.transform.localRotation = Quaternion.Euler(Vector3.zero);
        FloatingWord_FaceToCamera floatingWord_FaceToCamera = blockAttention.GetComponentInChildren<FloatingWord_FaceToCamera>();
        floatingWord_FaceToCamera.textToShow = floatingwordToShow;
        floatingWord_FaceToCamera.TextMeshPro.sortingOrder = 105;
        var main = blockAttention.main;
        Color color32 = new();
        switch (propsState)
        {
            case PropsData.PropsState.None:
                main.startColor = Color.white;
                break;
            case PropsData.PropsState.ChainBall:
                color32 = new Color32(0, 240, 255, 255);
                main.startColor = color32;
                break;
            case PropsData.PropsState.MoveDirectionChanger:
                color32 = new Color32(251, 139, 255, 255);
                main.startColor = color32;
                break;
            case PropsData.PropsState.Obstacle:
                color32 = new Color32(27, 255, 102, 255);
                main.startColor = color32;
                break;
        }
        floatingWord_FaceToCamera.TextMeshPro.outlineColor = color32;
        floatingWord_FaceToCamera.Active();
        Invoke(nameof(FloatingWordRecount),blockAttention.main.duration/6f);
    }
    void FloatingWordRecount()
    {
        UIData.player1FloatingWordIDTemp = 0;
        UIData.player2FloatingWordIDTemp = 0;
    }
#endregion 数据操作
#region 联网数据操作
    [Server]
    public void Server_LoadAttentionEffect(BlockDisplay blockDisplay, Player player = Player.NotReady,string floatingwordToShow = "增益",Color color32 = default)
    {
        Color colorIn = color32;
        blockAttentionForUnit = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttentionForUnit = Instantiate(blockAttentionForUnit,blockDisplay.transform);
        blockAttentionForUnit.transform.localPosition = Vector3.zero;
        blockAttentionForUnit.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttentionForUnit.transform.localRotation = Quaternion.Euler(Vector3.zero);
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().player = player;
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().Active();
        FloatingWord_FaceToCamera floatingWord_FaceToCamera = blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>();
        if(!floatingWord_FaceToCamera)return;
        floatingWord_FaceToCamera.TextMeshPro.fontSize = floatingWord_FaceToCamera.TextMeshPro.fontSize - 0.5f;
        floatingWord_FaceToCamera.TextMeshPro.fontStyle = TMPro.FontStyles.Normal;
        floatingWord_FaceToCamera.TextMeshPro.sortingOrder = 104;
        floatingWord_FaceToCamera.textToShow = floatingwordToShow;
        
        var main = blockAttentionForUnit.main;
        
        switch (player)
        {
            case Player.NotReady:
                color32 = Color.white;
                main.startColor = color32;
                break;
            case Player.Player1:
                color32 = Color.red;
                main.startColor = color32;
                break;
            case Player.Player2:
                color32 = Color.blue;
                main.startColor = color32;
                break;
        }
        if(colorIn != default)
        {
            color32 = colorIn;
            main.startColor = colorIn;
            floatingWord_FaceToCamera.TextMeshPro.outlineColor = colorIn;
        }
        floatingWord_FaceToCamera.Active();
        Invoke(nameof(FloatingWordRecount),blockAttentionForUnit.main.duration/6f);
        Client_LoadAttentionEffect(blockDisplay.posId,player,floatingwordToShow,color32);
    }
    [ClientRpc]
    public void Client_LoadAttentionEffect(Vector2 posId, Player player,string floatingwordToShow,Color color32)
    {
        BlockDisplay blockDisplay = BlocksCreator.blocks.Find((block) => block.posId == posId);
        if(!blockDisplay)return;
        Color colorIn = color32;
        blockAttentionForUnit = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttentionForUnit = Instantiate(blockAttentionForUnit,blockDisplay.transform);
        blockAttentionForUnit.transform.localPosition = Vector3.zero;
        blockAttentionForUnit.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttentionForUnit.transform.localRotation = Quaternion.Euler(Vector3.zero);
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().player = player;
        blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>().Active();
        FloatingWord_FaceToCamera floatingWord_FaceToCamera = blockAttentionForUnit.GetComponentInChildren<FloatingWord_FaceToCamera>();
        if(!floatingWord_FaceToCamera)return;
        floatingWord_FaceToCamera.TextMeshPro.fontSize = floatingWord_FaceToCamera.TextMeshPro.fontSize - 0.5f;
        floatingWord_FaceToCamera.TextMeshPro.fontStyle = TMPro.FontStyles.Normal;
        floatingWord_FaceToCamera.TextMeshPro.sortingOrder = 104;
        floatingWord_FaceToCamera.textToShow = floatingwordToShow;
        
        var main = blockAttentionForUnit.main;
        
        switch (player)
        {
            case Player.NotReady:
                color32 = Color.white;
                main.startColor = color32;
                break;
            case Player.Player1:
                color32 = Color.red;
                main.startColor = color32;
                break;
            case Player.Player2:
                color32 = Color.blue;
                main.startColor = color32;
                break;
        }
        if(colorIn != default)
        {
            main.startColor = colorIn;
            floatingWord_FaceToCamera.TextMeshPro.outlineColor = colorIn;
        }
        floatingWord_FaceToCamera.Active();
        Invoke(nameof(FloatingWordRecount),blockAttentionForUnit.main.duration/6f);
    }
    [Server]
    public void Server_LoadAttentionEffect(BlockDisplay blockDisplay, PropsData.PropsState propsState = PropsData.PropsState.None,string floatingwordToShow = "...")
    {
        blockAttention = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttention = Instantiate(blockAttention,blockDisplay.transform);
        blockAttention.transform.localPosition = Vector3.zero;
        blockAttention.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttention.transform.localRotation = Quaternion.Euler(Vector3.zero);
        FloatingWord_FaceToCamera floatingWord_FaceToCamera = blockAttention.GetComponentInChildren<FloatingWord_FaceToCamera>();
        floatingWord_FaceToCamera.textToShow = floatingwordToShow;
        floatingWord_FaceToCamera.TextMeshPro.sortingOrder = 105;
        var main = blockAttention.main;
        Color color32 = new();
        switch (propsState)
        {
            case PropsData.PropsState.None:
                main.startColor = Color.white;
                break;
            case PropsData.PropsState.ChainBall:
                color32 = new Color32(0, 240, 255, 255);
                main.startColor = color32;
                break;
            case PropsData.PropsState.MoveDirectionChanger:
                color32 = new Color32(251, 139, 255, 255);
                main.startColor = color32;
                break;
            case PropsData.PropsState.Obstacle:
                color32 = new Color32(27, 255, 102, 255);
                main.startColor = color32;
                break;
        }
        floatingWord_FaceToCamera.TextMeshPro.outlineColor = color32;
        floatingWord_FaceToCamera.Active();
        Invoke(nameof(FloatingWordRecount),blockAttention.main.duration/6f);
        Client_LoadAttentionEffect(blockDisplay.posId, propsState,floatingwordToShow);
    }
    [ClientRpc]
    public void Client_LoadAttentionEffect(Vector2 posId, PropsData.PropsState propsState, string floatingwordToShow)
    {
        BlockDisplay blockDisplay = BlocksCreator.blocks.Find((block) => block.posId == posId);
        BlocksCreator.GetComponent<BlocksEffects>().LoadAttentionEffect(blockDisplay, propsState, floatingwordToShow);
    }
#endregion 联网数据操作
}