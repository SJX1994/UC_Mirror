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
        blockAttention.transform.localScale = Vector3.one;
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
}