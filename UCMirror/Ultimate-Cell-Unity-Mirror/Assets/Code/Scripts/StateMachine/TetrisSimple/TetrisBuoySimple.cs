using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UC_PlayerData;
using Mirror;
public class TetrisBuoySimple : NetworkBehaviour
{
    public Dictionary<TetriBuoySimple,BlockBuoyHandler> TB_cache = new();
    public TetrisBlockSimple tetrisBlockSimple;
    public TetrisBlockSimple TetrisBlockSimple
    {
        get
        {
            if(tetrisBlockSimple == null)
            {
                tetrisBlockSimple = GetComponent<TetrisBlockSimple>();
            }
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
    public Tweener cantDropTweener;
    // 联网：
    public int serverID;
    public Player player;
    public int rotateTimes;
    public UnitData.Color colors;
    void Start()
    {
        foreach (Transform child in transform)
        {
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
            TetriBuoySimple tetriBuoySimple= child.GetComponent<TetriBuoySimple>();
            childTetris.Add(tetriBuoySimple);
            tetriBuoySimple.tetrisBuoySimple = this;
            
        }
        tetrisBlockSimple = GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnCacheUpdateForBuoyMarkers += CacheUpdateForBuoyMarkers;
        TB_cache = new();
    }
    public bool DoDropDragingCheck(List<TetriBuoySimple> checkSelfTetris)
    {
        List<bool> colliders = new();
        foreach(var child in childTetris)
        {
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
        foreach(var child in childTetris)
        {
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
        if(childTetris.Count == 0)Init();
        List<bool> colliders = new();
        foreach(var child in childTetris)
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
        if(childTetris.Count==0)Init();
        List<bool> colliders = new();
        foreach(var child in childTetris)
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
        foreach(TetriBuoySimple tetriBuoy in childTetris)
        {
            tetriBuoy.blockBuoyHandler = buoyMarkers[tetriBuoy];
            tetriBuoy.blockBuoyHandler.tetriBuoySimple = tetriBuoy;
            if (TB_cache.ContainsKey(tetriBuoy))return;
            TB_cache.Add(tetriBuoy,buoyMarkers[tetriBuoy]);
        }
    }
    public TetriBuoySimple GetTetriTemp(int id)
    {
        foreach(var child in childTetris)
        {
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
        if(childTetris.Count==0)Init();
        foreach(var tetri in childTetris)
        {
            tetri.transform.localScale = Vector3.one * 0.6f;
        }
    }
    public void Display_OnCantDragBuoy()
    {
        if(childTetris.Count==0)Init();
        foreach(var tetri in childTetris)
        {
            tetri.transform.localScale = Vector3.one * 0.3f;
        }
    }
    public void Display_Active()
    {
        InvokeRepeating(nameof(Display_Evaluate),0.0f,1.0f);
    }
    public void Display_Evaluate()
    {
        foreach(var child in childTetris)
        {
            child.Display_Evaluate();
        }
    }
    
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
}