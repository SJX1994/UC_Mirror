using System;
using UnityEngine;


[Serializable]
public class AICommand
{
      public enum CommandType
	{
		GoToAndIdle, // 前往目的地然后待机
		GoToAndGuard, // 前往目的地然后侦察
		AttackTarget, // 前往攻击特定目标 然后侦察
		Stop, // 停止
		//Flee, // 逃跑
		Die, // 死亡
		Collecting, // 回到士兵池子
		
	}
      public CommandType commandType;
      public Vector3 destination;
	public Unit target;
	public float duration;
      public AICommand(CommandType ty, Vector3 v, Unit ta)
	{
		commandType = ty;
		destination = v;
		target = ta;
	}

	public AICommand(CommandType ty, Vector3 v)
	{
		commandType = ty;
		destination = v;
	}

	public AICommand(CommandType ty, Unit ta)
	{
		commandType = ty;
		target = ta;
	}

	public AICommand(CommandType ty)
	{
		commandType = ty;
	}
	public AICommand(CommandType ty,float du)
	{
		commandType = ty;
		duration = du;
	}
}