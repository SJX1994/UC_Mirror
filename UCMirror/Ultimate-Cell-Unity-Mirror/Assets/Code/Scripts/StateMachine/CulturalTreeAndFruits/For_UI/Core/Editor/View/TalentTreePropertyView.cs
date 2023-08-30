using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace com.cygnusprojects.TalentTree.Editor
{
    [Serializable]
    public class TalentTreePropertyView : TalentTreeViewBase
    {
        #region Variables
        private Vector2 scrollPosition;
        #endregion

        #region Implementation
        public TalentTreePropertyView() : base("Properties") { }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, TalentTreeGraph curTree)
        {
            base.UpdateView(editorRect, percentageRect, e, curTree);

            if (curTree != null)
                curTree.propertyWidth = viewRect.width;

            GUI.Box(viewRect, "", viewSkin.GetStyle("PropertiesBG"));

            GUILayout.BeginArea(viewRect);

            float h = viewRect.height; // - 16f;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(h));

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (curTree != null)
            {
                if (!curTree.showProperties)
                {
                    EditorGUILayout.LabelField("No item selected");
                }
                else
                {
                    if (curTree.selectedConnection != null || curTree.selectedNode != null)
                    {
                        if (curTree.selectedNode != null)                       
                            curTree.selectedNode.DrawNodeProperties(e, viewSkin);
                        else
                        {
                            if (curTree.selectedConnection != null)
                                curTree.selectedConnection.DrawConnectionProperties(e, viewRect, viewSkin);
                            else EditorGUILayout.LabelField("No item selected");
                        } 
                    }
                    else
                        EditorGUILayout.LabelField("No item selected");
                }
            }
            else
                EditorGUILayout.LabelField("No item selected");
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
            GUILayout.EndArea();


            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            if (viewRect.Contains(e.mousePosition))
            {
                if (curTree != null)
                {
                    curTree.mouseOverProperties = true;
                }
                //Debug.Log("Inside " + viewTitle);
            } else
            {
                if (curTree != null)
                {
                    curTree.mouseOverProperties = false;
                }
            }
        }
        #endregion

        #region Utilities

        #endregion
    }
}
