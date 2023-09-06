using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class TetrisBuoySimple : MonoBehaviour
{
    public Dictionary<TetriBuoySimple,BlockBuoyHandler> TB_cache = new();
    public TetrisBlockSimple tetrisBlockSimple;
    public TetrisBuoySimple tetrisBuoyDragged;
    public List<TetriBuoySimple> childTetris;
    public Tweener cantDropTweener;
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
    public void DoDropDragingCheck(List<TetriBuoySimple> checkSelfTetris)
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
    }
    public void DoDropDragingCheck()
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
    }
    public bool DoDropCanPutCheck(List<TetriBuoySimple> BuoyTetriBuoys)
    {
        List<bool> colliders = new();
        foreach(var child in childTetris)
        {
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
        foreach(var child in childTetris)
        {
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
        cantDropTweener.Kill();
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

    public void Display_OnDragBuoy()
    {
        foreach(var tetri in childTetris)
        {
            tetri.transform.localScale = Vector3.one * 0.6f;
        }
    }
    public void Display_OnCantDragBuoy()
    {
        foreach(var tetri in childTetris)
        {
            tetri.transform.localScale = Vector3.one * 0.3f;
        }
    }
    public void Display_Active()
    {
        InvokeRepeating(nameof(Display_Evaluate),0.0f,1.0f);
    }
    void Display_Evaluate()
    {
        foreach(var child in childTetris)
        {
            child.Display_Evaluate();
        }
    }
    
}