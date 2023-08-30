using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriBlockSimple : MonoBehaviour
{
    TetrisBlockSimple tetrisBlockSimple;
    public PlayerData.Player player = PlayerData.Player.NotReady;
    public LayerMask blockTargetMask;
    public Vector2 posId;
    
    private bool canMove = false;
    public bool CanMove
    {
          get { return canMove; }
          set
          {
                if (value != canMove)
                {
                    canMove = value;
                    DoGroupMoveCheck();
                }
          }
    }
    BlockTetriHandler blockOccupying;
    // Start is called before the first frame update
    void Start()
    {
        tetrisBlockSimple = transform.parent.GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnTetrisMoveing += ()=>{ CanMove = false; };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Active()
    {
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
        {
            if(hit.collider.transform.TryGetComponent(out BlockTetriHandler block))
            {
                blockOccupying = block;
                posId = block.posId;
            }
        }
        
    }
    public void DoGroupMoveCheck()
    {
        // 占领
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        float raycastDistance = 100f; // 射线的长度

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
            {
                // 如果射线命中了对象
                // Debug.Log("射线命中了: " + hit.collider.gameObject.name);
                // Debug.DrawLine(ray.origin, hit.point, Color.red, 100f);
                // 进一步的处理
                if(hit.collider.transform.TryGetComponent(out BlockTetriHandler block))
                {
                    blockOccupying = block;
                    posId = block.posId;
                    if(player == PlayerData.Player.Player1)
                    {
                        if(block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player1)
                        {
                            block.State = BlockTetriHandler.BlockTetriState.Occupying;
                            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.1f);
                        }else
                        {
                            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.5f);
                        }
                        
                    }
                    else if(player == PlayerData.Player.Player2 )
                    {
                        if(block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player2)
                        {
                            block.State = BlockTetriHandler.BlockTetriState.Occupying;
                            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.1f);
                        }else
                        {
                            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.5f);
                        }
                        
                    }
                   
                    
                }
            }
        }
        else
        {
            // 如果射线没有命中任何对象
            Debug.Log("射线没有命中任何对象");
        }
    }
    void DoOccupied()
    {
        if(!blockOccupying)return;
        if(player == PlayerData.Player.Player1)
        {
            blockOccupying.State = BlockTetriHandler.BlockTetriState.Occupied_Player1;
        }
        else if(player == PlayerData.Player.Player2)
        {
            blockOccupying.State = BlockTetriHandler.BlockTetriState.Occupied_Player2;
        }
        CanMove = true;
    }
    
}
