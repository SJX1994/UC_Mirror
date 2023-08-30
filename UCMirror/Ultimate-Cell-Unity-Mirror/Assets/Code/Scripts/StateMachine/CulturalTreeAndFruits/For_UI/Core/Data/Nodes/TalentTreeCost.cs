using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
    public class TalentTreeCost : ScriptableObject
    {
        #region Variables
        public int Cost;
        public bool Bought;
        public bool WillBuy;
        public string Description;
        #endregion

        #region Implementation
#if UNITY_EDITOR
        public virtual void DrawProperties(Event e)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();           
            Cost = EditorGUILayout.IntField(Cost, GUILayout.Width(64));
            name = EditorGUILayout.TextField(name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description : ", GUILayout.Width(96));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            bool oldWrap = EditorStyles.textField.wordWrap;
            EditorStyles.textField.wordWrap = true;
            Description = EditorGUILayout.TextArea(Description, GUILayout.Height(3f * 14f)); 
            EditorStyles.textField.wordWrap = oldWrap;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
        }
#endif
        #endregion
    }
        
}
