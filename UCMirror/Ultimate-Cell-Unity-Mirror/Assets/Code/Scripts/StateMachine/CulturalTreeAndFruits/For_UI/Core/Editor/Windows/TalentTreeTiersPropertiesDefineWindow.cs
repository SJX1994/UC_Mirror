using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreeTiersPropertiesDefineWindow : EditorWindow 
	{
        #region Variables
        static TalentTreeTiersPropertiesDefineWindow curTiersPropManager;
        //static TalentTreeGraph curTree;
        #endregion

        #region Unity Methods
        public static void InitWindow()
        {
            //TalentTreeWindow curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
            //if (curWindow != null)
            //    curTree = curWindow.curTree;

            curTiersPropManager = (TalentTreeTiersPropertiesDefineWindow)EditorWindow.GetWindow<TalentTreeTiersPropertiesDefineWindow>();
            curTiersPropManager.titleContent = new GUIContent("Tiers Property Definition");
        }
        #endregion
    }
}
