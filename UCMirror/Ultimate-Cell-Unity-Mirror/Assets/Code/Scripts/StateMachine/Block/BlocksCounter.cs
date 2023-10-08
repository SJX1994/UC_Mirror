using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
using System.Linq;
using System.Collections;
public class BlocksCounter : MonoBehaviour
{
    BlocksCreator_Main blocksCreator;
    public BlocksCreator_Main BlocksCreator
    {
        get
        {
            if(!blocksCreator)blocksCreator = GetComponent<BlocksCreator_Main>();
            return blocksCreator;
        }
    }
    List<TetriBuoySimple> rowFullTetris = new();
    private BlocksEffects blocksEffects;
    public BlocksEffects BlocksEffects
    {
        get
        {
            if(!blocksEffects)blocksEffects = FindObjectOfType<BlocksEffects>();
            return blocksEffects;
        }
    }
    int P1BlocksNumb;
    int P2BlocksNumb;
    List<SoldierBehaviors> weakAssociationSoldiers = new();
    Player playerForAssociation = Player.NotReady;
    private object lockObject = new object(); // 用于锁的对象
    
    void Start()
    {
        BlocksData.OnPlayer1BlocksNumbChange += GetP1BlockNumb;
        BlocksData.OnPlayer2BlocksNumbChange += GetP2BlockNumb;    
    }
    // 获取颜色砖块
    public List<BlockTetriHandler> GetOccupiedP1Blocks()
    {
        return GetBlocks(BlockTetriHandler.BlockTetriState.Occupied_Player1);
    }
    public List<BlockTetriHandler> GetOccupiedP2Blocks()
    {
        return GetBlocks(BlockTetriHandler.BlockTetriState.Occupied_Player2);
    }
    public List<BlockTetriHandler> GetPeaceP1Blocks()
    {
        return GetBlocks(BlockTetriHandler.BlockTetriState.Peace_Player1);
    }
    public List<BlockTetriHandler> GetPeaceP2Blocks()
    {
        return GetBlocks(BlockTetriHandler.BlockTetriState.Peace_Player2);
    }
    List<BlockTetriHandler> GetBlocks(BlockTetriHandler.BlockTetriState state)
    {
        List<BlockTetriHandler> blocks = new();
        BlocksCreator.blocks.ForEach((block) => 
        {
            var blockTetriHandler = block.GetComponent<BlockTetriHandler>();
            if(blockTetriHandler.State != state)return;
            if(!blockTetriHandler)return;
            blocks.Add(blockTetriHandler);
        });
        return blocks;
    }
    // 获取砖块变化
    public void GetP1BlockNumb(int P1numb)
    {
        P1BlocksNumb = P1numb;
        
    }
    public void GetP2BlockNumb(int P2numb)
    {
        P2BlocksNumb = P2numb;
    }
    public void OnStateChange()
    {
        lock(lockObject){DoWeakAssociationCount();}
    }
    // 弱势关联计算
    public void DoWeakAssociationCount()
    {
        if(BlocksData.stopEventSend == true)return;
        float ratio = (float) P1BlocksNumb / P2BlocksNumb;
        if (Mathf.Approximately(ratio, 1f / 2f))
        {
            // 触发事件
            Debug.Log("P1需要帮助");
            playerForAssociation = Player.Player1;
        }else if(Mathf.Approximately(ratio, 2f))
        {
            // 触发事件
            Debug.Log("P2需要帮助");
            playerForAssociation = Player.Player2;
        }
        AllSoldiers(playerForAssociation);
        playerForAssociation = Player.NotReady;
        // Debug.Log("P1/P2 = " + ratio + "COUNT:" + weakAssociationSoldiers.Count);
        if(weakAssociationSoldiers.Count == 0)return;
        foreach (var soldier in weakAssociationSoldiers)
        {
            if(!soldier.WeakAssociation.Self) soldier.WeakAssociation.Start();
            soldier.WeakAssociation.soldiers = weakAssociationSoldiers;
            soldier.WeakAssociation.Active();
        }
        StartCoroutine(StopWeakAssociation(3f));
    }
    IEnumerator StopWeakAssociation(float waitTime)
    {
        BlocksData.stopEventSend = true;
        BlocksData.OnPlayer1BlocksNumbChange -= GetP1BlockNumb;
        BlocksData.OnPlayer2BlocksNumbChange -= GetP2BlockNumb;
        yield return new WaitForSeconds(waitTime);
        foreach (var soldier in weakAssociationSoldiers)
        {
            if(!soldier)continue;
            soldier.WeakAssociation.Stop();
        }
        weakAssociationSoldiers.Clear();
        BlocksData.stopEventSend = false;
        BlocksData.OnPlayer1BlocksNumbChange += GetP1BlockNumb;
        BlocksData.OnPlayer2BlocksNumbChange += GetP2BlockNumb;
    }
    void AllSoldiers(Player playerIn = Player.NotReady)
    {
        if(playerIn == Player.NotReady)return;
          this.weakAssociationSoldiers = new(FindObjectsOfType<SoldierBehaviors>().Where(x=>AllSoldiersChecker(x,playerIn)).ToList());
    }
    bool AllSoldiersChecker(SoldierBehaviors soldier,Player playerIn = Player.NotReady)
    {
          List<bool> condition = new();
          if(!soldier)return false;
          if(soldier.UnitSimple.IsDeadOrNull(soldier.UnitSimple))return false;
          // 不包含培养皿中的砖块
          Transform p = soldier.UnitSimple.tetriUnitSimple.TetrisBlockSimple.transform.parent;
          if(p==null)return false;
          // 不包含同类玩家
          bool player = soldier.Player == playerIn;
          condition.Add(player);
          // 不包含拖拽的临时士兵
          bool draging = !soldier.UnitSimple.tetriUnitSimple.TetrisBlockSimple.name.Contains(UnitData.Temp);
          condition.Add(draging);
          bool allTrue = condition.All(b => b);
          return allTrue;
    }
    // 底线事件计算
    public void DoReachBottomLineGain(Vector2 posId)
    {
        Player player = Player.NotReady;
        if(posId.x == 0)
        {
            player = Player.Player2;
            Debug.Log("Player2 到达底线 触达ID" + posId);
        }else if(posId.x == 19)
        {
            player = Player.Player1;
            Debug.Log("Player1 到达底线 触达ID" + posId);
        }
        for (int col = 0; col < 10; col++)
        {
            if(col != posId.y)continue;
            for (int row = 0; row < 20; row++)
            {
                var block = BlocksCreator.blocks.Find((block) => block.posId == new Vector2(row,col));
                BlocksEffects.LoadAttentionEffect(block,player);
                TetriBuoySimple tetriBuoy = block.BlockBuoyHandler.tetriBuoySimple;
                if(!tetriBuoy)continue;
                tetriBuoy.TetriBlockSimple.TetriUnitSimple.haveUnit.BlocksMechanismDoing(BlocksData.BlocksMechanismType.ReachBottomLineGain);
            }
        }
    }
    // 整行增益计算
    public void CheckFullRows()
    {
        for (int row = 0; row < 20; row++)
        {
            bool isRowFull = true;
            if(rowFullTetris.Count > 0)rowFullTetris.Clear();
            for (int col = 0; col < 10; col++)
            {
                
                var block = BlocksCreator.blocks.Find((block) => block.posId == new Vector2(row,col));
                if(!block)Debug.LogError("block is null:" + col + " " + row);
                var tetriBuoy = block.GetComponent<BlockBuoyHandler>().tetriBuoySimple;
                if(tetriBuoy)
                {
                    rowFullTetris.Add(tetriBuoy); 
                    continue;
                }
                isRowFull = false;
                DoFullRowsFail();
                break;
            }
            if (isRowFull)
            {
                Debug.Log("Row " + row + " is full.~~!!!!");
                DoFullRowsSuccess();
            }
        }
    }
    void DoFullRowsSuccess()
    {
        foreach (var tetri in rowFullTetris)
        {
            if(!tetri)continue;
            var tetriBlock = tetri.tetriBlockSimple;
            if(!tetriBlock)continue;
            var haveUnit = tetriBlock.TetriUnitSimple.haveUnit;
            if(!haveUnit)continue;
            haveUnit.BlocksMechanismDoing(BlocksData.BlocksMechanismType.FullRows);
        }
    }
    void DoFullRowsFail()
    {
        rowFullTetris.Clear();
    }
}