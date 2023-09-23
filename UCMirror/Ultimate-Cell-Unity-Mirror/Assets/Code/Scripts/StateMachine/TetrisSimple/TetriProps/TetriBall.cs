using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UC_PlayerData;
public class TetriBall : MonoBehaviour
{
    public Vector2 posId;
    public LayerMask blockTargetMask;
    public KeyValuePair<TetriBall, BlockBallHandler> tetriPairBlock = new();
    public UnityAction<TetriBall> OnTetriBallCollected;
    public BlocksCreator blocksCreator;
    Transform checker;


    void Start()
    {
        checker = transform.GetChild(0);    
    }
    bool Ray_TetriPairBlock()
    {
        // 发射射线向下进行检测
        if(!checker)checker = transform.GetChild(0);
        Ray ray = new Ray(checker.transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock) {return false;}
        // 进一步的处理
        BlockBallHandler block;
        hit.collider.transform.TryGetComponent(out block);
        tetriPairBlock = new(this, block);
        block.BlockPairTetri = new(block, this);
        posId = block.PosId;
        ResetRotation();
        return true;
    }
    // 看向摄像机
    public void ResetRotation()
    {
        Transform target = checker.Find("Display");
        target.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 directionToCamera = Camera.main.transform.position - target.position;
        directionToCamera.x = 0f;
        directionToCamera.z = -1f;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        target.rotation = rotationToCamera;
    }
    public bool Generate(Player turn)
    {
        if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator>();
        transform.SetParent(blocksCreator.transform);
        int width =  Random.Range(1,8); //blocksCreator.x-1);
        int height = turn == Player.Player1 ? Random.Range(6,9) : Random.Range(10,13); //blocksCreator.z-1);
        Vector2 checkId = new Vector2(height, width);
        var block = blocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple)
        {
            Generate(turn);
            return false;
        }
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        bool getDataSuccess = Ray_TetriPairBlock();
        return getDataSuccess;
    }
    public void Collect()
    {
        tetriPairBlock.Value.BlockPairTetri = new();
        tetriPairBlock = new();
        OnTetriBallCollected?.Invoke(this);
        Destroy(gameObject);
    }
}