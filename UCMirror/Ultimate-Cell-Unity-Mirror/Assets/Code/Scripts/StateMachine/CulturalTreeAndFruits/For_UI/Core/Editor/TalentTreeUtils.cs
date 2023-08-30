using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using com.cygnusprojects.TalentTree;

namespace com.cygnusprojects.TalentTree.Editor
{
	public static class TalentTreeUtils
	{
        #region Constants
        // Version major
        public static int versionMajor = 1;
        public static int versionMinor = 5;
        public static int versionRevision = 4;
        public static bool isBeta = false;
        #endregion

        #region Variables
        private static string[] options = new string[3] { "Left-Right","Top-Bottom","Bottom-Top"};

        // The Preferences
        public static string stringTalentusLocation = "/Talentus/Example/Database/";
        public static bool drawGrid = false;
        public static Color requiredConnectionColor = new Color(0, 204f / 255f, 0);
        public static Color optionalConnectionColor = new Color(0, 128f / 255f, 1f);
        public static Color simplifiedCircularOptionalConnectionColor = new Color(1f, 121f / 255f, 0f);
        public static int connectorPosition = 0;
        #endregion

        #region Preferences
        // Add preferences section named "My Preferences" to the Preferences Window
        private class TalentusPrefSettingsProvider : SettingsProvider
        {
            public TalentusPrefSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
                : base(path, scopes)
            { }

            public override void OnGUI(string searchContext)
            {
                PreferencesGUI();
            }
        }

        [SettingsProvider]
        static SettingsProvider TalentusPrefCode()
        {
            return new TalentusPrefSettingsProvider("Project/Talentus");
        }

        public static void PreferencesGUI()
        {
            // Load the preferences
            LoadPreferences();
            
            // Preferences GUI
            EditorGUIUtility.labelWidth = 60;
            stringTalentusLocation = EditorGUILayout.TextField("Location", stringTalentusLocation);
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = 120;
            drawGrid = EditorGUILayout.Toggle("Draw Grid in Editor", drawGrid);
            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.Space();
            connectorPosition = EditorGUILayout.Popup("Connectors", connectorPosition, options);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Connection Colors", EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 178f;
            requiredConnectionColor = EditorGUILayout.ColorField("Required", requiredConnectionColor);
            optionalConnectionColor = EditorGUILayout.ColorField("Optional", optionalConnectionColor);
            simplifiedCircularOptionalConnectionColor = EditorGUILayout.ColorField("Simplified Optional Circular", simplifiedCircularOptionalConnectionColor);
            EditorGUIUtility.labelWidth = 0f;

            // Save the preferences
            if (GUI.changed)
            {
                if (stringTalentusLocation[0] != '/')
                    stringTalentusLocation = "/" + stringTalentusLocation;
                if (stringTalentusLocation.Substring(stringTalentusLocation.Length - 1) != "/")
                    stringTalentusLocation = stringTalentusLocation + "/";
                EditorPrefs.SetString("TalentusLocationPreferenceKey", stringTalentusLocation);
                EditorPrefs.SetBool("TalentusDrawGridPreferenceKey", drawGrid);
                EditorPrefs.SetInt("TalentusConnectorsPositionPreferenceKey", connectorPosition);
                EditorPrefs.SetFloat("TalentusRequiredConnectionColorRPK", requiredConnectionColor.r);
                EditorPrefs.SetFloat("TalentusRequiredConnectionColorGPK", requiredConnectionColor.g);
                EditorPrefs.SetFloat("TalentusRequiredConnectionColorBPK", requiredConnectionColor.b);
                EditorPrefs.SetFloat("TalentusOptionalConnectionColorRPK", optionalConnectionColor.r);
                EditorPrefs.SetFloat("TalentusOptionalConnectionColorGPK", optionalConnectionColor.g);
                EditorPrefs.SetFloat("TalentusOptionalConnectionColorBPK", optionalConnectionColor.b);
                EditorPrefs.SetFloat("TalentusOptionalSimplifiedConnectionColorRPK", simplifiedCircularOptionalConnectionColor.r);
                EditorPrefs.SetFloat("TalentusOptionalSimplifiedConnectionColorGPK", simplifiedCircularOptionalConnectionColor.g);
                EditorPrefs.SetFloat("TalentusOptionalSimplifiedConnectionColorBPK", simplifiedCircularOptionalConnectionColor.b);
            }
        }

