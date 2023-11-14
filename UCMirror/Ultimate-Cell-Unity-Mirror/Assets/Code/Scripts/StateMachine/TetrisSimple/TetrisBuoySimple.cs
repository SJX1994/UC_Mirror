using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UC_PlayerData;
using Mirror;
public class TetrisBuoySimple : NetworkBehaviour
{
#region 数据对象
    public Dictionary<TetriBuoySimple,BlockBuoyHandler> TB_cache = new();
    public TetrisBlockSimple tetrisBlockSimple;
    public TetrisBlockSimple TetrisBlockSimple
    {
        get
        {
            if(tetrisBlockSimple)return tetrisBlockSimple;
            tetrisBlockSimple = GetComponent<TetrisBlockSimple>();
            return tetrisBlockSimple;
        }
        set
        {
            tetrisBlockSimple = value;
        }
    }
    private TetrisUnitSimple tetrisUnitSimple;
    public TetrisUnitSimple TetrisUnitSimple
    {
        get
        {
            if(!tetrisUnitSimple)tetrisUnitSimple = GetComponent<TetrisUnitSimple>();
            return tetrisUnitSimple;
        }
        
    }
    public TetrisBuoySimple tetrisBuoyDragged;
    private TetrisBuoySimple tetrisBuoyTemp;
    public TetrisBuoySimple TetrisBuoyTemp
    {
        get
        {
            return tetrisBuoyTemp;
        }
        set
        {
            if(value == null)return;
            tetrisBuoyTemp = value;
            if(tetrisBuoyTemp.childTetris.Count == 0)return;
            // ID 关联
            foreach(var tetri in tetrisBuoyTemp.childTetris)
            {
                if(!tetri)continue;
                tetri.TetriTemp = tetrisBuoyTemp.childTetris.Where(t => t.GetComponent<TetriUnitSimple>().tetriUnitTemplate.index == tetri.GetComponent<TetriUnitSimple>().tetriUnitTemplate.index).FirstOrDefault();
            }
        }
    }
    public List<TetriBuoySimple> childTetris;
    public List<TetriBuoySimple> ChildTetris
    {
        get
        {
            if(childTetris.Count != 0)return childTetris;
            foreach (Transform child in transform)
            {
                if(!child)continue;
                    TetriBuoySimple tetriBuoySimple= child.GetComponent<TetriBuoySimple>();
                    childTetris.Add(tetriBuoySimple);
                    tetriBuoySimple.tetrisBuoySimple = this;

            }
            return childTetris;
        }
        set
        {
            childTetris = value;
        }
    }
    public Tweener cantDropTweener;
    // 联网：
    public int serverID;
    public Player player;
  
