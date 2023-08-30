using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.cygnusprojects.TalentTree
{
    [Serializable]
    public class Tier : ScriptableObject, ITier
    {
        #region Variables
        public TalentTreeGraph parentTree;

        [SerializeField]
        private Color editorColor;
        [SerializeField]
        private string description;
        /*[SerializeField]
        private List<Property> properties = new List<Property>();*/

        #endregion

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Color EditorColor
        {
            get { return editorColor; }
            set { editorColor = value; }
        }

        public bool IsMaxedOut
        {
            get
            {
                if (parentTree != null)
                {
                    if (parentTree.talents != null)
                    {

                        bool retValue = true;
                        if (parentTree.talents.Count > 0)
                        {
                            foreach (var item in parentTree.talents)
                            {
                                if (item.Tier == this)
                                {
                                    if (item.Level < item.MaxLevel)
                                    {
                                        retValue = false;
                                        break;
                                    }
                                }
                                if (retValue == false)
                                    break;
                            }
                        }
                        return retValue;
                    }
                    else
                        return true; // if there are no talents then by default the tiers are maxed out
                }
                else
                    return false;
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /*public List<Property> Properties
        {
            get { return properties; }
        }

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

        public void OnGUI()
        {
#if UNITY_EDITOR
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            editorColor = EditorGUILayout.ColorField(editorColor, GUILayout.Width(48));
            name = EditorGUILayout.TextField(name); 
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description : ", GUILayout.Width(76));
            description = EditorGUILayout.TextArea(description, GUILayout.Height(16f * 3f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
#endif
        }

    }
}