        public static void LoadPreferences()
        {
            stringTalentusLocation = EditorPrefs.GetString("TalentusLocationPreferenceKey", "/Talentus/Example/Database/");
            drawGrid = EditorPrefs.GetBool("TalentusDrawGridPreferenceKey", false);
            connectorPosition = EditorPrefs.GetInt("TalentusConnectorsPositionPreferenceKey", 0);
            float requiredConnectionColorR = EditorPrefs.GetFloat("TalentusRequiredConnectionColorRPK", 0f);
            float requiredConnectionColorG = EditorPrefs.GetFloat("TalentusRequiredConnectionColorGPK", 204f / 255f);
            float requiredConnectionColorB = EditorPrefs.GetFloat("TalentusRequiredConnectionColorBPK", 0f);
            requiredConnectionColor = new Color(requiredConnectionColorR, requiredConnectionColorG, requiredConnectionColorB);
            float optionalConnectionColorR = EditorPrefs.GetFloat("TalentusOptionalConnectionColorRPK", 0f);
            float optionalConnectionColorG = EditorPrefs.GetFloat("TalentusOptionalConnectionColorGPK", 128f / 255f);
            float optionalConnectionColorB = EditorPrefs.GetFloat("TalentusOptionalConnectionColorBPK", 1f);
            optionalConnectionColor = new Color(optionalConnectionColorR, optionalConnectionColorG, optionalConnectionColorB);
            float optionalSConnectionColorR = EditorPrefs.GetFloat("TalentusOptionalSimplifiedConnectionColorRPK", 0f);
            float optionalSConnectionColorG = EditorPrefs.GetFloat("TalentusOptionalSimplifiedConnectionColorGPK", 128f / 255f);
            float optionalSConnectionColorB = EditorPrefs.GetFloat("TalentusOptionalSimplifiedConnectionColorBPK", 1f);
            simplifiedCircularOptionalConnectionColor = new Color(optionalSConnectionColorR, optionalSConnectionColorG, optionalSConnectionColorB);
        }
        #endregion

        #region Implementation
        public static string GetVersionString()
        {
            string version = string.Format("Version {0}.{1}.{2}", versionMajor, versionMinor, versionRevision);
            if (isBeta)
                version = version + " Beta";
            return version;
        }

        [MenuItem("GameObject/Cygnus Projects/Talentus", false, 10)]
        static void CreateTalentusGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Talentus");
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            go.AddComponent<TalentusEngine>();
            Selection.activeObject = go;
        }

