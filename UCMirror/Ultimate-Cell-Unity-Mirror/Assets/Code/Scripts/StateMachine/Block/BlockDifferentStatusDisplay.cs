using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UC_PlayerData;

public class BlockDifferentStatusDisplay : MonoBehaviour
{
    BlockDisplay blockDisplay;
    BlockDisplay BlockDisplay
    {
        get
        {
            if (!blockDisplay)blockDisplay = transform.GetComponent<BlockDisplay>();
            return blockDisplay;
        }
    }
    void Start()
    {
        UserAction.OnPlayer1UserActionStateChanged += Event_OnUserActionStateChanged;
        UserAction.OnPlayer2UserActionStateChanged += Event_OnUserActionStateChanged;
        Dispaly_UserWatchingFight();
    }
    void OnDisable()
    {
        UserAction.OnPlayer1UserActionStateChanged -= Event_OnUserActionStateChanged;
        UserAction.OnPlayer2UserActionStateChanged -= Event_OnUserActionStateChanged;
    }
    void Event_OnUserActionStateChanged(UserAction.State UserActionStateChanged)
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
    void Dispaly_UserWatchingFight()
    {
        float userWatchingFightAlpha_notOccupie = 0.0f;
        BlocksData.BlocksAlpha_watchingFight = userWatchingFightAlpha_notOccupie;
        // float userWatchingFightAlpha_Occupie = 0.0f;
        if(!BlockDisplay)return;
        Color originColor = BlockDisplay.SpriteRenderer_ExpressUserCommanding_DataDisplay.color;
        BlockDisplay.SpriteRenderer_ExpressUserCommanding_DataDisplay.color = new Color(originColor.r,originColor.g,originColor.b,userWatchingFightAlpha_notOccupie);
        // Color originColorBright = BlockDisplay.spriteRenderer_Bright.color;
        // BlockDisplay.spriteRenderer_Bright.color = new Color(originColorBright.r,originColorBright.g,originColorBright.b,userWatchingFightAlpha_Occupie);
    }
    void Display_UserCommandTheBattle()
    {
        float userUserCommandTheBattle_notOccupie = 0.5f;
        BlocksData.BlocksAlpha_watchingFight = userUserCommandTheBattle_notOccupie;
        // float userUserCommandTheBattle_Occupie = 1.0f;
        if(!BlockDisplay)return;
        Color originColor = BlockDisplay.SpriteRenderer_ExpressUserCommanding_DataDisplay.color;
        BlockDisplay.SpriteRenderer_ExpressUserCommanding_DataDisplay.color = new Color(originColor.r,originColor.g,originColor.b,userUserCommandTheBattle_notOccupie);
        // Color originColorBright = BlockDisplay.spriteRenderer_Bright.color;
        // BlockDisplay.spriteRenderer_Bright.color = new Color(originColorBright.r,originColorBright.g,originColorBright.b,userUserCommandTheBattle_Occupie);
    }
    void Dispaly_UserLoading()
    {
        
    }
}