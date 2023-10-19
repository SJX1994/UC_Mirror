using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UC_PlayerData;

public class BlockDisplay : MonoBehaviour
{
#region 数据对象
    public Vector2 posId = new();
    public Sprite sprite_player1_ExpressOccupation, sprite_player2_ExpressOccupation,sprite_Default_ExpressOccupation;
    public SpriteRenderer spriteRenderer_ExpressOccupation;
    public SpriteRenderer SpriteRenderer_ExpressOccupation{
        get
        {
            if(!spriteRenderer_ExpressOccupation)spriteRenderer_ExpressOccupation = transform.Find("Block_ExpressOccupation").GetComponent<SpriteRenderer>();
            return spriteRenderer_ExpressOccupation;
        }
        set
        {
            spriteRenderer_ExpressOccupation = value;
        }
    }
    public SpriteRenderer spriteRenderer_Weapon;
    public SpriteRenderer spriteRenderer_Faction;
    public Sprite sprite_Sword, sprite_Bow,sprite_Spear,sprite_Shield;
    public float finalHigh;
    [HideInInspector]
    public bool isOccupy = false;
    public Color blockColorDark = new Color(1,1,1,0);
    public SpriteRenderer spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay;
    public SpriteRenderer SpriteRenderer_ExpressUserCommanding_DataDisplay{
        get
        {
            if(!spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay)spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay = transform.GetComponent<SpriteRenderer>();
            return spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay;
        }
        set
        {
            spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay = value;
        }
    }
    List<SpriteRenderer> _rendererChilden = new();
    private BlockBuoyHandler blockBuoyHandler;
    public BlockBuoyHandler BlockBuoyHandler{
        get
        {
            if(!blockBuoyHandler)blockBuoyHandler = GetComponent<BlockBuoyHandler>();
            return blockBuoyHandler;
        }
    }
    #endregion 数据对象
    #region 数据关系
    void Start()
    {
        Init();
        spriteRenderer_ExpressOccupation = transform.Find("Block_ExpressOccupation").GetComponent<SpriteRenderer>();
        spriteRenderer_ExpressOccupation.gameObject.SetActive(true);
        if(!spriteRenderer_Weapon)spriteRenderer_Weapon = transform.Find("BlockWeapon").GetComponent<SpriteRenderer>();
        spriteRenderer_Weapon.gameObject.SetActive(false);
        if(!spriteRenderer_Faction)spriteRenderer_Faction = transform.Find("BlockFaction").GetComponent<SpriteRenderer>();
        spriteRenderer_Faction.gameObject.SetActive(false);
        spriteRenderer_ExpressUser_CommandTheBattleState_DataDisplay = transform.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        
    }
    public void InFlow()
    {
        SpriteRenderer_ExpressOccupation.sortingOrder = Dispaly.FlowOrder - 1;
    }
    public void OutFlow()
    {
        SpriteRenderer_ExpressOccupation.sortingOrder = Dispaly.NotFlowOrder;   
    }
#endregion 数据关系
#region 数据方法
    public void SetColor_ExpressOccupation(BlockTetriHandler.BlockTetriState OccupationState)
    {
        switch(OccupationState)
        {
            case BlockTetriHandler.BlockTetriState.Peace:
                SpriteRenderer_ExpressOccupation.sprite = sprite_Default_ExpressOccupation;
                SpriteRenderer_ExpressOccupation.color = Color.clear;
                Vector2 midOfBlocksY = new(0f,10f);
                Vector2 midOfBlocksX = new(9f,10f);
                if(posId.y >= midOfBlocksY.x && posId.y <= midOfBlocksY.y && posId.x >= midOfBlocksX.x && posId.x <= midOfBlocksX.y)
                {
                    SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
                }else if(posId.x < midOfBlocksX.x)
                {
                    GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Peace_Player1;
                }else if(posId.x > midOfBlocksX.y)
                {
                    GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Peace_Player2;
                }
            break;
            case BlockTetriHandler.BlockTetriState.Peace_Player1:
                SpriteRenderer_ExpressOccupation.sprite = sprite_player1_ExpressOccupation;
                SpriteRenderer_ExpressOccupation.color = Color.clear;
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
            case BlockTetriHandler.BlockTetriState.Peace_Player2:
                SpriteRenderer_ExpressOccupation.sprite = sprite_player2_ExpressOccupation;
                SpriteRenderer_ExpressOccupation.color = Color.clear;
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
            case BlockTetriHandler.BlockTetriState.Occupied_Player1:
                SpriteRenderer_ExpressOccupation.sprite = sprite_player1_ExpressOccupation;
                // SpriteRenderer_ExpressOccupation.color = new Color32(173,79,120,255);
                SpriteRenderer_ExpressOccupation.color = new Color32(255,255,255,155);
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
            case BlockTetriHandler.BlockTetriState.Occupied_Player2:
                SpriteRenderer_ExpressOccupation.sprite = sprite_player2_ExpressOccupation;
                // SpriteRenderer_ExpressOccupation.color = new Color32(53,121,179,255);
                SpriteRenderer_ExpressOccupation.color = new Color32(255,255,255,155);
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
            case BlockTetriHandler.BlockTetriState.Occupying:
                SpriteRenderer_ExpressOccupation.sprite = sprite_Default_ExpressOccupation;
                SpriteRenderer_ExpressOccupation.color = Color.yellow;
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
            default:
                SpriteRenderer_ExpressOccupation.sprite = sprite_Default_ExpressOccupation;
                SpriteRenderer_ExpressOccupation.color = Color.clear;
                SetColor_ExpressUser_CommandTheBattleState_DataDisplay(OccupationState);
            break;
        }
    }
    void SetColor_ExpressUser_CommandTheBattleState_DataDisplay(BlockTetriHandler.BlockTetriState commandTheBattleState)
    {
        switch(commandTheBattleState)
        {
            case BlockTetriHandler.BlockTetriState.Peace:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.cyan;
                Color originPeaceColor = SpriteRenderer_ExpressUserCommanding_DataDisplay.color;
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = new Color(originPeaceColor.r,originPeaceColor.g,originPeaceColor.b,BlocksData.BlocksAlpha_watchingFight);
            break;
            case BlockTetriHandler.BlockTetriState.Peace_Player1:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.red;
                Color originPeacePlayer1Color = SpriteRenderer_ExpressUserCommanding_DataDisplay.color;
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = new Color(originPeacePlayer1Color.r,originPeacePlayer1Color.g,originPeacePlayer1Color.b,BlocksData.BlocksAlpha_watchingFight);
            break;
            case BlockTetriHandler.BlockTetriState.Peace_Player2:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.blue;
                Color originPeacePlayer2Color = SpriteRenderer_ExpressUserCommanding_DataDisplay.color;
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = new Color(originPeacePlayer2Color.r,originPeacePlayer2Color.g,originPeacePlayer2Color.b,BlocksData.BlocksAlpha_watchingFight);
            break;
            case BlockTetriHandler.BlockTetriState.Occupied_Player1:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.clear;
            break;
            case BlockTetriHandler.BlockTetriState.Occupied_Player2:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.clear;
            break;
            case BlockTetriHandler.BlockTetriState.Occupying:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.clear;
            break;
            default:
                SpriteRenderer_ExpressUserCommanding_DataDisplay.color = Color.clear;
            break;
        }
    }
    void Init()
    {
        float initPosY = 0.0f;
        transform.position = new Vector3(transform.position.x, initPosY , transform.position.z);
    }
#endregion 数据方法
}