        public static void CreateNewTree(string treeName)
        {
            TalentTreeGraph curGraph = (TalentTreeGraph)ScriptableObject.CreateInstance<TalentTreeGraph>();
            if (curGraph != null)
            {
                curGraph.treeName = treeName;
                curGraph.InitGraph();

                AssetDatabase.CreateAsset(curGraph, "Assets" + stringTalentusLocation  + treeName + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //Add Tier 1 as default
                CreateTier(curGraph, "Tier 1", Color.blue);

                TalentTreeWindow curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
                if (curWindow != null)
                {
                    curWindow.curTree = curGraph;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Create Tree", "Unable to create a new tree, please see log for more info!", "Ok");
            }
            
        }

        public static void LoadTree()
        {
            TalentTreeGraph curGraph = null;
            string treePath = EditorUtility.OpenFilePanel("Load Tree", Application.dataPath + stringTalentusLocation, "");

            try
            {
                int appPathLength = Application.dataPath.Length;
                string finalPath = treePath.Substring(appPathLength - 6);
                curGraph = (TalentTreeGraph)AssetDatabase.LoadAssetAtPath(finalPath, typeof(TalentTreeGraph));
                if (curGraph != null)
                {
                    TalentTreeWindow curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
                    if (curWindow != null)
                    {
                        curWindow.curTree = curGraph;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Load Tree", "Unable to load the selected tree!", "Ok");
                }
            }
            catch { }             
        }

        public static void UnloadTree()
        {
            TalentTreeWindow curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
            if (curWindow != null)
            {
                curWindow.curTree = null;
            }
        }

        /*public static void CreateTierPropertyDefinition(TalentTreeGraph curGraph, string name)
        {
            if (curGraph != null)
            {
                PropertyDefinition def = null;
                def = (PropertyDefinition)ScriptableObject.CreateInstance<PropertyDefinition>();
                if (def != null)
                {
                    def.Name = name;
                    def.propertyType = PropertyType.String;
                    curGraph.tierPropertyDefinitions.Add(def);

                    AssetDatabase.AddObjectToAsset(def, curGraph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }*/

        /*public static void UpdatePropertiesOnTierObjects(TalentTreeGraph curGraph)
        {
            if (curGraph != null)
            {
                if (curGraph.tierPropertyDefinitions.Count > 0)
                {
                    for (int i = 0; i < curGraph.tierPropertyDefinitions.Count; i++)
                    {
                        PropertyDefinition pd = curGraph.tierPropertyDefinitions[i];
                        for (int j = 0; j < curGraph.tiers.Count; j++)
                        {
                            Property p = new Property(pd.Name, pd.propertyType);
                            Property c = curGraph.tiers[j].GetProperty(pd.Name);
                            if (c == null || c.PropertyType != p.PropertyType)
                            {
                                
                                curGraph.tiers[j].SetProperty(p);
                            }
                        }
                    }
                }
            }
        }*/

        public static void CreateTier(TalentTreeGraph curGraph, string name, Color color)
        {
            if (curGraph != null)
            {
                Tier tier = null;
                tier = (Tier)ScriptableObject.CreateInstance<Tier>();
                if (tier != null)
                {
                    tier.Name = name;
                    tier.EditorColor = color;

                    tier.parentTree = curGraph;
                    curGraph.tiers.Add(tier);

                    AssetDatabase.AddObjectToAsset(tier, curGraph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                } 
            }
        }

        public static bool DeleteTier(TalentTreeGraph curGraph, int tierToRemoveIdx, int tierDestinationIdx)
        {
            if (curGraph != null)
            {
                Tier tierToRemove = curGraph.tiers[tierToRemoveIdx];
                Tier tierDestination = curGraph.tiers[tierDestinationIdx];
                if (tierDestination == null)
                    tierDestinationIdx = 0;

                if (tierToRemove != null)
                {
                    if (curGraph.talents.Count > 0)
                    {
                        for (int i = 0; i < curGraph.talents.Count; i++)
                        {
                            if (curGraph.talents[i].Tier == tierToRemove)
                            {
                                curGraph.talents[i].Tier = curGraph.tiers[tierDestinationIdx];
                            }
                        }
                    }
                    /*foreach (var item in curGraph.talents)
                    {
                        if (item.Tier == tierToRemove)
                        {
                            item.Tier = curGraph.tiers[tierDestinationIdx];
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }*/
                    curGraph.tiers.RemoveAt(tierToRemoveIdx);
                    GameObject.DestroyImmediate(tierToRemove, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return true;
                }
            }
            return false;
        }

        public static void CreateNode(TalentTreeGraph curGraph, NodeType nodeType, Vector3 mousePos)
        {
            if (curGraph != null)
            {
                TalentTreeNodeBase curNode = null;
                switch (nodeType)
                {
                    case NodeType.Talent:
                        curNode = (TalentTreeNode)ScriptableObject.CreateInstance<TalentTreeNode>();
                        curNode.Name = "Talent/Skill";
                        if (curGraph.tiers.Count > 0)
                            curNode.Tier = curGraph.tiers[0];
                        break;
                    default:
                        break;
                }

                if (curNode != null)
                {
                    curNode.InitNode();
                    curNode.nodeRect.x = mousePos.x;
                    curNode.nodeRect.y = mousePos.y;
                    curNode.parentTree = curGraph;

                    curGraph.talents.Add(curNode);
                    AssetDatabase.AddObjectToAsset(curNode, curGraph);

                    //Add the default cost of 1
                    TalentTreeCost cost = (TalentTreeCost)ScriptableObject.CreateInstance<TalentTreeCost>();
                    cost.Cost = 1;
                    cost.name = "Default Cost";
                    curNode.Cost.Add(cost);
                    AssetDatabase.AddObjectToAsset(cost, curNode);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public static Color DetermineNextTierColor(TalentTreeGraph curGraph)
        {
            Color[] colors = { Color.blue, Color.green, Color.red, Color.cyan, Color.yellow, Color.magenta, Color.white};
            if (curGraph != null)
            {
                if (curGraph.tiers.Count < 7)
                    return colors[curGraph.tiers.Count];
                else
                    return Color.black;
            }
            else
                return Color.black;
        }

        public static void DeleteNode(int nodeID, TalentTreeGraph curTree)
        {
            if (curTree != null)
            {
                if (curTree.talents.Count >= nodeID)
                {
                    TalentTreeNodeBase deleteNode = curTree.talents[nodeID];
                    if (deleteNode != null)
                    {
                        curTree.talents.RemoveAt(nodeID);
                        GameObject.DestroyImmediate(deleteNode, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        public static void DeleteConnection(int connectionID, TalentTreeGraph curTree)
        {
            if (curTree != null)
            {
                if (curTree.connections.Count >= connectionID)
                {
                    TalentTreeConnectionBase deleteConnection = curTree.connections[connectionID];
                    if (deleteConnection != null)
                    {
                        deleteConnection.DeleteAllCondItems(curTree, deleteConnection);
                        curTree.connections.RemoveAt(connectionID);
                        GameObject.DestroyImmediate(deleteConnection, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpcacity, Color gridColor)
        {
            if (drawGrid)
            {
                int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);
                int heightDivs = Mathf.CeilToInt(viewRect.height / gridSpacing);

                Handles.BeginGUI();
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpcacity);
                for (int x = 0; x < widthDivs; x++)
                {
                    Handles.DrawLine(new Vector3(gridSpacing * x, 0f, 0f), new Vector3(gridSpacing * x, viewRect.height, 0f));

                }
                for (int y = 0; y < heightDivs; y++)
                {
                    Handles.DrawLine(new Vector3(0f, gridSpacing * y, 0f), new Vector3(viewRect.width, gridSpacing * y, 0f));

                }
                Handles.color = Color.white;
                Handles.EndGUI();
            }
        }
        #endregion
    }
}
