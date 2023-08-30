using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.cygnusprojects.TalentTree.Editor
{
	public class TalentTreeWindow : EditorWindow 
	{
        #region Variables
        public static TalentTreeWindow curWindow;
        public TalentTreePropertyView propertyView;
        public TalentTreeWorkView workView;
        public TalentTreeToolBar toolBar;

        public TalentTreeGraph curTree = null;
        public Rect WorkViewRect;

        public float viewPercentage = 0.75f;
        #endregion

        #region Implementation
        public static void InitEditorWindow()
        {
            curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
            curWindow.titleContent = new GUIContent("Talent Tree Editor"); //Can assign window Icon here!!!
            //curWindow.position = new Rect(0f, 0f, 640f, 320f );
            CreateViews();
        }

        private void OnEnable()
        {
            //Debug.Log("Enable window");
        }

        private void OnDestroy()
        {
            //Debug.Log("Disabling window");
        }

        private void Update()
        {
            //Debug.Log("Updating window");
        }

        private void OnGUI()
        {
            // Check for null views
            // 检查空视图
            if (propertyView == null || workView == null || toolBar == null)
            {
                CreateViews();
                return;
            }

            // Get and process current event
            // 获取并处理当前事件
            Event e = Event.current;
            ProcessEvents(e);

            // Update views
            // 更新视图
            workView.UpdateView(new Rect(0f, workView.toolBarHeight, position.width, position.height - (workView.toolBarHeight)), new Rect(1f, 1f, viewPercentage, 1f), e, curTree);

            WorkViewRect = new Rect(0f, workView.toolBarHeight, position.width, position.height - (workView.toolBarHeight));
            WorkViewRect.width = WorkViewRect.width * viewPercentage;
            curWindow.workView.WorkSpace = WorkViewRect;

            propertyView.UpdateView(new Rect(position.width, propertyView.toolBarHeight, position.width, position.height - propertyView.toolBarHeight),
                                    new Rect(viewPercentage, 1f, 1f - viewPercentage, 1f),
                                    e, curTree);
            toolBar.UpdateView(new Rect(0f, 0f, position.width, propertyView.toolBarHeight), new Rect(1f, 1f, 1f, 1f), e, curTree);
            //workView.UpdateView(position, new Rect(0f, 1 - toolBarHeightPercentage, viewPercentage, 1f), e, curTree);
            /*propertyView.UpdateView(new Rect(position.width, position.y, position.width, position.height),
                                    new Rect(viewPercentage, 0f, 1f - viewPercentage, 1f), 
                                    e, curTree);*/

            Repaint();
        }
        #endregion

        #region Utilities
        static void CreateViews()
        {
            if (curWindow != null)
            {
                curWindow.propertyView = new TalentTreePropertyView();
                curWindow.workView = new TalentTreeWorkView();
                curWindow.toolBar = new TalentTreeToolBar();

                curWindow.propertyView.toolBarHeight = 52f;
                curWindow.workView.toolBarHeight = 52f;
                curWindow.toolBar.toolBarHeight = 52f;
            }
            else
            {
                curWindow = (TalentTreeWindow)EditorWindow.GetWindow<TalentTreeWindow>();
            }
        }

        void ProcessEvents(Event e)
        {
            if (curTree == null || ((curTree.selectedNode == null) && (curTree.selectedConnection == null)))
            {
                if (e.type == UnityEngine.EventType.KeyDown && e.keyCode == KeyCode.LeftArrow)
                {
                    viewPercentage -= 0.01f;
                    if (viewPercentage < 0.0f) viewPercentage = 0.0f;
                }
                if (e.type == UnityEngine.EventType.KeyDown && e.keyCode == KeyCode.RightArrow)
                {
                    viewPercentage += 0.01f;
                    if (viewPercentage > 1f) viewPercentage = 1f;
                } 
            }
        }
        #endregion

    }
}
