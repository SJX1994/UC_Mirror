using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreeToolBar : TalentTreeViewBase
    {
        #region Variables
        private int toolIndex;
        #endregion

        #region Implementation
        public TalentTreeToolBar() : base("ToolBar") { }


        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, TalentTreeGraph curTree)
        {
            base.UpdateView(editorRect, percentageRect, e, curTree);

            GUI.Box(viewRect, viewTitle+"    ", viewSkin.GetStyle("Toolbar"));

            GUILayout.BeginHorizontal(); 
            Texture2D Image = (Texture2D)Resources.Load("Textures/Editor/logo");
            Rect imageRect = new Rect(8f, 8f, 192f, 36f);
            GUI.DrawTexture(imageRect, Image, ScaleMode.ScaleAndCrop, true, 0.0F);

            GUILayout.Space(240);

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            Texture2D[] imgs = new Texture2D[3];
            Image = (Texture2D)Resources.Load("Textures/Editor/Normal_mode");
            imgs[0] = Image;
            Image = (Texture2D)Resources.Load("Textures/Editor/Tiers");
            imgs[1] = Image;
            Image = (Texture2D)Resources.Load("Textures/Editor/Settings");
            imgs[2] = Image;
            toolIndex = GUILayout.Toolbar(toolIndex, imgs, GUILayout.Height(32), GUILayout.Width(32 * imgs.Length));
            if (toolIndex == 1)
            {
                toolIndex = 0;
                if (curTree != null)
                    TalentTreeTiersWindow.InitWindow();
                else
                    EditorUtility.DisplayDialog("Tiers Manager", "No current talent selected, please create or open a tree first!", "Ok");
            }
            if (toolIndex == 2)
            {
                toolIndex = 0;
                EditorUtility.DisplayDialog("Talentus", "TALENTUS, Skill/Talent Tree Asset for Unity."+ Environment.NewLine + Environment.NewLine + TalentTreeUtils.GetVersionString(), "Ok");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            /*GUILayout.BeginHorizontal();
            Image = (Texture2D)Resources.Load("Textures/Editor/Tiers");
            GUILayout.BeginVertical();
            GUILayout.Space(10); 
            if (GUILayout.Button(new GUIContent(Image,"Manage Tiers"), GUILayout.Height(32),GUILayout.Width(32)))
            {

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent(Image, "Manage Tiers"), GUILayout.Height(32), GUILayout.Width(32)))
            {

            }
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();*/

        }
        #endregion

        #region Utilities

        #endregion
    }
}
