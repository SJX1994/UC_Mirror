using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
	public class TalentTreeConnectionBase : ScriptableObject
    {
        #region Variables
        public int depth;
        public bool isSelected = false;
        public TalentTreeNodeBase fromNode;
        public TalentTreeNodeBase toNode;
        public Vector3[] curvePoints = null;
        public TalentTreeGraph curGraph;
        public string Name;
        public List<TalentTreeCondition> Conditions = new List<TalentTreeCondition>();
        public ConnectionType connectionType;
        #endregion

        #region Implementation
        public TalentTreeConnectionBase(){ }

        public void Init(TalentTreeGraph curGraph, TalentTreeNodeBase fromNode, TalentTreeNodeBase toNode)
        {
            this.curGraph = curGraph;
            this.fromNode = fromNode;
            this.toNode = toNode;
            this.name = fromNode.name.Replace(" ","") + "_" + toNode.name.Replace(" ","");
            this.connectionType = ConnectionType.Required;
        }

#if UNITY_EDITOR
        public virtual void UpdateConnectionGUI(Event e, Rect viewRect, GUISkin viewSkin, Color requiredTypeColor, Color optionalTypeColor, int connectorPosition)
        {
            ProcessEvents(e, viewRect);

            if (fromNode != null && toNode != null)
            {                
                curvePoints = DrawNodeCurve(fromNode.nodeRect, toNode.nodeRect, requiredTypeColor, optionalTypeColor, connectorPosition);
            }
            /*if (curvePoints.Length > 0)
            {
                for (int i = 1; i < curvePoints.Length; i++)
                {
                    float x1 = curvePoints[i - 1].x;
                    float x2 = curvePoints[i].x;
                    float y1 = curvePoints[i - 1].y;
                    float y2 = curvePoints[i].y;

                    if (x2 < x1)
                    {
                        float c = x1;
                        x1 = x2;
                        x2 = c;
                    }
                    if (y2 < y1)
                    {
                        float c = y1;
                        y1 = y2;
                        y2 = c;
                    }
                    Rect r = new Rect(x1 - 2, y1 - 2, (x2 + 2) - (x1 - 2), (y2 + 2) - (y1 - 2));
                    GUI.Box(r, "");
                }
            }*/
            if (this != null)
                EditorUtility.SetDirty(this);
        }

        public virtual void DrawConnectionProperties(Event e, Rect PropertyRect, GUISkin viewSkin)
        {
            if (e.type == UnityEngine.EventType.Layout)
            {
                isSelected = true;
            }
            if (e.type != UnityEngine.EventType.Ignore)
            {
                bool addCond = false;
                int delCond = -1;

                EditorGUILayout.BeginVertical();
                GUILayout.Space(86);
                EditorGUILayout.BeginHorizontal();
                float Ypos = 30f;
                float Xpos = 30f;                
                Rect imageFromRect = new Rect(Xpos, Ypos, 64f, 64f);
                GUI.DrawTexture(imageFromRect, fromNode.Image, ScaleMode.ScaleAndCrop, true, 0.0F);
                GUILayout.Label(fromNode.Name);

                Xpos = PropertyRect.width;
                Xpos = (Xpos - 64f) / 2f;

                Texture2D Image = (Texture2D)Resources.Load("Textures/Editor/property_link");
                Rect imageRect = new Rect(Xpos, Ypos + 8f, 64f, 48f);
                GUI.DrawTexture(imageRect, Image, ScaleMode.ScaleAndCrop, true, 0.0F);

                Xpos = PropertyRect.width - 64f - 30f;
                Rect imageToRect = new Rect(Xpos, Ypos, 64f, 64f);
                GUI.DrawTexture(imageToRect, toNode.Image, ScaleMode.ScaleAndCrop, true, 0.0F);
                TextAnchor oldA = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUILayout.Label(toNode.Name);
                GUI.skin.label.alignment = oldA;

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(24);
                name = EditorGUILayout.TextField("Name: ", name);
                connectionType = (ConnectionType)EditorGUILayout.EnumPopup("Connection Type: ", connectionType);
                GUILayout.Space(24);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Conditions : ", GUILayout.Width(128));
                //EditorGUILayout.LabelField(MaxLevel.ToString());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", viewSkin.GetStyle("SmallTool"), GUILayout.Width(20), GUILayout.Height(20)))
                {
                    addCond = true;
                }
                //EditorGUILayout.LabelField(MaxLevel.ToString());
                EditorGUILayout.EndHorizontal();
                if (Conditions.Count > 0)
                {
                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("x", viewSkin.GetStyle("SmallTool"), GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            delCond = i;
                        }
                        Conditions[i].DrawProperties(curGraph, e);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("No conditions structure found !", MessageType.Warning);
                    //EditorGUILayout.LabelField("No conditions structure found !");
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                if (addCond)
                {
                    AddCondItem(curGraph, this);
                }
                if (delCond != -1)
                {
                    DeleteCondItem(curGraph, this, delCond);
                }
            }
        }
#endif
        #endregion

        #region Utilities

#if UNITY_EDITOR
        public void AddCondItem(TalentTreeGraph curGraph, TalentTreeConnectionBase conn)
        {
            if (curGraph != null && conn != null)
            {
                //Add the default condition
                TalentTreeCondition cond = (TalentTreeCondition)ScriptableObject.CreateInstance<TalentTreeCondition>();
                cond.ConditionType = ConditionType.HasLevel;
                cond.Value = 1.0f;
                if (conn.Conditions.Count == 0)
                    cond.Description = string.Format("{0} must be active.", conn.fromNode.Name);
                else
                    cond.Description = "Condition " + (conn.Conditions.Count + 1).ToString().Trim();
                conn.Conditions.Add(cond);
                AssetDatabase.AddObjectToAsset(cond, conn);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public void DeleteCondItem(TalentTreeGraph curGraph, TalentTreeConnectionBase conn, int condIdx)
        {
            if (curGraph != null && conn != null && condIdx != -1)
            {
                if (condIdx < conn.Conditions.Count)
                {
                    //Remove the cost element
                    TalentTreeCondition c = conn.Conditions[condIdx];
                    conn.Conditions.RemoveAt(condIdx);
                    GameObject.DestroyImmediate(c, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public void DeleteAllCondItems(TalentTreeGraph curGraph, TalentTreeConnectionBase conn)
        {
            if (curGraph != null && conn != null )
            {
                for (int i = conn.Conditions.Count - 1; i >= 0; i--)
                {
                    //Remove the cost element
                    TalentTreeCondition c = conn.Conditions[i];
                    conn.Conditions.RemoveAt(i);
                    GameObject.DestroyImmediate(c, true);                    
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void ProcessEvents(Event e, Rect viewRect)
        {
            if (e.type == UnityEngine.EventType.MouseDown && e.button < 2) //Prevent mouse scrollwheel 
            {
                Vector2 mousePos = e.mousePosition;
                /*if (curGraph != null)
                {
                    Debug.Log("Curve mousepos adapted");
                    mousePos.x -= curGraph.PanX;
                    mousePos.y -= curGraph.PanY;
                }*/
                if (!curGraph.wantsConnection)
                {
                    if (OnCurve(mousePos))
                        isSelected = true;
                    else
                        isSelected = false;
                }
            }
            if (e.type == UnityEngine.EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Delete)
                {
                    if (isSelected)
                    {
                        curGraph.selectedConnection = null;
                        TalentTreeConnectionUtils.DeleteConnection(curGraph, this); 
                    }
                }
            }
        }

        public bool OnCurve(Vector2 pos)
        {
            if (curvePoints == null) return false;

            if (curvePoints.Length >= 2)
            {
                for (int i = 1; i < curvePoints.Length; i++)
                {
                    float x1 = curvePoints[i - 1].x;
                    float x2 = curvePoints[i].x;
                    float y1 = curvePoints[i - 1].y;
                    float y2 = curvePoints[i].y;

                    if (x2 < x1)
                    {
                        float c = x1;
                        x1 = x2;
                        x2 = c;
                    }
                    if (y2 < y1)
                    {
                        float c = y1;
                        y1 = y2;
                        y2 = c;
                    }
                    Rect r = new Rect(x1 - 2, y1 - 2, (x2 + 2) - (x1 - 2), (y2 + 2) - (y1 - 2));
                    if (r.Contains(pos))
                        return true;
                }
            }
            return false;
        }

        public Vector3[] DrawNodeCurve(Rect start, Rect end, Color requiredTypeColor, Color optionalTypeColor, int connectorPosition)
        {
            if (connectorPosition == 0)
            {
                 return DrawNodeCurve(start, end, new Vector2(1f, 0.5f), new Vector2(0f, 0.5f), requiredTypeColor, optionalTypeColor, connectorPosition);
            }
            else if(connectorPosition == 1)
            {
                return DrawNodeCurve(start, end, new Vector2(0.5f, 1f), new Vector2(0.5f, 0f), requiredTypeColor, optionalTypeColor, connectorPosition);
            }else if(connectorPosition == 2)
            {
                return DrawNodeCurve(start,end, new Vector2(0.5f, 0f),new Vector2(0.5f, 1f), requiredTypeColor, optionalTypeColor, connectorPosition);
            }
            return null;
                
        }

        public Vector3[] DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage, Color requiredTypeColor, Color optionalTypeColor, int connectorPosition)
        {
            Color curveColor = requiredTypeColor; // new Color(0, 204f / 255f, 0); 
            if (connectionType == ConnectionType.Optional)
                curveColor = optionalTypeColor; // new Color(0, 128f/255f, 1f);
            if (Conditions.Count == 0)
                curveColor = new Color(180f / 255f, 0, 0);
            if (isSelected)
                curveColor = new Color(255f/255f, 119f/255f, 0f);
            
            Vector3[] points;

            Vector3 startPos = new Vector3(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);
            Vector3 endPos = new Vector3(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);
            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);
            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);

            Color shadowCol = new Color(0, 0, 0, 0.06f);
            if (Conditions.Count == 0)
                shadowCol = new Color(180f / 255f, 0, 0, 0.06f);
            if (isSelected)
                shadowCol = new Color(255f/255f, 119f/255f, 0, 0.3f);
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
