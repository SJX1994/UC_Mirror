using System.Collections.Generic;
using UnityEngine;

// 技能类
[System.Serializable]
public class PuppetEffectDataStruct
{
    public enum EffectType
    {
    	Positive, 
    	Nagetive, 
    	ChainTransferBoom,
      ChainTransferTrail,
      FourDirectionsLinkerStart,
      FourDirectionsLinkerUpdate,
      FourDirectionsLinkerEnd,
      WeakAssociationStart,
      WeakAssociationUpdate,
      WeakAssociationEnd,
    }
    public EffectType effectType;
    public Vector3 targetPosition;
    public float duration;
    public int id;
    public string stringInfo;
    public FourDirectionsLink.Direction direction;
      public PuppetEffectDataStruct(EffectType ty, Vector3 v, float du)
      {
      	effectType = ty;
      	targetPosition = v;
      	duration = du;
      }

      public PuppetEffectDataStruct(EffectType ty, Vector3 v, string str)
      {
      	effectType = ty;
      	targetPosition = v;
      	stringInfo = str;
      }

	    public PuppetEffectDataStruct(EffectType ty, Vector3 v)
      {
      	effectType = ty;
      	targetPosition = v;
      }
      public PuppetEffectDataStruct(EffectType ty)
      {
      	effectType = ty;
      }
      public PuppetEffectDataStruct(EffectType ty, int i)
      {
      	effectType = ty;
      	id = i;
      }
      public PuppetEffectDataStruct(EffectType ty, int i,string str)
      {
      	effectType = ty;
      	id = i;
      	stringInfo = str;
      }
     public PuppetEffectDataStruct(EffectType ty, int i,string str,FourDirectionsLink.Direction dir)
      {
      	effectType = ty;
      	id = i;
      	stringInfo = str;
        direction = dir;
      }
      public PuppetEffectDataStruct(EffectType ty, FourDirectionsLink.Direction dir,Vector3 v)
      {
      	effectType = ty;
      	direction = dir;
      	targetPosition = v;
      }
      public PuppetEffectDataStruct(EffectType ty, FourDirectionsLink.Direction dir)
      {
      	effectType = ty;
      	direction = dir;
      	
      }
}