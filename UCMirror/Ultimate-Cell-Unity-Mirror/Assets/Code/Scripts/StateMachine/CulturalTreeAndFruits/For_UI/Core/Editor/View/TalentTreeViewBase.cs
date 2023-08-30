using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace com.cygnusprojects.TalentTree.Editor
{
    [Serializable]
	public class TalentTreeViewBase 
	{
        #region Variables
        public string viewTitle;
        public Rect viewRect;
        public float toolBarHeight;

        protected GUISkin viewSkin;
        protected TalentTreeGraph curTree;
        #endregion

        #region Implementation
        public TalentTreeViewBase(string title)
        {
            viewTitle = title;
            //GetEditorSkin();
        }

        public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e, TalentTreeGraph curTree)
        {
            if (viewSkin == null)
            {
                GetEditorSkin();
                return;
            }

            // Set the current view Tree
            // 设置当前视图树
            this.curTree = curTree;

            // Update viewtitle
            // 更新视图标题
            if (curTree != null)
            {
                viewTitle = curTree.treeName;
            }
            else
            {
                viewTitle = "No Talent Tree";
            }

            // Update view rectangle
            // 更新视图矩形
            viewRect = new Rect(editorRect.x * percentageRect.x,
                                editorRect.y * percentageRect.y,
                                editorRect.width * percentageRect.width,
                                editorRect.height * percentageRect.height);

        }

        public virtual void ProcessEvents(Event e) { }
        #endregion

        #region Utilities
        protected void GetEditorSkin()
        {
            if (EditorGUIUtility.isProSkin)
                viewSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/TalentTreeEditorSkin");
            else
                viewSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/TalentTreeEditorSkinPersonal");            
        }
        #endregion
    }
}
