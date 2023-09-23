using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System;
using System.Linq;

public class TetrisUnitSimple : MonoBehaviour
{
    public List<TetriUnitSimple> tetriUnits = new();
    public Dictionary<TetriUnitSimple,UnitSimple> TU_pair = new();
    private bool newTetrisUnit = true;
    public bool NewTetrisUnit
    {
        get
        {
            return newTetrisUnit;
        }
        set
        {
            if(newTetrisUnit == value)return;
            if(tetriUnits.Count == 0)Start();
            foreach(var tetriUnit in tetriUnits)
            {
                tetriUnit.newTetriUnit = value;
            }
            newTetrisUnit = value;
        }
    }
    private int initProcess = 0;
    public int InitProcess
    {
        get
        {
            return initProcess;
        }
        set
        {
            initProcess = value;
            if(initProcess != 4)return;
            OnUnitInitFinish();
            initProcess = 0;
        }
    }
    void Start()
    {
        foreach (Transform child in transform)
        {
            TetriUnitSimple tetriUnitSimple= child.GetComponent<TetriUnitSimple>();
            tetriUnits.Add(tetriUnitSimple);
            tetriUnitSimple.tetrisUnitSimple = this;
        }
    }
    void OnUnitInitFinish()
    {
        TU_pair = new();
        foreach(var tetriUnit in tetriUnits)
        {
            UnitSimple unitSimple = tetriUnit.haveUnit;
            if(TU_pair.ContainsKey(tetriUnit))continue;
            TU_pair.Add(tetriUnit,unitSimple);
        }
    }
    public List<KeyValuePair<int, UnitData.Color>> GetUnitsData()
    {
        List<KeyValuePair<int, UnitData.Color>> indexPairColors = new();
        foreach(var tetriUnit in tetriUnits)
        {
            indexPairColors.Add(tetriUnit.indexPairColor);
        }
        return indexPairColors;
    }
    public void LoadUnits(List<KeyValuePair<int, UnitData.Color>> indexPairColors)
    {
        foreach(var tetriUnit in tetriUnits)
        {
            if(!tetriUnit)continue;
            KeyValuePair<int, UnitData.Color> indexPairColor = indexPairColors.Find(x => x.Key == tetriUnit.tetriUnitIndex);
            tetriUnit.LoadUnit(indexPairColor);
        }
    }
    public void FailToCreat()
    {
        foreach(var tetriUnit in tetriUnits)
        {
            if(!tetriUnit)continue;
            tetriUnit.FailToCreat();
        }
    }
    public void CheckUnitTag(bool needFight = true)
    {
        if(tetriUnits.Count == 0)Start();
        if(!transform.parent)
        {
            foreach (var tetriUnit in tetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(false);
            }
            return;
        }
        if(needFight || transform.parent.TryGetComponent(out BlocksCreator tetrisBlockSimple))
        {
            foreach (var tetriUnit in tetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(true);
            }
        }else
        {
            foreach (var tetriUnit in tetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(false);
            }
            
        }
        
    }
    public void UnitActionInit()
    {
        foreach(var tetriUnit in tetriUnits)
        {
            if(!tetriUnit)continue;
            // 或许要拆开
            tetriUnit.haveUnit.OnTetrisMoveing();
        }
    }
    
}