using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTetriHandler : MonoBehaviour
{
    public enum BlockTetriState
    {
        Peace,
        Occupying,
        Occupied_Player1,
        Occupied_Player2,
    }
    private BlockTetriState state = BlockTetriState.Peace;
    public BlockTetriState State
    {
          get { return state; }
          set
          {
                if (value != state)
                {
                    state = value;
                    BlockTetriStateChanged();
                }
          }
    }
    bool NotReady = true;
    BlockDisplay blockDisplay;
    [HideInInspector]
    
    public Vector2 posId;
    public TetriBlockSimple tetriBlockSimpleHolder;
    // Start is called before the first frame update
    void Start()
    {
        State = BlockTetriState.Peace;
        NotReady = true;
        Invoke(nameof(LateStart), 0.1f);
    }
    void LateStart()
    {
        NotReady = false;
        blockDisplay = GetComponent<BlockDisplay>();
        posId = blockDisplay.posId;
    }

    // Update is called once per frame
    void Update()
    {
        if(NotReady)return;
        // 状态机
        BlockTetriStateChanged();
    }
    void BlockTetriStateChanged()
    {
        blockDisplay.spriteRenderer_Bright.gameObject.SetActive(true);
        switch (State)
        {
            case BlockTetriState.Occupying:
                
                blockDisplay.spriteRenderer_Bright.color = Color.yellow;
                break;
            case BlockTetriState.Occupied_Player1:
                
                blockDisplay.spriteRenderer_Bright.color = Color.red;
                break;
            case BlockTetriState.Occupied_Player2:
                blockDisplay.spriteRenderer_Bright.color = Color.blue;
                break;
            case BlockTetriState.Peace:
                blockDisplay.spriteRenderer_Bright.color = blockDisplay.blockColorDark;
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
}
