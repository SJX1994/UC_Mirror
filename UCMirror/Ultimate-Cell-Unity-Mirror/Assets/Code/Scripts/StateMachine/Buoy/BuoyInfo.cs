using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;
public class BuoyInfo : MonoBehaviour
{
    public Player player;
    int palyerId;
    public LayerMask blockTargetMask;
    Input input;
    public BlockBuoyHandler blockBuoyHandler;
    public Vector2 currentPosID;
    // Start is called before the first frame update
    void Start()
    {
        if(player == Player.Player1)
        {
            palyerId = 0;
        }else
        {
            palyerId = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // 设置位置
        if (Input.GetMouseButtonDown(palyerId))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // CommunicationManager.OnBlocksMove(hit.point);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
            {
                if(hit.collider.transform.TryGetComponent(out BlockBuoyHandler block))
                {
                    transform.parent = hit.collider.transform.parent;
                    transform.localPosition = Vector3.zero;
                    transform.localScale = Vector3.one;
                    transform.localPosition = new Vector3( block.posId.x, 0f, block.posId.y);
                    transform.localRotation = Quaternion.Euler(Vector3.zero);
                    blockBuoyHandler = block; // 尝试移动
                    currentPosID = block.posId;
                }
            }
        }
    }
    
}
