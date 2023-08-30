using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;
using System.Linq;
using UnityEngine.Events;
public class TetrisBlockSimple : MonoBehaviour
{
    public float occupyingTime = 3f;
    public Player player = Player.NotReady;
    public Vector3 rotationPoint;
    public List<TetriBlockSimple> pioneerBlocks;
    public UnityAction OnTetrisMoveing;
    public BlocksCreator blocksCreator;
    void Start()
    {
       occupyingTime = 3f;
    }
    public void Active()
    {
        blocksCreator = transform.parent.GetComponent<BlocksCreator>();
        foreach (Transform child in transform)
        {
            child.GetComponent<TetriBlockSimple>().player = player;
            child.GetComponent<TetriBlockSimple>().Active();
            child.GetComponent<TetriBlockSimple>().DoGroupMoveCheck();
        }
        EvaluatePioneers();
        InvokeRepeating(nameof(Move),0,occupyingTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void EvaluatePioneers()
    {
         // 取最前方计算碰撞的砖块
        Dictionary<Transform, float> posDictionary = new();

        foreach (Transform child in transform)
        {
            posDictionary.Add(child,child.position.x);
        }
        float[] xPos = posDictionary.Values.ToArray();

        // 获取最大值
        float maxValue = Mathf.Max(xPos);

        // 获取最小值
        float minValue = Mathf.Min(xPos);
        List<Transform> keys = new();

        if(player == Player.Player1)
        {
            keys = posDictionary.Where(x => x.Value == maxValue)
                                        .Select(x => x.Key)
                                        .ToList();
        }
        else if(player == Player.Player2)
        {
            keys = posDictionary.Where(x => x.Value == minValue)
                                        .Select(x => x.Key)
                                        .ToList();
        }

        pioneerBlocks = keys.Select(x => x.GetComponent<TetriBlockSimple>()).ToList();

        // foreach(var pioneerBlock in pioneerBlocks)
        // {
        //     pioneerBlock.transform.localScale += 1.5f * Vector3.one;
        // }
    }
    void Move()
    {
        if(!ValidMove())return;
        
        foreach (Transform child in transform)
        {
            child.GetComponent<TetriBlockSimple>().DoGroupMoveCheck();
        }
        if(player == Player.Player1)
        {
            transform.localPosition += new Vector3(1,0,0);
        }
        else if(player == Player.Player2)
        {
            transform.localPosition += new Vector3(-1,0,0);
        }

        
        
    }
    void Rotate()
    {
       transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
    }
    bool ValidMove()
    {
        foreach(var pineer in pioneerBlocks)
        {
            BlockDisplay block = null;
            if(player == Player.Player1)
            {
                block = blocksCreator.blocks.Find((block) => block.posId == new Vector2(pineer.posId.x + 2,pineer.posId.y));
            }else if(player == Player.Player2)
            {
                block = blocksCreator.blocks.Find((block) => block.posId == new Vector2(pineer.posId.x - 2,pineer.posId.y));
            }
            if(block == null)
            {
                return false;
            }
            BlockTetriHandler blockTetriHandler = block.GetComponent<BlockTetriHandler>();
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupied_Player2 && player == Player.Player1)
            {
                return false;
            }
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupied_Player1 && player == Player.Player2)
            {
                return false;
            }
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupying)
            {
                return false;
            }
            if(!pineer.CanMove)
            {
                return false;
            }
        }
        // foreach (Transform child in transform)
        // {
        //     if(!child.GetComponent<TetriBlockSimple>().CanMove)
        //     {
        //         return false;
        //     }
        // }
        OnTetrisMoveing?.Invoke();
        return true;
    }
}

