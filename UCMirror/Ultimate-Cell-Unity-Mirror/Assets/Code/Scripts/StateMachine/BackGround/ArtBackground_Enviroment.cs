using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UC_PlayerData;
public class ArtBackground_Enviroment : MonoBehaviour
{
    RectTransform env_Ground;
    public RectTransform Env_Ground
    {
        get
        {
            if(env_Ground == null)
            {
                env_Ground = transform.Find("WorldCanvas").Find("Env_Ground").GetComponent<RectTransform>();
            }
            return env_Ground;
        }
    }
    void Start()
    {
        ServerLogic.On_Local_palayer_ready += Event_On_Local_palayer_ready;
    }
    void OnDisable()
    {
        ServerLogic.On_Local_palayer_ready -= Event_On_Local_palayer_ready;
    }
    void Event_On_Local_palayer_ready()
    {
        float offect = 92f;
        float direction = 1f;
        if(ServerLogic.Local_palayer == Player.Player1)
        {
            direction = 1;
        }else if(ServerLogic.Local_palayer == Player.Player2)
        {
            direction = -1;
        }
        offect *= direction;
        Env_Ground.localPosition = new Vector3(Env_Ground.localPosition.x + offect ,Env_Ground.localPosition.y,Env_Ground.localPosition.z);
    }
}
