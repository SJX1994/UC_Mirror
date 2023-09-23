
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IdeaManager : MonoBehaviour
{
    // 想法枚举类型数组
    private List<EventType.IdeaType> ideaType;

    // Idea ID
    private int IdeaId = 3000;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        // 初始化生成想法枚举类型List
        ideaType = CreateIdeaTypes();

        broadcastClass.OnCreateNewIdea += CreateNewIdea;
    }

    /// <summary>
    /// 战场想法生成方法
    /// </summary>
    /// <returns></returns>     
    public void CreateNewIdea(int info) 
    {
        List<IdeaClass> ideaClassList = new();

        // 数量为0 则生成英雄砖块
        if (info == 0) 
        {
            IdeaClass ideaClass = new();

            // 设置想法ID
            ideaClass.IdeaId = IdeaId;

            IdeaId++;

            ideaClass.InfoIdea = EventType.IdeaType.Block;

            var Blocks = TetrisService.Instance.CreateHeroClass();

            ideaClass.BlocksInfo = Blocks;

            ideaClassList.Add(ideaClass);
        }

        var BlocksGrade = EventType.BlocksGrade.BottomGrade;

        for (int i = 0; i < info; i ++)
        {
            IdeaClass ideaClass = new();

            // 设置想法ID
            ideaClass.IdeaId = IdeaId;

            IdeaId++;

            // 设置想法类型
            var random = UnityEngine.Random.Range(0, ideaType.Count - 1);

            ideaClass.InfoIdea = (EventType.IdeaType)random;

            // 如果想法为砖块的话则设置砖块
            if ((EventType.IdeaType)random == EventType.IdeaType.Block)
            {
                var Blocks = TetrisService.Instance.CreateBlocksInfo();

                ideaClass.BlocksInfo = Blocks;

                BlocksGrade = Blocks.BlocksGrade;

                ideaClassList.Add(ideaClass);

            }

            /*// 如果想法为技能的话则设置技能
            if ((EventType.IdeaType)random == EventType.IdeaType.Skill)
            {
                var skill = SkillInfoManager.Instance.GetBattlefiledSkillByRandom();

                ideaClass.skillView = skill;
            }*/

        }

        CommunicationManager.ResponseCreateNewIdea(ideaClassList, BlocksGrade);

    }

    /// <summary>
    /// 刷新想法
    /// </summary>
    public void RefreshIdea() 
    {
        Debug.Log("Refresh Idea");
    }

    /// <summary>
    /// 初始化时生成IdeaType List
    /// </summary>
    /// <returns></returns>
    private List<EventType.IdeaType> CreateIdeaTypes()
    {
        List<EventType.IdeaType> ideaType = new();

        foreach (var temp in Enum.GetValues(typeof(EventType.IdeaType)))
        {
            ideaType.Add((EventType.IdeaType)temp);
        }

        return ideaType;
    }
}
