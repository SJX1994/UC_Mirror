using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreePopupWindow : EditorWindow
	{
        #region Variables
        static TalentTreePopupWindow curPopup;
        string wantedName = "Enter a name...";
        #endregion

        #region Implementation
        public static void InitNodePopup()
        {
            curPopup = (TalentTreePopupWindow)EditorWindow.GetWindow<TalentTreePopupWindow>();
            curPopup.titleContent = new GUIContent("Node popup");
        }

        private void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Create New Tree:", EditorStyles.boldLabel);
            wantedName = EditorGUILayout.TextField("Enter Name:", wantedName);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Create Tree", GUILayout.Height(40)))
            {
                if(!string.IsNullOrEmpty(wantedName) && wantedName != "Enter a name...")
                {
                    TalentTreeUtils.CreateNewTree(wantedName);
                    curPopup.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Tree","Please enter a valid tree name!", "Ok");
                }
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(40)))
            {
                curPopup.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        #endregion

        #region Utilities

        #endregion
    }
}
