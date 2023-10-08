using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BlockTetriHandler : MonoBehaviour
{
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
        if(!blockDisplay.spriteRenderer_Bright.gameObject.activeInHierarchy)blockDisplay.spriteRenderer_Bright.gameObject.SetActive(true);
        switch (State)
        {
            case BlockTetriState.Occupying:
                blockDisplay.spriteRenderer_Bright.color = Color.yellow;
                break;
            case BlockTetriState.Occupied_Player1:
                blockDisplay.spriteRenderer_Bright.color = Color.red;
                break;
            case BlockTetriState.Occupied_Player2:
                blockDisplay.spriteRenderer_Bright.color = Color.blue + Color.white * 0.3f;
                break;
            case BlockTetriState.Peace:
                blockDisplay.spriteRenderer_Bright.color = blockDisplay.blockColorDark;
                break;
            case BlockTetriState.Peace_Player1:
                blockDisplay.spriteRenderer_Bright.color = Color.red * 0.6f;
                break;
            case BlockTetriState.Peace_Player2:
                blockDisplay.spriteRenderer_Bright.color = Color.blue * 0.6f;
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
