using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
	public class TalentTreeCondition : ScriptableObject
    {
        #region Variables
        public ConditionType ConditionType;
        public float Value;
        public string Description;

        private string[] conditionTypes = { "Talent has level", "Talent is maxed out", "Tier is complete",};
        private int typeIndex = 0;
        private int tierIndex = 0;
        #endregion

        #region Implementation
        public virtual void DrawProperties(TalentTreeGraph curGraph, Event e)
        {
#if UNITY_EDITOR
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            switch (ConditionType)
            {
                case ConditionType.HasLevel:
                    typeIndex = 0;
                    break;
                case ConditionType.MaxedOut:
                    typeIndex = 1;
                    break;
                case ConditionType.TierCompleted:
                    typeIndex = 2;
                    break;
                default:
                    typeIndex = 0;
                    break;
            }
            typeIndex = EditorGUILayout.Popup(typeIndex, conditionTypes, GUILayout.Width(156));
            switch (typeIndex)
            {
                case 0:
                    ConditionType = ConditionType.HasLevel;
                    break;
                case 1:
                    ConditionType = ConditionType.MaxedOut;
                    break;
                case 2:
                    ConditionType = ConditionType.TierCompleted;
                    break;
                default:
                    ConditionType = ConditionType.HasLevel;
                    break;
            }
            if (typeIndex != 1)
            {
                if (typeIndex != 2)
                    Value = EditorGUILayout.FloatField(Value);
                else
                {
                    string[] Tiers = new string[curGraph.tiers.Count];
                    for (int i = 0; i < curGraph.tiers.Count; i++)
                    {
                        Tiers[i] = curGraph.tiers[i].Name;
                    }
                    if (tierIndex > curGraph.tiers.Count - 1)
                        tierIndex = 0;
                    tierIndex = EditorGUILayout.Popup(tierIndex, Tiers);
                    Value = tierIndex;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            bool oldWrap = EditorStyles.textField.wordWrap;
            EditorStyles.textField.wordWrap = true;
            Description = EditorGUILayout.TextArea(Description, GUILayout.Height(3f * 14f));
            EditorStyles.textField.wordWrap = oldWrap;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
#endif
        }
        #endregion
    }
}
