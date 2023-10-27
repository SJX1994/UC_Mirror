using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System;
using System.Linq;

public class TetrisUnitSimple : MonoBehaviour
{
    public List<TetriUnitSimple> tetriUnits = new();
    public List<TetriUnitSimple> TetriUnits
    {
        get
        {
            if(tetriUnits.Count != 0)return tetriUnits;
            foreach (Transform child in transform)
            {
                TetriUnitSimple tetriUnitSimple= child.GetComponent<TetriUnitSimple>();
                tetriUnits.Add(tetriUnitSimple);
                tetriUnitSimple.tetrisUnitSimple = this;
            }
            return tetriUnits;
        }
    }
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
            foreach(var tetriUnit in TetriUnits)
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
        foreach(var tetriUnit in TetriUnits)
        {
            UnitSimple unitSimple = tetriUnit.HaveUnit;
            if(TU_pair.ContainsKey(tetriUnit))continue;
            TU_pair.Add(tetriUnit,unitSimple);
        }
    }
    public List<KeyValuePair<int, UnitData.Color>> GetUnitsData()
    {
        List<KeyValuePair<int, UnitData.Color>> indexPairColors = new();
        foreach(var tetriUnit in TetriUnits)
        {
            indexPairColors.Add(tetriUnit.indexPairColor);
        }
        return indexPairColors;
    }
    public void LoadUnits(List<KeyValuePair<int, UnitData.Color>> indexPairColors)
    {
        foreach(var tetriUnit in TetriUnits)
        {
            if(!tetriUnit)continue;
            KeyValuePair<int, UnitData.Color> indexPairColor = indexPairColors.Find(x => x.Key == tetriUnit.tetriUnitIndex);
            tetriUnit.LoadUnit(indexPairColor);
        }
    }
    public void Server_LoadUnits(List<KeyValuePair<int, UnitData.Color>> indexPairColors, float unitRotationDifferent)
    {
        foreach(var tetriUnit in TetriUnits)
        {
            if(!tetriUnit)continue;
            KeyValuePair<int, UnitData.Color> indexPairColor = indexPairColors.Find(x => x.Key == tetriUnit.tetriUnitIndex);
            tetriUnit.Server_LoadCellUnit(indexPairColor.Value.ToString());
            tetriUnit.transform.RotateAround(tetriUnit.transform.position,tetriUnit.transform.forward,- unitRotationDifferent);
        }
    }
    public void Client_LoadUnits(float unitRotationDifferent)
    {
        
        foreach(var tetriUnit in TetriUnits)
        {
            if(!tetriUnit)continue;
            tetriUnit.transform.RotateAround(tetriUnit.transform.position,tetriUnit.transform.forward,- unitRotationDifferent);
        }
    }
    public void FailToCreat()
    {
        foreach(var tetriUnit in TetriUnits)
        {
            if(!tetriUnit)continue;
            tetriUnit.FailToCreat();
        }
        CheckUnitTag(false);
    }
    public void CheckUnitTag(bool needFight = true)
    {
        if(!transform.parent)
        {
            foreach (var tetriUnit in TetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(false);
            }
            return;
        }
        if(needFight || transform.parent.TryGetComponent(out BlocksCreator_Main tetrisBlockSimple))
        {
            foreach (var tetriUnit in TetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(true);
            }
            
        }else
        {
            foreach (var tetriUnit in TetriUnits)
            {
                if(!tetriUnit)continue;
                tetriUnit.SetFightTag(false);
            }
            
        }
        
    }
    public void InitPropDoing()
    {
        foreach(var tetriUnit in TetriUnits)
        {
            if(!tetriUnit)continue;
            PropsData.PropsState propChecker = tetriUnit.HaveUnit.InitPropDoing();
            if(propChecker != PropsData.PropsState.None)return;
        }
        transform.GetComponent<TetrisBlockSimple>().Active_X();
    }
    
    public void KillAllUnits()
    {
        for(int i = 0 ; i < TetriUnits.Count; i++)
        {
            if(!TetriUnits[i])continue;
            TetriUnits[i].Event_UnitDie(TetriUnits[i].HaveUnit);
            if(i == TetriUnits.Count - 1)
            {
                Destroy(gameObject);
            }
        }
    }
    public void OnBeginDragDisplay()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.OnBeginDragDisplay();
        }
    }
    public void OnEndDragDisplay()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.OnEndDragDisplay();
        }
    }
    public void OnEditingStatusAfterSelection()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.OnEditingStatusAfterSelection();
        }
    }
    public void LevelUp(int level)
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.LevelUp(level);
        }
    }
    public void SetUnitSortingOrderToFlow()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.SetUnitSortingOrderToFlow();
        }
    }
    public void SetUnitSortingOrderToNotNormal()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.SetUnitSortingOrderToNormal();
        }
    }
    public void Display_ShowUnit()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.Display_ShowUnit();
        }
    }
    public void Display_HideUnit()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.Display_HideUnit();
        }
    }
    public void Display_ShowForPlayerScreen()
    {
        foreach(var tetri in TetriUnits)
        {
            tetri.Display_ShowForPlayerScreen();
        }
    }
}