    public int rotateTimes;
    public UnitData.Color colors;
#endregion 数据对象
#region 数据关系
    void Start()
    {
        foreach (Transform child in transform)
        {
            if(!child)continue;
            TetriBuoySimple tetriBuoySimple= child.GetComponent<TetriBuoySimple>();
            childTetris.Add(tetriBuoySimple);
            tetriBuoySimple.tetrisBuoySimple = this;
        }
        tetrisBlockSimple = GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnCacheUpdateForBuoyMarkers += CacheUpdateForBuoyMarkers;
        TB_cache = new();
    }
    public void Init()
    {
        foreach (Transform child in transform)
        {
            if(!child)continue;
            TetriBuoySimple tetriBuoySimple= child.GetComponent<TetriBuoySimple>();
            childTetris.Add(tetriBuoySimple);
            tetriBuoySimple.tetrisBuoySimple = this;
        }
        tetrisBlockSimple = GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnCacheUpdateForBuoyMarkers += CacheUpdateForBuoyMarkers;
        TB_cache = new();
    }
#endregion 数据关系
#region 数据操作
    public bool DoDropDragingCheck(List<TetriBuoySimple> checkSelfTetris)
    {
        List<bool> colliders = new();
        foreach(var child in childTetris)
        {
            if(!child)continue;
            bool check = child.DoDropDragingCheck(checkSelfTetris);
            colliders.Add(check);
        }
        bool allTrue = colliders.All(b => b);
        if(allTrue)
        {
            Display_OnDragBuoy();
        }else
        {
            Display_OnCantDragBuoy();
        }
        return allTrue;
    }
    public bool DoDropDragingCheck()
    {
        List<bool> colliders = new();
        foreach(var child in ChildTetris)
        {
            if(!child)continue;
            bool check = child.DoDropDragingCheck();
            colliders.Add(check);
        }
        bool allTrue = colliders.All(b => b);
        if(allTrue)
        {
            Display_OnDragBuoy();
        }else
        {
            Display_OnCantDragBuoy();
        }
        return allTrue;
    }
    public bool DoDropCanPutCheck(List<TetriBuoySimple> BuoyTetriBuoys)
    {
        List<bool> colliders = new();
        foreach(var child in ChildTetris)
        {
            if(!child)continue;
            bool check = child.DoDropCanPutCheck(BuoyTetriBuoys);
            colliders.Add(check);
        }
        bool allTrue = colliders.All(b => b);
        
        if(!allTrue) return allTrue;

        if(TB_cache.Count>0)
        {
            foreach(var item in TB_cache)
            {
                item.Key.blockBuoyHandler = null;
                item.Value.tetriBuoySimple = null;
            }
            TB_cache.Clear();
        }
        cantDropTweener.Kill();
        
        return allTrue;
    }
    public bool DoDropCanPutCheck()
    {
        List<bool> colliders = new();
        foreach(var child in ChildTetris)
        {
            if(!child)continue;
            bool check = child.DoDropCanPutCheck();
            colliders.Add(check);
        }
        bool allTrue = colliders.All(b => b);
        
        if(!allTrue)return allTrue;

        if(TB_cache.Count>0)
        {
            foreach(var item in TB_cache)
            {
                item.Key.blockBuoyHandler = null;
                item.Value.tetriBuoySimple = null;
            }
            TB_cache.Clear();
        }
        if(cantDropTweener!=null)cantDropTweener.Kill();
        
        return allTrue;
    }
    void CacheUpdateForBuoyMarkers(Dictionary<TetriBuoySimple,BlockBuoyHandler> buoyMarkers)
    {
        if(TB_cache.Count>0)
        {
            foreach(var item in TB_cache)
            {
                item.Key.blockBuoyHandler = null;
                item.Value.tetriBuoySimple = null;
            }
            TB_cache.Clear();
        }
        foreach(TetriBuoySimple tetriBuoy in ChildTetris)
        {
            if(!tetriBuoy)continue;
            if(!buoyMarkers.ContainsKey(tetriBuoy))continue;
            tetriBuoy.blockBuoyHandler = buoyMarkers[tetriBuoy];
            tetriBuoy.blockBuoyHandler.tetriBuoySimple = tetriBuoy;
            if (TB_cache.ContainsKey(tetriBuoy))continue;
            TB_cache.Add(tetriBuoy,buoyMarkers[tetriBuoy]);
        }
    }
    public TetriBuoySimple GetTetriTemp(int id)
    {
        foreach(var child in ChildTetris)
        {
            if(!child)continue;
            if(child.GetComponent<TetriUnitSimple>().tetriUnitTemplate.index == id)
            {
                Debug.Log("Found!!" + child.TetriTemp);
                return child.TetriTemp;
            }
        }
        return null;
    }
    public void Display_OnDragBuoy()
    {
        TetrisUnitSimple.OnBeginDragDisplay();
        foreach(var tetri in ChildTetris)
        {
            if(!tetri)continue;
            tetri.transform.localScale = Vector3.one * 0.6f;
        }
    }
    public void Display_OnCantDragBuoy()
    {
        foreach(var tetri in ChildTetris)
        {
            if(!tetri)continue;
            tetri.transform.localScale = Vector3.one * 0.3f;
        }
    }
    public void Display_Active()
    {
        InvokeRepeating(nameof(Display_Evaluate),0.0f,1.0f);
    }
    public void Display_Evaluate()
    {
        foreach(var child in ChildTetris)
        {
            if(!child)continue;
            child.Event_Display_Evaluate();
        }
    }
#endregion 数据操作
#region 联网数据操作
    //----联网----
    [Client]
    void OnSync_Display_OnDragBuoyDisplay(bool oldValue,bool newValue)
    {
        if(newValue)
        {
            Display_OnDragBuoy();
        }else
        {
            Display_OnCantDragBuoy();
        }
    }
    [Server]
    public void Server_Init_TetriUnits()
    {
        foreach(var tetri in TetrisUnitSimple.TetriUnits )
        {
            if(tetri.FindUnitSimple())continue;
            tetri.Start();
        }
    }
#endregion 联网数据操作
}