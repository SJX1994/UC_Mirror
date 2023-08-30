using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
	public class TalentTreeNodeBase  : ScriptableObject, ITalent
	{
        #region Variables
        public bool isSelected = false;
        public Rect nodeRect;
        public TalentTreeGraph parentTree;
        public NodeType nodeType;
        public Texture2D Image = null;
        public Texture2D DisabledImage = null;
        public bool isValid = false;
                       
        private int tierIndex = 0;
        protected GUISkin nodeSkin;

        [SerializeField]
        private string description = string.Empty;
        [SerializeField]
        private string explanation = string.Empty;
        [SerializeField]
        private bool isInResearch = false;
        [SerializeField]
        private Tier tier;
        [SerializeField]
        private List<TalentTreeCost> cost = new List<TalentTreeCost>();
        /*[SerializeField]
        private List<Property> properties = new List<Property>();*/
        #endregion

        #region Properties
        public bool IsMaxedOut
        {
            get
            {
                if (Level >= MaxLevel)
                    return true;
                else
                    return false;
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Explanation
        {
            get { return explanation; }
            set { explanation = value; }
        }

        public bool IsInResearch
        {
            get { return isInResearch; }
            set { isInResearch = value; }
        }

        public Tier Tier
        {
            get { return tier; }
            set { tier = value; }
        }

        public bool IsEnabled
        {
            get { return (Level > 0); }
        }

        public int MaxLevel
        {
            get
            {
                return cost.Count;
            }
        }

        public int Level
        {
            get
            {
                return GetLevel(true); 
            }
        }

        public List<TalentTreeCost> Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        /*public List<Property> Properties
        {
            get { return properties; }
            set { properties = value; }
        }*/

        public Sprite ImageAsSprite
        {
            get
            {
                if (Image != null)
                    return Sprite.Create(Image, new Rect(0f, 0f, Image.width, Image.height), new Vector2(0.5f, 0.5f));
                else return null;
            }
        }

        public Sprite DisabledImageAsSprite
        {
            get
            {
                if (DisabledImage != null)
                    return Sprite.Create(DisabledImage, new Rect(0f, 0f, DisabledImage.width, DisabledImage.height), new Vector2(0.5f, 0.5f));
                else return null;
            }
        }
        #endregion

        #region Implementation

        #region SubClasses
        [Serializable]
        public class TalentTreeInput
        {
            public bool isOccupied = false;
            public TalentTreeNodeBase inputNode;
        }

        [Serializable]
        public class TalentTreeOutput
        {
            public bool isOccupied = false;
        }

        #endregion

        /// <summary>
        /// Determine the current level of the talent.
        /// 确定 果实 当前的水平。
        /// </summary>
        /// <param name="editMode">True if you would like to count those cost that will be bought on Accept()</param>
        /// <returns></returns>
        public int GetLevel(bool editMode)
        {
            if (cost != null)
            {
                if (cost.Count > 0)
                {
                    int level = 0;
                    for (int i = 0; i < cost.Count; i++)
                    {
                        if (cost[i].Bought)
                            level++;
                        else
                        {
                            if (editMode && cost[i].WillBuy)
                            {
                                level++;
                            }
                            else
                                break;
                        }
                    }
                    return level;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public virtual void InitNode()
        {
#if UNITY_EDITOR
            if (Image == null)
            {
                Image = (Texture2D)Resources.Load("Textures/Editor/no_image");
            }
#endif
        }

        // Resets the talent       
        public void Reset()
        {
            foreach (var item in cost)
            {
                item.WillBuy = false;
                item.Bought = false;
            }
        }

        public void Buy()
        {
            TalentTreeNodeNextLevel c = GetCostForNextLevel();
            if (c.Level != -1 && c.Cost <= parentTree.PointsToAssign)
            {
                cost[c.Level].WillBuy = true;
                //Debug.Log(string.Format("ParentTree PointsToAssign {0}", parentTree.PointsToAssign - c.Cost));
                parentTree.PointsToAssign = parentTree.PointsToAssign - c.Cost;

                TalentTreeGraph.TalentSelectedEventArgs args = new TalentTreeGraph.TalentSelectedEventArgs();
                args.Talent = this;
                args.Cost = cost[c.Level];
                parentTree.ThrowTalentSelected(args);
            }
            else
                Debug.Log("No GetCostForNextLevel() found!");
        }

        /// <summary>
        /// Apply the settings from the UI to the object
        /// </summary>
        public void Apply()
        {
            if (cost!= null && cost.Count > 0)
            {
                bool wasBought = false;
                int costItem = -1;
                for (int i = 0; i < cost.Count; i++)
                {
                    if (cost[i].WillBuy)
                    {
                        cost[i].Bought = true;
                        cost[i].WillBuy = false;
                        wasBought = true;
                        costItem = i;
                    }
                }
                if (wasBought && costItem != -1)
                {  
                    TalentTreeGraph.TalentBoughtEventArgs args = new TalentTreeGraph.TalentBoughtEventArgs();
                    args.Talent = this;
                    args.Cost = cost[costItem];
                    parentTree.ThrowTalentBought(args);
                }
            }
        }

        /// <summary>
        /// Revert the changes when editing
        /// </summary>
        public void Revert()
        {
            if (cost != null && cost.Count > 0)
            {
                for (int i = 0; i < cost.Count; i++)
                {
                    if (cost[i].WillBuy)
                    {
                        cost[i].WillBuy = false;
                        parentTree.PointsToAssign = parentTree.PointsToAssign + cost[i].Cost;
                        //Debug.LogWarning(string.Format("Recuperated {0} points.", cost[i].Cost));                        

                        TalentTreeGraph.TalentRevertedEventArgs args = new TalentTreeGraph.TalentRevertedEventArgs();
                        args.Talent = this;
                        args.Cost = cost[i];
                        parentTree.ThrowTalentReverted(args);
                    }
                }
            }
        }

        /// <summary>
        /// Determine how much it would cost to reach next level
        /// </summary>
        /// <returns>Points needed to buy the next level.</returns>
        public TalentTreeNodeNextLevel GetCostForNextLevel()
        {
            if (IsMaxedOut) return new TalentTreeNodeNextLevel(-1, 0);

            if (cost != null && cost.Count > 0)
            {
                int c = 0;
                int l = 0;
                for (int i = 0; i < cost.Count; i++)
                {
                    if (!cost[i].Bought && !cost[i].WillBuy)
                    {
                        c = cost[i].Cost;
                        l = i;
                        break;
                    }
                }
                //Debug.Log(string.Format("Before return Level {0} - Cost {1}", l, c));
                return new TalentTreeNodeNextLevel(l, c);
            }
            else
                return new TalentTreeNodeNextLevel(-1, 0);
        }

        /// <summary>
        /// Get the level cost of the talent
        /// Note: the talent tree needs to be applied, non bought talents will return a level 0 and cost of -1.
        /// </summary>
        /// A class with the level and the cost value
        /// <returns></returns>
        public TalentTreeNodeLevel GetLevelCost()
        {
            int currentlevel = GetLevel(false);
            if (currentlevel-1 >= 0 && cost.Count >= currentlevel-1)
            {
                return new TalentTreeNodeLevel(currentlevel, cost[currentlevel-1].Cost);
            }
            else
                return new TalentTreeNodeLevel(currentlevel, -1);
        }

        /*
        public Property GetProperty(string Name)
        {
            Property retValue = null;
            foreach (var property in Properties)
            {
                if (property.Name == Name)
                {
                    retValue = property;
                    break;
                }
            }
            return retValue;
        }

        public void SetProperty(Property Property)
        {
            if (properties.Contains(Property))
            {
                int propID = properties.FindIndex(x => x.Name == Property.Name);
                if (propID >= 0) properties[propID] = Property;
            }
            else
                properties.Add(Property);
        }*/

        /// <summary>
        /// Update the talent node, processing the events 
        /// </summary>
        /// <param name="e">The event that needs processing</param>
        /// <param name="viewRect">The rectangle visualy incapsulating the node.</param>
        public virtual void UpdateNode(Event e, Rect viewRect)
        {
            ProcessEvents(e, viewRect);
        }

        /// <summary>
        /// Save the current statusses of the node to a predefined string format
        /// </summary>
        /// <returns>String containing the current state of the talent node</returns>
        public string Save()
        {
            string retValue = string.Empty;
            retValue = retValue + "[" + name + ";";
            retValue = retValue + cost.Count.ToString() + ";";
            if (cost.Count > 0)
            {                
                for (int i = 0; i < cost.Count; i++)
                {
                    if (cost[i].Bought) retValue = retValue + "1;";
                    else retValue = retValue + "0;";
                }
            }
            retValue = retValue + "]";
            return retValue;
        }

        /// <summary>
        /// Load the statusses of the talent using a string in a predefined format
        /// </summary>
        /// <param name="objectStatus">String in predefined format containing the statusses of the talent.</param>
        public void Load(string objectStatus)
        {
            string[] elements = objectStatus.Split(';');
            if (elements.Length >= 2)
            {
                int nbrCosts = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0 && elements[i] != name) break;
                    if (i == 1) nbrCosts = Int32.Parse(elements[i]);
                }
                for (int i = 0; i < nbrCosts; i++)
                {
                    if (elements[2 + i] == "1") cost[i].Bought = true;
                    else cost[i].Bought = false;                    
                }
            } 
        }

#if UNITY_EDITOR
        /// <summary>
        /// Update the visual representation of the node
        /// </summary>
        /// <param name="e">Event to process</param>
        /// <param name="viewRect">The rectangle visualy incapsulating the node.</param>
        /// <param name="viewSkin">The UI skin in which the node needs to draw.</param>
        public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin, int connectorPosition)
        {
            ProcessEvents(e, viewRect);

            Rect glowRect = new Rect(nodeRect.x - (8f - 1f), nodeRect.y - (8f - 1f), nodeRect.width + (2f * 8f) - 1f, nodeRect.height + (2f * 8f) - 1f);
            Color oldColor = GUI.color;
            if (!isSelected)
            {
                GUI.color = new Color(0, 0, 0, 0.2f);
                if (tier != null)
                    GUI.color = tier.EditorColor;
                GUI.Box(glowRect, "", viewSkin.GetStyle("NodeGlow"));
                GUI.color = oldColor;
                GUI.Box(nodeRect, name, viewSkin.GetStyle("NodeDefault"));
            } else
            {
                GUI.color = new Color(255f / 255f, 119f / 255f, 0, 0.3f);
                if (tier != null) GUI.color = tier.EditorColor;
                GUI.Box(glowRect, "", viewSkin.GetStyle("NodeGlow"));
                GUI.color = oldColor;
                GUI.Box(nodeRect, name, viewSkin.GetStyle("NodeSelected"));
            }
            GUI.color = oldColor;

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Draw the properties of this node.
        /// </summary>
        /// <param name="e">The event to be processed</param>
        /// <param name="viewSkin">The UI skin in which the properties needs to be drawn.</param>
        public virtual void DrawNodeProperties(Event e, GUISkin viewSkin)
        {
            if (e.type == UnityEngine.EventType.Layout)
            {
                isSelected = true;
            }
            if (e.type !=  UnityEngine.EventType.Ignore)
            {
                bool addCost = false;
                int delCost = -1;
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name : ", GUILayout.Width(128));
                name = EditorGUILayout.TextField(name);
                EditorGUILayout.EndHorizontal();
                string[] Tiers = new string[parentTree.tiers.Count];
                for (int i = 0; i < parentTree.tiers.Count; i++)
                {
                    Tiers[i] = parentTree.tiers[i].Name;
                    if (tier == parentTree.tiers[i])
                        tierIndex = i;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tier : ", GUILayout.Width(128));
                tierIndex = EditorGUILayout.Popup(tierIndex, Tiers);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Image : ", GUILayout.Width(128));
                Image = (Texture2D)EditorGUILayout.ObjectField(Image, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Disabled Image : ", GUILayout.Width(128));
                DisabledImage = (Texture2D)EditorGUILayout.ObjectField(DisabledImage, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Description : ", GUILayout.Width(128));
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                bool oldWrap = EditorStyles.textField.wordWrap;
                EditorStyles.textField.wordWrap = true;
                description = EditorGUILayout.TextArea(description, GUILayout.Height(3f * 14f), GUILayout.Width(parentTree.propertyWidth - (2f * 15f)));
                EditorStyles.textField.wordWrap = oldWrap;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Explanation : ", GUILayout.Width(128));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                bool oldWrap2 = EditorStyles.textField.wordWrap;
                EditorStyles.textField.wordWrap = true;
                explanation = EditorGUILayout.TextArea(explanation, GUILayout.Height(3f * 14f), GUILayout.Width(parentTree.propertyWidth - (2f * 15f)));
                EditorStyles.textField.wordWrap = oldWrap2;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Maximum Level : ", GUILayout.Width(128));
                EditorGUILayout.LabelField(MaxLevel.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(); 

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Costs : ", GUILayout.Width(128));
                //EditorGUILayout.LabelField(MaxLevel.ToString());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", viewSkin.GetStyle("SmallTool"), GUILayout.Width(20), GUILayout.Height(20)))
                {
                    addCost = true;
                }
                //EditorGUILayout.LabelField(MaxLevel.ToString());
                EditorGUILayout.EndHorizontal();
                if (cost.Count > 0)
                {
                    for (int i = 0; i < cost.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("x", viewSkin.GetStyle("SmallTool"), GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            delCost = i;
                        }
                        cost[i].DrawProperties(e);
                        EditorGUILayout.EndHorizontal();
                    }
                } else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("No cost structure found !", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                if (addCost)
                {
                    AddCostItem(parentTree, this);
                }
                if (delCost != -1)
                {
                    DeleteCostItem(parentTree, this, delCost);
                }
                if (tierIndex < parentTree.tiers.Count)
                    Tier = parentTree.tiers[tierIndex];
                else
                    Debug.Log(string.Format("Unable to retrieve tier with index {0}", tierIndex));
            }
        }
#endif
        #endregion

        #region Utilities
        void ProcessEvents(Event e, Rect viewRect)
        {
            if (viewRect.Contains(e.mousePosition))
            {                
                if (e.type ==  UnityEngine.EventType.MouseDown && e.button < 2) //Prevent mouse scrollwheel 
                {                    
                    Vector2 mousePos = e.mousePosition;
                    if (nodeRect.Contains(mousePos))
                    {
                        //Check for only the topmost node
                        if (parentTree.talents.Count > 0)
                        {
                            foreach (var item in parentTree.talents)
                            {
                                item.isSelected = false;
                            }
                        }
                        isSelected = true;

                        //Uncheck any selected connection
                        if (parentTree.connections.Count > 0)
                        {
                            foreach (var item in parentTree.connections)
                            {
                                item.isSelected = false;
                            }
                        }
                    }
                    else
                        isSelected = false;
                }
                if (e.type ==  UnityEngine.EventType.MouseDrag)
                {
                    if (isSelected)
                    {
                        if (!parentTree.mouseOverProperties)
                        {
                            if (e.type ==  UnityEngine.EventType.MouseDrag)
                            {
                                nodeRect.x += e.delta.x;
                                nodeRect.y += e.delta.y;
                            }
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Add a new cost item to the talent
        /// </summary>
        /// <param name="curGraph">The tree to which the talent belongs.</param>
        /// <param name="node">The node for which a cost should be added.</param>
        public void AddCostItem(TalentTreeGraph curGraph, TalentTreeNodeBase node)
        {
            if (curGraph != null && node != null)
            {
                //Add the default cost of 1
                TalentTreeCost cost = (TalentTreeCost)ScriptableObject.CreateInstance<TalentTreeCost>();
                cost.Cost = 1;
                if (node.Cost.Count == 0)
                    cost.name = "Default Cost";
                else
                    cost.name = "Cost " + (node.Cost.Count + 1).ToString().Trim();
                node.Cost.Add(cost);
                AssetDatabase.AddObjectToAsset(cost, node);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Deletes a cost from the talents cost list.
        /// </summary>
        /// <param name="curGraph">The tree to which the talent belongs.</param>
        /// <param name="node">The node for which a cost should be deleted.</param>
        /// <param name="costIdx">The list index of the cost to be removed.</param>
        public void DeleteCostItem(TalentTreeGraph curGraph, TalentTreeNodeBase node, int costIdx)
        {
            if (curGraph != null && node != null && costIdx != -1)
            {
                if (costIdx < node.cost.Count)
                {
                    //Remove the cost element
                    TalentTreeCost c = node.cost[costIdx];
                    node.cost.RemoveAt(costIdx);
                    GameObject.DestroyImmediate(c, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
#endif
        #endregion
    }
}
