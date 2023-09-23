using UnityEngine;
using System.Collections.Generic;
public class BlocksCounter : MonoBehaviour
{
    BlocksCreator blocksCreator;
    List<TetriBuoySimple> rowFullTetris = new();
    void Start()
    {

    }
    public void CheckFullRows()
    {
        if(!blocksCreator) blocksCreator = transform.GetComponent<BlocksCreator>();

        for (int row = 0; row < 20; row++)
        {
            bool isRowFull = true;
            if(rowFullTetris.Count > 0)rowFullTetris.Clear();
            for (int col = 0; col < 10; col++)
            {
                
                var block = blocksCreator.blocks.Find((block) => block.posId == new Vector2(row,col));
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
            haveUnit.Display_onFullRows();
        }
    }
    void DoFullRowsFail()
    {
        rowFullTetris.Clear();
    }
}