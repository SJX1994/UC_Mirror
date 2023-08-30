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
    public class PropertyDefinition : ScriptableObject 
	{
        #region Variables
        public PropertyType propertyType = PropertyType.String;

        private string[] propertyTypes = { "Int", "Float", "Boolean", "String", };
        private int propIndex = 3;
        #endregion
        #region Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Implementation
        public void OnGUI()
        {
#if UNITY_EDITOR
            GUILayout.BeginHorizontal();           
            switch (propertyType)
            {
                case PropertyType.Int:
                    propIndex = 0;
                    break;
                case PropertyType.Float:
                    propIndex = 1;
                    break;
                case PropertyType.Bool:
                    propIndex = 2;
                    break;
                case PropertyType.String:
                    propIndex = 3;
                    break;
                default:
                    propIndex = 3;
                    break;
            }
            propIndex = EditorGUILayout.Popup(propIndex, propertyTypes, GUILayout.Width(96));
            switch (propIndex)
            {
                case 0:
                    propertyType = PropertyType.Int;
                    break;
                case 1:
                    propertyType = PropertyType.Float;
                    break;
                case 2:
                    propertyType = PropertyType.Bool;
                    break;
                case 3:
                    propertyType = PropertyType.String;
                    break;
                default:
                    propertyType = PropertyType.String;
                    break;
            }
            name = EditorGUILayout.TextField(name);
            GUILayout.EndHorizontal();
#endif
        }
        #endregion
    }
}
