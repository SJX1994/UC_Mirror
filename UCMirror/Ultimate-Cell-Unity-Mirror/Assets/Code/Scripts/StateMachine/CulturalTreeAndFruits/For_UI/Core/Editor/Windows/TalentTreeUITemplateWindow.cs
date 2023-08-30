using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using System;

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreeUITemplateWindow : EditorWindow 
	{
        #region Variables
        public static TalentTreeUITemplateWindow curWindow;

        public TalentTreeGraph TalentTree;
        public TalentusEngine TalentusEngine;
        public RectTransform UIRoot;
        public GameObject UITalentPrefab;

        public MonoScript ScriptTalent;
        public MonoScript ScriptConnection;
        public MonoScript ScriptOptionalConnection;

        public bool ColorConnections;
        public bool AddConnectionSettings;
        public GameObject UIConnectionPrefab;
        public GameObject UIOptionalConnectionPrefab;

        private bool sameAsRequiredConnections;
        private bool isValid = true;
        private bool m_ShowGeneralSection = true;
        private bool m_ShowTalentSection;
        private bool m_ShowConnectionSection;

        private bool simplifyOptionalCircularConnections;
        TreeGrowingDispaly treeGrowingDispaly;
        #endregion

        #region Implementation
        public static void InitEditorWindow()
        {
            curWindow = (TalentTreeUITemplateWindow)EditorWindow.GetWindow<TalentTreeUITemplateWindow>();
            curWindow.titleContent = new GUIContent("Talentus UI Template"); //Can assign window Icon here!!!
            curWindow.position = new Rect(curWindow.position.x + 64f , curWindow.position.y + 64f, 356f, 408f );
        }

        private void OnEnable()
        {
            
        }

        private void OnDestroy()
        {
            //Debug.Log("Disabling window");
        }

        private void Update()
        {
            //Debug.Log("Updating window");
        }

        private void OnGUI()
        {
            isValid = true;
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);

            // Extra block that can be toggled on and off.
            // 可以打开和关闭的额外块。
            m_ShowGeneralSection = EditorGUILayout.Foldout(m_ShowGeneralSection, "General Settings", true);
            if (m_ShowGeneralSection)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Talent Tree : ", GUILayout.Width(128));
                TalentTree = (TalentTreeGraph)EditorGUILayout.ObjectField(TalentTree, typeof(TalentTreeGraph), false);
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                if (TalentTree == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Please select a Talent Tree!", MessageType.Error);
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Talentus Engine : ", GUILayout.Width(128));
                TalentusEngine = (TalentusEngine)EditorGUILayout.ObjectField(TalentusEngine, typeof(TalentusEngine), true);
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                if (TalentusEngine == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("You need a TalentusEngine for the system to work!", MessageType.Error);
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("UI Root : ", GUILayout.Width(128));
                UIRoot = (RectTransform)EditorGUILayout.ObjectField(UIRoot, typeof(RectTransform), true);
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                if (UIRoot == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Please select the Root RectTransform of the UI!", MessageType.Error);
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("UI Talent Prefab : ", GUILayout.Width(128));
                UITalentPrefab = (GameObject)EditorGUILayout.ObjectField(UITalentPrefab, typeof(GameObject), false);
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();

                if (UITalentPrefab != null)
                {
                    TalentUI ui = UITalentPrefab.GetComponent<TalentUI>();
                    if (ui == null)
                    {
                        isValid = false;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("Selected gameobject must contain a TalentUI component!", MessageType.Error);
                        GUILayout.Space(10);
                        EditorGUILayout.EndHorizontal();
                    }
                    
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Please select the Talent UI Prefab!", MessageType.Error);
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                }

                //GUILayout.Space(20);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFadeGroup();

            GUILayout.Space(12);

            // Extra block that can be toggled on and off.
            // 可以打开和关闭的额外块。
            m_ShowTalentSection = EditorGUILayout.Foldout(m_ShowTalentSection, "Talent Settings", true);
            if (m_ShowTalentSection)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Script to add", GUILayout.Width(128));
                EditorGUILayout.BeginHorizontal();
                ScriptTalent = (MonoScript)EditorGUILayout.ObjectField(ScriptTalent, typeof(MonoScript), true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.Space(12);

            //Extra block that can be toggled on and off.
            // 可以打开和关闭的额外块。
            m_ShowConnectionSection = EditorGUILayout.Foldout(m_ShowConnectionSection, "Connection Settings", true);
            if (m_ShowConnectionSection)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                AddConnectionSettings = EditorGUILayout.Toggle(AddConnectionSettings, GUILayout.MaxWidth(24));
                EditorGUILayout.LabelField("Attach connection settings when using prefabs");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                ColorConnections = EditorGUILayout.Toggle(ColorConnections, GUILayout.MaxWidth(24));
                EditorGUILayout.LabelField("Color according to there type");
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);
                EditorGUILayout.LabelField("Required Connections", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();                
                EditorGUILayout.LabelField("UI Connection Prefab : ", GUILayout.Width(142));
                UIConnectionPrefab = (GameObject)EditorGUILayout.ObjectField(UIConnectionPrefab, typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Script to add", GUILayout.Width(128));
                EditorGUILayout.BeginHorizontal();
                ScriptConnection = (MonoScript)EditorGUILayout.ObjectField(ScriptConnection, typeof(MonoScript), true);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                GUILayout.Space(8);
                EditorGUILayout.LabelField("Optional Connections", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                sameAsRequiredConnections = EditorGUILayout.Toggle(sameAsRequiredConnections, GUILayout.MaxWidth(24));
                EditorGUILayout.LabelField("Same settings as Required connections");
                EditorGUILayout.EndHorizontal();

                if (!sameAsRequiredConnections)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 212;
                    simplifyOptionalCircularConnections = EditorGUILayout.Toggle("Simplify Circular Connections", simplifyOptionalCircularConnections);
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(4);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("UI Connection Prefab : ", GUILayout.Width(142));
                    UIOptionalConnectionPrefab = (GameObject)EditorGUILayout.ObjectField(UIOptionalConnectionPrefab, typeof(GameObject), false);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Script to add", GUILayout.Width(128));
                    EditorGUILayout.BeginHorizontal();
                    ScriptOptionalConnection = (MonoScript)EditorGUILayout.ObjectField(ScriptOptionalConnection, typeof(MonoScript), true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Build UI", GUILayout.Height(32)))
            {
                if (TalentTree != null)
                {
                    if (UIRoot != null)
                    {
                        if (isValid)
                        {
                            if(UIRoot.TryGetComponent(out treeGrowingDispaly))
                            {
                                BuildUI();
                            }else
                            {
                                Debug.LogError("运行时无法产生表现，请添加 TreeGrowingDispaly 组件");
                                BuildUI();
                            }
                            
                        }
                    }
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Close", GUILayout.Height(32)))
            {
                if (curWindow == null)
                    curWindow = (TalentTreeUITemplateWindow)EditorWindow.GetWindow<TalentTreeUITemplateWindow>();
                curWindow.Close();
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            
        }

        private void BuildUI()
        {
            if (TalentTree.talents.Count == 0) return;
           
            TalentTreeUtils.LoadPreferences();

            float minY = TalentTree.talents[0].nodeRect.y;
            float minX = TalentTree.talents[0].nodeRect.x;

            for (int i = 0; i < TalentTree.talents.Count; i++)
            {
                if (TalentTree.talents[i].nodeRect.y < minY)
                    minY = TalentTree.talents[i].nodeRect.y;
                if (TalentTree.talents[i].nodeRect.x < minX)
                    minX = TalentTree.talents[i].nodeRect.x;
            }

            if (TalentTree.connections.Count > 0) // 是否需要贝塞尔曲线连接
            {
                // Determine the simplified optional circular connections
                // 确定简化的可选圆形连接
                List<SimplifiedConnection> simplifiedList = new List<SimplifiedConnection>();
                if (simplifyOptionalCircularConnections)
                {
                    for (int i = 0; i < TalentTree.connections.Count; i++)
                    {
                        TalentTreeConnectionBase conn = TalentTree.connections[i];
                        
                        TalentTreeNodeBase fromNode = conn.fromNode;
                        TalentTreeNodeBase toNode = conn.toNode;
                        
                       
                        

                        SimplifiedConnection c = new SimplifiedConnection(conn, fromNode, toNode);
                        c.DrawConnection = true;

                        if (conn.connectionType == ConnectionType.Optional)
                        {
                            bool found = false;
                            for (int j = 0; j < TalentTree.connections.Count; j++)
                            {
                                TalentTreeConnectionBase conx = TalentTree.connections[j];
                                if (conx.connectionType == ConnectionType.Optional)
                                {
                                    if (conx.fromNode == toNode && conx.toNode == fromNode)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (found)
                            {
                                // Make sure we only draw one connection
                                foreach (SimplifiedConnection oC in simplifiedList)
                                {
                                    if (oC.fromNode == c.toNode && oC.toNode == c.fromNode)
                                    {
                                        c.DrawConnection = false;
                                        break;
                                    }
                                }
                                simplifiedList.Add(c);
                            }
                        }
                    }
                }

                // Let's determine if we need to draw the connection and just do it.
                // 让我们确定是否需要绘制连接并执行它。
                for (int i = 0; i < TalentTree.connections.Count; i++)
                {
                    TalentTreeConnectionBase conn = TalentTree.connections[i];
                    TalentTreeNodeBase fromNode = conn.fromNode;
                    TalentTreeNodeBase toNode = conn.toNode;

                    

                    // conn.depth = i;
                    // if(i>0)
                    // {
                    //     if(TalentTree.connections[i].fromNode == TalentTree.connections[i-1].fromNode)
                    //     {
                    //         conn.depth = TalentTree.connections[i-1].depth;
                    //     }else
                    //     {
                    //         conn.depth = i;
                    //     }
                    // }

                    bool isSimplified = false;
                    bool drawConnection = false;
                    if (conn.connectionType == ConnectionType.Optional)
                    {
                        if (simplifyOptionalCircularConnections)
                        {
                            drawConnection = true;
                            bool found = false;
                            foreach (SimplifiedConnection sc in simplifiedList)
                            {
                                if (sc.fromNode == fromNode && sc.toNode == toNode)
                                {
                                    found = true;
                                    drawConnection = sc.DrawConnection;
                                    break;
                                }
                            }
                            if (found)
                            {
                                isSimplified = true;
                            }

                        }
                        else
                            drawConnection = true;
                    }
                    else
                        drawConnection = true;

                    if (drawConnection)
                    {
                        RectTransform r = UITalentPrefab.GetComponent<RectTransform>();
                        float width = r.sizeDelta.x;
                        float height = r.sizeDelta.y;

                        Vector3 vpos = new Vector3(fromNode.nodeRect.x, fromNode.nodeRect.y, 0);
                        vpos.x = (vpos.x - minX) + 10;
                        vpos.y = -((vpos.y - minY) + 10);

                        Vector3 tpos = new Vector3(toNode.nodeRect.x, toNode.nodeRect.y, 0);
                        tpos.x = (tpos.x - minX) + 10;
                        tpos.y = -((tpos.y - minY) + 10);
                        Rect fromPos = new Rect(vpos.x, vpos.y, width, height);
                        Rect toPos = new Rect(tpos.x, tpos.y, width, height);
                        DrawNodeCurve(fromPos, toPos, minX, minY, conn, fromNode, toNode, isSimplified);
                    }
                }
            }

            // Create the talent objects in the UI
            // 在UI中创建天赋对象
            for (int i = 0; i < TalentTree.talents.Count; i++)
            {
                TalentTreeNodeBase talent = TalentTree.talents[i];
                // Create an instance and assign it to the UI Root
                // 创建一个实例并将其分配给UI根
                GameObject go = Instantiate(UITalentPrefab);
                go.name = talent.Name;

                if (ScriptTalent != null)
                {
                    try
                    {                        
                        go.AddComponent(ScriptTalent.GetClass()); 
                    }
                    catch
                    {
                        Debug.LogError("Something went wrong assigning the extra script to the Talent object."); 
                    }
                }

                RectTransform r = go.GetComponent<RectTransform>();
                r.anchoredPosition = Vector3.zero;
                r.SetParent(UIRoot, true);

                Vector3 pos = new Vector3(talent.nodeRect.x, talent.nodeRect.y, 0);
                pos.x = (pos.x - minX) + 10;
                pos.y = -((pos.y - minY) + 10);
                r.anchoredPosition = pos;
                
                TalentUI UI = go.GetComponent<TalentUI>();
                UI.Engine = TalentusEngine;
                UI.Talent = talent;
                // 为连接特效做准备
                if(TalentTree.GetInputConnections(talent).Count > 0)
                {
                    List<TalentTreeConnectionStatus> talentTreeConnectionBases = TalentTree.GetInputConnections(talent);
                    List<TalentTreeConnectionBase> conns = new();
                    foreach (var item in talentTreeConnectionBases)
                    {
                        conns.Add(item.Connection);
                    }
                    if(conns.Count>0)
                    {
                        UI.fromConns = conns;
                    }
                }
                
                if(TalentTree.GetOutputConnections(talent).Count > 0)
                {
                    List<TalentTreeConnectionStatus> talentTreeConnectionBases = TalentTree.GetOutputConnections(talent);
                    List<TalentTreeConnectionBase> conns = new();
                    foreach (var item in talentTreeConnectionBases)
                    {
                        conns.Add(item.Connection);
                    }
                    if(conns.Count>0)
                    {
                        UI.toConns = conns;
                    }
                }
                
                
                treeGrowingDispaly.fruits.Add(UI);
                
            }
        }

        private void DrawNodeCurve(Rect start, Rect end, float minX, float minY, TalentTreeConnectionBase conn, TalentTreeNodeBase fromNode, TalentTreeNodeBase toNode, bool isSimplified)
        {
            if (TalentTreeUtils.connectorPosition == 0)
            {
                DrawNodeCurve(start, end, new Vector2(1f, 0.5f), new Vector2(0f, 0.5f), minX, minY, conn, fromNode, toNode, isSimplified);
            }
            else if(TalentTreeUtils.connectorPosition == 1)
            {
                DrawNodeCurve(start, end, new Vector2(0.5f, 0f), new Vector2(0.5f, 0.75f), minX, minY, conn, fromNode, toNode, isSimplified);
            }
            else if(TalentTreeUtils.connectorPosition == 2)
            {
                DrawNodeCurve(start, end, new Vector2(0.5f, 0.75f),new Vector2(0.5f, 0f), minX, minY, conn, fromNode, toNode, isSimplified);
            }
            
        }

        private void DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage, float minX, float minY, TalentTreeConnectionBase conn, TalentTreeNodeBase fromNode, TalentTreeNodeBase toNode, bool isSimplified)
        {
            Color curveColor = Color.black;

            Vector3 upperLeft = new Vector3(fromNode.nodeRect.x, fromNode.nodeRect.y, 0);

            Vector3 startPos = new Vector3(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);
            Vector3 endPos = new Vector3(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);

            // Make sure we position the prefabs on the correct place
            // 确保我们将prefab放置在正确的位置
            if (UIConnectionPrefab != null)
            {
                startPos = new Vector3(startPos.x - upperLeft.x, startPos.y - upperLeft.y, 0);
                endPos = new Vector3(endPos.x - upperLeft.x, endPos.y - upperLeft.y, 0);
            }

            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);
            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);

            
            // Create a gameobject depending on the settings
            // 根据设置创建一个gameobject
            GameObject go;
         
            
            if (UIConnectionPrefab == null)
            {
                go = new GameObject();
         
            }
                
            else
            {
                if (!sameAsRequiredConnections && UIOptionalConnectionPrefab != null)
                {
                    if (conn.connectionType == ConnectionType.Optional)
                    {
                        go = Instantiate(UIOptionalConnectionPrefab);
                       
                    } else
                    {
                        go = Instantiate(UIConnectionPrefab);
                        
                    }
                        
                }
                else
                {
                    go = Instantiate(UIConnectionPrefab);
                    
                }
                    
            }
            go.name = string.Format("Connection {0} - {1} - dynamic", fromNode.name, toNode.name);  
            RectTransform r = go.GetComponent<RectTransform>();
            if (r == null) r = go.AddComponent<RectTransform>();
            
                
            // add position it
            // 添加位置
            r.pivot = new Vector2(0f, 1f);
            r.anchorMin = new Vector2(0f, 1f);
            r.anchorMax = new Vector2(0f, 1f);
            r.anchoredPosition = Vector3.zero;
            r.SetParent(UIRoot, true);

            Vector3 pos = new Vector3(startPos.x, startPos.y, 0);
            pos.x = (pos.x - minX) + 10;
            pos.y = -((pos.y - minY) + 10);
            r.anchoredPosition = Vector3.zero;

            // In case of normal connection object, draw and color those as requested.
            // 如果是普通连接对象，则按要求绘制和着色。
            if (UIConnectionPrefab == null)
            {
                go.AddComponent<CanvasRenderer>();
                TalentUILineRenderer line = go.AddComponent<TalentUILineRenderer>();
                
                // TODO sjx 美化树枝
                line.colorToAdd = fromNode.Tier.EditorColor;
                
                // ---
                line.FromTalent = fromNode;
                line.ToTalent = toNode;
                line.Connection = conn;
                Vector3[] points = Handles.MakeBezierPoints(startPos, endPos, startTan, endTan, 10);
                Vector2[] pointsV2 = new Vector2[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    pointsV2[i] = new Vector2(points[i].x, points[i].y);
                }
                line.Points = pointsV2;

                line.color = Color.gray;
                if (ColorConnections)
                {
                    if (conn.connectionType == ConnectionType.Required)
                        line.color = TalentTreeUtils.requiredConnectionColor;
                    if (conn.connectionType == ConnectionType.Optional)
                    {
                        line.color = TalentTreeUtils.optionalConnectionColor;
                        if (isSimplified)
                            line.color = TalentTreeUtils.simplifiedCircularOptionalConnectionColor;
                    }
                }   
                line.LineThickness = 21f;// - conn.depth*2;
                if(line.LineThickness< 6f )
                {
                    line.LineThickness = 6f;
                }
                line.BezierMode = TalentUILineRenderer.BezierType.Improved;
                
                treeGrowingDispaly.conns.Add(line); // 为表现收集所有连接
            } else
            {
                // Position the prefab objects
                Vector3 Ppos = new Vector3(fromNode.nodeRect.x + fromNode.nodeRect.width, fromNode.nodeRect.y, 0);
                Ppos.x = (Ppos.x - minX) + 10;
                Ppos.y = -((Ppos.y - minY) + 10);
                r.anchoredPosition = Ppos;

                if (AddConnectionSettings)
                {
                    ConnectionProperties props = go.AddComponent<ConnectionProperties>();
                    props.FromTalent = fromNode;
                    props.ToTalent = toNode;
                    props.Connection = conn;
                }
            }

            // Add the optional scripts to the connection gameobject
            // 将可选脚本添加到连接gameobject
            if ((!sameAsRequiredConnections) && ScriptOptionalConnection != null)
            {
                if (conn.connectionType == ConnectionType.Optional)                
                    addScriptToGameObject(go, ScriptOptionalConnection);                
                else                
                    addScriptToGameObject(go, ScriptConnection);                
            }
            else
            if (ScriptConnection != null)
            {
                addScriptToGameObject(go, ScriptConnection);    
            }
        }

        /// <summary>
        /// Add a monobehaviour to a gameobject based upon a specified script.
        /// 根据指定的脚本将monobehaviour添加到gameobject。
        /// </summary>
        /// <param name="go">the receiving gameobject</param>
        /// <param name="script">The script we want to add</param>
        private void addScriptToGameObject(GameObject go, MonoScript script)
        {
            try
            {
                go.AddComponent(script.GetClass());
            }
            catch
            {
                Debug.LogError("Something went wrong assigning the extra script to the Connection object.");
            }
        }
        #endregion
    }

    class SimplifiedConnection
    {
        public TalentTreeNodeBase fromNode;
        public TalentTreeNodeBase toNode;
        public TalentTreeConnectionBase conn;
        public bool DrawConnection = false;

        public SimplifiedConnection(TalentTreeConnectionBase c, TalentTreeNodeBase f, TalentTreeNodeBase t)
        {
            conn = c;
            fromNode = f;
            toNode = t;
        }
    }
}
