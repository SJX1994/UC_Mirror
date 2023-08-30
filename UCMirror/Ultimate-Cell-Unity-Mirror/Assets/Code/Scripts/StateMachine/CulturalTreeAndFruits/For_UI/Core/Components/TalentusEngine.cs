using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.cygnusprojects.TalentTree
{
    [AddComponentMenu("Cygnus Projects/Talentus Engine")]
    public class TalentusEngine : MonoBehaviour 
	{
        #region Variables
        public TalentTreeGraph TalentTree;
        public int AvailableSkillPoints = 0;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Do some initializing.
        /// In case a talenttree is specified, clean it up, assign the available skill points and evaluate the tree.
        /// 如果指定了天赋树，请将其清理，分配可用的技能点并评估该树。
        /// </summary>
        public virtual void Start () 
		{
            if (TalentTree != null)
            {
                TalentTree.CleanUp();
                TalentTree.PointsToAssign = AvailableSkillPoints;
                Evaluate();
            }
		}
        #endregion

        #region Implementation
        /// <summary>
        /// Evaluate the talent tree (which skills can be bought?).
        /// 评估天赋树（哪些技能可以购买？）。
        /// </summary>                	
        public virtual void Evaluate()
        {
            if (TalentTree != null)
            {
                TalentTree.Evaluate();
            }
        }

        /// <summary>
        /// Apply the selected buy operations towards the actual tree.
        /// 将所选的购买操作应用于实际的树。
        /// </summary>
        public virtual void Apply()
        {            
            if (TalentTree != null)
            {                
                TalentTree.Apply();
            }
        }

        /// <summary>
        /// Undo the selected by operations of the talent tree
        /// 撤消天赋树操作所选择的
        /// </summary>
        public virtual void Revert()
        {
            if (TalentTree != null)
            {
                TalentTree.Revert();
            }
        }

        /// <summary>
        /// Buy a specific talent from the tree.
        /// 从树上购买特定的天赋。
        /// </summary>
        /// <param name="talent">Talent you want to buy.</param>
        public virtual void BuyTalent(TalentTreeNodeBase talent)
        {
            talent.Buy();
            Evaluate();
        }

        /// <summary>
        /// Unbuy a specific talent from the tree (can only be done when not applied yet).
        /// 从树上取消购买特定天赋（只能在尚未应用时执行）。
        /// </summary>
        /// <param name="talent">The talent you want to 'unbuy'.</param>
        public virtual void RevertTalent(TalentTreeNodeBase talent)
        {
            talent.Revert();
            Evaluate();
        }
        
        /// <summary>
        /// Prepare a string to be saved containing the actual statusses of all the talents within the tree.
        /// 准备一个要保存的字符串，其中包含树中所有天赋的实际状态。
        /// </summary>
        /// <returns>
        /// A string that can be used for saving.
        /// 可用于保存的字符串
        /// </returns>
        public string SaveToString()
        {
            string retValue = string.Empty;
            for (int i = 0; i < TalentTree.talents.Count; i++)
            {
                retValue = retValue + TalentTree.talents[i].Save();
            }
            return retValue;
        }

        /// <summary>
        /// Updated the talents statusses using a string.
        /// 使用字符串更新天赋状态。
        /// Evaluates afterwards so all dependencies are correctly updated.
        /// 之后进行评估，以便正确更新所有依赖项。
        /// </summary>
        /// <param name="statuses">
        /// A string in a predefined format containing the statusses of all talents within the tree.
        /// 预定义格式的字符串，包含树中所有天赋的状态
        /// </param>
        public void LoadFromString(string statuses)
        {            
            for (int i = 0; i < TalentTree.talents.Count; i++)
            {
                string name = "[" + TalentTree.talents[i].name;                
                int startpos = statuses.IndexOf(name) + 1;
                int stoppos = statuses.IndexOf("]", startpos + 1);
                string substring = statuses.Substring(startpos, stoppos - startpos);
                TalentTree.talents[i].Load(substring);                 
            }
            Evaluate();
        }
                
        #endregion
    }
}
