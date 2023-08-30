using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
	public class TalentTreeGraph : ScriptableObject 
	{
        #region Variables
        public string treeName = "New Tree";
        public List<TalentTreeNodeBase> talents;
        public List<TalentTreeConnectionBase> connections;
        public List<Tier> tiers;
        /*public List<PropertyDefinition> tierPropertyDefinitions;
        public List<PropertyDefinition> talentPropertyDefinitions;*/
        public TalentTreeNodeBase selectedNode;
        public TalentTreeConnectionBase selectedConnection;

        public bool wantsConnection = false;
        public TalentTreeNodeBase connectionNode;
        public bool showProperties = false;

        public float PanX = 0f;
        public float PanY = 0f;
        public bool mouseOverProperties = false;
        public float propertyWidth;

        public int PointsToAssign = 0;

        private bool ThrowEvaluatedEvent = true;
        #endregion

        #region Event delegations
        #region Evaluation
        public class TreeEvaluatedEventArgs : EventArgs
        {
            // In case we need to pass parameters in future version of Talentus
            // 如果我们需要在Talentus的未来版本中传递参数
        }
        protected virtual void OnTreeEvaluated(TreeEvaluatedEventArgs e)
        {
            EventHandler<TreeEvaluatedEventArgs> handler = TreeEvaluated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public event EventHandler<TreeEvaluatedEventArgs> TreeEvaluated;

        public void ThrowTreeEvaluated(TreeEvaluatedEventArgs args)
        {
            OnTreeEvaluated(args);
        }
        #endregion
        #region Bought - triggers after Apply
        public class TalentBoughtEventArgs : EventArgs
        {
            public TalentTreeNodeBase Talent { get; set; }
            public TalentTreeCost Cost { get; set; }
        }

        protected virtual void OnTalentBought(TalentBoughtEventArgs e)
        {
            EventHandler<TalentBoughtEventArgs> handler = TalentBought;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<TalentBoughtEventArgs> TalentBought;

        public void ThrowTalentBought(TalentBoughtEventArgs args)
        {
            OnTalentBought(args);
        }
        #endregion
        #region UnBought - triggers after Respec
        public class TalentUnBoughtEventArgs : EventArgs
        {
            public TalentTreeNodeBase Talent { get; set; }
            public TalentTreeNodeLevel Info { get; set; }
        }

        protected virtual void OnTalentUnBought(TalentUnBoughtEventArgs e)
        {
            EventHandler<TalentUnBoughtEventArgs> handler = TalentUnBought;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<TalentUnBoughtEventArgs> TalentUnBought;

        public void ThrowTalentUnBought(TalentUnBoughtEventArgs args)
        {
            OnTalentUnBought(args);
        }
        #endregion
        #region Selected - triggers when you buy a talent (can still be reverted)
        public class TalentSelectedEventArgs : EventArgs
        {
            public TalentTreeNodeBase Talent { get; set; }
            public TalentTreeCost Cost { get; set; }
        }

        protected virtual void OnTalentSelected(TalentSelectedEventArgs e)
        {
            EventHandler<TalentSelectedEventArgs> handler = TalentSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<TalentSelectedEventArgs> TalentSelected;

        public void ThrowTalentSelected(TalentSelectedEventArgs args)
        {
            OnTalentSelected(args);
        }
        #endregion
        #region Reverted - triggers when you revert the buy action of a talent
        public class TalentRevertedEventArgs : EventArgs
        {
            public TalentTreeNodeBase Talent { get; set; }
            public TalentTreeCost Cost { get; set; }
        }

        protected virtual void OnTalentReverted(TalentRevertedEventArgs e)
        {
            EventHandler<TalentRevertedEventArgs> handler = TalentReverted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<TalentRevertedEventArgs> TalentReverted;

        public void ThrowTalentReverted(TalentRevertedEventArgs args)
        {
            OnTalentReverted(args);
        }
        #endregion
        #endregion

        #region Implementation
        private void OnEnable()
        {
            if (tiers == null)
                tiers = new List<Tier>();
            if (talents == null)
                talents = new List<TalentTreeNodeBase>();
            if (connections == null)
                connections = new List<TalentTreeConnectionBase>();
            /*if (tierPropertyDefinitions == null)
                tierPropertyDefinitions = new List<PropertyDefinition>();
            if (talentPropertyDefinitions == null)
                talentPropertyDefinitions = new List<PropertyDefinition>();*/
        }

        public void InitGraph()
        {
            if (talents.Count > 0)
            {
                for (int i = 0; i < talents.Count; i++)
                {
                    talents[i].InitNode();
                }
            }
        }

        public void Apply()
        {
            if (talents != null)
            {
                if (talents.Count > 0)
                {
                    foreach (var t in talents)
                    {
                        t.Apply();
                    }
                }
            }
            Evaluate();
        }

        public void Revert()
        {
            if (talents != null)
            {
                if (talents.Count > 0)
                {
                    foreach (var t in talents)
                    {
                        t.Revert();
                    }
                }
            }
            Evaluate();
        }

        public void CleanUp()
        {
            if (talents != null)
            {
                if (talents.Count > 0)
                {
                    foreach (var t in talents)
                    {
                        foreach (var c in t.Cost)
                        {
                            c.WillBuy = false;
                        }
                    }
                }
            }
        }

        public TalentTreeNodeBase FindTalent(string name)
        {
            TalentTreeNodeBase returnNode = null;
            string lowerName = name.ToLower();
            if (talents != null)
            {
                if (talents.Count > 0)
                {
                    foreach (var t in talents)
                    {
                        if (t.name.ToLower() == lowerName)
                        {
                            returnNode = t;
                            break;
                        }
                    }
                }
            }
            return returnNode;
        }

        /// <summary>
        /// Get all the connections that are linked towards the provided talent
        /// 获取与所提供天赋相关的所有联系
        /// </summary>
        /// <param name="talent">
        /// The talent/skill to check
        /// </param>
        /// <returns>
        /// List of all connections entering the talent
        /// 进入天赋的所有连接列表
        /// </returns>
        public List<TalentTreeConnectionStatus> GetInputConnections(TalentTreeNodeBase talent)
        {
            List<TalentTreeConnectionStatus> list = new List<TalentTreeConnectionStatus>();
            foreach (var conn in connections)
            {
                if (conn.toNode == talent)
                {
                    TalentTreeConnectionStatus status = new TalentTreeConnectionStatus();
                    status.Connection = conn;
                    list.Add(status);
                }
            }
            return list;
        }

        /// <summary>
        /// Get all the connections that are departing from the provided talent
        /// 获取与所提供的天赋依赖的所有联系
        /// </summary>
        /// <param name="talent">
        /// The talent/skill to check
        /// 要检查的天赋/技能
        /// </param>
        /// <returns>
        /// List of all connections exiting the talent
        /// 天赋依赖的所有连接列表
        /// </returns>
        public List<TalentTreeConnectionStatus> GetOutputConnections(TalentTreeNodeBase talent)
        {
            List<TalentTreeConnectionStatus> list = new List<TalentTreeConnectionStatus>();
            foreach (var conn in connections)
            {
                if (conn.fromNode == talent)
                {
                    TalentTreeConnectionStatus status = new TalentTreeConnectionStatus();
                    status.Connection = conn;
                    list.Add(status);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the number of active input connections, active means both in and output talents of the connections are bought.
        /// 返回输入连接的数量，激活意味着连接的输入和输出天赋都被购买。
        /// </summary>
        /// <param name="talent">
        /// Talent/Skill to check
        /// 要检查的天赋/技能
        /// </param>
        /// <returns>
        /// Number of active input connections.
        /// 输入连接的数量。
        /// </returns>
        public int NumberOfActiveInConnections(TalentTreeNodeBase talent)
        {
            List<TalentTreeConnectionStatus> list = GetInputConnections(talent);
            int cnt = 0;
            foreach (var item in list)
            {
                if (item.IsActive) cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// Returns the number of active output connections, active means both in and output talents of the connections are bought.
        /// 返回 活跃 输出连接的数量，活跃 意味着连接的输入和输出人才都被购买。
        /// </summary>
        /// <param name="talent">Talent/Skill to check</param>
        /// <returns>
        /// Number of active output connections.
        /// 活动输出连接数。
        /// </returns>
        public int NumberOfActiveOutConnections(TalentTreeNodeBase talent)
        {
            List<TalentTreeConnectionStatus> list = GetOutputConnections(talent);
            int cnt = 0;
            foreach (var item in list)
            {
                if (item.IsActive) cnt++;
            }
            return cnt;
        }

        public void Evaluate()
        {
            float startTime = Time.deltaTime;
            //Debug.Log(string.Format("Start evaluating {0}", startTime));

            if (talents != null)
            {
                if (talents.Count > 0)
                {
                    foreach (var t in talents)
                    {                        
                        bool connectionFound = false;
                        // See if there is a connection that ends on this talent
                        // 看看是否存在以该天赋结束的连接
                        if (connections != null && connections.Count > 0)
                        {
                            foreach (var c in connections)
                            {
                                if (c.toNode == t)
                                {
                                    // We found a connection so no need to evaluate longer
                                    // 我们找到了联系，因此无需再进行评估
                                    connectionFound = true;
                                    break;
                                }
                            }
                        }
                        
                        if (!connectionFound)
                        {
                            // Evaluate the talents that have no connection assigned
                            // 评估没有分配关系的天赋

                            if (t.GetCostForNextLevel().Cost <= PointsToAssign)
                            {
                                t.isValid = true;
                                //Debug.Log(string.Format("{0} is valid !", t.name));
                            }
                            else
                            {
                                t.isValid = false;
                                //Debug.Log(string.Format("{0} can't be bought (not enough points) !", t.name));
                            }
                        }
                        else
                        {
                            // Evaluate thosetalents that have connections
                            // 评估那些有联系的天赋

                            // Let's see if you have enough to buy the talent
                            // 看看你是否有足够的钱购买天赋
                            if (t.GetCostForNextLevel().Cost <= PointsToAssign)
                                t.isValid = true;
                            else
                                t.isValid = false;

                            // If we could buy the talent let's check the connections
                            // this is done to reduce calculations and thus increase performance
                            // 如果我们可以购买天赋，让我们检查一下连接
                            // 这样做是为了减少计算，从而提高性能
                            if (t.isValid)
                            {
                                // Determine the optional and required connections for the talent
                                // 确定天赋可选和必需的联系
                                List<TalentTreeConnectionBase> optionalConnections = connections.FindAll(x => x.toNode == t && x.connectionType == ConnectionType.Optional);
                                List<TalentTreeConnectionBase> requiredConnections = connections.FindAll(x => x.toNode == t && x.connectionType == ConnectionType.Required);
                                //Debug.Log(string.Format("Talent {0} has {1} optional and {2} required connections.", t.name, optionalConnections.Count, requiredConnections.Count));

                                // Get the validation result of all the optional connections
                                // 获取所有可选连接的验证结果
                                bool optionalConnectionResult = true;
                                int current = 0;
                                foreach (var conn in optionalConnections)
                                {
                                    if (current == 0)
                                        optionalConnectionResult = EvaluateConnectionConditions(conn);
                                    else
                                        optionalConnectionResult = optionalConnectionResult || EvaluateConnectionConditions(conn);
                                    current++;
                                }
                                //Debug.Log(string.Format("> Optional connections returned : {0}", optionalConnectionResult.ToString()));

                                // Get the validation result of all the required connections
                                // 获取所有所需连接的验证结果
                                bool requiredConnectionResult = true;
                                current = 0;
                                foreach (var conn in requiredConnections)
                                {
                                    if (current == 0)
                                        requiredConnectionResult = EvaluateConnectionConditions(conn);
                                    else
                                        requiredConnectionResult = requiredConnectionResult && EvaluateConnectionConditions(conn);
                                    current++;
                                }
                                //Debug.Log(string.Format("> Required connections returned : {0}", requiredConnectionResult.ToString()));
                                
                                // AND operate the optional and the required connection results
                                // AND 操作可选和所需的连接结果
                                t.isValid = (requiredConnectionResult && optionalConnectionResult);

                            }                            
                        }
                    }
                }
            }
            
            float endTime = Time.deltaTime;
            //Debug.Log(string.Format("End evaluating {0}", endTime));
            //Debug.Log(string.Format("{0} seconds needed for evaluating the tree.", (endTime - startTime)));

            //Make sure we through an event when the tree was evaluated
            // 确保我们在评估树时经历了一个事件
            if (ThrowEvaluatedEvent)
            {
                TreeEvaluatedEventArgs args = new TreeEvaluatedEventArgs();
                ThrowTreeEvaluated(args);
            }
        }

        /// <summary>
        /// Evaluate all the conditions of the connection
        /// 评估连接的所有条件
        /// </summary>
        /// <param name="conn">
        /// Connection to validate
        /// 连接验证
        /// </param>
        private bool EvaluateConnectionConditions(TalentTreeConnectionBase conn)
        {
            bool isValid = false;
            if (conn.Conditions != null && conn.Conditions.Count > 0)
            {
                isValid = EvaluateTalent(conn);                                                  
            }
            return isValid;
        }

        public bool CanBeUnbought(TalentTreeNodeBase talent)
        {
            return CanBeUnbought(talent, null);  
        }

        /// <summary>
        /// Check if it's ok to unbuy a talent that has been applied
        /// 检查是否可以取消购买已应用的天赋
        /// </summary>
        /// <param name="talent">The talent to check</param>
        /// <returns>
        /// True or False indicating it will be save to unbuy the talent without breaking the tree.
        /// 指示取消购买天赋是否会破坏树的真假。
        /// </returns>
        public bool CanBeUnbought(TalentTreeNodeBase talent, List<TalentTreeNodeBase> boughtTalents)
        {
            if (talent != null)
            {
                if (boughtTalents == null)
                {
                    boughtTalents = new List<TalentTreeNodeBase>();
                    foreach (TalentTreeNodeBase item in talents)
                    {
                        if (item != talent)
                        {
                            TalentTreeNodeLevel level = item.GetLevelCost();
                            if (level.Level > 0 && level.Cost > 0)
                            {
                                boughtTalents.Add(item);
                            }
                        }
                    }
                }

                if (boughtTalents.Count > 0)
                {
                    TalentTreeNodeLevel skillLevel = talent.GetLevelCost();
                    if (skillLevel.Level > 0 && skillLevel.Cost > 0)
                    {
                        // Do not throw the evaluatedevent when checking the can be unbought routine
                        // 检查可取消购买例程时不要抛出评估事件
                        ThrowEvaluatedEvent = false;

                        // Revert the applied level
                        // 恢复应用的级别
                        talent.Cost[skillLevel.Level - 1].Bought = false;

                        //Evaluate the tree
                        // 评估树
                        Evaluate();

                        //Check the previous bought talents if they are still valid
                        // 检查以前购买的天赋是否仍然有效

                        bool treeStillOk = true;
                        foreach (TalentTreeNodeBase item in boughtTalents)
                        {
                            if (!item.isValid)
                            {
                                treeStillOk = false;
                                Debug.Log("tree not valid on talent " + item.name + " for " + talent.name);
                                break;
                            }
                        }

                        // Reapply the previous level
                        // 重新应用以前的级别
                        talent.Cost[skillLevel.Level - 1].Bought = true;

                        // Evaluate the tree to reset to previous state
                        // 评估树以重置到以前的状态
                        Evaluate();

                        // Reset the flag so evaluatedevent is throw after tree evaluation
                        // 重置标志，因此在树评估后抛出评估事件
                        ThrowEvaluatedEvent = true;

                        return treeStillOk;
                    }
                    else return false;
                }
                else return false;                
            }
            return false;
        }

        /// <summary>
        /// Unbuy the specified talent, make sure the check if this doesn't break the tree by calling CanBeUnbought()
        /// 取消购买指定的天赋，确保通过调用CanBeUnbought()来检查是否会破坏树
        /// </summary>
        /// <param name="talent">The skill to revert after apply.</param>
        /// <returns>
        /// A class containing the level and cost of the unbought talent.
        /// 包含未购买天赋的级别和成本的类。
        /// </returns>
        public TalentTreeNodeLevel UnBuy(TalentTreeNodeBase talent)
        {
            if (talent != null)
            {              
                TalentTreeNodeLevel skillLevel = talent.GetLevelCost();
                if (skillLevel.Level > 0 && skillLevel.Cost > 0)
                {
                    // Revert the applied level
                    talent.Cost[skillLevel.Level - 1].Bought = false;

                    // Evaluate the tree
                    Evaluate();

                    // Throw an event so the UI can be updated
                    TalentUnBoughtEventArgs args = new TalentTreeGraph.TalentUnBoughtEventArgs();
                    args.Talent = talent;
                    args.Info = skillLevel;
                    ThrowTalentUnBought(args);

                    return skillLevel;
                }
            }
            return null;
        }

        private bool EvaluateTalent(TalentTreeConnectionBase conn)
        {
            TalentTreeNodeBase fromTalent = conn.fromNode;
            TalentTreeNodeBase toTalent = conn.toNode;
            if (fromTalent != null && toTalent != null)
            {
                bool retValue = true;
                string condEval = string.Empty;                 
                foreach (var cond in conn.Conditions)
                {
                    condEval = conn.name + ": ";
                    switch (cond.ConditionType)
                    {
                        case ConditionType.HasLevel:
                            int level = fromTalent.Level;
                            if (level < cond.Value)
                                retValue = false;
                            condEval = condEval + string.Format("HasLevel {0} >= {1} return {2}", level, cond.Value, retValue);
                            break;
                        case ConditionType.MaxedOut:
                            int maxLevel = fromTalent.MaxLevel;
                            int lvl = fromTalent.Level;
                            if (lvl < maxLevel)
                                retValue = false;
                            condEval = condEval + string.Format("MaxedOut {0} >= {1} return {2}", lvl, cond.Value, retValue);
                            break;
                        case ConditionType.TierCompleted:
                            int tierIdx = (int)cond.Value;
                            if (tierIdx < tiers.Count)
                            {
                                Tier t = tiers[tierIdx];
                                if (!t.IsMaxedOut)
                                    retValue = false;
                                condEval = condEval + string.Format("TierComplete {0} return {1}", t.Name, retValue);
                            }
                            else
                            {
                                retValue = false;
                                Debug.LogError(string.Format("Connection '{0}' is invalid as the assigned Tier cannot be found!", conn.name));
                            }
                            break;
                        default:
                            break;
                    }

                    //Debug.Log(condEval);
                    // As soon as one condition is false there is no need to evaluate more
                    // 一旦一个条件为假，就没有必要再评估了
                    if (retValue == false)
                        break;
                }

                //Debug.Log(string.Format("Connection {0} => {1}", conn.name, retValue));                
                return retValue;
            }
            else
            {
                Debug.LogError(string.Format("Connection '{0}' is invalid, either the from or to talent (or both) are not assigned!", conn.name));
                return false;
            }
        }

        public void UpdateGraph()
        {
            if (talents.Count > 0)
            {
                
            }
        }        

#if UNITY_EDITOR
        public void UpdateGraphGUI(Event e, Rect viewRect, Rect origRect, GUISkin viewSkin, Color requiredTypeColor, Color optionalTypeColor, int connectorPosition)
        {
            if (connections.Count > 0)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].UpdateConnectionGUI(e, viewRect, viewSkin, requiredTypeColor, optionalTypeColor, connectorPosition);
                }
            }

            if (talents.Count > 0)
            {
                ProcessEvents(e, viewRect);
                for (int i = 0; i < talents.Count; i++)
                {                    
                    talents[i].UpdateNodeGUI(e, viewRect, viewSkin, connectorPosition);
                }
            }

            //Lets look for connection mode
            if (wantsConnection)
            {
                if (connectionNode != null)
                {
                    DrawConnectionToMouse(e.mousePosition, connectorPosition);
                }
            }

            if (e.type == UnityEngine.EventType.Layout)
            {
                if (selectedNode != null)
                {
                    showProperties = true;
                }
            }

            EditorUtility.SetDirty(this);
        }

#endif
        #endregion

        #region Utilities
        void ProcessEvents(Event e, Rect viewRect)
        {
            Vector2 pos = e.mousePosition;
            if (viewRect.Contains(pos))
            {
                if (e.button == 0)
                {
                    if (e.type == UnityEngine.EventType.MouseDown)
                    {
                        /*DeselectAllNodes();
                        showProperties = false;
                        bool setNode = false;
                        selectedNode = null;
                        for (int i = 0; i < talents.Count; i++)
                        {
                            if(talents[i].nodeRect.Contains(e.mousePosition))
                            {
                                talents[i].isSelected = true;
                                selectedNode = talents[i];
                                setNode = true;
                            }
                        }
                        if (!setNode)
                        {
                            DeselectAllNodes();
                        }*/

                        if (wantsConnection)
                        {
                            wantsConnection = false;
                            //connectionNode = null;
                        }
                    }
                }
            }
        }

        public void ClearSelection()
        {
            selectedConnection = null;
            selectedNode = null;
            for (int i = 0; i < talents.Count; i++)
            {
                talents[i].isSelected = false;
            }
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].isSelected = false;
            }
        }

        void DeselectAllNodes()
        {
            for (int i = 0; i < talents.Count; i++)
            {
                talents[i].isSelected = false;
            }
        }

#if UNITY_EDITOR
        private void DrawConnectionToMouse(Vector2 mousePosition, int connectorPosition)
        {
            /*Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawLine(new Vector3(connectionNode.nodeRect.x + connectionNode.nodeRect.width + 12f,
                                         connectionNode.nodeRect.y + (connectionNode.nodeRect.height * 0.5f), 0),
                             new Vector3(mousePosition.x, mousePosition.y, 0f));
            Handles.EndGUI();*/

            DrawNodeCurve(connectionNode.nodeRect, mousePosition, connectorPosition);
        }
        
        private Vector3[] DrawNodeCurve(Rect start, Vector2 end, int connectorPosition)
        {
            Rect endRect = new Rect(end.x, end.y, 1f, 1f);
            if (connectorPosition == 0)
            {
                return DrawNodeCurve(start, endRect, new Vector2(1f, 0.5f), new Vector2(0f, 0.5f));
            }else if(connectorPosition == 1)
            {
                return DrawNodeCurve(start, endRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 0f));
            }else if(connectorPosition == 2)
            {
                return DrawNodeCurve(start, endRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));
            }
            return null;
               
        }

        private Vector3[] DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage)
        {
            Color curveColor = new Color(255f / 255f, 119f / 255f, 0f);

            Vector3[] points;
            Vector3 startPos = new Vector3(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);
            Vector3 endPos = new Vector3(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);
            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);
            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);
            Color shadowCol = new Color(255, 119, 0, 0.06f);
            Handles.BeginGUI();
            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 2);
            points = Handles.MakeBezierPoints(startPos, endPos, startTan, endTan, 10);
            Handles.EndGUI();

            return points;
        }
#endif

        #endregion
    }
}
