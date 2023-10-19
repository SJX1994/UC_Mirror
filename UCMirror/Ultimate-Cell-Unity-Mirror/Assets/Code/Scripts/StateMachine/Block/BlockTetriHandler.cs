using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UC_PlayerData;
public class BlockTetriHandler : MonoBehaviour
{
    float bugCheckerForOccupyingForever = 0;
    public enum BlockTetriState
    {
        Peace,
        Peace_Player1,
        Peace_Player2,
        Occupied_Player1,
        Occupied_Player2,
        Occupying,
    }
    public BlockTetriState state = BlockTetriState.Peace;
    public BlockTetriState State
    {
          get { return state; }
          set
          {
                if (value == state)return;
                state = value;
                BlockTetriStateChanged();
                OnBlockTetriStateChanged?.Invoke(posId,(int)state);
          }
    }
    bool NotReady = true;
    BlockDisplay blockDisplay;
    [HideInInspector]
    
    public Vector2 posId;
    public TetriBlockSimple tetriBlockSimpleHolder;
    [Header("联网")]
    public BlocksCreator_Main blocksCreator;
    public UnityAction<Vector2,int> OnBlockTetriStateChanged;
    // Start is called before the first frame update
    void Awake()
    {
        blockDisplay = GetComponent<BlockDisplay>();
        bugCheckerForOccupyingForever = UnitData.MaxOccupyingTime;
    }
    void Start()
    {
        State = BlockTetriState.Peace;
        NotReady = true;
        Invoke(nameof(LateStart), 0.1f);
    }
    void LateStart()
    {
        NotReady = false;
        posId = blockDisplay.posId;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(NotReady)return;
        // 状态机
        BlockTetriStateChanged();
    }
    void BlockTetriStateChanged()
    {
        if(!blockDisplay.SpriteRenderer_ExpressOccupation.gameObject.activeInHierarchy)blockDisplay.SpriteRenderer_ExpressOccupation.gameObject.SetActive(true);
        switch (State)
        {
            case BlockTetriState.Occupying:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Occupying);
                bugCheckerForOccupyingForever -= Time.deltaTime;
                if(bugCheckerForOccupyingForever > 0)return;
                Debug.Log("出现占领超时砖块:"+ posId +" _已重置");
                bugCheckerForOccupyingForever = UnitData.MaxOccupyingTime;
                State = BlockTetriState.Peace;
                break;
            case BlockTetriState.Occupied_Player1:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Occupied_Player1);
                break;
            case BlockTetriState.Occupied_Player2:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Occupied_Player2);
                break;
            case BlockTetriState.Peace:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Peace);
                break;
            case BlockTetriState.Peace_Player1:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Peace_Player1);
                break;
            case BlockTetriState.Peace_Player2:
                blockDisplay.SetColor_ExpressOccupation(BlockTetriState.Peace_Player2);
                break;
            default:
                break;
        }
    }
    public void Reset()
    {
        State = BlockTetriState.Peace;
        tetriBlockSimpleHolder = null;
    }
    // ----------------- 联网 -----------------
}
