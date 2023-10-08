using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlocksEffects : MonoBehaviour
{
    ParticleSystem blockAttention;
    public void LoadAttentionEffect(BlockDisplay blockDisplay, Player player = Player.NotReady)
    {
        if(!blockAttention)blockAttention = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttention = Instantiate(blockAttention,blockDisplay.transform);
        blockAttention.transform.localPosition = Vector3.zero;
        blockAttention.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttention.transform.localRotation = Quaternion.Euler(Vector3.zero);
        var main = blockAttention.main;
        switch (player)
        {
            case Player.NotReady:
                main.startColor = Color.white;
                break;
            case Player.Player1:
                main.startColor = Color.red;
                break;
            case Player.Player2:
                main.startColor = Color.blue;
                break;
        }
    }
    public void LoadAttentionEffect(BlockDisplay blockDisplay, PropsData.PropsState propsState = PropsData.PropsState.None)
    {
        if(!blockAttention)blockAttention = Resources.Load<ParticleSystem>("Effect/BlockEffect_attention");
        blockAttention = Instantiate(blockAttention,blockDisplay.transform);
        blockAttention.transform.localPosition = Vector3.zero;
        blockAttention.transform.localScale = Vector3.one + Vector3.one*0.3f;
        blockAttention.transform.localRotation = Quaternion.Euler(Vector3.zero);
        var main = blockAttention.main;
        switch (propsState)
        {
            case PropsData.PropsState.None:
                main.startColor = Color.white;
                break;
            case PropsData.PropsState.ChainBall:
                main.startColor = Color.yellow;
                break;
            case PropsData.PropsState.MoveDirectionChanger:
                main.startColor = Color.cyan;
                break;
            case PropsData.PropsState.Obstacle:
                main.startColor = Color.magenta;
                break;
        }
    }
}