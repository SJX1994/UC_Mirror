using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace com.cygnusprojects.TalentTree
{
	public class TalentTreeNode : TalentTreeNodeBase
    {
        #region Variables
        private float HandleHeight = 24f;
        private float HandleWidth = 12f;
        #endregion

        #region Implementation
        public override void InitNode()
        {
            base.InitNode();
            nodeType = NodeType.Talent;
            // nodeRect = new Rect(0f, 0f, 96f, 112f);
            // TODO SJX
            nodeRect = new Rect(0f, 0f, 96f, 96f);
        }

        public override void UpdateNode(Event e, Rect viewRect)
        {
            base.UpdateNode(e, viewRect);

        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin, int connectorPosition)
        {
            base.UpdateNodeGUI(e, viewRect, viewSkin, connectorPosition);

            if (Image != null)
            {
                float xv = nodeRect.x + ((nodeRect.width - 64f) / 2);
                float yv = nodeRect.y + (nodeRect.height - (64f + 8f));
                Color oldColor;
                oldColor = GUI.color;
                GUI.color = new Color(16f / 255f, 16f / 255f, 16f / 255f, 1f);
                GUI.Box(new Rect(xv - 1f, yv - 1f, 66f, 66f), "");
                GUI.color = new Color(240f / 255f, 240f / 255f, 240f / 255f, 1f);
                GUI.Box(new Rect(xv, yv, 65f, 65f), "");
                GUI.color = oldColor;
                Rect imageRect = new Rect(xv, yv, 64f, 64f);
                GUI.DrawTexture(imageRect, Image, ScaleMode.ScaleAndCrop, true, 0.0F);
            }

            // Input
            Rect inputRect = new();
            string style = "NodeInput";
            if (connectorPosition == 0)
            {
                inputRect = new Rect(nodeRect.x - HandleWidth, (nodeRect.y + (nodeRect.height - HandleHeight) / 2), HandleWidth, HandleHeight);
            }  
            else if(connectorPosition == 1)
            {
                inputRect = new Rect(nodeRect.x + (nodeRect.width - HandleHeight) / 2, nodeRect.y - HandleWidth, HandleHeight, HandleWidth);
                style = "NodeInputVertical";
            }else if (connectorPosition == 2) 
            {
                inputRect = new Rect((nodeRect.x + (nodeRect.width - HandleHeight) / 2), nodeRect.y + nodeRect.height, HandleHeight, HandleWidth);
                style = "NodeOutputVertical";
            }

            if (GUI.Button(inputRect, "", viewSkin.GetStyle(style)))
            {
                if (parentTree != null)
                {
                    TalentTreeConnection conn = TalentTreeConnectionUtils.CreateConnection(parentTree, parentTree.connectionNode, this);
                    parentTree.wantsConnection = false;
                    if (conn != null)
                    {
                        parentTree.selectedConnection = conn;
                    }
                    //parentTree.connectionNode = null;
                }
            }

            // Output
            Rect outputRect = new();
            style = "NodeOutput";
            if (connectorPosition == 0)
            {
                 outputRect = new Rect(nodeRect.x + nodeRect.width, (nodeRect.y + (nodeRect.height - HandleHeight) / 2), HandleWidth, HandleHeight);
            }
            else if(connectorPosition == 1)
            {
                outputRect = new Rect((nodeRect.x + (nodeRect.width - HandleHeight) / 2), nodeRect.y + nodeRect.height, HandleHeight, HandleWidth);
                style = "NodeOutputVertical";
            }
            else if(connectorPosition == 2)
            {
                outputRect = new Rect(nodeRect.x + (nodeRect.width - HandleHeight) / 2, nodeRect.y - HandleWidth, HandleHeight, HandleWidth);
                style = "NodeInputVertical";
            }

            if (GUI.Button(outputRect, "", viewSkin.GetStyle(style)))
            {
                if (parentTree != null)
                {
                    parentTree.ClearSelection();
                    parentTree.wantsConnection = true;
                    parentTree.connectionNode = this;
                }
            }
        }

        public override void DrawNodeProperties(Event e, GUISkin viewSkin)
        {
            base.DrawNodeProperties(e, viewSkin);

        }
#endif

        #endregion

        #region Utilities

        #endregion
    }
}
