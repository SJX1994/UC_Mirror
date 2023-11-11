using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using UnityEngine.UI;
public class AvatarUI_Buff : MonoBehaviour
{
    Sprite buff_ChainTransfer;
    public Sprite Buff_ChainTransfer
    {
        get
        {
            if(!buff_ChainTransfer)buff_ChainTransfer = Resources.Load<Sprite>("Buff/Buff_ChainTransfer");
            return buff_ChainTransfer;
        }
    }
    Sprite buff_FullRows;
    public Sprite Buff_FullRows
    {
        get
        {
            if(!buff_FullRows)buff_FullRows = Resources.Load<Sprite>("Buff/Buff_FullRows");
            return buff_FullRows;
        }
    }
    Sprite buff_MoraleAccumulationMaxed;
    public Sprite Buff_MoraleAccumulationMaxed
    {
        get
        {
            if(!buff_MoraleAccumulationMaxed)buff_MoraleAccumulationMaxed = Resources.Load<Sprite>("Buff/Buff_MoraleAccumulationMaxed");
            return buff_MoraleAccumulationMaxed;
        }
    }
    Sprite buff_WeakAssociation;
    public Sprite Buff_WeakAssociation
    {
        get
        {
            if(!buff_WeakAssociation)buff_WeakAssociation = Resources.Load<Sprite>("Buff/Buff_WeakAssociation");
            return buff_WeakAssociation;
        }
    }
    public enum Buff
    {
        ChainTransfer,
        FullRows,
        MoraleAccumulationMaxed,
        WeakAssociation
    }

    public void AddBuff(Buff buff)
    {
        string buffName = buff.ToString();
        Sprite imageSprite = null;
        switch(buff)
        {
            case Buff.ChainTransfer:
                imageSprite = Buff_ChainTransfer;
                break;
            case Buff.FullRows:
                imageSprite = Buff_FullRows;
                break;
            case Buff.MoraleAccumulationMaxed:
                imageSprite = Buff_MoraleAccumulationMaxed;
                break;
            case Buff.WeakAssociation:
                imageSprite = Buff_WeakAssociation;
                break;
        }
        GameObject imageObject = new GameObject("Buff_Image_" + buffName);
        Image imageComponent = imageObject.AddComponent<Image>();
        imageComponent.sprite = imageSprite;
        imageComponent.raycastTarget = false;
        imageObject.transform.SetParent(transform);
        Destroy(imageObject,3f);
    }
}
