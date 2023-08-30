using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreeTiersWindow : EditorWindow
    {
        #region Variables
        static TalentTreeTiersWindow curTiersManager;
        static TalentTreeGraph curTree;

        private Vector2 scrollPosition;
        #endregion

        #region Implementation
        public static void InitWindow()
        {
            TalentTreeWindow curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
            if (curWindow != null)
                curTree = curWindow.curTree;

            curTiersManager = (TalentTreeTiersWindow)EditorWindow.GetWindow<TalentTreeTiersWindow>();
            curTiersManager.titleContent = new GUIContent("Tiers Manager");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(18), GUILayout.Height(18)))
            {
                Color nextColor = TalentTreeUtils.DetermineNextTierColor(curTree);
                string newName = "Tier " + (curTree.tiers.Count + 1).ToString().Trim();
                TalentTreeUtils.CreateTier(curTree, newName, nextColor);
            }
            /*if (GUILayout.Button("P", GUILayout.Width(18), GUILayout.Height(18)))
            {
                TalentTreeTiersPropertiesDefineWindow.InitWindow();
            }*/
            GUILayout.EndHorizontal();
            //GUILayout.Space(20);

            float h = curTiersManager.position.height - 24f;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(h)); 
            GUILayout.BeginVertical();
            //GUILayout.Space(20);

            int cnt = 0;
            int tierToDelete = -1;
            //int tierProperties = -1;
            foreach (var item in curTree.tiers)
            {
                GUILayout.BeginHorizontal();
                if (cnt > 0)
                {
                    if (GUILayout.Button("X", GUILayout.Width(32)))
                    {
                        tierToDelete = cnt;
                    }
                }
                else
                    GUILayout.Space(40);

                /*if (GUILayout.Button("P", GUILayout.Width(32)))
                {
                    tierProperties = cnt;
                }*/
                item.OnGUI();

                GUILayout.EndHorizontal();

                cnt++;
            }

            //GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            //GUILayout.Space(20);
            /*if (tierProperties != -1)
            {
                if (curTree != null)
                {
                    if (curTree.tiers[tierProperties].Properties.Count > 0)
                    {
                        Debug.Log("Show properties");  
                    } else
                        EditorUtility.DisplayDialog("Tier Properties", "Before editing the properties you should define them using the toolbar of this window.", "Ok");
                }
            }*/
            if (tierToDelete != -1)
            {
                if (!TalentTreeUtils.DeleteTier(curTree, tierToDelete, tierToDelete - 1))
                {
                    EditorUtility.DisplayDialog("Delete Tier", "There was an error delete the selected tier, deletion was prevented!", "Ok");
                }
                tierToDelete = -1;
            }
        }

        #endregion
    }
}
